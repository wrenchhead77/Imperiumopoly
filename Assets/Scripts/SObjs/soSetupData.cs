using UnityEngine;

public enum eSetupMenu { Difficulty, Sleight, terminator };
[CreateAssetMenu(fileName = "SetupData", menuName = "SetupData")]
public class soSetupData : ScriptableObject
{
    public  eSetupMenu setupMenu;

    [System.Serializable]
    public class SetupInfo
    {
        public string title;
        public string[] options;
        public int selectedIndex;
    }

    public SetupInfo[] dropdowns = new SetupInfo[2];
}
