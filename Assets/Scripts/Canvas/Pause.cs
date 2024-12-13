using UnityEngine;

public class Pause : MonoBehaviour
{
    public void OnSaveAndExitPress()
    {
        Popup popup = new Popup(
            _sender: this.gameObject,
            _confirmAction: nameof(SaveAndExit),  // Pass the method to call on confirmation
            _cancelAction: null,                 // No action needed on cancel
            _title: "Save And Exit?",
            _message: "Are you sure you want to save and exit the game?",
            _confirmText: "Yes",
            _cancelText: "No"
        );

        // Instantiate the CanvasMessage prefab
        GameObject obj = Instantiate(Resources.Load("Canvas/CanvasMessage") as GameObject);
        Message message = obj.GetComponent<Message>();
        message.InitCanvas(popup);
    }
    public void OnAbandonPress()
    {
        Popup popup = new Popup(
            _sender: this.gameObject,
            _confirmAction: nameof(Abandon),     // Pass the method to call on confirmation
            _cancelAction: null,                 // No action needed on cancel
            _title: "Abandon?",
            _message: "Are you sure you want to abandon the game?\nYour progress will not be saved.",
            _confirmText: "Yes",
            _cancelText: "No"
        );

        // Instantiate the CanvasMessage prefab
        GameObject obj = Instantiate(Resources.Load("Canvas/CanvasMessage") as GameObject);
        Message message = obj.GetComponent<Message>();

        if (message != null)
        {
            message.InitCanvas(popup);
        }
        else
        {
            Debug.LogError("Message component not found on CanvasMessage prefab.");
        }
    }

    public void SaveAndExit()
    {
        SceneMgr.Instance.LoadScene(eScene.FrontEnd);
    }

    public void Abandon()
    {
        SceneMgr.Instance.LoadScene(eScene.FrontEnd);
    }

    public void OnResumeClick()
    {
        Destroy(this.gameObject);
    }

    public void OnOptionsClick()
    {
        Options options = Instantiate(Resources.Load("Canvas/CanvasOptions")
            as GameObject).GetComponent<Options>();
    }

    public void OnHelpClick()
    {
        Help help = Instantiate(Resources.Load("Canvas/CanvasHelp")
            as GameObject).GetComponent<Help>();
    }

    public void OnRulesClick()
    {
        RuleMenu rules = Instantiate(Resources.Load("Canvas/CanvasHouseRules")
            as GameObject).GetComponent<RuleMenu>();
    }
}

