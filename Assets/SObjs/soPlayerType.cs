using UnityEngine;
public enum ePlayerType { none, human, AI, terminator }

[CreateAssetMenu(fileName = "New PlayerType", menuName = "Create PlayerType")]
public class soPlayerType : ScriptableObject
{
        public ePlayerType playerType;
        public string playerTypeName;
        public Sprite playerTypeArt;

}
