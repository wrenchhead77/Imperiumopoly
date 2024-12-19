using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Wsetup : MonoBehaviour
{
    [SerializeField] private soRef _soRef;             // Reference to the ScriptableObject
    [SerializeField] private GameObject button_AddPlayer;
    [SerializeField] private GameObject grp_PlayerSetup;
    [SerializeField] private Image[] buttonsChangeColor; // UI elements that reflect player color
    [SerializeField] private Button colorButton1;     // Button for color 1
    [SerializeField] private Button colorButton2;     // Button for color 2
    [SerializeField] private Button colorButton3;     // Button for color 3
    [SerializeField] private Button colorButton4;     // Button for color 4
    [Space]
    [SerializeField] private Image imageToken;
    [SerializeField] private TMP_Text textToken;
    [SerializeField] private Image imageType;
    [SerializeField] private TMP_Text textType;
    [SerializeField] private iPlayer player;

    public void InitWidget(iPlayer _player)
    {
        player = _player;
        ShowButtons();
        SetColor();
        SetupColorButtons();
    }

    void SetColor()
    {
        foreach (Image i in buttonsChangeColor)
        {
            i.color = player.playerColor;
        }
    }

    void ShowButtons()
    {
        if (player.so_PlayerType == null)
        {
            Debug.LogError($"Player type for Player {player.playerIdx} is null! Check initialization.");
            return;
        }
        soPlayerToken token = player.so_PlayerToken;
        soPlayerType type = player.so_PlayerType;

        if (type == GameMgr.Instance.so_Ref.playerTypes[(int)ePlayerType.none])
        {
            button_AddPlayer.SetActive(true); // Show the "Add Player" button
            grp_PlayerSetup.SetActive(false); // Hide the Token/Type/Color buttons
        }
        else
        {
            button_AddPlayer.SetActive(false); // Hide the "Add Player" button
            grp_PlayerSetup.SetActive(true); // Show the Token/Type/Color buttons
        }

        imageToken.sprite = token.playerTokenArt;
        textToken.text = token.playerTokenName;
        imageType.sprite = type.playerTypeArt;
        textType.text = type.playerTypeName;
    }

    private void SetupColorButtons()
    {
        // Ensure the number of buttons matches the colors
        Button[] colorButtons = { colorButton1, colorButton2, colorButton3, colorButton4 };

        for (int i = 0; i < colorButtons.Length; i++)
        {
            if (i < _soRef.playerColors.Length)
            {
                Color color = _soRef.playerColors[i];
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
        // Update the player's color
        player.playerColor = selectedColor;
        SetColor(); // Update any UI elements that reflect player color
        Debug.Log($"Player color updated to: {selectedColor}");
    }

    public void OnPieceClicked()
    {
        player.IncrementPlayerPiece();
        ShowButtons();
        if (PlayerManager.Instance.CheckForDupeTokens(player.playerIdx))
        {
            OnPieceClicked();
        }
    }

    public void OnTypeClicked()
    {
        player.IncrementPlayerType();
        ShowButtons();
        GetComponentInParent<Setup>().CheckForPlayButton();
        if (PlayerManager.Instance.CheckForDupeTokens(player.playerIdx))
        {
            OnTypeClicked();
        }
    }
}
