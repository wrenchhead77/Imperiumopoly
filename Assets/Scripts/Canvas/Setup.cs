using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Setup : MonoBehaviourPunCallbacks
{
    public RectTransform grp_PlayerButtons; // Parent for player widgets
    public GameObject playerPrefab;         // Prefab for player widgets
    public Button buttonPlay;               // Play button

    private void Awake()
    {
        Debug.Log("Setup Initialized.");
        InitCanvas(); // Initialize player widgets
        if (PhotonNetwork.PhotonServerSettings != null)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            Debug.Log("<color=green>Photon AutomaticallySyncScene enabled.</color>");
        }
    }

    public void InitCanvas()
    {
        Debug.Log("Initializing Player Widgets...");

        // Ensure PlayerManager is ready
        if (PlayerManager.Instance == null || PlayerManager.Instance.players.Count == 0)
        {
            Debug.LogError("PlayerManager is not initialized or no players found.");
            return;
        }

        // Instantiate UI widgets for each player
        foreach (var playerEntry in PlayerManager.Instance.players)
        {
            int playerId = playerEntry.Key;
            iPlayer player = playerEntry.Value;

            if (player == null)
            {
                Debug.LogWarning($"Player with ID {playerId} is null. Skipping...");
                continue;
            }

            GameObject obj = Instantiate(playerPrefab, grp_PlayerButtons);
            Wsetup scr = obj.GetComponent<Wsetup>();

            if (scr != null)
            {
                scr.InitWidget(player); // Pass player data to the widget
                Debug.Log($"Initialized player widget for: {player.playerName} (ID: {playerId})");
            }
            else
            {
                Debug.LogError("Wsetup script is missing from the player prefab.");
            }
        }

        CheckForPlayButton();
    }

    private void SetButtonPlay(bool _active)
    {
        buttonPlay.interactable = _active;
    }

    public void CheckForPlayButton()
    {
        int activePlayers = PlayerManager.Instance.players.Count;
        SetButtonPlay(activePlayers > 1); // Enable Play button if there are at least 2 players
        Debug.Log($"Play button state updated. Active players: {activePlayers}");
    }

    public void OnPlayClicked()
    {
        Debug.Log("<color=green>Play Clicked</color>");
        SceneMgr.Instance.LoadScene(eScene.InGame);
    }

    public void OnCancelClicked()
    {
        Debug.Log("<color=red>Cancel Clicked</color>");
        PhotonNetwork.LeaveRoom();
        Destroy(this.gameObject);
    }
    public void OnEnvironClicked()
    {
        Debug.Log("<color=red>Environment Clicked</color>");
        CanvasManager.Instance.showCanvasEnviron();
    }

    public void OnHouseRulesClicked()
    {
        Debug.Log("<color=blue>House Rules Clicked</color>");
        CanvasManager.Instance.showCanvasHouseRules();
    }
}
