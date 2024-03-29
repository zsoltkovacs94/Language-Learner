using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrialGameManager : MonoBehaviour
{
    public TextMeshProUGUI characterText;
    public TextMeshProUGUI pronunciation;
    public TMP_InputField inputField;
    public DictionaryHandler dh;
    public GameObject gameEndScreen;
    public GameObject gameContent;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timer;
    public TextMeshProUGUI lastSolution;
    public Sprite minimizeSprite;
    public Sprite maximizeSprite;
    public GameObject minimizeButton;
    WordTile[] wordTiles;
    WordTile currentTile;
    int wordCount;
    bool gameStarted = false;
    bool gameEnded = false;
    int solved = 0;
    int score = 0;
    int trialLength;
    float time;
    bool solutionLock = true;
    int startCountdown = 3;
    bool addAchi = true;
    public GameObject rightButton;
    public GameObject rightPanel;
    public GameObject leftButton;
    public GameObject leftPanel;
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
        if (PlayerPrefs.GetInt("classroomMode") == 0)
        {
            rightButton.SetActive(true);
            leftButton.SetActive(true);
        }
        LoadFromPrefs();
        CreateWordTileList();
        trialLength = PlayerPrefs.GetInt("TrialLength");
        time = trialLength;
        StartCountdown();
        Invoke("StartGame", 3f);
        //Count(time);
    }
    void Update()
    {
        if (gameStarted && !gameEnded)
        {
            inputField.Select();
            inputField.ActivateInputField();
            if (pronunciation.text.Substring(0, pronunciation.text.Length - 1).ToLower() == "---" && !solutionLock)
            {
                solved++;
                solutionLock = true;
                pronunciation.color = Color.blue;
                lastSolution.text = "Last solution: " + currentTile.GetPronunciation();
                Invoke("SetCurrentState", 0.1f);
            }
            if (currentTile.GetPronunciation() == pronunciation.text.Substring(0, pronunciation.text.Length - 1).ToLower() && !solutionLock)
            {
                if (!soundPlayed)
                {
                    soundPlayed = true;
                    soundManager.GetComponent<SoundManager>().PlayCorrect();
                }
                solved++;
                score++;
                solutionLock = true;
                lastSolution.text = "Last solution: " + currentTile.GetPronunciation();
                pronunciation.color = Color.green;
                Invoke("SetCurrentState", 0.1f);
            }
            else if (currentTile.GetPronunciation().Length <= pronunciation.text.Length - 1 && !solutionLock)
            {
                if (!soundPlayed)
                {
                    soundPlayed = true;
                    soundManager.GetComponent<SoundManager>().PlayWrong();
                }
                pronunciation.color = Color.red;
            }
            else if (currentTile.GetPronunciation().Length > pronunciation.text.Length - 1 && !solutionLock)
            {
                soundPlayed = false;
                pronunciation.color = Color.white;
            }
        }
        else if(gameEnded)
        {
            if (!soundPlayed)
            {
                soundPlayed = true;
                soundManager.GetComponent<SoundManager>().PlayCompleted();
            }
            if (addAchi)
            {
                int sp = PlayerPrefs.GetInt("SinglePlayed");
                sp += 1;
                PlayerPrefs.SetInt("SinglePlayed", sp);
                Debug.Log(sp);
                int tp = PlayerPrefs.GetInt("TimeTrialPlayed");
                tp += 1;
                PlayerPrefs.SetInt("TimeTrialPlayed", tp);
                Debug.Log(tp);
                addAchi = false;
            }
            rightButton.SetActive(false);
            rightPanel.SetActive(false);
            leftButton.SetActive(false);
            leftPanel.SetActive(false);
            gameEndScreen.SetActive(true);
            gameContent.SetActive(false);
            int cpm = PlayerPrefs.GetInt("TimeTrialCharactersperMinute");
            float speed = (60 / trialLength) * score;
            if (cpm < speed)
            {
                PlayerPrefs.SetInt("TimeTrialCharactersperMinute", (int)speed);
                Debug.Log(speed);
            }
            else
            {
                Debug.Log(cpm);
            }
            scoreText.text = "Your speed is " + speed + " characters/minute";
        }
        /*
        else
        {
            SetCurrentState();
            gameStarted = true;
        }
        */
    }
    void StartGame()
    {
        CancelInvoke("ProgressStartCountdown");
        SetCurrentState();
        gameStarted = true;
        solutionLock = false;
        Count(time);
    }
    void StartCountdown()
    {
        inputField.text = "Starting...";
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
        pronunciation.color = Color.white;
        inputField.Select();
        inputField.text = null;
        inputField.ActivateInputField();
        currentTile = GetNextTile();
        characterText.text = currentTile.GetCharacter();
        solutionLock = false;
    }
    WordTile GetNextTile()
    {
        WordTile selectedWordTile;
        int randomIndex;
        randomIndex = Random.Range(0, wordTiles.Length);
        selectedWordTile = new WordTile(wordTiles[randomIndex].GetCharacter(), wordTiles[randomIndex].GetPronunciation());
        return selectedWordTile;
    }
    void Countdown()
    {
        timer.color = Color.white;
        switch (time.ToString().Length)
        {
            case 1:
                timer.text = "0" + time.ToString() + ",0";
                break;
            case 2:
                timer.text = time.ToString() + ",0";
                break;
            case 3:
                timer.text = time.ToString() + "0";
                break;
            default:
                if(time < 10)
                {
                    timer.text = "0" + time.ToString().Substring(0, 3);
                }
                else
                {
                    timer.text = time.ToString().Substring(0, 4);
                }
                break;

        }
        time -= 0.1f;
    }
    void Count(float time)
    {
        InvokeRepeating("Countdown", 0f, 0.1f);
        Invoke("OutOfTime", time);
    }
    void OutOfTime()
    {
        CancelInvoke();
        timer.text = "00,0";
        timer.color = Color.red;
        solutionLock = true;
        gameEnded = true;
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
        Debug.Log("cSolve");
        if (!solutionLock)
        {
            Debug.Log(currentTile.GetPronunciation());
            inputField.text = currentTile.GetPronunciation();
        }
    }
    public void ClassroomSkip()
    {
        Debug.Log("cSkip");
        if (!solutionLock)
        {
            inputField.text = "---";
        }
    }
    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
