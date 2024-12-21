using UnityEngine;
using TMPro;

/// <summary>
///  Widget (inherits from Wcenter):
///  Input: Handles paying Tax when landing on tax spots
///  output: shows message specific to paying Tax
/// </summary>

public class WcenterTax : Wcenter
{
    private Message _message;
    public override void InitWidget(soSpot _soSpot)
    {
        base.InitWidget(_soSpot);
        hud.HideHud();
        // Set the message and property image
        message.text = "Pay Tax of $ " + _soSpot.tax + " for landing on " + _soSpot.spotName + ".";
        property.sprite = _soSpot.spotArtFront;
    }

    public void OnPayTaxPressed()
    {
        Debug.Log("Pay Tax Pressed");
        player.AdjustCash(-so_Spot.tax);
        bm.AddToSafeHarbor(so_Spot.tax);
        hud.ShowHud();
        Destroy();
    }
}
