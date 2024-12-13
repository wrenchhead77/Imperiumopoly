using System.Collections.Generic;
using UnityEngine;

public class PersistentGameData : MonoBehaviour
{
    public static PersistentGameData Instance;
    public soEnvironData.EnvironInfo SelectedEnvironment;

    public Dictionary<string, bool> propertyMortgageStatus = new Dictionary<string, bool>();

    public int[] rules; // Array to track dropdown indices for each rule
    public int difficulty; // Difficulty setting
    public int sleight; // Sleight setting
    public List<soCard> chanceDeck = new List<soCard>();
    public List<soCard> communityChestDeck = new List<soCard>();
    // Dice variables
    public int lastDiceRoll;
    public int lastDice1;
    public int lastDice2;
    public int doublesCount;
    public bool doublesRolled;
    [Header("Cheats")]
    public bool isRiggedDice;
    public int riggedDice1 = 0;
    public int riggedDice2 = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaults();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void InitializeDefaults()
    {
        rules = new int[System.Enum.GetValues(typeof(eRuleMenu)).Length - 1];
        for (int i = 0; i < rules.Length; i++)
        {
            rules[i] = 0; // Default index
        }

        difficulty = 0; // Default difficulty
        sleight = 0; // Default sleight

    }
    public void ResetGameData()
    {
        SelectedEnvironment = null;
//        chanceDeck.Clear();
  //      communityChestDeck.Clear();
        Debug.Log("PersistentGameData: Data reset.");
    }
    public void PrintGameData()
    {
        if (SelectedEnvironment != null)
        {
            Debug.Log($"Selected Environment: {SelectedEnvironment.environName}");
        }
        else
        {
            Debug.Log("No environment selected.");
        }
    }
    public void SetMortgageStatus(string propertyName, bool isMortgaged)
    {
        if (propertyMortgageStatus.ContainsKey(propertyName))
        {
            propertyMortgageStatus[propertyName] = isMortgaged;
        }
        else
        {
            propertyMortgageStatus.Add(propertyName, isMortgaged);
        }
    }

    public bool GetMortgageStatus(string propertyName)
    {
        if (propertyMortgageStatus.TryGetValue(propertyName, out bool isMortgaged))
        {
            return isMortgaged;
        }
        return false; // Default to not mortgaged
    }
    public void ResetDice()
    {
        lastDiceRoll = 0;
        lastDice1 = 0;
        lastDice2 = 0;

        // Only reset doublesCount if the conditions are not met
        if (!(doublesRolled && doublesCount > 0 && doublesCount < 3))
        {
            doublesCount = 0;
        }

        doublesRolled = false; // Always reset doublesRolled
        Debug.Log($"Dice Reset. Doubles Count: {doublesCount}");
    }
    public void UpdateDiceState(int dice1, int dice2)
    {
        lastDice1 = dice1;
        lastDice2 = dice2;
        lastDiceRoll = dice1 + dice2;

        if (dice1 == dice2)
        {
            doublesCount++;
            doublesRolled = true;
            Debug.Log($"Doubles rolled! Count: {doublesCount}");
        }
        else
        {
            doublesCount = 0;
            doublesRolled = false;
            Debug.Log("Doubles not rolled. Count reset to 0.");
        }
    }
}
