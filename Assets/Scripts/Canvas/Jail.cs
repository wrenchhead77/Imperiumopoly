using TMPro;
using UnityEngine;

public class Jail : MonoBehaviour
{
    public GameObject buttonRollDice;
    public GameObject buttonEndTurn;
    public GameObject buttonJailCard;
    public GameObject buttonPayFine; // Ensure this is assigned in the editor
    private iPlayer player;

    public TMP_Text jailMessage;

    public void InitCanvas(iPlayer _player)
    {
        player = _player;

        if (player.turnsInJail >= 3)
        {
            // Automatically pay the fine and exit if the player has been in jail for 3 turns
            Debug.Log($"{player.playerName} has been in Jail for 3 turns. Automatically paying $50 fine.");
            OnPayFinePressed();
            return; // No further setup needed, as the player is exiting jail
        }

        jailMessage.text = $"You are in Jail! Turns in Jail: {player.turnsInJail}. Roll doubles to get out, pay $50, or use a card.";

        // Activate the "Use Card" button if the player has a GOJF card
        buttonJailCard.SetActive(player.GOJFCard > 0);
        buttonPayFine.SetActive(true); // Ensure Pay Fine button is active initially
    }

    public void SetDiceButton(bool _active)
    {
        buttonRollDice.SetActive(_active);
        buttonEndTurn.SetActive(!_active);
    }

    public void OnRollDicePressed()
    {
        DiceManager dm = DiceManager.Instance;
        int roll = dm.RollDice();

        if (PersistentGameData.Instance.doublesRolled)
        {
            Debug.Log("Rolled doubles! Player gets out of Jail.");
            ExitJail(roll); // Exit and move the player
        }
        else
        {
            // Disable Pay Fine and Use Card buttons after rolling
            buttonPayFine.SetActive(false);
            buttonJailCard.SetActive(false);

            player.turnsInJail++;
            Debug.Log("No doubles. Stay in Jail.");
            jailMessage.text = $"No doubles. You are still in Jail. Turns in Jail: {player.turnsInJail}.";
            SetDiceButton(false); // Hide dice button for this turn
        }
    }

    public void OnEndTurnPressed()
    {
        PlayerManager.Instance.AdvancePlayer();
        DiceManager.Instance.ResetDice(); // Reset dice state for the next player
        Destroy(this.gameObject);
        Hud.Instance.ShowHud();
    }

    public void OnPayFinePressed()
    {
        Debug.Log("Player paid $50 to get out of Jail.");
        player.AdjustCash(-50);
        ExitJail(); // Exit without moving (dice roll will handle movement)
    }

    public void OnUseCardPressed()
    {
        if (player.GOJFCard > 0)
        {
            Debug.Log($"{player.playerName} used a Get Out of Jail Free card.");
            player.GOJFCard--; // Decrement the GOJF card count
            ExitJail(); // Exit without moving (dice roll will handle movement)

            // Optionally, return the card to the deck
            CardManager.Instance.UseGetOutOfJailFreeCard(player, eDeckType.CommChest);
        }
        else
        {
            ErrorLogger.Instance.LogWarning("No Get Out of Jail Free cards available.");
        }
    }

    private void ExitJail(int diceRoll = 0)
    {
        player.isInJail = false;
        player.turnsInJail = 0;

        if (diceRoll > 0)
        {
            Debug.Log($"{player.playerName} is moving {diceRoll} spaces after exiting Jail.");
            player.MovePlayer(diceRoll); // Move the player
            PersistentGameData.Instance.doublesRolled = false;
            PersistentGameData.Instance.doublesCount = 0;
        }
        Destroy(this.gameObject);
        Hud.Instance.ShowHud();
    }
}
