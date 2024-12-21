using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public RectTransform playerListContainer; // Container for player buttons
    public GameObject playerButtonPrefab;     // Prefab for player buttons
    public TMP_Text lobbyStatusText;          // Text to show lobby status
    public TMP_InputField LobbyCodeInputField; // Input for creating or displaying a lobby code
    public TMP_InputField PlayerIdInputField;  // Input for Player ID
    public TMP_Text StatusText;               // Text for lobby updates

    private Dictionary<string, GameObject> playerButtons = new Dictionary<string, GameObject>();

    private void Awake()
    {
        UpdateLobbyStatus();

        // Populate existing players if already in a room
        if (PhotonNetwork.InRoom)
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                AddPlayer(player);
            }
        }
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("<color=green>Host is starting the game. Syncing scene across players...</color>");
            PhotonNetwork.AutomaticallySyncScene = true; // Explicitly enable
            PhotonNetwork.LoadLevel("Setup"); // Ensure the scene name is correct
        }
        else
        {
            Debug.LogWarning("<color=yellow>Only the Host can start the game.</color>");
        }
    }

    public void UpdateLobbyStatus()
    {
        if (PhotonNetwork.InRoom)
        {
            lobbyStatusText.text = $"In Lobby: {PhotonNetwork.CurrentRoom.Name} - {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";
        }
        else
        {
            lobbyStatusText.text = "Not currently in a lobby.";
        }
    }

    public void OnSignIn()
    {
        string playerId = PlayerIdInputField.text.Trim();

        if (string.IsNullOrEmpty(playerId))
        {
            StatusText.text = "Player ID cannot be empty.";
            Debug.LogError("Player ID is empty.");
            return;
        }

        PhotonNetwork.NickName = playerId;
        StatusText.text = $"Signed in as: {playerId}";
        Debug.Log($"Player ID set to: {playerId}");
    }

    public void OnCreateRoomClicked()
    {
        string roomCode = GenerateLobbyCode();

        LobbyCodeInputField.text = roomCode; // Display the lobby code
        StatusText.text = $"Creating Lobby with Code: {roomCode}";
        Debug.Log($"Creating room with code: {roomCode}");

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = PlayerManager.maxPlayers,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomCode, options);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Room created: {PhotonNetwork.CurrentRoom.Name}");
        StatusText.text = $"Lobby Created: {PhotonNetwork.CurrentRoom.Name}";
        LobbyCodeInputField.text = PhotonNetwork.CurrentRoom.Name; // Update the lobby code in the input field
        UpdateLobbyStatus();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player joined: {newPlayer.NickName}");
        AddPlayer(newPlayer);
        UpdateLobbyStatus();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player left: {otherPlayer.NickName}");
        RemovePlayer(otherPlayer);
        UpdateLobbyStatus();
    }

    public void AddPlayer(Player photonPlayer)
    {
        if (playerButtons.ContainsKey(photonPlayer.NickName)) return;

        GameObject newButton = Instantiate(playerButtonPrefab, playerListContainer);
        TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = photonPlayer.NickName;
        }
        playerButtons.Add(photonPlayer.NickName, newButton);

        PlayerManager.Instance.InitializePlayers();
    }

    public void RemovePlayer(Player photonPlayer)
    {
        if (playerButtons.ContainsKey(photonPlayer.NickName))
        {
            Destroy(playerButtons[photonPlayer.NickName]);
            playerButtons.Remove(photonPlayer.NickName);
        }
    }

    public void OnJoinLobbyClicked()
    {
        string roomCode = LobbyCodeInputField.text.Trim();

        if (string.IsNullOrEmpty(roomCode))
        {
            StatusText.text = "Lobby Code cannot be empty.";
            Debug.LogError("Lobby Code is empty.");
            return;
        }

        if (!PhotonNetwork.IsConnected)
        {
            StatusText.text = "Not connected to Photon.";
            Debug.LogError("Cannot join room. Not connected to Photon.");
            return;
        }

        if (PhotonNetwork.Server == ServerConnection.GameServer)
        {
            Debug.LogWarning("Client is on Game Server. Reconnecting to Master Server...");
            PhotonNetwork.Disconnect(); // Disconnect from Game Server
            PhotonNetwork.ConnectUsingSettings(); // Reconnect to Master Server
            return;
        }

        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.Server == ServerConnection.MasterServer)
        {
            Debug.Log($"Attempting to join room with code: {roomCode}");
            PhotonNetwork.JoinRoom(roomCode);
            StatusText.text = "Joining lobby...";
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name}");
        StatusText.text = $"Joined Lobby: {PhotonNetwork.CurrentRoom.Name}";
        UpdateLobbyStatus();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            AddPlayer(player);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("<color=green>Host is starting the game. Loading Setup scene...</color>");
            PhotonNetwork.LoadLevel("Setup"); // Ensure this is the correct name of your game setup scene
        }
        else
        {
            Debug.LogWarning("<color=yellow>Only the Host can start the game.</color>");
        }
    }

    private string GenerateLobbyCode()
    {
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        System.Text.StringBuilder code = new System.Text.StringBuilder(6);

        for (int i = 0; i < 6; i++)
        {
            int index = UnityEngine.Random.Range(0, characters.Length);
            code.Append(characters[index]);
        }

        return code.ToString();
    }

    public void CancelAndDestroy()
    {
        Debug.Log("Canceling lobby and destroying this game object.");
        PhotonNetwork.LeaveRoom();
        Destroy(this.gameObject);
    }
}
