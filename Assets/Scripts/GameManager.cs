using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private PersistentGameData gameData;

    public soRef so_Ref;
    public soRuleMenu so_RuleMenu;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        // Ensure PersistentGameData is attached
        gameData = GetComponent<PersistentGameData>();
        if (gameData == null)
        {
            gameData = gameObject.AddComponent<PersistentGameData>();
        }
    }

    void Start()
    {
        Debug.Log("Starting Game");
        AudioManager.Instance.volume[(int)eMixers.music] = .25f;
        AudioManager.Instance.volume[(int)eMixers.effects] = .25f;
    }

    public soEnvironData.EnvironInfo GetSelectedEnvironment()
    {
        return gameData.SelectedEnvironment;
    }

    public void SetSelectedEnvironment(soEnvironData.EnvironInfo environ)
    {
        gameData.SelectedEnvironment = environ;
        Debug.Log($"GameManager: Environment set to {environ.environName}");
    }
}
