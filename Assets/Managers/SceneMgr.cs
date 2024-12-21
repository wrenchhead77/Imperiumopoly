using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public enum eScene { Splash, FrontEnd, Setup, InGame }

public class SceneMgr : MonoBehaviour
{
    public static SceneMgr Instance;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        Debug.Log($"Scene Loaded: {_scene.name}");

        switch ((eScene)_scene.buildIndex)
        {
            case eScene.Splash:
                break;

            case eScene.FrontEnd:
                CanvasManager.Instance?.showCanvasFE();
                break;

            case eScene.Setup:
                HandleSetupScene(_scene);
                break;

            case eScene.InGame:
                HandleInGameScene(_scene);
                break;

            default:
                Debug.LogWarning("Unhandled scene case.");
                break;
        }
    }

    /// <summary>
    /// Handles the Setup Scene Initialization
    /// </summary>
    private void HandleSetupScene(Scene _scene)
    {
        Debug.Log("Initializing Setup Scene...");

        // Ensure players are ready
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.InitializePlayers();
        }
        else
        {
            Debug.LogError("PlayerManager instance is missing in Setup Scene.");
        }

        // Locate Setup script in the scene explicitly
        foreach (GameObject rootObject in _scene.GetRootGameObjects())
        {
            Setup setup = rootObject.GetComponentInChildren<Setup>();
            if (setup != null)
            {
                setup.InitCanvas();
                setup.CheckForPlayButton();
                Debug.Log("Setup initialized successfully.");
                break;
            }
        }

        // Show Setup UI
        CanvasManager.Instance?.showCanvasSetup();
    }

    /// <summary>
    /// Handles the InGame Scene Initialization
    /// </summary>
    private void HandleInGameScene(Scene _scene)
    {
        Debug.Log("Initializing InGame Scene...");

        // Initialize players and create their game pieces
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.InitializePlayers();
            PlayerManager.Instance.CreatePieces();
        }
        else
        {
            Debug.LogError("PlayerManager instance is missing in InGame Scene.");
        }

        // Initialize the HUD explicitly
        foreach (GameObject rootObject in _scene.GetRootGameObjects())
        {
            Hud hud = rootObject.GetComponentInChildren<Hud>();
            if (hud != null)
            {
                hud.InitCanvas();
                Debug.Log("HUD initialized successfully.");
                break;
            }
        }

        // Show HUD
        CanvasManager.Instance?.showCanvasHUD();

        // Set the camera
        CameraManager.Instance?.SetCurrentCamera(eCameraPositions.main);
    }

    /// <summary>
    /// Loads a scene and ensures Photon players are initialized after scene load.
    /// </summary>
    public void LoadScene(eScene _scene)
    {
        Debug.Log($"Load Scene: {_scene}");
        PhotonNetwork.LoadLevel((int)_scene); // Photon will ensure all players sync to the same scene
    }
}
