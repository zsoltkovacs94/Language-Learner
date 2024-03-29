using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchGameManager : MonoBehaviour
{
    public DictionaryHandler dh;
    public GameObject gameEndScreen;
    public GameObject gameContent;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timer;
    WordTile[] wordTiles;
    public TextMeshProUGUI[] optionalPronunciations;
    public TextMeshProUGUI characterText;
    int wordCount;
    WordTile currentTile;
    bool gameStarted = false;
    int solutionIndex;
    int givenAnswer = 10;
    int solved = 0;
    int score = 0;
    public float time;
    bool endShown = false;
    int matchLength;
    bool clickLock = true;
    public Sprite minimizeSprite;
    public Sprite maximizeSprite;
    public GameObject minimizeButton;
    public GameObject rightButton;
    public GameObject rightPanel;
    public GameObject leftButton;
    public GameObject leftPanel;
    int startCountdown = 3;
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
        LoadFromPrefs();
        CreateWordTileList();
        matchLength = PlayerPrefs.GetInt("MatchLength");
        Debug.Log(matchLength);
        if (wordCount > matchLength)
        {
            wordCount = matchLength;
        }
        Invoke("StartGame", 3f);
        StartCountdown();
    }
    void Update()
    {
        Debug.Log(wordCount);
        if (gameStarted)
        {
            //Debug.Log("Game in progress");
            if(givenAnswer != 10 && wordCount != solved)
            {
                if(givenAnswer == solutionIndex)
                {
                    CancelInvoke();
                    if(!soundPlayed)
                    {
                        soundPlayed = true;
                        soundManager.GetComponent<SoundManager>().PlayCorrect();
                    }
                    optionalPronunciations[solutionIndex].color = Color.green;
                    score++;
                }
                else if(givenAnswer == 5)
                {
                    CancelInvoke();
                    if (!soundPlayed)
                    {
                        soundPlayed = true;
                        soundManager.GetComponent<SoundManager>().PlayWrong();
                    }
                    optionalPronunciations[solutionIndex].color = Color.green;
                }
                else
                {
                    CancelInvoke();
                    if (!soundPlayed)
                    {
                        soundPlayed = true;
                        soundManager.GetComponent<SoundManager>().PlayWrong();
                    }
                    optionalPronunciations[solutionIndex].color = Color.green;
                    optionalPronunciations[givenAnswer].color = Color.red;
                }
                solved++;
                Debug.Log(solved + "/" + wordCount);
                givenAnswer = 10;
                if(solved < wordCount)
                {
                    Invoke("SetCurrentState",1f);
                }
            }
            else if(wordCount == solved)
            {
                if (!endShown)
                {
                    soundPlayed = false;
                    Invoke("ShowGameOver", 1);
                    endShown = true;
                }
            }
        }
        else
        {
            Debug.Log(wordCount);
            if(wordCount < 3)
            {
                PlayerPrefs.SetString("ErrorText", "Select a file with at least 3 characters");
                PlayerPrefs.SetString("ErrorResolved", "false");
                Debug.Log("Match not enough chars");
                BTM();
            }
        }
    }
    public void StartGame()
    {
        CancelInvoke("ProgressStartCountdown");
        gameStarted = true;
        SetCurrentState();
    }
    void StartCountdown()
    {
        InvokeRepeating("ProgressStartCountdown", 0f, 1f);
    }
    void ProgressStartCountdown()
    {
        characterText.text = startCountdown.ToString();
        startCountdown--;
    }
    public void SetCurrentState()
    {
        soundPlayed = false;
        clickLock = false;
        CancelInvoke();
        for (int i = 0; i < 3; i++)
        {
            optionalPronunciations[i].color=Color.white;
            optionalPronunciations[i].text = "Pronunciation";
        }
        solutionIndex = Random.Range(1, 4);
        solutionIndex -= 1;
        Debug.Log(solutionIndex);
        currentTile = GetNextTile();
        optionalPronunciations[solutionIndex].text = currentTile.GetPronunciation();
        for (int i = 0; i < 3; i++)
        {
            if(i != solutionIndex)
            {
                optionalPronunciations[i].text = GetRandomPronunciation();
            }
        }
        characterText.text = currentTile.GetCharacter();
        Count();
        Invoke("OutOfTime", 4.9f);
    }
    void Countdown()
    {
            timer.color = Color.white;
            time -= 0.1f;
            timer.text = time.ToString().Substring(0, 3);
    }
    void Count()
    {
        time = 5f;
        InvokeRepeating("Countdown", 0f, 0.1f);
    }
    void OutOfTime()
    {
        CancelInvoke();
        timer.text = "0,0";
        timer.color = Color.red; 
        givenAnswer = 5;
    }
    public WordTile GetNextTile()
    {
        WordTile selectedWordTile;
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, wordTiles.Length);
            selectedWordTile = new WordTile(wordTiles[randomIndex].GetCharacter(), wordTiles[randomIndex].GetPronunciation());
        } while (selectedWordTile.GetCharacter() == null);
        wordTiles[randomIndex] = new WordTile(null, wordTiles[randomIndex].GetPronunciation());
        return selectedWordTile;
    }
    public string GetRandomPronunciation()
    {
        string pronunciation;
        do
        {
            pronunciation = wordTiles[Random.Range(0, wordTiles.Length)].GetPronunciation();
        } while (pronunciation == optionalPronunciations[0].text ||
        pronunciation == optionalPronunciations[1].text ||
        pronunciation == optionalPronunciations[2].text
        );
        return pronunciation;
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
    public void ClickFirst()
    {
        if (!clickLock)
        {
            clickLock = true;
            givenAnswer = 0;
        }
    }
    public void ClickSecond()
    {
        if (!clickLock)
        {
            clickLock = true;
            givenAnswer = 1;
        }
    }
    public void ClickThird()
    {
        if (!clickLock)
        {
            clickLock = true;
            givenAnswer = 2;
        }
    }
    void ShowGameOver()
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
        int sp = PlayerPrefs.GetInt("SinglePlayed");
        sp += 1;
        PlayerPrefs.SetInt("SinglePlayed", sp);
        Debug.Log(sp);
        int mp = PlayerPrefs.GetInt("MatchPlayed");
        mp += 1;
        PlayerPrefs.SetInt("MatchPlayed", mp);
        Debug.Log(mp);
        gameContent.SetActive(false);
        gameEndScreen.SetActive(true);
        scoreText.text = "Your score: " + score + "/" + wordCount;
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
        if (rightPanel.activeSelf)
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
        if (!clickLock)
        {
            givenAnswer = solutionIndex;
            clickLock = true;
        }
    }
    public void ClassroomSkip()
    {
        if(solutionIndex == 2 && !clickLock)
        {
            givenAnswer = 1;
            clickLock = true;
        }
        else if(!clickLock)
        {
            givenAnswer = 2;
            clickLock = true;
        }
    }
    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
