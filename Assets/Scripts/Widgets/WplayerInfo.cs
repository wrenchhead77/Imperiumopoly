using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Widget (attached to HUD):
/// Output: Displays player information in game related to piece, current player and cash on hand
/// </summary>

public class WplayerInfo : MonoBehaviour
{

    [Tooltip("Drag the objects that need to change to player color here")]
    [Space]
    [SerializeField] private Image imageToken;
    public TMP_Text textCash;
    [SerializeField] private GameObject selectedPlayerPanel;
    [SerializeField] private Player player;

    public void InitWidget(Player _player)
    {
        player = _player;
        ShowWidget();
        SetColor();
        SetPlayerSelect(player.playerIdx == 0); 
    }

    void SetColor()
    {
        imageToken.color = player.playerColor; 
    }

    void ShowWidget()
    {
        imageToken.sprite = player.so_PlayerToken.playerTokenArt;
        textCash.text = "$" + player.cashOnHand;
    }
    public void SetPlayerSelect (bool _active)
    {
        selectedPlayerPanel.SetActive(_active);
    }
    public void UpdateCashDisplay(int cash)
    {
        textCash.text = $"${cash}";
    }
}
