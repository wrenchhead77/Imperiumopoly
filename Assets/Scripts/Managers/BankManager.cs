using UnityEngine;

public class BankManager : MonoBehaviour
{
    DiceManager dm;
    CanvasManager cm;
    PlayerManager pm;
//    CardManager card;
    Hud hud;

    public static BankManager Instance;

    public int safeHarborPot = 0; // Track money collected for Safe Harbor

    private void Awake()
    {
        Instance = this;
        dm = DiceManager.Instance;
        cm = CanvasManager.Instance;
        pm = PlayerManager.Instance;
//        card = CardManager.Instance;
        hud = Hud.Instance;
    }
    public void AddToSafeHarbor(int amount)
    {
        safeHarborPot += amount;
        Debug.Log($"Added ${amount} to Safe Harbor pot. Total: ${safeHarborPot}");
    }

    // Claim the Safe Harbor pot
    public int ClaimSafeHarborPot()
    {
        int amount = safeHarborPot;
        safeHarborPot = 0;
        Debug.Log($"Player claimed ${amount} from Safe Harbor pot.");
        return amount;
    }

    // Get the current pot value
    public int GetSafeHarborPot()
    {
        return safeHarborPot;
    }
    public int CalculateRent(soSpot _soSpot, bool doubleRent = false, bool chanceCardUtility = false)
    {
        Player owner = pm.WhoOwnsProperty(_soSpot);

        // Check if the property is mortgaged
        if (PersistentGameData.Instance.GetMortgageStatus(_soSpot.spotName))
        {
            Debug.Log($"{_soSpot.spotName} is mortgaged. No rent is collected.");
            return 0; // No rent if the property is mortgaged
        }

        if (owner == null)
        {
            Debug.LogError("No owner found for this property.");
            return 0;
        }

        switch (_soSpot.spotType)
        {
            case eSpotType.property:
                // Standard rent based on houses or hotels for properties
                if (_soSpot.rent == null || _soSpot.rent.Length == 0)
                {
                    Debug.LogError($"Rent array is not initialized for {_soSpot.spotName}");
                    return 0;
                }
                int houseCount = owner.propertyManager.GetHouseCount(_soSpot);
                int propertyRent = _soSpot.rent[houseCount];
                Debug.Log($"Calculating property rent: {propertyRent} for house count: {houseCount}");
                return propertyRent;

            case eSpotType.railRoad:
                // Railroad rent depends on the number of railroads owned
                if (_soSpot.rentRR == null || _soSpot.rentRR.Length == 0)
                {
                    Debug.LogError($"Railroad rent array is not initialized for {_soSpot.spotName}");
                    return 0;
                }
                int railroadsOwned = owner.propertyManager.CountRailroadsOwned();
                int railroadRent = _soSpot.rentRR[railroadsOwned - 1];
                Debug.Log($"Calculating railroad rent: {railroadRent} for railroads owned: {railroadsOwned}");
                return doubleRent ? railroadRent * 2 : railroadRent;

            case eSpotType.utility:
                // Utility rent logic
                if (_soSpot.rentUte == null || _soSpot.rentUte.Length == 0)
                {
                    Debug.LogError($"Utility rent array is not initialized for {_soSpot.spotName}");
                    return 0;
                }

                int diceRoll = PersistentGameData.Instance.lastDiceRoll;
                if (chanceCardUtility)
                {
                    int chanceRent = diceRoll * 10;
                    Debug.Log($"Chance card utility rent: {chanceRent} for dice roll: {diceRoll}");
                    return chanceRent;
                }
                else
                {
                    int multiplier = owner.propertyManager.CountUtilitiesOwned() == 2 ? 10 : 4;
                    int utilityRent = diceRoll * multiplier;
                    Debug.Log($"Calculating utility rent: {utilityRent} for dice roll: {diceRoll} and multiplier: {multiplier}");
                    return utilityRent;
                }

            default:
                Debug.LogError($"Invalid spot type for rent calculation: {_soSpot.spotType}");
                return 0;
        }
    }
    public void TransferFunds(Player fromPlayer, Player toPlayer, int amount)
    {
        if (fromPlayer.cashOnHand >= amount)
        {
            fromPlayer.AdjustCash(-amount);
            toPlayer.AdjustCash(amount);
            Debug.Log($"Transferred ${amount} from {fromPlayer.playerName} to {toPlayer.playerName}");
        }
        else
        {
            Debug.LogError($"{fromPlayer.playerName} does not have enough cash to transfer ${amount}");
        }
    }

    public void ChargePlayer(Player player, int amount)
    {
        if (player.cashOnHand >= amount)
        {
            player.AdjustCash(-amount);
            Debug.Log($"Charged ${amount} to {player.playerName}");
        }
        else
        {
            Debug.LogError($"{player.playerName} does not have enough cash to pay ${amount}");
            // You could add additional logic here for bankruptcy or loans
        }
    }

    public void RewardPlayer(Player player, int amount)
    {
        player.AdjustCash(amount);
        Debug.Log($"Awarded ${amount} to {player.playerName}");
    }
    public int CalculateRepairCosts(Player player, int costPerHouse, int costPerHotel)
    {
        int totalHouses = 0;
        int totalHotels = 0;

        // Iterate through the player's owned properties
        foreach (var property in player.propertyManager.listPropertiesOwned)
        {
            // Check the house amount
            if (property.houseAmt < 5) // Less than 5 indicates houses
            {
                totalHouses += property.houseAmt;
            }
            else if (property.houseAmt == 5) // Exactly 5 indicates a hotel
            {
                totalHotels += 1;
            }
        }

        // Calculate the total repair cost
        int repairCost = (totalHouses * costPerHouse) + (totalHotels * costPerHotel);
        Debug.Log($"Repair Costs: Houses={totalHouses}, Hotels={totalHotels}, TotalCost={repairCost}");

        return repairCost;
    }
}
