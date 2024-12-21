using UnityEngine;
using System.Linq;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [SerializeField] private soDice diceData; // Reference to the ScriptableObject holding dice faces

    private void Awake()
    {
        Instance = this;

        if (PersistentGameData.Instance == null)
        {
            ErrorLogger.Instance.LogError("PersistentGameData instance is missing! Ensure it's added to the GameManager GameObject.");
        }

        if (diceData == null)
        {
            ErrorLogger.Instance.LogError("Dice data is missing. Ensure `soDice` is assigned in the inspector.");
        }
        else if (diceData.faces == null || diceData.faces.Length < 6)
        {
            ErrorLogger.Instance.LogError($"Dice data is incomplete. Expected 6 entries, but found {(diceData.faces == null ? 0 : diceData.faces.Length)}. Ensure `soDice.faces` is configured properly.");
        }
        else
        {
            Debug.Log("Dice data successfully loaded.");
        }
    }
    public void ResetDice()
    {
        PersistentGameData.Instance?.ResetDice();
    }

    public int RollDice()
    {
        int dice1 = Random.Range(1, 7);
        int dice2 = Random.Range(1, 7);

        if (PersistentGameData.Instance?.isRiggedDice == true)
        {
            dice1 = PersistentGameData.Instance.riggedDice1;
            dice2 = PersistentGameData.Instance.riggedDice2;
        }

        Debug.Log($"Dice Rolled: {dice1}, {dice2}");

        PersistentGameData.Instance?.UpdateDiceState(dice1, dice2);

        if (PersistentGameData.Instance != null && PersistentGameData.Instance.doublesCount > 2)
        {
            return -99; // Go to Jail
        }

        return PersistentGameData.Instance?.lastDiceRoll ?? 0;
    }

    public Sprite GetDiceFaceSprite(eDiceNumber face)
    {
        if (diceData != null && diceData.faces != null)
        {
            soDiceInfo faceInfo = diceData.faces.FirstOrDefault(f => (int)f.faceNumber == (int)face + 1); // Match number

            if (faceInfo != null)
            {
                Debug.Log($"Sprite found for face: {face}");
                return faceInfo.faceSprite;
            }
        }

        ErrorLogger.Instance.LogError($"Failed to fetch sprite for face: {face}");
        return null;
    }
}
