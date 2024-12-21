using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

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

    public int playerIdx; // Assigned based on Photon ActorNumber
    public string playerName; // From Photon NickName
    public bool isInJail; // Track Jail status
    public int turnsInJail; // Track turns in Jail

    private int _cashOnHand = 1500; // Default starting cash
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

    public void InitializePlayer(int idx, string name, Color color, soPlayerToken token)
    {
        playerIdx = idx;
        playerName = name;
        playerColor = color;
        so_PlayerToken = token;

        CreatePropertyManager();
        UpdatePlayerUI();
    }

    public void CreatePropertyManager()
    {
        if (!propertyManager)
        {
            propertyManager = gameObject.AddComponent<PropertyManager>();
            propertyManager.player = this;
        }
    }

    private void UpdatePlayerUI()
    {
        if (wPlayerInfo != null)
        {
            wPlayerInfo.UpdatePlayerInfo(playerName, playerColor, cashOnHand);
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
            Debug.LogWarning($"{playerName} has no Get Out of Jail Free cards to use.");
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
        UpdatePlayerUI();
    }

    public void MovePlayer(int diceRoll)
    {
        Vector3[] positions = GetPositionsForDiceRoll(diceRoll);
        StartCoroutine(MovementMgr.Instance.MovePlayerToPosition(playerPiece, positions, moveSpeed));
    }

    private Vector3[] GetPositionsForDiceRoll(int diceRoll)
    {
        List<Vector3> positions = new List<Vector3>();
        int currentPos = (int)pos;

        for (int i = 1; i <= diceRoll; i++)
        {
            int nextPos = (currentPos + 1) % (int)ePos.terminator;
            positions.Add(Board.Instance.spots[nextPos].transform.position);
            currentPos = nextPos;
        }

        return positions.ToArray();
    }

    public void MovePieceToSpot(int position)
    {
        Transform targetTransform = Board.Instance.spots[position].transform;
        MovementMgr.Instance.SetPlayerPosition(playerPiece, targetTransform.position);
    }

    public void MoveToTarget(ePos targetPosition, bool collect200 = false)
    {
        startPos = pos;
        int steps = (int)targetPosition - (int)pos;

        if (steps < 0)
        {
            steps += (int)ePos.terminator;
        }

        pos = targetPosition;

        coMove = MoveToSpotRoutine(steps, collect200);
        StartCoroutine(coMove);
    }

    private IEnumerator MoveToSpotRoutine(int steps, bool collect200)
    {
        int curPos = (int)startPos;

        for (int i = 1; i <= steps; i++)
        {
            int nextPos = (curPos + 1) % (int)ePos.terminator;

            Transform startTransform = Board.Instance.spots[curPos].transform;
            Transform targetTransform = Board.Instance.spots[nextPos].transform;

            float elapsedTime = 0f;

            while (elapsedTime < moveSpeed)
            {
                elapsedTime += Time.deltaTime;
                playerPiece.transform.position = Vector3.Lerp(startTransform.position, targetTransform.position, elapsedTime / moveSpeed);
                yield return null;
            }

            playerPiece.transform.position = targetTransform.position;
            curPos = nextPos;

            if (curPos == (int)ePos.go && collect200)
            {
                AdjustCash(200);
                Debug.Log($"{playerName} collected $200 for passing GO!");
            }
        }

        SetLandingSpot();
    }
    public void AdvanceToNearest(eSpotType targetType, bool collect200 = false, bool doubleRent = false)
    {
        Board board = Board.Instance;

        if (board == null)
        {
            Debug.LogError("Board instance is null. Ensure the board is set up correctly.");
            return;
        }

        int currentPos = (int)pos;
        int boardSize = (int)ePos.terminator;
        int stepsToNearest = boardSize; // Max possible steps (full board loop)
        int targetPosition = currentPos;

        // Find the nearest spot of the target type
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

        // Move the player to the nearest spot of the target type
        MoveToTarget((ePos)targetPosition, collect200);

        if (doubleRent)
        {
            Debug.Log("Double rent will be charged.");
            // Additional logic for handling double rent can be implemented here
        }
    }

    public void SetLandingSpot()
    {
        soSpot currentSpot = Board.Instance.spots[(int)pos].so_Spot;
        Debug.Log($"{playerName} landed on {currentSpot.spotName}");
        Hud.Instance.HandleSpot(this, currentSpot);
    }
}
