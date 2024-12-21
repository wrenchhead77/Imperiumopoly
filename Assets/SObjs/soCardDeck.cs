using UnityEngine;

public enum eDeckType { Chance, CommChest}
[CreateAssetMenu(fileName = "New Card Deck", menuName = "Card Deck")]
public class soCardDeck : ScriptableObject
{
    public eDeckType type;
    public soCard[] cards;  // Array of cards in the deck
}
