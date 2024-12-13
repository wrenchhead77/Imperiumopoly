using UnityEngine;
using UnityEngine.UI;

public class WcenterRent : Wcenter
{
    [SerializeField] private Button confirmButton; // Button for paying rent
    [SerializeField] private Button continueButton; // Button for no-rent scenarios
    [SerializeField] private Button battleButton; // Button for initiating battles
    private Player attacker;
    private Player defender;
    private int dockingFee;

    public override void InitWidget(soSpot _soSpot)
    {
        base.InitWidget(_soSpot);
        hud.HideHud();

        dockingFee = bm.CalculateRent(_soSpot);
        defender = pm.WhoOwnsProperty(_soSpot);
        attacker = pm.players[pm.curPlayer];

        // Initial button states
        confirmButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        battleButton.gameObject.SetActive(false);

        if (defender == null || dockingFee <= 0)
        {
            // If no rent is due or no owner
            HandleNoRentScenario(_soSpot);
            return;
        }

        // Check if the property is mortgaged
        bool isMortgaged = defender.propertyManager.IsPropertyMortgaged(_soSpot);
        if (isMortgaged)
        {
            HandleMortgagedProperty(_soSpot);
            return;
        }

        // Check if the defender belongs to a different faction
        bool isDifferentFaction = attacker.playerColor != defender.playerColor;

        // Display rent and battle options
        property.sprite = _soSpot.spotArtFront;
        message.text = $"Pay Rent of ${dockingFee} for landing on {defender.playerName}'s {_soSpot.spotName}.";
        confirmButton.gameObject.SetActive(true);

        if (isDifferentFaction && attacker.cashOnHand >= dockingFee * 2)
        {
            battleButton.gameObject.SetActive(true);
        }
    }

    private void HandleMortgagedProperty(soSpot _soSpot)
    {
        property.sprite = _soSpot.spotArtBack;
        message.text = $"Docking fees are waived as {_soSpot.spotName} is mortgaged.";
        continueButton.gameObject.SetActive(true);
    }

    private void HandleNoRentScenario(soSpot _soSpot)
    {
        property.sprite = _soSpot.spotArtFront;
        message.text = $"No rent is due for {_soSpot.spotName}.";
        continueButton.gameObject.SetActive(true);
    }

    public void OnPayRentPressed()
    {
        if (defender != null && defender != attacker)
        {
            bm.TransferFunds(attacker, defender, dockingFee);
            Debug.Log($"Player {attacker.playerName} paid ${dockingFee} to {defender.playerName}.");
        }

        hud.ShowHud();
        Destroy();
    }

    public void OnBattlePressed()
    {
        Destroy();
        cm.ShowCanvasBattle(attacker, defender, dockingFee);
    }

    public void OnContinuePressed()
    {
        hud.ShowHud();
        Destroy();
    }
}
