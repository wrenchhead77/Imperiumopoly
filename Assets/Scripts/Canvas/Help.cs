using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Help : MonoBehaviour
{
    public void OnBackClick()
    {
        Debug.Log("<color=yellow>Back Clicked</color>");
        Destroy(this.gameObject);
    }


}
