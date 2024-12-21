using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public static PlayerManager Instance;
    public const int maxPlayers = 8;

    public Dictionary<int, iPlayer> players = new Dictionary<int, iPlayer>(); // Players indexed by Photon Player ID
    public int curPlayer; // Track the current player's Photon Actor Number
    public Vector3[] offset = {
        new Vector3(0f, 0.011f, 0f),
        new Vector3(0, 0.022f, 0f),
        new Vector3(0, 0.033f, 0f),
        new Vector3(0, 0.044f, 0f)
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializePlayers()
    {
        foreach (Player photonPlayer in PhotonNetwork.PlayerList)
        {
            if (!players.ContainsKey(photonPlayer.ActorNumber))
            {
                CreatePlayer(photonPlayer);
            }
        }

        Debug.Log($"Players initialized. Total players: {players.Count}");
    }

    private void CreatePlayer(Player photonPlayer)
    {
        Debug.Log($"Creating player for {photonPlayer.NickName}");

        iPlayer newPlayer = new GameObject($"Player_{photonPlayer.ActorNumber}").AddComponent<iPlayer>();
        newPlayer.transform.SetParent(transform);
        newPlayer.playerIdx = photonPlayer.ActorNumber;
        newPlayer.playerName = photonPlayer.NickName;

        // Assign default token and color
        newPlayer.playerColor = GameMgr.Instance.so_Ref.playerColors[photonPlayer.ActorNumber % maxPlayers];
        newPlayer.so_PlayerToken = GameMgr.Instance.so_Ref.playerTokens[photonPlayer.ActorNumber % maxPlayers];
        newPlayer.cashOnHand = 1500;
        newPlayer.CreatePropertyManager();

        players[photonPlayer.ActorNumber] = newPlayer;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player joined: {newPlayer.NickName}");
        CreatePlayer(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player left: {otherPlayer.NickName}");

        if (players.ContainsKey(otherPlayer.ActorNumber))
        {
            Destroy(players[otherPlayer.ActorNumber].gameObject);
            players.Remove(otherPlayer.ActorNumber);
        }
    }

    public iPlayer WhoOwnsProperty(soSpot _soSpot)
    {
        foreach (var player in players.Values)
        {
            if (player.propertyManager.IsPropertyOwned(_soSpot))
            {
                return player;
            }
        }
        return null;
    }

    public void CreatePieces()
    {
        foreach (var player in players.Values)
        {
            Transform t = Board.Instance.spots[(int)ePos.go].transform;
            Vector3 newPos = t.position + offset[player.playerIdx % offset.Length];
            player.playerPiece = Instantiate(player.so_PlayerToken.playerTokenModel, newPos, t.rotation);

            Renderer pieceRenderer = player.playerPiece.GetComponent<Renderer>();
            pieceRenderer.material.color = player.playerColor;
        }
    }

    public void AdvancePlayer()
    {
        List<int> playerKeys = new List<int>(players.Keys);
        int currentPlayerIdx = playerKeys.IndexOf(curPlayer);

        int nextPlayerIdx = (currentPlayerIdx + 1) % playerKeys.Count;
        curPlayer = playerKeys[nextPlayerIdx];

        iPlayer nextPlayer = players[curPlayer];
        Debug.Log($"Advancing to player {nextPlayer.playerName}");

        CameraManager.Instance.SetCurrentCamera(eCameraPositions.main);
    }

    public bool CheckForDupeTokens(int playerIdx)
    {
        if (!players.ContainsKey(playerIdx)) return false;

        var currentPlayerToken = players[playerIdx].so_PlayerToken;
        foreach (var playerEntry in players)
        {
            if (playerEntry.Key != playerIdx && playerEntry.Value.so_PlayerToken == currentPlayerToken)
            {
                return true; // Duplicate token found
            }
        }
        return false; // No duplicates
    }
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AutomaticallySyncScene = true; // Ensures all clients sync to the host's scene
    }

    public void SyncPlayersInSetupScene()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!players.ContainsKey(player.ActorNumber))
            {
                CreatePlayer(player);
            }
        }
    }

}
