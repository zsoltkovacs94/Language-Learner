using TMPro;
using UnityEngine;

// A set dictionary men�pont listaelemeit reprezent�lja
public class ListItem : MonoBehaviour
{
    // Index
    private int index;
    // F�jln�v
    private string filename;
    // Ki van-e a f�jl v�lasztva
    public bool isSelected;
    // A f�jln�v megjelen�t�s��rt felel�s TextMeshProUGUI komponens
    public TextMeshProUGUI filenameText;
    // A gomb sz�veg�nek TextMeshProUGUI komponense
    public TextMeshProUGUI buttonText;
    // Az utols� h�ny karaktert jelen�tse meg a f�jln�v sz�vege
    const int LASTX = 32;
    
    // Be�ll�tja az alapvet� adatokat
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
    // Visszaadja a ListItem index�t
    public int GetIndex() 
    { 
        return index;
    }
    // Visszaadja a ListItem f�jlnev�t
    public string GetFileName()
    {
        return filename;
    }
    // A gombra kattint�skor megfordul a kiv�laszt�s st�tusza �s a megfelel� jel ker�l be�r�sra a gomb sz�veg�be
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
