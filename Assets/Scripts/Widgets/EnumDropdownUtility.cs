using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class EnumDropdownUtility
{
    public static void PopulateDropdown<T>(TMP_Dropdown dropdown, T selectedValue) where T : System.Enum
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>();

        foreach (T value in System.Enum.GetValues(typeof(T)))
        {
            options.Add(value.ToString());
        }

        dropdown.AddOptions(options);
        dropdown.value = System.Array.IndexOf(System.Enum.GetValues(typeof(T)), selectedValue);
        dropdown.RefreshShownValue();
    }

    public static T GetSelectedEnum<T>(TMP_Dropdown dropdown) where T : System.Enum
    {
        return (T)System.Enum.GetValues(typeof(T)).GetValue(dropdown.value);
    }
}
