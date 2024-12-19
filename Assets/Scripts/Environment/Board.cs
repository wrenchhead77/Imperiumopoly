using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Instance;
    [NamedArray(typeof(ePos))] public Spot[] spots;

    private void Awake()
    {
        Instance = this;
        foreach (var spot in spots)
        {
            if (spot.so_Spot == null)
            {
                ErrorLogger.Instance.LogError($"soSpot is not assigned for spot at position {spot.transform.position}");
            }
        }
        if (Board.Instance.spots == null || Board.Instance.spots.Length == 0)
        {
            ErrorLogger.Instance.LogError("Board spots array is empty or null!");
        }
    }

    public List<soSpot> GetSpotsOfSameColorOrType(soSpot spot)
    {
        List<soSpot> relatedSpots = new List<soSpot>();

        relatedSpots = spots.Where(s => s.so_Spot.spotColor == spot.spotColor).Select(s => s.so_Spot).ToList();

        return relatedSpots;
    }
    public Transform GetSpotTransform(soSpot spot)
    {
        var boardSpot = spots.FirstOrDefault(s => s.so_Spot == spot);
        return boardSpot?.transform;
    }
}
