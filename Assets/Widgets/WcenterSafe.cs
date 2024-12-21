using TMPro;
using UnityEngine;

public class WcenterSafe : Wcenter
{

    public void InitWidget(int _potAmount)
    {
        hud.HideHud();
        message.text = $"You landed on Free Docking! Collect ${_potAmount} from the pot!";
    }

        public void OnConfirmClicked()
    {
        hud.ShowHud();
        Destroy(); // Close the Safe Harbor popup
    }
}
