using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PieceBehaviour : MonoBehaviour
{
    public Image circle;
    public TextMeshProUGUI charText;
    public Sprite GToken;
    public Sprite RToken;
    public Sprite BToken;
    private string character;
    public void Set(Color color, string character)
    {
        if(color == Color.red)
        {
            circle.sprite = RToken;
        }
        else if(color == Color.blue)
        {
            circle.sprite = BToken;
        }
        else
        {
            circle.sprite = GToken;
        }
        this.character = character;
        charText.text = character;
    }
    public Color GetColor()
    {
        return circle.color;
    }
}
