using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Wsetup : MonoBehaviour
{
    [SerializeField] private Image[] buttonsChangeColor; // UI elements reflecting player color
    [SerializeField] private Button colorButton1;
    [SerializeField] private Button colorButton2;
    [SerializeField] private Button colorButton3;
    [SerializeField] private Button colorButton4;
    [SerializeField] private Image imageToken;           // Player token icon
    [SerializeField] private TMP_Text textToken;         // Player token name
    [SerializeField] private Image imageType;
    [SerializeField] private TMP_Text textType;
    [SerializeField] private iPlayer player;             // Linked iPlayer instance
    [SerializeField] private TMP_Text playerName;

    public void InitWidget(iPlayer _player)
    {
        player = _player;
        UpdatePlayerUI();    // Populate UI with player data
        SetupColorButtons(); // Initialize color buttons
    }

    private void UpdatePlayerUI()
    {
        soPlayerToken token = player.so_PlayerToken;

        imageToken.sprite = token.playerTokenArt;
        textToken.text = token.playerTokenName;

        playerName.text = player.playerName; // Ensure player name is displayed correctly
        SetColor();
    }

    private void SetColor()
    {
        foreach (Image i in buttonsChangeColor)
        {
            i.color = player.playerColor;
        }
    }

    private void SetupColorButtons()
    {
        Button[] colorButtons = { colorButton1, colorButton2, colorButton3, colorButton4 };

        for (int i = 0; i < colorButtons.Length; i++)
        {
            if (i < GameMgr.Instance.so_Ref.playerColors.Length)
            {
                Color color = GameMgr.Instance.so_Ref.playerColors[i];
                colorButtons[i].GetComponent<Image>().color = color; // Set button color
                int index = i; // Capture index for closure
                colorButtons[i].onClick.RemoveAllListeners();
                colorButtons[i].onClick.AddListener(() => OnColorSelected(color));
                colorButtons[i].gameObject.SetActive(true); // Show button
            }
            else
            {
                colorButtons[i].gameObject.SetActive(false); // Hide extra buttons
            }
        }
    }

    private void OnColorSelected(Color selectedColor)
    {
        player.playerColor = selectedColor;
        SetColor(); // Update UI elements to reflect the color
        Debug.Log($"Player color updated to: {selectedColor}");
    }

    public void OnPieceClicked()
    {
        player.IncrementPlayerPiece();
        UpdatePlayerUI();

        if (PlayerManager.Instance.CheckForDupeTokens(player.playerIdx))
        {
            Debug.LogWarning($"Duplicate token detected for Player {player.playerName}. Cycling again.");
            OnPieceClicked();
        }
    }

    /* public void OnTypeClicked()
    {
        player.IncrementPlayerType();
        UpdatePlayerUI();
        GetComponentInParent<Setup>().CheckForPlayButton();

        if (PlayerManager.Instance.CheckForDupeTokens(player.playerIdx))
        {
            Debug.LogWarning($"Duplicate token detected for Player {player.playerName}. Cycling again.");
            OnTypeClicked();
        }
    } */
}
