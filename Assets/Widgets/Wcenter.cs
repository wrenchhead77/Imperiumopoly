using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Wcenter : MonoBehaviour
{
    public static Wcenter Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Image property;
    public TMP_Text message;

    protected soSpot so_Spot;
    protected iPlayer player;
    protected Hud hud = Hud.Instance;
    protected PlayerManager pm = PlayerManager.Instance;
    protected DiceManager dm = DiceManager.Instance;
    protected CanvasManager cm = CanvasManager.Instance;
    protected BankManager bm = BankManager.Instance;
    protected GameMgr gm = GameMgr.Instance;
    protected CameraManager camM = CameraManager.Instance;

    public virtual void InitWidget(soSpot _soSpot)
    {
        so_Spot = _soSpot;
        player = pm.players[pm.curPlayer];
        if (property != null && so_Spot != null)
        {
            property.sprite = so_Spot.spotArtFront;
        }
    }

    public void OnPassPressed()
    {
        hud.ShowHud();
        Debug.Log("Pass Pressed");
        Destroy();
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    public void EvaluateEndTurn()
    {
        // Show HUD and reset the camera
        hud.grp_bottom.SetActive(true);
        hud.SetRollDiceButton(PersistentGameData.Instance.doublesRolled);
        camM.SetCurrentCamera(eCameraPositions.main);
        dm.ResetDice();
    }
}
