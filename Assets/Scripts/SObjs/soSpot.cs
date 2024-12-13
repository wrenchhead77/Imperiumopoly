using UnityEngine;

public enum eSpotType { doNothing, property, tax, chance, commChest, utility, railRoad, goToJail, SafeHarbor, jail, terminator }
public enum eSpotColor { none, Purple, LightBlue, Pink, Orange, Red, Yellow, Green, DarkBlue, White, Gray, terminator}

[CreateAssetMenu(fileName = "New Spot", menuName = "Create Spot")]
public class soSpot : ScriptableObject
{
    public eSpotType spotType;
    public eSpotColor spotColor;
    public string spotName;
    public ePos spotPosition;
    public Sprite spotArtFront;
    public Sprite spotArtBack;
    public int price;
    public int[] rent;
    public int[] rentRR;
    public int[] rentUte;
    public int tax;
    public int BunkerCost;
    public int FortressCost;
    public int MortgageCost;
}
