
using TMPro;
using UnityEngine;

public class RuleMenu : MonoBehaviour
{
    public soRuleMenu dropdownData;  
    public RectTransform dropdownContainer;  
    public GameObject dropdownPrefab;  

    private void Start()
    {
        PopulateDropdowns();
    }

    void PopulateDropdowns()
    {
        for (int i = 0; i < dropdownData.dropdowns.Length; i++)
        {
            soRuleMenu.DropdownInfo dropdownInfo = dropdownData.dropdowns[i];
            GameObject dropdownObj = Instantiate(dropdownPrefab, dropdownContainer);
            TMP_Dropdown dropdown = dropdownObj.GetComponentInChildren<TMP_Dropdown>();
            TMP_Text title = dropdownObj.GetComponentInChildren<TMP_Text>();

            if (dropdown == null || title == null)
            {
                Debug.LogError("Dropdown or Title missing from prefab!");
                continue;
            }
            title.text = dropdownInfo.dropdownTitle;
            dropdown.ClearOptions();
            dropdown.AddOptions(new System.Collections.Generic.List<string>(dropdownInfo.options));
            dropdown.value = dropdownInfo.defaultIndex; 

            int index = i; 
            dropdown.onValueChanged.AddListener((value) =>
            {
                dropdownData.dropdowns[index].defaultIndex = value;
            });
        }
    }

    public void OnBackClicked()
    {
        Debug.Log("<color=red>Back Clicked</color>");
        Destroy(this.gameObject);
    }

    public void OnResetClicked()
    {
        Debug.Log("<color=yellow>reset Clicked</color>");
    }

}
