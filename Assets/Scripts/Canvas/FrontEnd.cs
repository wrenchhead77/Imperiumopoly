using UnityEngine;
using Photon.Pun;

public class FrontEnd : MonoBehaviourPunCallbacks
{
    private bool isConnecting;

    private void Start()
    {
        // Ensure Photon is not already connected
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("Connecting to Photon...");
            PhotonNetwork.ConnectUsingSettings();
            isConnecting = true;
        }
    }

    public void OnPlayNowClicked()
    {
        Debug.Log("<color=yellow>Play Now Clicked</color>");

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogError("Cannot proceed. Not connected to Photon.");
            return;
        }

        // Open CanvasLobby locally
        Debug.Log("Opening CanvasLobby...");
        ShowCanvasLobby();

        // Attempt to join or create a room
        Debug.Log("Attempting to join a random room...");
        PhotonNetwork.JoinRandomRoom();
    }

    private void ShowCanvasLobby()
    {
        // Load the CanvasLobby locally, not as a networked object
        GameObject canvasLobby = Instantiate(Resources.Load<GameObject>("Canvas/CanvasLobby"));
        if (canvasLobby == null)
        {
            Debug.LogError("CanvasLobby prefab not found in Resources/Canvas!");
            return;
        }
        Debug.Log("CanvasLobby opened successfully.");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server.");
        isConnecting = false;
    }

public void OnTeacherModeClicked()
    {
        Debug.Log("<color=yellow>Teacher Mode Clicked</color>");
    }
    public void OnTableTopClicked()
    {
        Debug.Log("<color=yellow>Tabletop Mode Clicked</color>");
    }
    public void OnNetworkPlayClicked()
    {
        Debug.Log("<color=yellow>Local Network Play Clicked</color>");
    }
    public void OnTellAFriendClicked()
    {
        Debug.Log("<color=yellow>Tell A Friend Clicked</color>");
    }
    public void OnStatsClicked()
    {
        Debug.Log("<color=yellow>Stats Clicked</color>");
    }
    public void OnOptionsClicked()
    {
        Debug.Log("<color=yellow>Options Clicked</color>");
        CanvasManager.Instance.showCanvasOptions();
    }
    public void OnMusicClicked()
    {
        Debug.Log("<color=yellow>Music Clicked</color>");
    }
    public void OnHelpAboutClicked()
    {
        Debug.Log("<color=yellow>Help & About Clicked</color>");
        CanvasManager.Instance.showCanvasHelp();
    }
}
