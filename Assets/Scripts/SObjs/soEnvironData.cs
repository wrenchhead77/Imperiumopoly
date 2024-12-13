using UnityEngine;

public enum eEnvironName { environ0, environ1, environ2, environ3, environ4, terminator }

[CreateAssetMenu(fileName = "EnvironData", menuName = "EnvironData")]
public class soEnvironData : ScriptableObject
{
    public EnvironInfo[] environs = new EnvironInfo[5]; // Array of environment data

    [System.Serializable]
    public class EnvironInfo
    {
        public eEnvironName environName;
        public Sprite _largeImage; // Large image for the environment
        public Sprite _thumbnail; // Thumbnail image for the environment
        // Add any other relevant properties here
    }
}
