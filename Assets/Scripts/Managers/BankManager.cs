using UnityEngine;

public class BankManager : MonoBehaviour
{
    public soSpot spot;
    public static BankManager Instance;
    public int safeHarborPot = 0; // Track money collected for Safe Harbor

    private void Awake()
    {
        Instance = this;
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
    // Calculate rent for a property (based on ownership and property type)
    public int CalculateRent(iPlayer owner, soSpot spot)
    {
        int rent = 0;

        // Check if the property is mortgaged
        bool isMortgaged = PersistentGameData.Instance.GetMortgageStatus(spot.spotName);
        if (isMortgaged)
        {
            Debug.LogWarning($"{spot.spotName} is mortgaged. Rent is not charged.");
            return rent;
        }

        // Calculate rent based on property type
        switch (spot.spotType)
        {
            case eSpotType.property:
                // Rent with no houses or hotels
                rent = spot.rent[0];

                // Rent with houses or hotels
                int houseCount = owner.propertyManager.GetHouseCount(spot);
                if (houseCount > 0 && houseCount <= 4)
                {
                    rent = spot.rent[houseCount];  // Rent with houses
                }
                else if (owner.propertyManager.GetHotelCount(spot) > 0)
                {
                    rent = spot.rent[5];  // Rent with a hotel (6th entry)
                }
                break;

            case eSpotType.railRoad:
                // Rent based on the number of railroads owned
                int railroadsOwned = owner.propertyManager.CountRailroadsOwned();
                if (railroadsOwned >= 1 && railroadsOwned <= 4)
                {
                    rent = spot.rentRR[railroadsOwned - 1];  // Rent for owning 1-4 railroads
                }
                break;

            case eSpotType.utility:
                // Rent based on the number of utilities owned (x4 or x10 multipliers)
                int utilitiesOwned = owner.propertyManager.CountUtilitiesOwned();
                if (utilitiesOwned == 1)
                {
                    rent = spot.rentUte[0] * PersistentGameData.Instance.lastDiceRoll; // Rent * x4 multiplier
                }
                else if (utilitiesOwned == 2)
                {
                    rent = spot.rentUte[1] * PersistentGameData.Instance.lastDiceRoll; // Rent * x10 multiplier
                }
                break;

            default:
                Debug.LogWarning($"Unknown spot type: {spot.spotType}");
                break;
        }

        Debug.Log($"Rent for {spot.spotName} is ${rent}");
        return rent;
    }
    public void TransferFunds(iPlayer fromPlayer, iPlayer toPlayer, int amount)
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
    public void ChargePlayer(iPlayer player, int amount)
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

    public void RewardPlayer(iPlayer player, int amount)
    {
        player.AdjustCash(amount);
        Debug.Log($"Awarded ${amount} to {player.playerName}");
    }
    public int CalculateRepairCosts(iPlayer player, int costPerHouse, int costPerHotel)
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
