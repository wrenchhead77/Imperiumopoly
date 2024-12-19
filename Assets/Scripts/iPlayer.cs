using System.Collections;
using UnityEngine;

public class iPlayer : MonoBehaviour
{
    [Header("Setup Info")]
    public WplayerInfo wPlayerInfo;
    public PropertyManager propertyManager;
    public soPlayerType so_PlayerType;
    public soPlayerToken so_PlayerToken;
    public GameObject playerPiece;

    public Color playerColor;
    public ePos pos;
    private ePos startPos;
    private IEnumerator coMove;

    [SerializeField] private float moveSpeed = .3f;

    public int playerIdx;
    public string playerName;
    public bool isInJail; // Track Jail status
    public int turnsInJail; // Track turns in Jail

    public int _cashOnHand;
    public int cashOnHand
    {
        get => _cashOnHand;
        set
        {
            _cashOnHand = value;
            UpdateCashDisplay();
        }
    }
    public int GOJFCard = 0;

    public void CreatePropertyManager()
    {
        if (!propertyManager)
        {
            propertyManager = gameObject.AddComponent<PropertyManager>();
            propertyManager.player = this;
        }
    }

    public void UpdateCashDisplay()
    {
        if (wPlayerInfo != null)
        {
            wPlayerInfo.UpdateCashDisplay(_cashOnHand);
        }
    }

    public void AddGetOutOfJailCard()
    {
        GOJFCard++;
        Debug.Log($"{playerName} now has {GOJFCard} Get Out of Jail Free cards.");
    }

    public void UseGetOutOfJailCard()
    {
        if (GOJFCard > 0)
        {
            GOJFCard--;
            Debug.Log($"{playerName} used a Get Out of Jail Free card. Remaining: {GOJFCard}");
        }
        else
        {
            ErrorLogger.Instance.LogWarning($"{playerName} has no Get Out of Jail Free cards to use.");
        }
    }

    public void AdjustCash(int amount)
    {
        cashOnHand += amount;
    }

    public void IncrementPlayerPiece()
    {
        ePlayerToken piece = so_PlayerToken.playerToken;
        piece++;
        if (piece == ePlayerToken.terminator)
        {
            piece = 0;
        }
        so_PlayerToken = GameMgr.Instance.so_Ref.playerTokens[(int)piece];
    }

    public void IncrementPlayerType()
    {
        ePlayerType playerType = so_PlayerType.playerType;
        playerType++;
        if (playerType == ePlayerType.terminator)
        {
            playerType = 0;
        }
        so_PlayerType = GameMgr.Instance.so_Ref.playerTypes[(int)playerType];
    }

    public void MovePlayer(int _diceRoll)
    {
        startPos = pos;

        // Update position and determine if passing Go
        pos += _diceRoll;
        bool isPassGo = false;

        if (pos >= ePos.terminator)
        {
            isPassGo = true;
            pos -= ePos.terminator; // Wrap around the board
        }

        // Start coroutine for dice-based movement
        coMove = MoveToSpot(isPassGo, diceRoll: _diceRoll);
        StartCoroutine(coMove);
    }

    IEnumerator MoveToSpot(bool _isPassGo, int diceRoll, ePos? targetPosition = null, bool collect200 = false)
    {
        PlayerManager pm = PlayerManager.Instance;
        int curPos = (int)startPos;

        // Ensure the center camera is active during movement
        CameraManager.Instance.SetCurrentCamera(eCameraPositions.center, playerPiece.transform);

        for (int i = 1; i <= diceRoll; i++)
        {
            int nextPos = (curPos + 1) % (int)ePos.terminator;

            Transform startTransform = Board.Instance.spots[curPos].transform;
            Transform targetTransform = Board.Instance.spots[nextPos].transform;

            Vector3 startPosition = startTransform.position + pm.offset[playerIdx];
            Vector3 targetPositionVector = targetTransform.position + pm.offset[playerIdx];

            Quaternion startRotation = playerPiece.transform.rotation;
            Quaternion targetRotation = targetTransform.rotation;

            float elapsedTime = 0f;
            float duration = moveSpeed;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;

                playerPiece.transform.position = Vector3.Lerp(startPosition, targetPositionVector, t);
                playerPiece.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

                yield return null;
            }

            playerPiece.transform.position = targetPositionVector;
            playerPiece.transform.rotation = targetRotation;

            curPos = nextPos;

            MovePieceToSpot(curPos);

            if (curPos == (int)ePos.go && _isPassGo)
            {
                Debug.Log($"{playerName} passed Go!");
                Hud.Instance.ShowGoPopup(this);
                _isPassGo = false; // Reset flag after handling
            }
        }

        // After completing movement, set the camera to overhead
        CameraManager.Instance.SetCurrentCamera(eCameraPositions.overhead, spotIndex: (int)pos);

        SetLandingSpot();
    }

    public void SetLandingSpot()
    {
        iPlayer player = this;
        soSpot so_Spot = Board.Instance.spots[(int)pos].so_Spot;
        Debug.Log($"Player landed on: {so_Spot.spotName} ({so_Spot.spotType})");
        StartCoroutine(SwitchToOverheadCamera());
        switch (so_Spot.spotType)
        {
            case eSpotType.property:
            case eSpotType.railRoad:
            case eSpotType.utility:
            case eSpotType.tax:
                Hud.Instance.HandleSpot(player, so_Spot);
                break;
            case eSpotType.chance:
                CardManager.Instance.DrawCard(eDeckType.Chance);
                break;
            case eSpotType.commChest:
                CardManager.Instance.DrawCard(eDeckType.CommChest);
                break;
            case eSpotType.goToJail:
                Hud.Instance.GoToJail();
                break;
            case eSpotType.SafeHarbor:
                Hud.Instance.HandleSafeHarborSpot();
                break;
            case eSpotType.doNothing:
            default:
                Debug.Log("Nothing happens on this spot.");
                break;
        }
        Wcenter.Instance.EvaluateEndTurn();
    }
    private IEnumerator SwitchToOverheadCamera()
    {
        yield return new WaitForSeconds(0.1f);
        CameraManager.Instance.SetCurrentCamera(eCameraPositions.overhead, spotIndex: (int)pos);
    }

    public void MoveToTarget(ePos targetPosition, bool collect200 = false)
    {
        startPos = pos;
        int steps = (int)targetPosition - (int)pos;

        bool isPassGo = false;
        if (steps < 0)
        {
            steps += (int)ePos.terminator;
            isPassGo = true;
        }

        pos = targetPosition;

        // Set the camera to follow the player
        CameraManager.Instance.SetCurrentCamera(eCameraPositions.center, playerPiece.transform);

        coMove = MoveToSpot(isPassGo, diceRoll: steps, targetPosition: targetPosition, collect200: collect200);
        StartCoroutine(coMove);
    }
    public void AdvanceToNearest(eSpotType targetType, bool collect200 = false, bool doubleRent = false)
    {
        Board board = Board.Instance;
        int currentPos = (int)pos;
        int boardSize = (int)ePos.terminator;
        int stepsToNearest = boardSize; // Max possible steps (full board loop)
        int targetPosition = currentPos;

        for (int i = 1; i < boardSize; i++)
        {
            int nextPos = (currentPos + i) % boardSize;

            if (board.spots[nextPos].so_Spot.spotType == targetType)
            {
                stepsToNearest = i;
                targetPosition = nextPos;
                break;
            }
        }

        // Move the player without triggering HandleSpot
        MoveToTarget((ePos)targetPosition, collect200);
        CameraManager.Instance.SetCurrentCamera(eCameraPositions.center, playerPiece.transform);

        // Remove this line to avoid double handling:
        // Hud.Instance.HandleSpot(board.spots[targetPosition].so_Spot, doubleRent);
    }

    public void MovePieceToSpot(int _pos)
    {
        PlayerManager pm = PlayerManager.Instance;
        Transform t = Board.Instance.spots[_pos].transform;
        Vector3 newPos = t.position + pm.offset[playerIdx];
        playerPiece.transform.SetPositionAndRotation(newPos, t.rotation);
    }
}
