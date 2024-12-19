using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public const int maxPlayers = 4;
    public int numPlayers;

    public iPlayer[] players;
    public int curPlayer;

    public Vector3[] offset = { new Vector3(0f, 0.011f, 0f), new Vector3(0, 0.022f, 0f),
                                new Vector3(0, 0.033f, 0f), new Vector3(0, 0.044f, 0f),};

    private void Awake()
    {
        Instance = this;
    }

    public void CreatePlayers()
    {
        GameMgr gm = GameMgr.Instance;
        soPlayerType[] playerTypes = gm.so_Ref.playerTypes;
        soPlayerToken[] playerTokens = gm.so_Ref.playerTokens;
        Color[] playerColors = gm.so_Ref.playerColors;

        players = new iPlayer[maxPlayers];

        for (int i = 0; i < maxPlayers; i++)
        {
            players[i] = new GameObject().AddComponent<iPlayer>(); // Create a new Game Object and attach Player script to it
            players[i].transform.SetParent(this.transform);
            players[i].playerIdx = i;
            players[i].name = "Player" + (i + 1).ToString();
            players[i].playerName = "Player" + (i + 1).ToString();
            players[i].playerColor = playerColors[i];
            players[i].so_PlayerToken = playerTokens[i];
            players[i].so_PlayerType = gm.so_Ref.playerTypes[(int)ePlayerType.none];
            players[i].cashOnHand = 1500;
            players[i].CreatePropertyManager();
        }

        players[0].so_PlayerType = playerTypes[(int)ePlayerType.human];
        players[1].so_PlayerType = playerTypes[(int)ePlayerType.AI];
    }

    internal void SendPlayerToJail(iPlayer currentPlayer)
    {
        throw new NotImplementedException();
    }

    public iPlayer WhoOwnsProperty(soSpot _soSpot)
    {
        foreach (iPlayer p in players)
        {
            if (p.so_PlayerType.playerType != ePlayerType.none)
            {
                if (p.propertyManager.IsPropertyOwned(_soSpot)) return p;
            }
        }
        return null;
    }

    public bool CheckForDupeTokens(int _playerIndex)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (i != _playerIndex && players[i].so_PlayerType.playerType != ePlayerType.none)
            {
                if (players[_playerIndex].so_PlayerToken.playerToken == players[i].so_PlayerToken.playerToken)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void CreatePieces()
    {
        Debug.Log("Offset Vector: " + offset[0].x + ", " + offset[0].y + ", " + offset[0].z);
        for (int i = 0; i < maxPlayers; i++)
        {
            if (players[i].so_PlayerType.playerType != ePlayerType.none)
            {
                Transform t = Board.Instance.spots[(int)ePos.go].transform;
                Vector3 newPos = new Vector3(
                    t.position.x + offset[i].x,
                    t.position.y + offset[i].y,
                    t.position.z + offset[i].z);
                players[i].playerPiece = Instantiate(players[i].so_PlayerToken.playerTokenModel, newPos, t.rotation);
                Renderer pieceRenderer = players[i].playerPiece.GetComponent<Renderer>();
                pieceRenderer.material.color = players[i].playerColor;
                numPlayers++;
            }
        }
    }

    /*    public Player CheckForOwnership(soSpot _soSpot)
        {
            for (int i = 0; i < maxPlayers; i++)
            {
                if (players[i].so_PlayerType.playerType != ePlayerType.none)
                {
                    if (players[i].propertyManager.IsPropertyOwned(_soSpot)) return players[i];
                }
            }
            return null;
        } */

    public void AdvancePlayer()
    {
        DiceManager dm = DiceManager.Instance;
        CanvasManager cm = CanvasManager.Instance;

        do
        {
            curPlayer++;
            if (curPlayer >= players.Length)
            {
                curPlayer = 0; // Loop back to Player 1
            }

        } while (players[curPlayer].so_PlayerType.playerType == ePlayerType.none);

        iPlayer nextPlayer = players[curPlayer];
        Debug.Log($"Advancing to player {nextPlayer.playerName}");

        // Highlight the current player's info
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].wPlayerInfo != null)
            {
                players[i].wPlayerInfo.SetPlayerSelect(i == curPlayer);
            }
        }

        // Reset dice and enable Roll Dice button for the next player
        dm.ResetDice();
        cm.hud.SetRollDiceButton(true);
        CameraManager.Instance.SetCurrentCamera(eCameraPositions.main);
    }
}
