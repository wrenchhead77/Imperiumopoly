using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WpropItem : MonoBehaviour
{
    [SerializeField] private TMP_Text propertyNameText;
    [SerializeField] private TMP_Text ownerText;
    [SerializeField] private Image propertyColorImage;

    private soSpot so_Spot;

    public void InitPropertyItem(soSpot spot, Player owner)
    {
        so_Spot = spot;

        // Set UI elements
        propertyNameText.text = so_Spot.spotName;

        // Show the color of the property (or hide if not applicable)
        propertyColorImage.color = GetPropertyColor(so_Spot.spotColor);
        propertyColorImage.gameObject.SetActive(so_Spot.spotColor != eSpotColor.none);

        // Display owner info
        ownerText.text = owner != null ? $"{owner.playerName}" : "Unowned";
    }

    private Color GetPropertyColor(eSpotColor spotColor)
    {
        switch (spotColor)
        {
            case eSpotColor.Purple: return new Color(0.5f, 0.0f, 0.5f);
            case eSpotColor.LightBlue: return new Color(0.678f, 0.847f, 0.902f);
            case eSpotColor.Pink: return new Color(1.0f, 0.753f, 0.796f);
            case eSpotColor.Orange: return new Color(1f, 0.65f, 0f); // Approx. orange
            case eSpotColor.Red: return Color.red;
            case eSpotColor.Yellow: return Color.yellow;
            case eSpotColor.Green: return Color.green;
            case eSpotColor.DarkBlue: return Color.blue;
            case eSpotColor.White: return Color.white;
            case eSpotColor.Gray: return Color.gray;
            default: return Color.clear;
        }
    }
}
