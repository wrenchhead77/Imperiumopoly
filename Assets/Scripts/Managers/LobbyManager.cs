using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public RectTransform playerListContainer; // Container for player buttons
    public GameObject playerButtonPrefab;     // Prefab for player buttons
    public TMP_Text lobbyStatusText;          // Text to show lobby status
    public TMP_InputField PlayerIdInputField; // Input for custom Player ID
    public TMP_InputField RoomCodeInputField; // Input for Room Code
    public TMP_Text StatusText;               // Text for status updates

    private Dictionary<string, GameObject> playerButtons = new Dictionary<string, GameObject>();
    private string playerName = "Player";
    private string roomCode = "Room";

    void Start()
    {
        // Connect to the Photon master server
        StatusText.text = "Connecting to Photon...";
        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        StatusText.text = "Connected to Photon. Enter Player ID and Room Code.";
        Debug.Log("Connected to Photon Master Server.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        StatusText.text = $"Disconnected from Photon: {cause}";
        Debug.LogError($"Disconnected from Photon with cause: {cause}");
    }

    public override void OnJoinedRoom()
    {
        StatusText.text = $"Joined Room: {PhotonNetwork.CurrentRoom.Name}";
        Debug.Log($"Joined Room: {PhotonNetwork.CurrentRoom.Name}. Players: {PhotonNetwork.CurrentRoom.PlayerCount}");
        UpdateLobbyStatus();

        // Add existing players to the list
        foreach (var player in PhotonNetwork.PlayerList)
        {
            AddPlayer(player.NickName);
        }
    }

    public override void OnCreatedRoom()
    {
        StatusText.text = $"Room created: {PhotonNetwork.CurrentRoom.Name} (Code: {PhotonNetwork.CurrentRoom.Name})";
        Debug.Log($"Created Room: {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StatusText.text = $"Failed to create room: {message}";
        Debug.LogError($"Room creation failed. Return Code: {returnCode}, Message: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        StatusText.text = $"Failed to join room: {message}";
        Debug.LogError($"Failed to join room. Return Code: {returnCode}, Message: {message}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player joined: {newPlayer.NickName}");
        AddPlayer(newPlayer.NickName);
        UpdateLobbyStatus();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player left: {otherPlayer.NickName}");
        RemovePlayer(otherPlayer.NickName);
        UpdateLobbyStatus();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left the room.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("FrontEnd"); // Replace with your main menu scene name
    }

    #endregion

    #region Public Methods

    public void OnSetPlayerId()
    {
        playerName = PlayerIdInputField.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            StatusText.text = "Player ID cannot be empty.";
            Debug.LogError("Player ID is empty.");
            return;
        }

        PhotonNetwork.NickName = playerName;
        StatusText.text = $"Player ID set to: {playerName}";
        Debug.Log($"Player ID set to: {playerName}");
    }

    public void OnCreateRoom()
    {
        // Generate a random lobby code (e.g., 6-character alphanumeric)
        roomCode = GenerateLobbyCode();

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 8, // Limit to 8 players
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(roomCode, options);
        StatusText.text = "Creating room...";
        Debug.Log($"Creating room with code: {roomCode}");
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

    public void OnJoinRoom()
    {
        roomCode = RoomCodeInputField.text.Trim();

        if (string.IsNullOrEmpty(roomCode))
        {
            StatusText.text = "Room Code cannot be empty.";
            Debug.LogError("Room Code is empty.");
            return;
        }

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            StatusText.text = "Not connected to Photon.";
            Debug.LogError("Cannot join room. Not connected to Photon.");
            return;
        }

        PhotonNetwork.JoinRoom(roomCode);
        StatusText.text = "Joining room...";
        Debug.Log($"Attempting to join room with code: {roomCode}");
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

    public void AddPlayer(string playerName)
    {
        if (playerButtons.ContainsKey(playerName))
        {
            Debug.LogWarning($"Player {playerName} is already in the lobby.");
            return;
        }

        // Instantiate a new player button
        GameObject newPlayerButton = Instantiate(playerButtonPrefab, playerListContainer);
        WlobbyPlayer lobbyPlayerScript = newPlayerButton.GetComponent<WlobbyPlayer>();

        if (lobbyPlayerScript != null)
        {
            lobbyPlayerScript.SetPlayerName(playerName);
        }

        // Track the button
        playerButtons.Add(playerName, newPlayerButton);
    }

    public void RemovePlayer(string playerName)
    {
        if (playerButtons.ContainsKey(playerName))
        {
            Destroy(playerButtons[playerName]);
            playerButtons.Remove(playerName);
        }
        else
        {
            Debug.LogWarning($"Player {playerName} not found in the lobby.");
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Starting the game...");
            PhotonNetwork.LoadLevel("InGame"); // Replace with your game scene name
        }
        else
        {
            Debug.LogWarning("Only the host can start the game.");
        }
    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveRoom();
        StatusText.text = "Leaving lobby...";
    }

    #endregion
}
