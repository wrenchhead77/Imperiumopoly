using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Wmanage : MonoBehaviour
{
    [SerializeField] private Button propButton; // Reference to the button
    [SerializeField] private TMP_Text propName; // Reference to the text displaying property name

    private PropertyOwnership propertyData;     // Data for the property
    private WcenterManage parentManager;       // Reference to the WcenterManage script

    public void Init(PropertyOwnership data, WcenterManage manager)
    {
        propertyData = data;
        parentManager = manager;
        propName.text = propertyData.spotName;
        propButton.onClick.RemoveAllListeners();    
        propButton.onClick.AddListener(OnPropertyClicked);
    }

    private void OnPropertyClicked()
    {
        parentManager.OnPropertySelected(propertyData);
    }
    private void OnBuildPressed()
    {
        GameObject buildCanvas = Instantiate(Resources.Load("Canvas/CanvasBuild") as GameObject);
        WcenterBuild wcenterBuild = buildCanvas.GetComponent<WcenterBuild>();        
    }
}
