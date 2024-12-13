using UnityEngine;

public enum eRuleMenu { PropsAtStart, BunkersPerFortress, BunkerFortressLimit, SafeHarbor, InitialCredits, PassGOSalary, LandOnGOSalary, Auctions, SpaceBattles, DiceColor, terminator };
[CreateAssetMenu(fileName = "New RuleMenuData", menuName = "RuleMenuData", order = 1)]
public class soRuleMenu : ScriptableObject
{
    public eRuleMenu ruleMenu;

    [System.Serializable]
    public class DropdownInfo
    {
        public string dropdownTitle;  // Title or description for each dropdown
        public string[] options;      // List of options for each dropdown
        public int defaultIndex;      // Default selected index
    }

     public DropdownInfo[] dropdowns;  // Array to hold data for multiple dropdowns 
}
