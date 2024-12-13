using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
///  Widget (inherits from Wcenter):
///  Input: Handles purchases when landing on properties
///  output: shows message specific to purchases
/// </summary>

public class WcenterPurchase : Wcenter
{
    public Transform relatedPropertiesContainer; // Parent object for related properties
    public GameObject propertyItemPrefab; // Prefab to represent a single related property
    private Message _message;

    public override void InitWidget(soSpot _soSpot)
    {
        base.InitWidget(_soSpot);
        hud.HideHud();
        // Set the message and property image
        message.text = "Purchase " + _soSpot.spotName + " for $" + _soSpot.price + "?";
        property.sprite = _soSpot.spotArtFront;
        // Get all properties of the same color/type
        List<soSpot> relatedProperties = Board.Instance.GetSpotsOfSameColorOrType(_soSpot);
        // Populate the UI with related properties that are owned by others
        PopulateRelatedPropertiesUI(relatedProperties);
    }
    private List<soSpot> GetRelatedProperties()
    {
        // Validate Board instance
        if (Board.Instance == null || Board.Instance.spots == null)
        {
            Debug.LogError("Board instance or spots array is not initialized.");
            return new List<soSpot>();
        }

        // Extract spots and find related properties
        List<soSpot> relatedProperties = new List<soSpot>();
        foreach (Spot spot in Board.Instance.spots)
        {
            if (spot.so_Spot != null &&
                (spot.so_Spot.spotType == so_Spot.spotType ||
                 (so_Spot.spotType == eSpotType.property && spot.so_Spot.spotColor == so_Spot.spotColor)))
            {
                relatedProperties.Add(spot.so_Spot);
            }
        }

        return relatedProperties;
    }
    private void PopulateRelatedPropertiesUI(List<soSpot> relatedProperties)
    {
        // Clear any existing items
        foreach (Transform child in relatedPropertiesContainer)
        {
            Destroy(child.gameObject);
        }

        // Populate related properties only if they are owned by another player
        foreach (soSpot relatedSpot in relatedProperties)
        {
            Player owner = PlayerManager.Instance.WhoOwnsProperty(relatedSpot);

            if (owner != null && owner != pm.players[pm.curPlayer]) // Exclude unowned and current player's properties
            {
                GameObject item = Instantiate(propertyItemPrefab, relatedPropertiesContainer);
                item.GetComponent<WpropItem>().InitPropertyItem(relatedSpot, owner);
            }
        }
    }

    public void OnBuyPressed()
    {
        Popup popup = new Popup(
            _sender: this.gameObject,           // Set the sender to this gameObject
            _confirmAction: nameof(OnBuyConfirmed), // Confirmation method to be called
            _cancelAction: null,                // No specific action needed on cancel
            _title: "Confirm Purchase",
            _message: $"Are you sure you want to purchase {so_Spot.spotName} for ${so_Spot.price}?",
            _confirmText: "Yes",
            _cancelText: "No"
        );

        // Instantiate the CanvasMessage prefab
        GameObject obj = Instantiate(Resources.Load("Canvas/InGame/CanvasMessage") as GameObject);
        Message message = obj.GetComponent<Message>();
        message.InitCanvas(popup);
    }
    public void OnBuyConfirmed()
    {
        if (player.cashOnHand >= so_Spot.price)
        {
            // Deduct price from cash and add property to the ownership list
            player.cashOnHand -= so_Spot.price;
            player.propertyManager.BuyProperty(so_Spot);

            Debug.Log($"Player {player.playerName} bought {so_Spot.spotName} for ${so_Spot.price}");
        }
        else
        {
            Debug.LogError("Player does not have enough cash to purchase this property.");
        }
        hud.ShowHud();
        // Close the purchase prompt
        Destroy();
    }
}
