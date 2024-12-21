using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Widget (attached to HUD):
/// Output: Displays player information in game related to piece, current player and cash on hand
/// </summary>

public class WplayerInfo : MonoBehaviour
{

    [Header("UI References")]
    public TMP_Text playerNameText; // Text to display the player's name
    public Image playerColorImage; // Image to display the player's color
    public TMP_Text cashText;      // Text to display the player's cash
    [Space]
    [SerializeField] private Image imageToken;
    [SerializeField] private GameObject selectedPlayerPanel;
    [SerializeField] private iPlayer player;

    public void InitWidget(iPlayer _player)
    {
        player = _player;
        ShowWidget();
        SetColor();
        SetPlayerSelect(player.playerIdx == 0); 
    }
    /// <summary>
    /// Updates the player's displayed information (name, color, and cash).
    /// </summary>
    /// <param name="name">The player's name.</param>
    /// <param name="color">The player's color.</param>
    /// <param name="cash">The player's starting cash.</param>
    public void UpdatePlayerInfo(string name, Color color, int cash)
    {
        if (playerNameText != null)
            playerNameText.text = name;

        if (playerColorImage != null)
            playerColorImage.color = color;

        if (cashText != null)
            cashText.text = $"${cash}";
    }

    /// <summary>
    /// Updates the cash display separately.
    /// </summary>
    /// <param name="cash">The player's current cash.</param>
    public void UpdateCashDisplay(int cash)
    {
        if (cashText != null)
            cashText.text = $"${cash}";
    }
    void SetColor()
    {
        imageToken.color = player.playerColor; 
    }

    void ShowWidget()
    {
        imageToken.sprite = player.so_PlayerToken.playerTokenArt;
        cashText.text = "$" + player.cashOnHand;
    }
    public void SetPlayerSelect (bool _active)
    {
        selectedPlayerPanel.SetActive(_active);
    }
}
