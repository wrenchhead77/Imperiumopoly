 using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum eScene { Splash, FrontEnd, InGame}
public class SceneMgr : MonoBehaviour
{ 
    public static SceneMgr Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded (Scene _scene, LoadSceneMode _mode)
    {
        Debug.Log("Scene Loaded" + _scene.name);
        switch ((eScene)_scene.buildIndex)
        {
            case eScene.Splash:
                PlayerManager.Instance.CreatePlayers();
                break;
            case eScene.FrontEnd:
                CanvasManager.Instance.showCanvasFE();
                //TODO: add scene code for front end
                break;
            case eScene.InGame:
                PlayerManager.Instance.CreatePieces();
                CanvasManager.Instance.showCanvasHUD();
                CameraManager.Instance.SetCurrentCamera(eCameraPositions.main);
                //TODO: add scene code for inGame
                break;
            default:
                break;
        }
    }
    public void LoadScene (eScene _scene)
    {
        Debug.Log("Load Scene" + _scene);
        SceneManager.LoadScene((int)_scene);

    }
}
