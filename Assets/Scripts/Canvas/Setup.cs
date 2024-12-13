using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class Setup : MonoBehaviour
{
    public RectTransform grp_PlayerButtons;
    public GameObject playerPrefab;
    public Button buttonPlay;
    public TMP_Dropdown difficultyDropdown;
    public TMP_Dropdown sleightDropdown;
    private soRef _soRef;

    private void Start()
    {
        // Get the Dropdown components directly from the Canvas
        difficultyDropdown = GetComponentInChildren<TMP_Dropdown>();
        sleightDropdown = GetComponentInChildren<TMP_Dropdown>();

        PersistentGameData data = PersistentGameData.Instance;
        difficultyDropdown.value = data.difficulty;
        sleightDropdown.value = data.sleight;

        difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        sleightDropdown.onValueChanged.AddListener(OnSleightChanged);
    }

    private void OnDifficultyChanged(int value)
    {
        PersistentGameData.Instance.difficulty = value;
    }

    private void OnSleightChanged(int value)
    {
        PersistentGameData.Instance.sleight = value;
    }

    public void InitCanvas()
    {
        Debug.Log("Instantiate Widgets");

        for (int i = 0; i < PlayerManager.maxPlayers; i++)
        {
            GameObject obj = Instantiate(Resources.Load("Widgets/Wsetup") as GameObject, grp_PlayerButtons);
            Wsetup scr = obj.GetComponent<Wsetup>();
            scr.InitWidget(PlayerManager.Instance.players[i]);
        }

    }
    private void SetButtonPlay(bool _active)
    {
        buttonPlay.interactable = _active;
    }

    public void CheckForPlayButton()
    {
        int humanPlayers = 0;
        int numActivePlayers = 0;

        foreach (var p in PlayerManager.Instance.players)
        {
            if (p.so_PlayerType.playerType == ePlayerType.human)
            {
                humanPlayers++;
            }
            if (p.so_PlayerType.playerType != ePlayerType.none)
            {
                numActivePlayers++;
            }
        }

        SetButtonPlay(humanPlayers > 0 && numActivePlayers > 1);
    }

    public void OnPlayClicked()
    {
        Debug.Log("<color=green>Play Clicked</color>");
        SceneMgr.Instance.LoadScene(eScene.InGame);
    }

    public void OnCancelClicked()
    {
        Debug.Log("<color=red>Cancel Clicked</color>");
        Destroy(this.gameObject);
    }

    public void OnEnvironClicked()
    {
        Debug.Log("<color=red>Environ Clicked</color>");
        CanvasManager.Instance.showCanvasEnviron();
    }

    public void OnHouseRulesClicked()
    {
        Debug.Log("<color=blue>House Rules Clicked</color>");
        CanvasManager.Instance.showCanvasHouseRules();
    }
}

/* 
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class Setup : MonoBehaviour
{
    public RectTransform grp_PlayerButtons;
    public GameObject playerPrefab;
    public Button buttonPlay;
    public TMP_Dropdown difficultyDropdown;
    public TMP_Dropdown sleightDropdown;
    private soRef _soRef;

    public void InitCanvas()
    {
        Debug.Log("Instantiate Widgets");
        _soRef = GameManager.Instance.so_Ref; // Ensure soRef is assigned

        for (int i = 0; i < PlayerManager.maxPlayers; i++)
        {
            GameObject obj = Instantiate(Resources.Load("Widgets/" + "wSetup") as GameObject, grp_PlayerButtons);
            Wsetup scr = obj.GetComponent<Wsetup>();
            scr.InitWidget(PlayerManager.Instance.players[i]); // Pass soRef here
        }

    }
    private void Start()
    {
        // Get the Dropdown components directly from the Canvas
        difficultyDropdown = GetComponentInChildren<TMP_Dropdown>();
        sleightDropdown = GetComponentInChildren<TMP_Dropdown>();

        PersistentGameData data = PersistentGameData.Instance;
        difficultyDropdown.value = data.difficulty;
        sleightDropdown.value = data.sleight;

        difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
        sleightDropdown.onValueChanged.AddListener(OnSleightChanged);
    }

    private void OnDifficultyChanged(int value)
    {
        PersistentGameData.Instance.difficulty = value;
    }

    private void OnSleightChanged(int value)
    {
        PersistentGameData.Instance.sleight = value;
    }

    private void SetButtonPlay(bool _active)
    {
        buttonPlay.interactable = _active;
    }

    public void CheckForPlayButton()
    {
        int humanPlayers = 0;
        int numActivePlayers = 0;

        foreach (var p in PlayerManager.Instance.players)
        {
            if (p.so_PlayerType.playerType == ePlayerType.human)
            {
                humanPlayers++;
            }
            if (p.so_PlayerType.playerType != ePlayerType.none)
            {
                numActivePlayers++;
            }
        }

        SetButtonPlay(humanPlayers > 0 && numActivePlayers > 1);
    }

    public void OnPlayClicked()
    {
        Debug.Log("<color=green>Play Clicked</color>");
        SceneMgr.Instance.LoadScene(eScene.InGame);
    }

    public void OnCancelClicked()
    {
        Debug.Log("<color=red>Cancel Clicked</color>");
        Destroy(this.gameObject);
    }

    public void OnEnvironClicked()
    {
        Debug.Log("<color=red>Environ Clicked</color>");
        CanvasManager.Instance.showCanvasEnviron();
    }

    public void OnHouseRulesClicked()
    {
        Debug.Log("<color=blue>House Rules Clicked</color>");
        CanvasManager.Instance.showCanvasHouseRules();
    }
} */
