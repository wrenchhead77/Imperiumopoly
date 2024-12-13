using System;
using System.Data;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public Hud hud;
    private Player player;

    public static CanvasManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void showCanvasFE()
    {
        Instantiate(Resources.Load("Canvas/CanvasFE") as GameObject);
    }
    public void showCanvasEnviron()
    {
        Environments environ = Instantiate(Resources.Load("Canvas/CanvasEnviron")
            as GameObject).GetComponent<Environments>();
    }
    public void showCanvasSetup()
    {
        Setup setup = Instantiate(Resources.Load("Canvas/CanvasSetup") as GameObject).GetComponent<Setup>();
        setup.InitCanvas(); 
    }

    public void showCanvasOptions()
    {
        Options options = Instantiate(Resources.Load("Canvas/CanvasOptions")
            as GameObject).GetComponent<Options>();
    }

    public void showCanvasHelp()
    {
        Help help = Instantiate(Resources.Load("Canvas/CanvasHelp")
            as GameObject).GetComponent<Help>();
    }

    public void showCanvasHouseRules()
    {
        RuleMenu rules = Instantiate(Resources.Load("Canvas/CanvasHouseRules")
            as GameObject).GetComponent<RuleMenu>();
    }

    public void showCanvasPause()
    {
        Pause pause = Instantiate(Resources.Load("Canvas/CanvasPause")
            as GameObject).GetComponent<Pause>();
    }

    public void showCanvasMessage(Popup _popup, Sprite propertyImage = null)
    {
        // Load and instantiate the CanvasMessage prefab
        GameObject messageObj = Instantiate(Resources.Load("Canvas/CanvasMessage") as GameObject);
        if (messageObj == null)
        {
            Debug.LogError("Failed to load CanvasMessage prefab. Check that it exists in Resources/Canvas.");
            return;
        }

        // Initialize the Message component with the popup data
        Message message = messageObj.GetComponent<Message>();
        if (message != null)
        {
            message.InitCanvas(_popup, propertyImage);
        }
        else
        {
            Debug.LogError("Message component missing from CanvasMessage prefab.");
        }
    }
    public void showCanvasHUD()
    {
        hud = Instantiate(Resources.Load("Canvas/CanvasHUD") as GameObject).GetComponent<Hud>();
    }
    public void showCanvasPurchase(soSpot _soSpot)
    {
        WcenterPurchase purchase = Instantiate(Resources.Load("Canvas/CanvasPurchase") as GameObject).GetComponent<WcenterPurchase>();
        purchase.InitWidget(_soSpot);
    }
    public void showCanvasRent(soSpot _soSpot)
    {
        WcenterRent rent = Instantiate(Resources.Load("Canvas/CanvasRent") as GameObject).GetComponent<WcenterRent>();
        rent.InitWidget(_soSpot);
    }

    public void ShowCanvasBattle(Player attacker, Player defender, int dockingFee)
    {
        CanvasBattle battle = Instantiate(Resources.Load("Canvas/CanvasBattle") as GameObject).GetComponent<CanvasBattle>();
        battle.InitBattle(attacker, defender, dockingFee);
    }
    public void showCanvasTax(soSpot _soSpot)
    {
        WcenterTax tax = Instantiate(Resources.Load("Canvas/CanvasTax") as GameObject).GetComponent<WcenterTax>();
        tax.InitWidget(_soSpot);
    }

    public void showCanvasJail(Player _player)
    {
        // Instantiate the jail canvas
        Jail jail = Instantiate(Resources.Load("Canvas/CanvasJail") as GameObject).GetComponent<Jail>();
        jail.InitCanvas(_player);
    }
    public void showCanvasManage(Player _player)
    {
        // Load and instantiate the CanvasManage prefab
        GameObject canvasManage = Resources.Load<GameObject>("Canvas/CanvasManage");
        GameObject canvasManageInstance = Instantiate(canvasManage);

        // Get the WcenterManage component from the instantiated prefab
        WcenterManage manageWidget = canvasManageInstance.GetComponent<WcenterManage>();

        // Initialize the WcenterManage widget, passing both the player and the WcenterManage instance
        manageWidget.InitWidget(_player, manageWidget);
    }
    public void showCanvasSafe(int potAmount)
    {
        GameObject safeCanvas = Instantiate(Resources.Load("Canvas/CanvasSafe") as GameObject);
        WcenterSafe safeWidget = safeCanvas.GetComponent<WcenterSafe>();
        safeWidget.InitWidget(potAmount); // Pass the pot amount to the widget
    }

    public void showCanvasBuild(soSpot _soSpot)
    {
        GameObject canvasBuild = Instantiate(Resources.Load("Canvas/CanvasBuild") as GameObject);
        WcenterBuild buildWidget = canvasBuild.GetComponent<WcenterBuild>();
        buildWidget.InitWidget(_soSpot);

    }

}
