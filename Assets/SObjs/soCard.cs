using UnityEngine;

public enum eCardType { Chance, CommChest, GOJF }

[CreateAssetMenu(fileName = "New Card", menuName = "Card Data")]
public class soCard : ScriptableObject
{
    public eCardType cardType;
    public eDeckType deckType; // New field to specify the deck (Chance or CommChest)
    public Sprite cardImage;   // Image of the card
    public string actionName;  // Action tied to the card
}
