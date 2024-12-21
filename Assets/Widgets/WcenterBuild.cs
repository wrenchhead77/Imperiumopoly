using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WcenterBuild : Wcenter
{
    [SerializeField] private TMP_Text BunkersCount;
    [SerializeField] private TMP_Text FortresssCount;
    [SerializeField] private Button buildBunkerButton;
    [SerializeField] private Button buildFortressButton;

    public PropertyOwnership selectedProperty;

    public void InitWidget(PropertyOwnership property)
    {
        selectedProperty = property;  // Make sure the property is passed here
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Make sure the text fields are updated with the property data
        BunkersCount.text = $"Bunkers: {selectedProperty.houseAmt}";
        FortresssCount.text = $"Fortresss: {selectedProperty.hotelAmt}";

        // Check the current build conditions
        buildBunkerButton.interactable = selectedProperty.houseAmt < 4 && selectedProperty.hotelAmt == 0;
        buildFortressButton.interactable = selectedProperty.houseAmt == 4 && selectedProperty.hotelAmt == 0;
    }

    public void OnBuildBunkerPressed()
    {
        // Ensure the player can build the Bunker before proceeding
        if (selectedProperty.houseAmt < 4 && selectedProperty.hotelAmt == 0)
        {
            pm.players[pm.curPlayer].propertyManager.BuildBunker(selectedProperty.so_Spot);
            UpdateUI();  // Update the UI after the building action
        }
        else
        {
            Debug.LogWarning("Cannot build Bunker: Max Bunkers reached or a Fortress is present.");
        }
    }

    public void OnBuildFortressPressed()
    {
        // Ensure the player has 4 Bunkers before building a Fortress
        if (selectedProperty.houseAmt == 4 && selectedProperty.hotelAmt == 0)
        {
            pm.players[pm.curPlayer].propertyManager.BuildFortress(selectedProperty.so_Spot);
            UpdateUI();  // Update the UI after the building action
        }
        else
        {
            Debug.LogWarning("Cannot build Fortress: 4 Bunkers are required.");
        }
    }

    public void OnBackPressed()
    {
        Destroy();  // Only destroy this Canvas, no need to show HUD again
    }
}
