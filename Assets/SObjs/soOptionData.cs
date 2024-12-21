using UnityEngine;

[CreateAssetMenu(fileName = "OptionData", menuName = "OptionData")]
public class soOptionData : ScriptableObject
{
    [System.Serializable]
    public class DropdownInfo
    {
        public string dropdownTitle;  // Title or description for each dropdown
        public string[] options;
        public int selectedIndex;
    }

    public DropdownInfo[] dropdowns = new DropdownInfo[4];
}
