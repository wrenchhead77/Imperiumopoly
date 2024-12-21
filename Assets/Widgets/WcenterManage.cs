using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WcenterManage : Wcenter
{
    [SerializeField] private RectTransform propertyListParent; // Parent for property list items
    [SerializeField] private GameObject propertyItemPrefab;    // Prefab for individual property items
    [SerializeField] private Image propertyCardImage;          // Image to display the selected property card
    [SerializeField] private Button buildButton;               // Button for building on the selected property
    [SerializeField] private Button mortgageButton;            // Button for mortgaging the selected property
    [SerializeField] private Button unmortgageButton;          // Button for unmortgaging the selected property
    [SerializeField] private TMP_Text cashOnHand;              // Text field for displaying player's cash
    [SerializeField] private TMP_Text gojfText;                // Text field for displaying Get Out of Jail Free card status

    private PropertyOwnership curProperty;                     // Currently selected property
    private iPlayer curPlayer;                                  // Reference to the current player

    public void InitWidget(iPlayer player, WcenterManage wcenterManage)
    {
        hud.HideHud();
        curPlayer = player;
        curPlayer.propertyManager.SetManageReference(wcenterManage); // Set the reference
        PopulatePropertyList();
        cashOnHand.text = $"Cash: ${player.cashOnHand}";
        gojfText.text = curPlayer.GOJFCard > 0 ? $"Get Out of Jail Free Card: {curPlayer.GOJFCard}" : "Get Out of Jail Free Card: No";

        buildButton.gameObject.SetActive(false);
        mortgageButton.gameObject.SetActive(false);
        unmortgageButton.gameObject.SetActive(false);
        propertyCardImage.gameObject.SetActive(false);
    }

    private void PopulatePropertyList()
    {
        foreach (var property in curPlayer.propertyManager.listPropertiesOwned)
        {
            GameObject item = Instantiate(propertyItemPrefab, propertyListParent);
            Wmanage widget = item.GetComponent<Wmanage>();
            widget.Init(property, this);
        }
    }

    public void OnPropertySelected(PropertyOwnership property)
    {
        curProperty = property;
        propertyCardImage.gameObject.SetActive(true);
        propertyCardImage.sprite = property.isMortgaged ? property.so_Spot.spotArtBack : property.so_Spot.spotArtFront;
        bool canBuild = property.so_Spot.spotType == eSpotType.property &&
                        curPlayer.propertyManager.OwnsAllPropertiesOfColor(property.so_Spot.spotColor);
        buildButton.gameObject.SetActive(canBuild);
        bool isMortgaged = property.isMortgaged;
        mortgageButton.gameObject.SetActive(!isMortgaged);
        unmortgageButton.gameObject.SetActive(isMortgaged);
    }

    public void OnMortgagePressed()
    {
        if (curProperty != null && !curProperty.isMortgaged)
        {
            curPlayer.propertyManager.MortgageProperty(curProperty.so_Spot);
            curProperty.isMortgaged = true;
            OnPropertySelected(curProperty); // Refresh UI
        }
    }

    public void OnUnmortgagePressed()
    {
        if (curProperty != null && curProperty.isMortgaged)
        {
            curPlayer.propertyManager.UnmortgageProperty(curProperty.so_Spot);
            curProperty.isMortgaged = false;
            OnPropertySelected(curProperty); // Refresh UI
        }
    }

    public void OnBuildPressed()
    {
        GameObject canvasBuild = Instantiate(Resources.Load("Canvas/CanvasBuild") as GameObject);
        WcenterBuild buildWidget = canvasBuild.GetComponent<WcenterBuild>();
        buildWidget.InitWidget(curProperty);
    }

    public void RefreshCashDisplay()
    {
        cashOnHand.text = $"Cash: ${curPlayer.cashOnHand}";
    }
}
