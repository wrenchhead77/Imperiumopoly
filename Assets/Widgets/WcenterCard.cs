using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WcenterCard : Wcenter
{
    [SerializeField] private TMP_Text cardType;
    [SerializeField] private Image cardImage;
    [SerializeField] private Button confirmButton;

    private System.Action onCardConfirmed;
    private bool isActionExecuted = false; // Prevent multiple confirmations

    public void InitWidget(soCard cardData, System.Action onCardConfirmed)
    {
        cardType.text = cardData.cardType.ToString();
        cardImage.sprite = cardData.cardImage;

        this.onCardConfirmed = onCardConfirmed;

        confirmButton.onClick.RemoveAllListeners(); // Ensure no duplicate listeners
        confirmButton.onClick.AddListener(OnConfirmPressed);
    }

    public void OnConfirmPressed()
    {
        if (isActionExecuted) return; // Skip if already executed

        Debug.Log("Card confirmed.");
        isActionExecuted = true; // Mark as executed
        onCardConfirmed?.Invoke(); // Trigger the action
        Destroy(); // Clean up card UI
    }
}
