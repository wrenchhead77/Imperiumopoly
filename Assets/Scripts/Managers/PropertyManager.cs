using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable] // Ensure the class is marked as Serializable
public class PropertyOwnership
{
    public soSpot so_Spot;
    public string spotName;
    public bool isMortgaged;
    public int houseAmt;
    public int hotelAmt;

    public PropertyOwnership(soSpot _soSpot, bool _isMortgaged, int _houseAmt, int _hotelAmt)
    {
        spotName = _soSpot.spotName;
        so_Spot = _soSpot;
        isMortgaged = _isMortgaged;
        houseAmt = _houseAmt;
        hotelAmt = _hotelAmt;
    }
}

public class PropertyManager : MonoBehaviour
{
    public iPlayer player;                                  // Reference to the current player
    private WcenterManage wcenterManage;
    [SerializeField] private GameObject flagPrefab;       // Prefab for the flag
    private Dictionary<soSpot, GameObject> spotFlags = new Dictionary<soSpot, GameObject>();
    [SerializeField] public List<PropertyOwnership> listPropertiesOwned; // Expose to the Inspector

    private void Awake()
    {
        listPropertiesOwned = new List<PropertyOwnership>();
    }

    public void SetManageReference(WcenterManage manageInstance)
    {
        wcenterManage = manageInstance;
    }

    public void BuyProperty(soSpot _soSpot)
    {
        listPropertiesOwned.Add(new PropertyOwnership(_soSpot, false, 0, 0));
        PlaceFlagOnProperty(_soSpot); // Place the flag when a property is purchased
    }

    private void PlaceFlagOnProperty(soSpot spot)
    {
        // Check if a flag already exists for the property
        if (spotFlags.ContainsKey(spot))
        {
            ErrorLogger.Instance.LogWarning($"Flag already exists for {spot.spotName}");
            return;
        }

        // Get the position, rotation, and scale of the spot
        Transform spotTransform = Board.Instance.GetSpotTransform(spot);
        if (spotTransform == null)
        {
            ErrorLogger.Instance.LogError($"Could not find the transform for {spot.spotName}");
            return;
        }

        // Instantiate the flag at the same transform as the spot
        GameObject flagInstance = Instantiate(flagPrefab, spotTransform.position, spotTransform.rotation);

        // Store the flag reference for the property
        spotFlags[spot] = flagInstance;

        // Customize the flag's material to match the player's color
        Renderer flagRenderer = flagInstance.GetComponentInChildren<Renderer>();
        flagRenderer.material.color = player.playerColor;
    }

    public bool IsPropertyOwned(soSpot _soSpot)
    {
        return listPropertiesOwned.Any(item => item.so_Spot == _soSpot);
    }

    public bool OwnsAllPropertiesOfColor(eSpotColor color)
    {
        var allPropertiesOfColor = Board.Instance.spots
            .Where(spot => spot.so_Spot.spotType == eSpotType.property && spot.so_Spot.spotColor == color)
            .Select(spot => spot.so_Spot);

        return allPropertiesOfColor.All(property =>
            listPropertiesOwned.Any(owned => owned.so_Spot == property));
    }

    // Returns the number of railroads owned by the player
    public int CountRailroadsOwned()
    {
        int count = listPropertiesOwned.Count(p => p.so_Spot.spotType == eSpotType.railRoad);
        Debug.Log($"Railroads owned: {count}");
        return count;
    }

    // Returns the number of utilities owned by the player
    public int CountUtilitiesOwned()
    {
        int count = listPropertiesOwned.Count(p => p.so_Spot.spotType == eSpotType.utility);
        Debug.Log($"Utilities owned: {count}");
        return count;
    }

    // Returns the number of houses on a property
    public int GetHouseCount(soSpot _soSpot)
    {
        // Find the property and return the house count
        var property = listPropertiesOwned.Find(p => p.so_Spot == _soSpot);
        return property != null ? property.houseAmt : 0;
    }

    // Returns the number of hotels on a property
    public int GetHotelCount(soSpot _soSpot)
    {
        var property = listPropertiesOwned.Find(p => p.so_Spot == _soSpot);
        return property?.hotelAmt ?? 0;
    }

    public void BuildBunker(soSpot _soSpot)
    {
        var property = listPropertiesOwned.Find(p => p.so_Spot == _soSpot);
        if (property != null && property.houseAmt < 4 && property.hotelAmt == 0)
        {
            property.houseAmt++;
            Debug.Log($"Built a Bunker on {property.spotName}. Total Bunkers: {property.houseAmt}");
        }
        else
        {
            ErrorLogger.Instance.LogWarning($"Cannot build a Bunker on {property.spotName}. Max Bunkers reached or has a Fortress.");
        }
    }

    public void BuildFortress(soSpot _soSpot)
    {
        var property = listPropertiesOwned.Find(p => p.so_Spot == _soSpot);
        if (property != null && property.houseAmt == 4 && property.hotelAmt == 0)
        {
            property.houseAmt = 0;
            property.hotelAmt = 1;
            Debug.Log($"Built a Station on {property.spotName}.");
        }
        else
        {
            ErrorLogger.Instance.LogWarning($"Cannot build a Station on {property.spotName}. Requires 4 Outposts.");
        }
    }

    public void MortgageProperty(soSpot property)
    {
        var ownedProperty = listPropertiesOwned.FirstOrDefault(p => p.so_Spot == property);
        if (ownedProperty != null && !ownedProperty.isMortgaged)
        {
            ownedProperty.isMortgaged = true;
            PlayerManager.Instance.players[PlayerManager.Instance.curPlayer].AdjustCash(property.MortgageCost);
            wcenterManage?.RefreshCashDisplay(); // Update cash display in WcenterManage
            Debug.Log($"{property.spotName} is now mortgaged. Player received ${property.MortgageCost}.");
        }
        else
        {
            ErrorLogger.Instance.LogWarning($"{property.spotName} is already mortgaged.");
        }
    }

    public void UnmortgageProperty(soSpot property)
    {
        var ownedProperty = listPropertiesOwned.FirstOrDefault(p => p.so_Spot == property);
        if (ownedProperty != null && ownedProperty.isMortgaged)
        {
            int unmortgageCost = Mathf.CeilToInt(property.MortgageCost * 1.1f); // 10% interest
            iPlayer currentPlayer = PlayerManager.Instance.players[PlayerManager.Instance.curPlayer];

            if (currentPlayer.cashOnHand >= unmortgageCost)
            {
                ownedProperty.isMortgaged = false;
                currentPlayer.AdjustCash(-unmortgageCost);
                wcenterManage?.RefreshCashDisplay(); // Update cash display in WcenterManage
                Debug.Log($"{property.spotName} is now unmortgaged. Player paid ${unmortgageCost}.");
            }
            else
            {
                ErrorLogger.Instance.LogError($"Player does not have enough cash to unmortgage {property.spotName}.");
            }
        }
        else
        {
            ErrorLogger.Instance.LogWarning($"{property.spotName} is not mortgaged.");
        }
    }

    public List<soSpot> GetMortgagedProperties()
    {
        return listPropertiesOwned
            .Where(property => property.isMortgaged) // Filter by mortgaged properties
            .Select(property => property.so_Spot)   // Select the soSpot object
            .ToList();
    }

    public bool IsPropertyMortgaged(soSpot _soSpot)
    {
        var property = listPropertiesOwned.FirstOrDefault(p => p.so_Spot == _soSpot);
        return property != null && property.isMortgaged;
    }
}
