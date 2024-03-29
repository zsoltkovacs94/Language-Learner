using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TTTGameManager : MonoBehaviour
{
    TTTModel tttModel;
    bool vsAI = false;
    public GameObject pieceHolder;
    public TextMeshProUGUI currentPlayer;
    public GameObject piecePrefab;
    public GameObject[] Anchors;
    public TMP_InputField inputField;
    public TextMeshProUGUI pronunciation;
    public TextMeshProUGUI lastSolution;
    public TextMeshProUGUI lastSolution2;
    public GameObject gameContent;
    public TextMeshProUGUI scoreText;
    public DictionaryHandler dh;
    public GameObject gameEndScreen;
    public TextMeshProUGUI[] chars;
    public WordTile[] currentTiles = new WordTile[9];
    public GameObject[] pieces;
    WordTile[] wordTiles;
    public Sprite minimizeSprite;
    public Sprite maximizeSprite;
    public GameObject minimizeButton;
    int wordCount;
    bool gameBegun = false;
    bool gameEnded = false;
    bool solutionLock = false;
    bool addAchi = true;
    public GameObject[] cSelectButtons;
    public GameObject classroomButton;
    public GameObject classroomPanel;
    float scale = 2.38806f;
    GameObject soundManager;
    //bool soundPlayed = false;
    private void Start()
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
        tttModel = new TTTModel();
        LoadFromPrefs();
        CreateWordTileList();
        int versusAI = PlayerPrefs.GetInt("versusAI");
        if (versusAI == 0)
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
        if (CheckFull() && !gameEnded)
        {
            if(tttModel.GetWinner() == '1' || tttModel.GetWinner() == '2')
            {
                StartCoroutine(ShowGameEnd(false));
                Debug.Log("Show game end called in check full");
                gameEnded = true;
            }
            else
            {
                StartCoroutine(ShowGameEnd(true));
                Debug.Log("Show game end called in check full tie");
                gameEnded = true;
            }
        }
        if (gameBegun && !gameEnded)
        {
            if(!solutionLock)
            {
                if (vsAI && tttModel.GetTurn() == '2')
                {
                    Debug.Log("CPU input");
                    solutionLock = true;
                    Invoke("PlaceRandom", 1f);
                }
                int solutionIndex = IsSolution(inputField.text);
                if(solutionIndex > -1)
                {
                    Debug.Log("Player input");
                    solutionLock = true;
                    int i = solutionIndex / 3;
                    int j = solutionIndex - (i * 3);
                    switch (tttModel.Put(i, j))
                    {
                        case 0:
                            soundManager.GetComponent<SoundManager>().PlayCorrect();
                            lastSolution2.text = lastSolution.text;
                            lastSolution.text = currentTiles[solutionIndex].GetPronunciation();
                            RefreshState(currentTiles[solutionIndex].GetCharacter());
                            break;
                        case 1:
                            soundManager.GetComponent<SoundManager>().PlayCorrect();
                            lastSolution2.text = lastSolution.text;
                            lastSolution.text = currentTiles[solutionIndex].GetPronunciation();
                            RefreshState(currentTiles[solutionIndex].GetCharacter());
                            StartCoroutine(ShowGameEnd(false));
                            Debug.Log("Show game end called in player put");
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
        else if(!gameBegun && !gameEnded)
        {
            if (wordCount < 9)
            {
                PlayerPrefs.SetString("ErrorText", "Select a file with at least 9 characters");
                PlayerPrefs.SetString("ErrorResolved", "false");
                Debug.Log("TTT not enough chars");
                BTM();
            }
            SetLabels();
            gameBegun = true;
        }
    }
    private void PlaceRandom()
    {
        soundManager.GetComponent<SoundManager>().PlayCorrect();
        Debug.Log("Placing random");
        int randomI = Random.Range(0, 3);
        int randomj = Random.Range(0, 3);
        int randomIndex = randomI * 3 + randomj;
        int status = tttModel.Put(randomI, randomj);
        if (status == -1)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (tttModel.GetStateAt(i, j) == '0')
                    {
                        status = tttModel.Put(i, j);
                        if (status == 1)
                        {
                            lastSolution2.text = lastSolution.text;
                            lastSolution.text = currentTiles[i].GetPronunciation();
                            RefreshState(currentTiles[i].GetCharacter());
                            StartCoroutine(ShowGameEnd(false));
                            Debug.Log("Show game end called in random put");
                            gameEnded = true;
                        }
                        else
                        {
                            lastSolution2.text = lastSolution.text;
                            lastSolution.text = currentTiles[i].GetPronunciation();
                            RefreshState(currentTiles[i].GetCharacter());
                        }
                        return;
                    }
                }
            }
        }
        else if (status == 1)
        {
            lastSolution2.text = lastSolution.text;
            lastSolution.text = currentTiles[randomIndex].GetPronunciation();
            RefreshState(currentTiles[randomIndex].GetCharacter());
            StartCoroutine(ShowGameEnd(false));
            Debug.Log("Show game end called random put random try");
            gameEnded = true;
            return;
        }
        else
        {
            lastSolution2.text = lastSolution.text;
            lastSolution.text = currentTiles[randomIndex].GetPronunciation();
            RefreshState(currentTiles[randomIndex].GetCharacter());
            return;
        }
    }
    private bool CheckFull()
    {
        int full = 0;
        for (int i = 0; i < 9; i++)
        {
            if (pieces[i].activeSelf == true)
            {
                full++;
            }
        }
        if (full == 9)
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
        Debug.Log("Showing game end");
        yield return new WaitForSeconds(1);
        soundManager.GetComponent<SoundManager>().PlayCompleted();
        classroomButton.SetActive(false);
        classroomPanel.SetActive(false);
        HideButtons();
        if (addAchi)
        {
            if (vsAI)
            {
                int vai = PlayerPrefs.GetInt("TicTacToePlayedversusAI");
                vai += 1;
                PlayerPrefs.SetInt("TicTacToePlayedversusAI", vai);
                Debug.Log(vai);
            }
            int vp = PlayerPrefs.GetInt("VersusPlayed");
            vp += 1;
            PlayerPrefs.SetInt("VersusPlayed", vp);
            Debug.Log(vp);
            int tp = PlayerPrefs.GetInt("TicTacToePlayed");
            tp += 1;
            PlayerPrefs.SetInt("TicTacToePlayed", tp);
            Debug.Log(tp);
        }
        gameContent.SetActive(false);
        gameEndScreen.SetActive(true);
        if (tie)
        {
            scoreText.text = "Tie!";
            if (addAchi)
            {
                int t = PlayerPrefs.GetInt("TicTacToeTies");
                t += 1;
                PlayerPrefs.SetInt("TicTacToeTies", t);
                Debug.Log(t);
            }
        }
        else
        {
            char winner = tttModel.GetWinner();
            scoreText.text = "Player" + winner + " won!";
            if (winner == '1' && addAchi)
            {
                int pw = PlayerPrefs.GetInt("TicTacToePlayer1Wins");
                pw += 1;
                PlayerPrefs.SetInt("TicTacToePlayer1Wins", pw);
                Debug.Log(pw);
            }
            else if (addAchi)
            {
                int pw = PlayerPrefs.GetInt("TicTactoePlayer2Wins");
                pw += 1;
                PlayerPrefs.SetInt("TicTactoePlayer2Wins", pw);
                Debug.Log(pw);
            }
        }
        addAchi = false;
    }
    private void RefreshState(string character)
    {
        Debug.Log("Refreshing");
        string[] cords = tttModel.GetLastPut().Split(',');
        int c1 = int.Parse(cords[0]);
        int c2 = int.Parse(cords[1]);
        char p = tttModel.GetStateAt(c1, c2);
        int index = (c1 * 3) + c2;
        if (p == '1')
        {
            pieces[index].GetComponent<PieceBehaviour>().Set(Color.red, character);
            pieces[index].SetActive(true);
            chars[index].gameObject.SetActive(false);
        }
        else
        {
            pieces[index].GetComponent<PieceBehaviour>().Set(Color.blue, character);
            pieces[index].SetActive(true);
            chars[index].gameObject.SetActive(false);
        }
        solutionLock = false;
        ClearInput();
        Debug.Log("Refreshed");
    }
    private int IsSolution(string s)
    {
        for (int i = 0; i < 9; i++)
        {
            if (s == currentTiles[i].GetPronunciation())
            {
                return i;
            }
        }
        return -1;
    }
    private void SetLabels()
    {
        GetChars();
        for (int i = 0; i < 9; i++)
        {
            chars[i].text = currentTiles[i].GetCharacter();
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
        currentPlayer.text = "Current player: Player" + tttModel.GetTurn();
        if (tttModel.GetTurn() == '1') currentPlayer.color = Color.red;
        else currentPlayer.color = Color.blue;
    }
    private void ClearInput()
    {
        inputField.Select();
        inputField.text = null;
        inputField.ActivateInputField();
    }
    private void InitBoard()
    {
        pieces = new GameObject[9];
        for (int i = 0; i < 9; i++)
        {
            GameObject nGO = Instantiate(piecePrefab);
            nGO.transform.position = Anchors[i].transform.position;
            //nGO.transform.localScale = Vector3.one;
            nGO.transform.SetParent(pieceHolder.transform);
            nGO.transform.localScale = new Vector3(scale, scale, scale);
            nGO.GetComponent<PieceBehaviour>().Set(Color.gray, "");
            nGO.SetActive(false);
            pieces[i] = nGO;
        } 
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
        foreach (GameObject go in cSelectButtons)
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
    private void HideButtons()
    {
        foreach (GameObject go in cSelectButtons)
        {
            go.SetActive(false);
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
    public void cSolve8()
    {
        inputField.text = currentTiles[7].GetPronunciation();
        HideButtons();
    }
    public void cSolve9()
    {
        inputField.text = currentTiles[8].GetPronunciation();
        HideButtons();
    }
    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
