using TMPro;
using UnityEngine;

// A szavak listázásakor egy megjelenítendõ karakter-szó párost reprezentál
public class WordListItem : MonoBehaviour
{
    // A karakter megjelenítéséért felelõs TextMeshProUGUI komponens
    public TextMeshProUGUI charText;
    // A kiejtés megjelenítéséért felelõs TextMeshProUGUI komponens
    public TextMeshProUGUI pronText;
    // Inicializálja a TextMeshProUGUI komponenseket a megfelelõ szöveggel
    public void InitWordListItem(string asianChar, string pronunciation)
    {
        charText.text = asianChar;
        pronText.text = pronunciation;

    }
}
