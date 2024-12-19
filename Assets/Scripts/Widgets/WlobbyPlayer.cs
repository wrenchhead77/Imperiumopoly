using UnityEngine;
using TMPro;

public class WlobbyPlayer : MonoBehaviour
{
    public TMP_Text playerNameText;



    public void SetPlayerName(string name)
    {
        playerNameText.text = name;
    }

}