using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CanvasBattle : MonoBehaviour
{
    [SerializeField] private TMP_Text _message; // General message for the battle
    [SerializeField] private TMP_Text _attackerResults; // Text field for attacker's results
    [SerializeField] private TMP_Text _defenderResults; // Text field for defender's results
    [SerializeField] private Button attackerRollButton; // Button for attacker to roll
    [SerializeField] private Button defenderRollButton; // Button for defender to roll

    private Player attacker;
    private Player defender;
    private int dockingFee;
    private int[] attackerRolls = new int[3];
    private int[] defenderRolls = new int[3];
    private bool attackerRolled = false;
    private bool defenderRolled = false;

    public void InitBattle(Player _attacker, Player _defender, int _dockingFee)
    {
        attacker = _attacker;
        defender = _defender;
        dockingFee = _dockingFee;

        _message.text = $"{attacker.playerName} challenges {defender.playerName} in a space battle!";
        _attackerResults.text = ""; // Clear any previous attacker results
        _defenderResults.text = ""; // Clear any previous defender results

        attackerRollButton.onClick.AddListener(() => RollForPlayer(attacker, true));
        defenderRollButton.onClick.AddListener(() => RollForPlayer(defender, false));
    }

    private void RollForPlayer(Player player, bool isAttacker)
    {
        string rollMessage = "";

        for (int i = 0; i < 3; i++)
        {
            int roll1 = Random.Range(1, 7);
            int roll2 = Random.Range(1, 7);
            int total = roll1 + roll2;

            rollMessage += $"Round {i + 1}: {roll1} + {roll2} = {total}\n";

            if (isAttacker)
            {
                attackerRolls[i] = total;
            }
            else
            {
                defenderRolls[i] = total;
            }
        }

        // Update the respective results text
        if (isAttacker)
        {
            _attackerResults.text = rollMessage;
            attackerRolled = true;
            attackerRollButton.interactable = false;
        }
        else
        {
            _defenderResults.text = rollMessage;
            defenderRolled = true;
            defenderRollButton.interactable = false;
        }

        if (attackerRolled && defenderRolled)
        {
            DetermineBattleResult();
        }
    }

    private void DetermineBattleResult()
    {
        int attackerWins = 0;
        int defenderWins = 0;

        for (int i = 0; i < 3; i++)
        {
            if (attackerRolls[i] > defenderRolls[i])
            {
                attackerWins++;
            }
            else if (defenderRolls[i] > attackerRolls[i])
            {
                defenderWins++;
            }
        }

        // Announce the winner
        if (attackerWins > defenderWins)
        {
            _message.text += $"\n{attacker.playerName} wins the battle with {attackerWins} rounds won, and owes no docking fees!";
        }
        else if (defenderWins > attackerWins)
        {
            _message.text += $"\n{defender.playerName} wins the battle with {defenderWins} rounds won and owes double docking fees!";
            BankManager.Instance.TransferFunds(attacker, defender, dockingFee * 2);
        }
        else
        {
            _message.text += "\nThe battle ends in a draw! No docking fees exchanged.";
        }

        attackerRollButton.interactable = false;
        defenderRollButton.interactable = false;
    }

    public void OnCloseBattlePressed()
    {
        Destroy(this.gameObject);
        Hud.Instance.ShowHud();
    }
}
