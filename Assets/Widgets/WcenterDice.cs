using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WcenterDice : Wcenter
{
    [SerializeField] private Image dice1Image; // Image for dice 1
    [SerializeField] private Image dice2Image; // Image for dice 2
    [SerializeField] private Button moveButton; // Button to confirm and move

    private iPlayer curPlayer;
    private bool rollCompleted = false;

    public void InitWidget(iPlayer player)
    {
        Hud.Instance.HideHud();
        curPlayer = player;

        ResetUI();
        OnRollDicePressed();
    }

    private void ResetUI()
    {
        dice1Image.sprite = null;
        dice2Image.sprite = null;
        message.text = "Roll the dice!";
        moveButton.gameObject.SetActive(false);
        rollCompleted = false;
    }

    public void OnRollDicePressed()
    {
        if (curPlayer.isInJail)
        {
            cm.showCanvasJail(curPlayer);
            Destroy(); // Close this canvas
            return;
        }

        int roll = DiceManager.Instance.RollDice();

        if (roll == -99)
        {
            // Player rolled triples and goes to jail
            message.text = "You rolled triples! Go to jail.";

            // Retrieve the soSpot for the Jail position
            soSpot jailSpot = Board.Instance.spots[(int)ePos.AwaitingOrders].so_Spot;
            // Show Jail popup
            GameObject canvasJail = Instantiate(Resources.Load("Canvas/CanvasSentJail") as GameObject);
            WcenterJail jailWidget = canvasJail.GetComponent<WcenterJail>();
            jailWidget.InitWidget(curPlayer, jailSpot);

            Destroy(); // Close the dice roll canvas
            return;
        }

        // Fetch dice results
        int dice1 = PersistentGameData.Instance.lastDice1;
        int dice2 = PersistentGameData.Instance.lastDice2;
        Sprite dice1Sprite = DiceManager.Instance.GetDiceFaceSprite((eDiceNumber)(dice1 - 1));
        Sprite dice2Sprite = DiceManager.Instance.GetDiceFaceSprite((eDiceNumber)(dice2 - 1));

        if (dice1Sprite == null || dice2Sprite == null)
        {
            Debug.LogError($"Failed to fetch dice sprites for results: {dice1}, {dice2}");
            return;
        }

        // Update UI
        dice1Image.sprite = dice1Sprite;
        dice2Image.sprite = dice2Sprite;
        message.text = $"You rolled {dice1} and {dice2}, total: {roll}";

        moveButton.gameObject.SetActive(true); // Enable move button
        rollCompleted = true;
    }

    public void OnMovePressed()
    {
        if (!rollCompleted) return;
        int roll = PersistentGameData.Instance.lastDiceRoll;
        curPlayer.MovePlayer(roll);
        CameraManager.Instance.SetCurrentCamera(eCameraPositions.center, curPlayer.playerPiece.transform);
        Destroy(gameObject);
        Hud.Instance.ShowHud();
        Hud.Instance.SetRollDiceButton(PersistentGameData.Instance.doublesRolled);
    }
}
