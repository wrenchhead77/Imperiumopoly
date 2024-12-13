using UnityEngine;

[CreateAssetMenu(fileName = "DropdownData", menuName = "DropdownData")]
public class soDropdownData : ScriptableObject
{
    [System.Serializable]
    public class DropdownInfo
    {
        public string title;
        public string[] options;
        public int selectedIndex;  // This will store the selected index to persist
    }

    public DropdownInfo[] dropdowns = new DropdownInfo[9];  // Fixed array for 9 dropdowns
}
