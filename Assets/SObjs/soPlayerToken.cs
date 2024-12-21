using UnityEngine;

public enum ePlayerToken { bolter, boltRound, chainSword, powerAxe, seal, thunderHammer, thunderhawk, typhoon, terminator}

[CreateAssetMenu(fileName = "New PlayerToken", menuName = "Create PlayerToken")]
public class soPlayerToken : ScriptableObject
{
    public ePlayerToken playerToken;
    public string playerTokenName;
    public Sprite playerTokenArt;
    public GameObject playerTokenModel;
}
