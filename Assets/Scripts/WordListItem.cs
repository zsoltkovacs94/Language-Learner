using TMPro;
using UnityEngine;

// A szavak list�z�sakor egy megjelen�tend� karakter-sz� p�rost reprezent�l
public class WordListItem : MonoBehaviour
{
    // A karakter megjelen�t�s��rt felel�s TextMeshProUGUI komponens
    public TextMeshProUGUI charText;
    // A kiejt�s megjelen�t�s��rt felel�s TextMeshProUGUI komponens
    public TextMeshProUGUI pronText;
    // Inicializ�lja a TextMeshProUGUI komponenseket a megfelel� sz�veggel
    public void InitWordListItem(string asianChar, string pronunciation)
    {
        charText.text = asianChar;
        pronText.text = pronunciation;

    }
}
