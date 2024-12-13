using UnityEngine;

[CreateAssetMenu(fileName ="New Ref", menuName ="Create Ref")]

public class soRef : ScriptableObject
{

    [NamedArray(typeof(ePlayerToken))] public soPlayerToken[] playerTokens;
    [NamedArray(typeof(ePlayerType))] public soPlayerType[] playerTypes;
    [NamedArray(typeof(eRuleMenu))] public soRuleMenu[] ruleMenus;
    [NamedArray(typeof(eDeckType))] public soCardDeck[] cardDeck;
    [NamedArray(typeof(eDiceNumber))]
    public soDice[] diceFaces; // Add dice faces array


    public Color[] playerColors;
    public static string GetColorName(Color color)
    {
        if (color == Color.red) return "Red";
        if (color == Color.blue) return "Blue";
        if (color == Color.green) return "Green";
        if (color == Color.yellow) return "Yellow";
        return $"Custom Color ({color.r:F2}, {color.g:F2}, {color.b:F2})";
    }
}
