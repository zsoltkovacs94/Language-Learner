using TMPro;
using UnityEngine;

// A set dictionary menüpont listaelemeit reprezentálja
public class ListItem : MonoBehaviour
{
    // Index
    private int index;
    // Fájlnév
    private string filename;
    // Ki van-e a fájl választva
    public bool isSelected;
    // A fájlnév megjelenítéséért felelõs TextMeshProUGUI komponens
    public TextMeshProUGUI filenameText;
    // A gomb szövegének TextMeshProUGUI komponense
    public TextMeshProUGUI buttonText;
    // Az utolsó hány karaktert jelenítse meg a fájlnév szövege
    const int LASTX = 32;
    
    // Beállítja az alapvetõ adatokat
    public void InitListItem(int index, string filename, bool selected)
    {
        this.index = index;
        this.filename = filename;
        isSelected = selected;
        if(isSelected)
        {
            buttonText.text = "X";
        }
        else
        {
            buttonText.text = "";
        }
        filenameText.text = "..." + filename.Substring(filename.Length-LASTX);
    }
    // Visszaadja a ListItem indexét
    public int GetIndex() 
    { 
        return index;
    }
    // Visszaadja a ListItem fájlnevét
    public string GetFileName()
    {
        return filename;
    }
    // A gombra kattintáskor megfordul a kiválasztás státusza és a megfelelõ jel kerül beírásra a gomb szövegébe
    public void Click()
    {
        if(isSelected)
        {
            buttonText.text = "";
        }
        else
        {
            buttonText.text = "X";
        }
        isSelected = !isSelected;
    }
}
