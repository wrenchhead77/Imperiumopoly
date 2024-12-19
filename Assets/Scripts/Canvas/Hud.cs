using System;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Canvas: Input: Roll dice, options, etc
/// Output: spawns wPlayerInfo
/// Holds Wcenter widgets for purchase, rent, etc
/// </summary>

public class Hud : MonoBehaviour
{
    public RectTransform grp_activePlayers;
    private soPlayerType so_playerType;
    private soSpot _soSpot;
    public GameObject grp_bottom;
    public GameObject buttonRollDice;
    public GameObject buttonEndTurn;
    public bool isHudTemporarilyHidden = false;
    public iPlayer player;

    PlayerManager pm = PlayerManager.Instance;
    DiceManager dm = DiceManager.Instance;
    CanvasManager cm = CanvasManager.Instance;
    BankManager bm = BankManager.Instance;

    public static Hud Instance;
    private void Awake()
    {
        Instance = this;
        InitCanvas();
        SetRollDiceButton(buttonRollDice);
    }

    public void InitCanvas()
    {
        for (int i = 0; i < PlayerManager.maxPlayers; i++)
        {
            if (pm.players[i].so_PlayerType.playerType != ePlayerType.none)
            {
                GameObject obj = Instantiate(Resources.Load("Widgets/WplayerInfo") as GameObject, grp_activePlayers);
                WplayerInfo playerInfo = obj.GetComponent<WplayerInfo>();
                playerInfo.InitWidget(pm.players[i]);
                pm.players[i].wPlayerInfo = playerInfo; // Link WplayerInfo to the Player
                pm.players[i].UpdateCashDisplay();
            }
        }
        player = pm.players[pm.curPlayer];
    }
    public void SetTopActive(bool _active)
    {
        grp_activePlayers.gameObject.SetActive(_active);
    }
    public void SetBottomActive(bool _active)
    {
        grp_bottom.SetActive(_active);
    }

    public void HideHud()
    {
        isHudTemporarilyHidden = true;
        SetTopActive(false);
        SetBottomActive(false);
    }

    public void ShowHud()
    {
        isHudTemporarilyHidden = false;
        SetTopActive(true);
        SetBottomActive(true);
    }

    public void SetRollDiceButton(bool _active)
    {
        buttonRollDice.SetActive(_active);
        buttonEndTurn.SetActive(!_active);
    }

    public void OnRollDicePressed()
    {
        GameObject canvasDiceRoll = Instantiate(Resources.Load("Canvas/CanvasRollDice") as GameObject);
        WcenterDice diceWidget = canvasDiceRoll.GetComponent<WcenterDice>();
        diceWidget.InitWidget(pm.players[pm.curPlayer]);
    }

    public void OnEndTurnPressed()
    {
        pm.AdvancePlayer();

    }
    public void HandleSpot(iPlayer player, soSpot _soSpot)
    {
        iPlayer curPlayer = pm.players[pm.curPlayer];
        iPlayer owner = pm.WhoOwnsProperty(_soSpot);

        switch (_soSpot.spotType)
        {
            case eSpotType.property:
                 if (owner != curPlayer)
                {
                    if (owner == null)
                    {
                        cm.showCanvasPurchase(_soSpot);
                    }
                    else
                    {
                        int rent = bm.CalculateRent(curPlayer, _soSpot);
                        Debug.Log($"Pay rent to {owner.playerName}, Rent: {rent}");
                        cm.showCanvasRent(_soSpot);
                    }
                }
                else
                {
                    Debug.Log($"Player {curPlayer.playerName} owns {_soSpot.spotName}. No rent popup needed.");
                }
                break;
            case eSpotType.tax:
                Debug.Log($"Pay tax for landing on {_soSpot.spotName}");
                cm.showCanvasTax(_soSpot);
                break;

            case eSpotType.utility:
                if (owner != curPlayer)
                {
                    if (owner == null)
                    {
                        cm.showCanvasPurchase(_soSpot);
                    }
                    else
                    {
                        int rent = bm.CalculateRent(curPlayer, _soSpot);
                        Debug.Log($"Pay rent to {owner.playerName}, Rent: {rent}");
                        cm.showCanvasRent(_soSpot);
                    }
                }
                else
                {
                    Debug.Log($"Player {curPlayer.playerName} owns {_soSpot.spotName}. No rent popup needed.");
                }
                break;
            case eSpotType.railRoad:
                if (owner != curPlayer)
                {
                    if (owner == null)
                    {
                        cm.showCanvasPurchase(_soSpot);
                    }
                    else
                    {
                        int rent = bm.CalculateRent(curPlayer, _soSpot);
                        Debug.Log($"Pay rent to {owner.playerName}, Rent: {rent}");
                        cm.showCanvasRent(_soSpot);
                    }
                }
                else
                {
                    Debug.Log($"Player {curPlayer.playerName} owns {_soSpot.spotName}. No rent popup needed.");
                }
                break;

            default:
                Debug.Log($"Unhandled spot type: {_soSpot.spotType} for {_soSpot.spotName}");
                break;
        }
    }


    public void GoToJail()
    {
        Debug.Log("Go directly to Jail!");

        // Get the current player
        iPlayer curPlayer = pm.players[pm.curPlayer];

        // Retrieve the soSpot for the Jail position
        soSpot jailSpot = Board.Instance.spots[(int)ePos.AwaitingOrders].so_Spot;

        // Show Jail popup
        GameObject canvasJail = Instantiate(Resources.Load("Canvas/CanvasSentJail") as GameObject);
        WcenterJail jailWidget = canvasJail.GetComponent<WcenterJail>();
        jailWidget.InitWidget(curPlayer, jailSpot);

        Debug.Log($"{curPlayer.playerName} is being sent to Jail.");
    }
    public void HandleSafeHarborSpot()
    {
        iPlayer curPlayer = pm.players[pm.curPlayer];
        int potAmount = bm.ClaimSafeHarborPot();
        if (potAmount > 0)
        {
            Debug.Log($"Player {curPlayer.playerName} landed on Free Docking and collected ${potAmount}.");
            curPlayer.AdjustCash(potAmount);
            cm.showCanvasSafe(potAmount);
        }
        else
        {
            Debug.Log($"Player {curPlayer.playerName} landed on Free Docking, but the pot is empty.");
        }
    }
    public void ShowGoPopup(iPlayer player)
    {
        GameObject popupObj = Instantiate(Resources.Load("Canvas/CanvasMessage") as GameObject);
        Message messageComponent = popupObj.GetComponent<Message>();

        Popup popup = new Popup(
            _sender: this.gameObject,           // Set the sender to this gameObject
            _confirmAction: "CollectGoReward",
            _cancelAction: null, // No cancel action needed
            _title: "Passed Go",
            _message: "You've passed Go! Click Confirm to collect $200.",
            _confirmText: "Confirm",
            _cancelText: null // No cancel button
        );

        messageComponent.InitCanvas(popup);
    }

    // This method will handle the reward logic
    public void CollectGoReward(string actionName)
    {
        iPlayer curPlayer = PlayerManager.Instance.players[PlayerManager.Instance.curPlayer];
        curPlayer.AdjustCash(200);
        Debug.Log($"{curPlayer.playerName} collected $200 for passing Go!");
    }
    public void OnPauseClicked()
    {
        cm.showCanvasPause();
    }

    public void OnManageClicked()
    {
        iPlayer curPlayer = pm.players[pm.curPlayer];
        cm.showCanvasManage(curPlayer);
    }
}
