using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WcenterJail : Wcenter
{
    private ePos newPosition = ePos.AwaitingOrders; // Position for Jail

    [SerializeField] private Button confirmButton; // Confirm button

    public void InitWidget(iPlayer player, soSpot jailSpot)
    {
        this.player = player;

        // Assign the jail spot data
        this.so_Spot = jailSpot;

        property.sprite = so_Spot.spotArtBack; // Use the back art of the Jail spot
        message.text = $"{player.playerName} has been sent to the Black Ships!";

        // Set up the confirm button listener
        confirmButton.onClick.AddListener(OnConfirmPressed);
    }

    private void OnConfirmPressed()
    {
        // Update player state
        player.pos = newPosition; // Move to Jail position
        player.isInJail = true; // Mark as in Jail
        player.turnsInJail = 0; // Reset Jail turn count

        // Destroy the Jail popup canvas
        Destroy();

        // End the turn
        PlayerManager.Instance.AdvancePlayer();
    }
}
