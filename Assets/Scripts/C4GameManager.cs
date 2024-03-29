using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class C4GameManager : MonoBehaviour
{
    C4Model c4Model;
    public TextMeshProUGUI currentPlayer;
    public GameObject board;
    public GameObject piecePrefab;
    public GameObject animPiece;
    public GameObject[] xAnchors;
    public GameObject[] YAnchors;
    private GameObject[] pieces = new GameObject[42];
    public TMP_InputField inputField;
    public TextMeshProUGUI pronunciation;
    public TextMeshProUGUI lastSolution;
    public TextMeshProUGUI lastSolution2;
    public GameObject gameContent;
    public TextMeshProUGUI scoreText;
    public DictionaryHandler dh;
    public GameObject gameEndScreen;
    public Sprite minimizeSprite;
    public Sprite maximizeSprite;
    public GameObject minimizeButton;
    WordTile[] wordTiles;
    int wordCount;
    bool gameBegun = false;
    bool gameEnded = false;
    public WordTile[] currentTiles = new WordTile[7];
    public TextMeshProUGUI[] charTexts;
    bool solutionLock = false;
    bool vsAI = false;
    bool addAchi = true;
    public GameObject[] cSelectButtons;
    public GameObject classroomButton;
    public GameObject classroomPanel;
    GameObject soundManager;
    //bool soundPlayed = false;
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
            classroomButton.SetActive(true);
        }
        Debug.Log("C4GM started");
        LoadFromPrefs();
        CreateWordTileList();
        c4Model = new C4Model();
        int versusAI = PlayerPrefs.GetInt("versusAI");
        if(versusAI == 0)
        {
            vsAI = true;
        }
        else
        {
            vsAI = false;
        }
        InitBoard();
    }
    private void FixedUpdate()
    {
        inputField.Select();
        inputField.ActivateInputField();
        SetCurrentPlayer();
        if (CheckFull())
        {
            StartCoroutine(ShowGameEnd(true));
            gameEnded = true;
        }
        if (gameBegun && !gameEnded)
        {
            if (!solutionLock)
            {
                if (vsAI && c4Model.GetTurn() == '2')
                {
                    solutionLock = true;
                    PlaceRandom();
                }
                int solutionIndex = IsSolution(inputField.text);
                if (-1 < solutionIndex)
                {
                    solutionLock = true;
                    switch (c4Model.Put(solutionIndex))
                    {
                        case 0:
                            soundManager.GetComponent<SoundManager>().PlayCorrect();
                            lastSolution2.text = lastSolution.text;
                            lastSolution.text = currentTiles[solutionIndex].GetPronunciation();
                            RefreshState(currentTiles[solutionIndex].GetCharacter());
                            SetNewLabel(solutionIndex);
                            break;
                        case 1:
                            lastSolution2.text = lastSolution.text;
                            lastSolution.text = currentTiles[solutionIndex].GetPronunciation();
                            RefreshState(currentTiles[solutionIndex].GetCharacter());
                            StartCoroutine(ShowGameEnd(false));
                            gameEnded = true;
                            break;
                        default:
                            soundManager.GetComponent<SoundManager>().PlayWrong();
                            inputField.text = null;
                            solutionLock = false;
                            break;
                    }
                }
                else if (inputField.text == "---")
                {
                    inputField.text = null;
                    solutionLock = true;
                    PlaceRandom();
                }
            }
        }
        else
        {
            if (wordCount < 8)
            {
                PlayerPrefs.SetString("ErrorText", "Select a file with at least 8 characters");
                PlayerPrefs.SetString("ErrorResolved", "false");
                Debug.Log("C4 not enough chars");
                BTM();
            }
            SetLabels();
            gameBegun = true;
        }
    }
    private void PlaceRandom()
    {
        soundManager.GetComponent<SoundManager>().PlayCorrect();
        int randomIndex = Random.Range(0, 7);
        int status = c4Model.Put(randomIndex);
        if (status == -1)
        {
            for (int i = 0; i < 7; i++)
            {
                if (c4Model.GetStateAt(0, i) == '0')
                {
                    status = c4Model.Put(i);
                    if (status == 1)
                    {
                        lastSolution2.text = lastSolution.text;
                        lastSolution.text = currentTiles[i].GetPronunciation();
                        RefreshState(currentTiles[i].GetCharacter());
                        StartCoroutine(ShowGameEnd(false));
                        gameEnded = true;
                    }
                    else
                    {
                        lastSolution2.text = lastSolution.text;
                        lastSolution.text = currentTiles[i].GetPronunciation();
                        RefreshState(currentTiles[i].GetCharacter());
                        SetNewLabel(i);
                    }
                    break;
                }
            }
        }
        else if (status == 1)
        {
            lastSolution2.text = lastSolution.text;
            lastSolution.text = currentTiles[randomIndex].GetPronunciation();
            RefreshState(currentTiles[randomIndex].GetCharacter());
            StartCoroutine(ShowGameEnd(false));
            gameEnded = true;
        }
        else
        {
            lastSolution2.text = lastSolution.text;
            lastSolution.text = currentTiles[randomIndex].GetPronunciation();
            RefreshState(currentTiles[randomIndex].GetCharacter());
            SetNewLabel(randomIndex);
        }
    }
    private bool CheckFull()
    {
        int full = 0;
        for (int i = 0; i < 7; i++)
        {
            if(c4Model.GetStateAt(0,i) != '0') full++;
        }
        if(full == 7)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private IEnumerator ShowGameEnd(bool tie)
    {
        yield return new WaitForSeconds(1);
        soundManager.GetComponent<SoundManager>().PlayCompleted();
        classroomButton.SetActive(false);
        classroomPanel.SetActive(false);
        HideButtons();
        if (addAchi)
        {
            if (vsAI)
            {
                int vai = PlayerPrefs.GetInt("Connect4PlayedversusAI");
                vai += 1;
                PlayerPrefs.SetInt("Connect4PlayedversusAI", vai);
                Debug.Log(vai);
            }
            int vp = PlayerPrefs.GetInt("VersusPlayed");
            vp += 1;
            PlayerPrefs.SetInt("VersusPlayed", vp);
            Debug.Log(vp);
            int cp = PlayerPrefs.GetInt("Connect4Played");
            cp += 1;
            PlayerPrefs.SetInt("Connect4Played", cp);
            Debug.Log(cp);
        }
        gameContent.SetActive(false);
        gameEndScreen.SetActive(true);
        if (tie)
        {
            scoreText.text = "Tie!";
            if(addAchi)
            {
                int t = PlayerPrefs.GetInt("Connect4Ties");
                t += 1;
                PlayerPrefs.SetInt("Connect4Ties", t);
                Debug.Log(t);
            }
        }
        else
        {
            char winner = c4Model.GetWinner();
            scoreText.text = "Player" + winner + " won!";
            if(winner == '1' && addAchi)
            {
                int pw = PlayerPrefs.GetInt("Connect4Player1Wins");
                pw += 1;
                PlayerPrefs.SetInt("Connect4Player1Wins", pw);
                Debug.Log(pw);
            }
            else if(addAchi)
            {
                int pw = PlayerPrefs.GetInt("Connect4Player2Wins");
                pw += 1;
                PlayerPrefs.SetInt("Connect4Player2Wins", pw);
                Debug.Log(pw);
            }
        }
        addAchi = false;
    }
    private void SetNewLabel(int index)
    {
        currentTiles[index] = GetRandomTile();
        charTexts[index].text = currentTiles[index].GetCharacter();
    }
    private int IsSolution(string s)
    {
        for (int i = 0; i < 7; i++)
        {
            if(s == currentTiles[i].GetPronunciation())
            {
                return i;
            }
        }
        return -1;
    }
    private void SetLabels()
    {
        GetChars();
        for (int i = 0; i < 7; i++) 
        {
            charTexts[i].text = currentTiles[i].GetCharacter();
        }
    }
    private void GetChars()
    {
        for (int i = 0; i < currentTiles.Length; i++)
        {
            currentTiles[i] = GetRandomTile();
        }
    }
    private WordTile GetRandomTile()
    {
        int randomIndex;
        string randomChar;
        string foundChar;
        do
        {
            randomIndex = Random.Range(0, wordCount);
            randomChar = wordTiles[randomIndex].GetCharacter();
            foundChar = "";
            foreach (WordTile wt in currentTiles)
            {
                if (wt == null) continue;
                if (wt.GetCharacter() == randomChar)
                {
                    foundChar = wt.GetCharacter();
                    break;
                }
            }
        } while (randomChar == foundChar);
        return wordTiles[randomIndex];
    }
    private void SetCurrentPlayer()
    {
        currentPlayer.text = "Current player: Player" + c4Model.GetTurn();
        if (c4Model.GetTurn() == '1') currentPlayer.color = Color.red;
        else currentPlayer.color = Color.blue;
    }
    private void InitBoard()
    {
        for(int i = 0; i < 42; i++)
        {
            GameObject nGO = Instantiate(piecePrefab);
            nGO.transform.SetParent(board.transform);
            nGO.transform.localScale = Vector3.one;
            nGO.GetComponent<PieceBehaviour>().Set(Color.gray,"");
            pieces[i] = nGO;
        }
        ClearInput();
    }
    private void RefreshState(string character)
    {
        string[] cords = c4Model.GetLastPut().Split(',');
        int c1 = int.Parse(cords[0]);
        int c2 = int.Parse(cords[1]);
        char p = c4Model.GetStateAt(c1, c2);
        float duration = 1f / 6f + (float)c1 / 6f;
        StartCoroutine(ShowResult(p, c1 * 7 + c2, duration, character));
        StartCoroutine(PlayAnim(p, c1, c2, duration,character));
    }
    private IEnumerator PlayAnim(char p, int i, int j, float duration, string character) 
    {
        animPiece.SetActive(true);
        if (p == '1')
            animPiece.GetComponent<PieceBehaviour>().Set(Color.red,character);
        else if (p == '2')
            animPiece.GetComponent<PieceBehaviour>().Set(Color.blue,character);
        float time = 0f;
        animPiece.transform.position = xAnchors[j].transform.position + new Vector3(0f, 50f, 0f);
        float startY = animPiece.transform.position.y;
        float endY = YAnchors[i].transform.position.y;
        while (time < duration)
        {
            float moveBy = Mathf.Lerp(startY, endY, time / duration);
            animPiece.transform.position = new Vector3(animPiece.transform.position.x, moveBy, animPiece.transform.position.z);
            time += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator ShowResult(char p, int i, float duration, string character)
    {
        yield return new WaitForSeconds(duration);
        animPiece.SetActive(false);
        if (p == '1')
        {
            pieces[i].GetComponent<PieceBehaviour>().Set(Color.red,character);
        }
        else if (p == '2')
        {
            pieces[i].GetComponent<PieceBehaviour>().Set(Color.blue,character);
        }
        solutionLock = false;
        ClearInput();
    }
    private void ClearInput()
    {
        inputField.Select();
        inputField.text = null;
        inputField.ActivateInputField();
    }
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
    public void BTM()
    {
        SceneManager.LoadScene(0);
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
        if (classroomPanel.activeSelf)
        {
            classroomPanel.SetActive(false);
        }
        else
        {
            classroomPanel.SetActive(true);
        }
    }
    public void ClassroomSolve()
    {
        foreach(GameObject go in cSelectButtons)
        {
            go.SetActive(true);
        }
    }
    public void ClassroomSkip()
    {
        HideButtons();
        if (!solutionLock)
        {
            inputField.text = "---";
        }
    }
    public void cSolve1()
    {
        inputField.text = currentTiles[0].GetPronunciation();
        HideButtons();
    }
    public void cSolve2()
    {
        inputField.text = currentTiles[1].GetPronunciation();
        HideButtons();
    }
    public void cSolve3()
    {
        inputField.text = currentTiles[2].GetPronunciation();
        HideButtons();
    }
    public void cSolve4()
    {
        inputField.text = currentTiles[3].GetPronunciation();
        HideButtons();
    }
    public void cSolve5()
    {
        inputField.text = currentTiles[4].GetPronunciation();
        HideButtons();
    }
    public void cSolve6()
    {
        inputField.text = currentTiles[5].GetPronunciation();
        HideButtons();
    }
    public void cSolve7()
    {
        inputField.text = currentTiles[6].GetPronunciation();
        HideButtons();
    }
    private void HideButtons()
    {
        foreach (GameObject go in cSelectButtons)
        {
            go.SetActive(false);
        }
    }
    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
