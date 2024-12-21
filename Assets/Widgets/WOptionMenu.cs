using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class WOptionMenu : MonoBehaviour
{
    public soOptionData so_OptionData;
    [SerializeField] private RectTransform grp_dropDowns;
    [SerializeField] private GameObject _dropDownWidget;

    public void InitWidget()
    {
        ShowButtons();
    }
    private void ShowButtons()
    {
        PersistentGameData data = PersistentGameData.Instance;

        for (int i = 0; i < so_OptionData.dropdowns.Length; i++)
        {
            soOptionData.DropdownInfo dropdownInfo1 = so_OptionData.dropdowns[i];
            GameObject dropdownObj = Instantiate(_dropDownWidget, grp_dropDowns.transform);
            TMP_Dropdown dropdown = dropdownObj.GetComponent<TMP_Dropdown>();

            dropdown.ClearOptions();
            dropdown.AddOptions(new List<string>(dropdownInfo1.options));
            dropdown.value = data.rules[i]; // Initialize from PersistentGameData

            int index = i; // Avoid closure issue
            dropdown.onValueChanged.AddListener(value =>
            {
                data.rules[index] = value;
            });
            Debug.Log($"Dropdown instantiated with title: {dropdownInfo1.dropdownTitle}");
        }
    }

    public void OnBackClicked()
    {
        Debug.Log("<color=red>Back Clicked</color>");
        Destroy(this.gameObject);
    }
}
