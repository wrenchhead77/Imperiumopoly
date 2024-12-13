using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DiceInfo", menuName = "Create DiceInfo")]
public class soDiceInfo : ScriptableObject
{
    public int faceNumber; // The number on the dice face (1 to 6)
    public Sprite faceSprite; // The sprite representing the dice face
}
