using TMPro;
using UnityEngine;

public class AchiItem : MonoBehaviour
{
    public TextMeshProUGUI achiNameText;
    public TextMeshProUGUI achiValueText;
    public void Init(string achiName, string achiValue)
    {
        achiNameText.text = achiName;
        achiValueText.text = achiValue;
    }
}
