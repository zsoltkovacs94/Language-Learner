using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * Training mód menetét leíró fájl
 */
public class TrainingGameManager : MonoBehaviour
{
    // Használandó szavak
    public DictionaryHandler dh;
    // WordTile prefab
    public GameObject wordTile;
    // A WordTile-t megjelenítő canvas
    public GameObject canvas;
    // Jelenlegi tile gameobjectje
    GameObject currentTile;
    // Törlendő tile gameobjectje
    GameObject oldTile;
    // Játék vége panel
    public GameObject gameEndScreen;
    // A pontot megjelenítő TextMeshProUGUI komponens
    public TextMeshProUGUI scoreText;
    // A tippeket megjelenítő ablak
    public GameObject hintWindow;
    // A tippek szövege
    public TextMeshProUGUI hintText;
    // A szótárfájlból alkotott WordTileok listája
    WordTile[] wordTiles;
    // Legutóbb melyik karakter lett skipelve
    string latestSkip;
    // Eltalált karakterek száma, csakis practice módban
    int correctCounter = 0;
    // Eddig mejelenített karakterek száma
    int counter = 0;
    // Összes karakter számas
    int wordCount;
    // Zárolja új tile létrehozását
    bool creationLock = false;
    // A játék kezdetét jelzi
    bool gameBegun = false;
    // A játék végét jelzi
    bool gameEnded = false;
    // A tanuló módot jelzi
    bool learning;
    // Betöltjük a kiválasztott fájlokat, létrehozzuk a wordtileokat és beállítjuk, hogy milyen módot indított a játékos
    bool endShown = false;
    public TextMeshProUGUI lastSolution;
    public Sprite minimizeSprite;
    public Sprite maximizeSprite;
    public GameObject minimizeButton;
    public GameObject rightButton;
    public GameObject rightPanel;
    public GameObject leftButton;
    public GameObject leftPanel;
    public GameObject skipRButton;
    public GameObject skipLButton;
    bool cSolve = false;
    GameObject soundManager;
    bool soundPlayed = false;
    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager");
        if (!Screen.fullScreen)
        {
            minimizeButton.GetComponent<Image>().sprite = maximizeSprite;
        }
        else
        {
            minimizeButton.GetComponent<Image>().sprite = minimizeSprite;
        }
        if (PlayerPrefs.GetInt("classroomMode") == 0)
        {
            rightButton.SetActive(true);
            leftButton.SetActive(true);
        }
        Debug.Log("GameManager started");
        LoadFromPrefs();
        CreateWordTileList();
        if(PlayerPrefs.GetInt("TrainingMode") == 2)
        {
            Debug.Log("Practice mode");
            learning = false;
        }
        else
        {
            Debug.Log("Learning mode");
            learning = true;
            skipLButton.GetComponent<Button>().interactable = false;
            skipRButton.GetComponent<Button>().interactable = false;
            lastSolution.gameObject.SetActive(false);
        }
    }
    // A játék lefolyása
    private void FixedUpdate()
    {
        if(currentTile != null)
        {
            currentTile.GetComponent<WordTileBehaviour>().inputField.Select();
            currentTile.GetComponent<WordTileBehaviour>().inputField.ActivateInputField();
        }
        //Debug.Log(counter + "/" + wordCount);
        // Ha a megjelenített szavak száma egyenlő az összes szó számával, akkor vége a játéknak
        if (wordCount == counter) gameEnded = true;
        // Ha a játék már elkezdődött, de még nem ért véget
        if (gameBegun && !gameEnded)
        {
            // Ha a megadott válasz helyes és nincs létrehozási zár aktiválva
            if (((currentTile.GetComponent<WordTileBehaviour>().isCorrect() && !creationLock) || cSolve))
            {
                cSolve = false;
                if (!learning)
                {
                    lastSolution.text = "Last solution: " + currentTile.GetComponent<WordTileBehaviour>().GetPronunciation();
                }
                correctCounter++;
                oldTile = currentTile;
                // 1 másodperc múlva törli a jelenlegi tilet
                Invoke("DestroyCurrentTile", 1);
                // Elrejti a tippablakot
                hintWindow.SetActive(false);
                // Visszavonja a későbbre időzített tippmutatást
                CancelInvoke();
                counter++;
                Debug.Log(counter);
                // Ha ezután még nincs vége a játéknak, akkor létrehozási zárat aktiválja és 1 másodperc múlva létrehoz egy új tilet
                if (counter < wordCount)
                {
                    creationLock = true;
                    Invoke("GenerateNewTile", 1);
                }
            }
        }
        // Ha a játék még nem kezdődött el, de nem is ért véget, majd egy rövid leírás megjelenítésére kell
        else if(!gameEnded)
        {
            // A játék első tilejának létrehozása
            GenerateNewTile();
            gameBegun = true;
        }
        // Ha a játék elkezdődött és már véget is ért
        else
        {
            // Az összes időzített függvényhívást visszavonja
            if (!endShown)
            {
                CancelInvoke();
                // Megjeleníti a játék vége panelt
                Invoke("ShowGameEnd", 1);
                // A módhoz igazítja a kiírnadó szöveget
                if (learning)
                {
                    scoreText.text = "You learned " + wordCount + " words";
                }
                else
                {
                    scoreText.text = "Your score: " + correctCounter + "/" + wordCount;
                }
                endShown = true;
            }
        }
    }
    // Kiválasztja az összes kiválasztott fájlt
    public void LoadFromPrefs()
    {
        dh = new DictionaryHandler();
        dh.DeSelectAll();
        int count = PlayerPrefs.GetInt("SNumber");
        for (int i = 0; i < count; i++)
        {
            dh.Select(PlayerPrefs.GetString("SName" + i));
        }
    }
    // Létrehoz egy új wordtilet a képernyőn
    public void GenerateNewTile()
    {
        currentTile = Instantiate(wordTile, canvas.transform.position, Quaternion.identity);
        currentTile.GetComponent<WordTileBehaviour>().InitWordTile(wordTiles[counter], learning);
        currentTile.transform.SetParent(canvas.transform);
        currentTile.transform.localScale = Vector3.one;
        Debug.Log("Generated " + currentTile.GetComponent<WordTileBehaviour>().character.text);
        creationLock = false;
        // 3, majd 7 másodperc múlva mutat egy tippet
        if(!learning)
        {
            Invoke("NextHint", 3);
            Invoke("NextHint", 7);
        }
    }
    // Törli a törlendő wordtilet
    public void DestroyCurrentTile()
    {
        Destroy(oldTile);
    }
    // Feltölti a szótárosztályt szavakkal, majd beállítja a szavak számát és létrehozza a wordTiles tömböt a megfelelő szavakkal
    public void CreateWordTileList()
    {
        dh.FillDictionary();
        wordCount = dh.dictionary.Keys.Count;
        wordTiles = new WordTile[wordCount];
        int i = 0;
        foreach (string key in dh.dictionary.Keys)
        {
            wordTiles[i] = new WordTile(key, dh.dictionary[key]);
            i++;
        }
    }
    // Vissza a menübe gomb működése
    public void BTM()
    {
        SceneManager.LoadScene(0);
    }
    // Átugorja a jelenlegi tilet, amiért a játékos nem kap pontot
    public void Skip(string skippedText)
    {
        if(latestSkip!=skippedText)
        {
            correctCounter--;
            latestSkip = skippedText;
        }
    }
    // Megjeleníti a játék vége panelt
    void ShowGameEnd()
    {
        if (!soundPlayed)
        {
            soundPlayed = true;
            soundManager.GetComponent<SoundManager>().PlayCompleted();
        }
        rightButton.SetActive(false);
        rightPanel.SetActive(false);
        leftButton.SetActive(false);
        leftPanel.SetActive(false);
        // Törli az összes képernyőn lévő tilet
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tiles)
        {
            Destroy(tile);
        }
        int tp = PlayerPrefs.GetInt("TrainingPlayed");
        tp += 1;
        PlayerPrefs.SetInt("TrainingPlayed", tp);
        Debug.Log(tp);
        if(learning)
        {
            int lp = PlayerPrefs.GetInt("LearnPlayed");
            lp += 1;
            PlayerPrefs.SetInt("LearnPlayed", lp);
            Debug.Log(lp);
            List<string> learnedWords = new List<string>();
            foreach (WordTile wt in wordTiles)
            {
                learnedWords.Add(wt.GetCharacter());
            }
            LearnedCharManager lcm = new LearnedCharManager();
            lcm.Add(learnedWords);
        }
        else
        {
            int pp = PlayerPrefs.GetInt("PracticePlayed");
            pp += 1;
            PlayerPrefs.SetInt("PracticePlayed", pp);
            Debug.Log(pp);
        }
        gameEndScreen.SetActive(true);
    }
    // Mutatja a következő tippet
    void NextHint()
    {
        hintWindow.SetActive(true);
        string hint = currentTile.GetComponent<WordTileBehaviour>().GetHint();
        StringBuilder sb = new StringBuilder();
        foreach (char c in hint)
        {
            sb.Append(c).Append(" ");
        }
        for (int i = hint.Length; i < currentTile.GetComponent<WordTileBehaviour>().GetLength(); i++)
        {
            sb.Append("_ ");
        }
        hintText.text = sb.ToString();
    }
    public void ToggleFS()
    {
        if (!Screen.fullScreen)
        {
            minimizeButton.GetComponent<Image>().sprite = minimizeSprite;
            Screen.SetResolution(Screen.currentResolution.width * 2, Screen.currentResolution.height * 2, true);
        }
        else
        {
            minimizeButton.GetComponent<Image>().sprite = maximizeSprite;
            Screen.SetResolution(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2, false);
        }
    }
    public void ShowRightPanel()
    {
        if(rightPanel.activeSelf)
        {
            rightPanel.SetActive(false);
        }
        else
        {
            rightPanel.SetActive(true);
            leftPanel.SetActive(false);
        }
    }
    public void ShowLeftPanel()
    {
        if (leftPanel.activeSelf)
        {
            leftPanel.SetActive(false);
        }
        else
        {
            rightPanel.SetActive(false);
            leftPanel.SetActive(true);
        }
    }
    public void ClassroomSolve()
    {
        if(!creationLock)
        {
            currentTile.GetComponent<WordTileBehaviour>().inputField.text = currentTile.GetComponent<WordTileBehaviour>().wt.GetPronunciation();
        }
    }
    public void ClassroomSkip()
    {
        currentTile.GetComponent<WordTileBehaviour>().inputField.text = "---";
    }
    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
