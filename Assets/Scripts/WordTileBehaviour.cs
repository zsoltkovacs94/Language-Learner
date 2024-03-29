using System;
using TMPro;
using UnityEngine;

// A j�t�k k�zben megjelen� WordTile-ok viselked�s�t �rja le
public class WordTileBehaviour : MonoBehaviour
{
    // A megjelen�tend� karakter TextMeshProUGUI komponense
    public TextMeshProUGUI character;
    // A megjelen�tend� kiejt�s TextMeshProUGUI komponense
    public TextMeshProUGUI pronunciation;
    // A v�lasz megad�s�ra szolg�l� input field komponens
    public TMP_InputField inputField;
    // Learning m�dban megjelen�ti a kiejt�st a j�t�kosnak
    public TextMeshProUGUI pronunciationHelper;
    // Bels� WordTile oszt�ly
    public WordTile wt;
    // H�ny seg�ts�get kapott a j�t�kos
    int hintCounter = 0;
    // Tanul� m�dban vagyunk-e
    bool isLearning;
    // Be�ll�tja a bels� WordTile oszt�lyt �s, hogy tanul� m�dot ind�tott-e a j�t�kos
    GameObject soundManager;
    bool soundPlayed = false;
    public void InitWordTile(WordTile wt, bool isLearning)
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager");
        this.isLearning = isLearning;
        this.wt = wt;
        character.text = wt.GetCharacter();
        inputField.Select();
        inputField.ActivateInputField();
        if (isLearning)
        {
            Debug.Log("Learning mode");
            pronunciationHelper.text = wt.GetPronunciation();
        }
    }
    // Helyes-e a jelenleg be�rt megold�s
    public bool isCorrect()
    {
        // "---" be�r�sakor �tugorjuk a jelenlegi tile-t, de erre csak practice m�dban van lehet�s�g
        if (!isLearning && pronunciation.text.Substring(0, pronunciation.text.Length - 1).ToLower() == "---")
        {
            Skip();
            return true;
        }
        // Ha helyes a be�rt v�lasz
        else if(pronunciation.text.Substring(0,pronunciation.text.Length-1).ToLower() == wt.GetPronunciation())
        {
            if (!soundPlayed)
            {
                soundPlayed = true;
                soundManager.GetComponent<SoundManager>().PlayCorrect();
            }
            ChangeColor(Color.green);
            return true;
        }
        // Ha a be�rt v�lasz olyan hossz�, mint a helyes v�lasz, de nem helyes
        else if ((pronunciation.text.Length-1 == wt.GetPronunciation().Length))
        {
            if (!soundPlayed)
            {
                soundPlayed = true;
                soundManager.GetComponent<SoundManager>().PlayWrong();
            }
            ChangeColor(Color.red);
        }
        // Ha a be�rt v�lasz r�videbb, mint a helyes v�lasz
        else if ((pronunciation.text.Length - 1 < wt.GetPronunciation().Length))
        {
            soundPlayed = false;
            ChangeColor(Color.white);
        }
        // B�rmi m�s
        return false;
    }
    // Be�ll�tja a beg�pelt v�lasz sz�n�t a megadott sz�nre
    void ChangeColor(Color c)
    {
        pronunciation.color = c;
    }
    // Ugorja a jelenlegi tile-t
    public void Skip()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<TrainingGameManager>().Skip(character.text);
    }
    // Visszaadja a megold�s hossz�t
    public int GetLength()
    {
        return wt.GetPronunciation().Length;
    }
    // Seg�ts�g visszaadja az els� seg�ts�gek sz�ma + 1 karaktert, majd n�veli a seg�ts�gek sz�m�t
    public string GetHint()
    {
        if(hintCounter != wt.GetPronunciation().Length)
            hintCounter++;
        return wt.GetPronunciation().Substring(0,hintCounter).ToLower();
    }
    public string GetPronunciation()
    {
        return wt.GetPronunciation();
    }
}
