using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Options : MonoBehaviour
{
    public soOptionData optionsData;
    public RectTransform grp_OptionsButtons;
    public GameObject dropdownPrefab;

    public Slider musicSlider;
    public Slider effectsSlider;

    [SerializeField] private string inputText;



    private void Start()
    {
        musicSlider.value = AudioManager.Instance.volume[(int)eMixers.music];
        PopulateDropdowns();
    }

    void PopulateDropdowns()
    {
        for (int i = 0; i < optionsData.dropdowns.Length; i++)
        {
            soOptionData.DropdownInfo dropdownInfo = optionsData.dropdowns[i];
            GameObject dropdownObj = Instantiate(dropdownPrefab, grp_OptionsButtons);
            TMP_Dropdown dropdown = dropdownObj.GetComponentInChildren<TMP_Dropdown>();
            TMP_Text title = dropdownObj.GetComponentInChildren<TMP_Text>();
            title.text = dropdownInfo.dropdownTitle;
            dropdown.ClearOptions();
            dropdown.AddOptions(new System.Collections.Generic.List<string>(dropdownInfo.options));
            dropdown.value = dropdownInfo.selectedIndex;
            int index = i;
            dropdown.onValueChanged.AddListener((value) =>
            {
                optionsData.dropdowns[index].selectedIndex = value;
            });
        }
    }

    public void OnMusicVolumeChanged(float _value)
        {
            Debug.Log("<color=yellow>Music Volume Changed</color>");
            AudioManager.Instance.SetMixerLevel(eMixers.music, _value);
        }
        public void OnEffectsVolumeChanged(float _value)
        {
            Debug.Log("<color=yellow>Effects Volume Changed</color>");
        }

        public void OnBackClick()
        {
            Debug.Log("<color=yellow>Back Clicked</color>");
            Destroy(this.gameObject);
        }

    public void SetPlayerName(string input)
    {
        inputText = input;
    }

}

