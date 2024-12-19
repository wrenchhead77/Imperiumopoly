using UnityEngine;

public class FrontEnd : MonoBehaviour
{
    public void OnPlayNowClicked()
    {
        Debug.Log("<color=yellow>Play Now Clicked</color>");
        CanvasManager.Instance.showCanvasLobby();
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
