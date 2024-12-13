using UnityEngine;

public enum eDiceNumber { One, Two, Three, Four, Five, Six}
[CreateAssetMenu(fileName = "New Dice Face", menuName = "Create Dice")]
public class soDice : ScriptableObject
{
    public eDiceNumber dice; // Enum representing the dice face
    public soDiceInfo[] faces;  // Array of cards in the deck
}
