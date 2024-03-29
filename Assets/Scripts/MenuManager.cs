using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * A főmenü viselkedéséért felelős függvényeket tartalmazza
 */
public class MenuManager : MonoBehaviour
{
    // Főmenü
    public GameObject mainMenu;
    // Szótármenü
    public GameObject dictMenu;
    // Opciók
    public GameObject optMenu;
    // Játékmódválasztó
    public GameObject gmMenu;
    // Szólista
    public GameObject WLMenu;
    // Szótármenü listaelemei
    public GameObject listItemPrefab;
    // Szólista listaelemei
    public GameObject wordListItemPrefab;
    public GameObject achiListItemPrefab;
    // A szótármenü listaelemeit befogadó grid
    public GameObject filenameList;
    public GameObject achiList;
    // A szólista listaelemeit befogadó grid
    public GameObject wordList;
    // A training módjainak kiválasztását lehetővé tevő ablak
    public GameObject trainingModes;
    public GameObject singleModes;
    public GameObject versusModes;
    // A játékban használandó szótárfájl
    private DictionaryHandler dh;
    // Inicializál egy szótárosztályt, kiválasztja a korábban kiválasztott fájlokat és betölti a szólistát
    private int wordsInDict = 0;
    public TMP_Dropdown matchLengthDD;
    public TMP_Dropdown trialLengthDD;
    public TMP_Dropdown versusAI;
    public TMP_Dropdown classroomMode;
    public GameObject errorPanel;
    public GameObject blocker;
    public TextMeshProUGUI errorText;
    public GameObject infoPanel;
    public GameObject[] infoTexts;
    public GameObject[] cInfoTexts;
    public TextMeshProUGUI infoHeader;
    public GameObject achiMenu;
    public GameObject ackPanel;
    public GameObject cInfoButton;
    public Sprite minimizeSprite;
    public Sprite maximizeSprite;
    public GameObject minimizeButton;
    int currentInfo;
    int matchLength = 10;
    int trialLength = 10;
    string[] achiNames = { "Characters Learned", 
        "Training Played", "Learn Played", "Practice Played", 
        "Single Played", 
        "Match Played", 
        "Time Trial Played", "Time Trial Characters per Minute",
        "Versus Played", 
        "Connect 4 Played", "Connect 4 Played versus AI", 
        "Connect 4 Player 1 Wins", "Connect 4 Player 2 Wins", "Connect 4 Ties",
        "Tic Tac Toe Played", "Tic Tac Toe Played versus AI", 
        "Tic Tac Toe Player 1 Wins", "Tic Tac Toe Player 2 Wins", "Tic Tac Toe Ties"};
    void Start()
    {
        if (!Screen.fullScreen)
        {
            minimizeButton.GetComponent<Image>().sprite = maximizeSprite;
        }
        else
        {
            minimizeButton.GetComponent<Image>().sprite = minimizeSprite;
        }
        PlayerPrefs.SetString("ErrorResolved", "true");
        dh = new DictionaryHandler();
        LoadFromPrefs();
        InitFileList();
        InitWordList();
        LoadMatchLength();
        LoadTrialLength();
        LoadVersusAI();
        LoadClassroomMode();
    }
    private void Update()
    {
        if (PlayerPrefs.GetString("ErrorResolved") == "false")
        {
            errorPanel.SetActive(true);
            blocker.SetActive(true);
            errorText.text = PlayerPrefs.GetString("ErrorText");
        }
    }
    public void CloseAllInfo()
    {
        foreach(GameObject gm in infoTexts)
        {
            gm.SetActive(false);
        }
        blocker.SetActive(true);
        cInfoButton.SetActive(false);
    }
    public void LearningInfo()
    {
        currentInfo = 0;
        CloseAllInfo();
        infoPanel.SetActive(true);
        infoTexts[currentInfo].SetActive(true);
        cInfoButton.SetActive(true);
    }
    public void PracticeInfo()
    {
        currentInfo = 1;
        CloseAllInfo();
        infoPanel.SetActive(true);
        infoTexts[currentInfo].SetActive(true);
        cInfoButton.SetActive(true);
    }
    public void MatchInfo()
    {
        currentInfo = 2;
        CloseAllInfo();
        infoPanel.SetActive(true);
        infoTexts[currentInfo].SetActive(true);
        cInfoButton.SetActive(true);
    }
    public void TTInfo()
    {
        currentInfo = 3;
        CloseAllInfo();
        infoPanel.SetActive(true);
        infoTexts[currentInfo].SetActive(true);
        cInfoButton.SetActive(true);
    }
    public void C4Info()
    {
        currentInfo = 4;
        CloseAllInfo();
        infoPanel.SetActive(true);
        infoTexts[currentInfo].SetActive(true);
        cInfoButton.SetActive(true);
    }
    public void TTTInfo()
    {
        currentInfo = 5;
        CloseAllInfo();
        infoPanel.SetActive(true);
        infoTexts[currentInfo].SetActive(true);
        cInfoButton.SetActive(true);
    }
    public void MLInfo()
    {
        CloseAllInfo();
        infoPanel.SetActive(true);
        infoTexts[6].SetActive(true);
    }
    public void TTLInfo()
    {
        CloseAllInfo();
        infoPanel.SetActive(true);
        infoTexts[7].SetActive(true);
    }
    public void VAIInfo()
    {
        CloseAllInfo();
        infoPanel.SetActive(true);
        infoTexts[8].SetActive(true);
    }
    public void ClassroomInfo()
    {
        CloseAllInfo();
        infoPanel.SetActive(true);
        infoTexts[9].SetActive(true);
    }
    public void ShowClassroomInfo()
    {
        if (cInfoTexts[currentInfo].activeSelf == false)
        {
            infoHeader.text = "Info - Classroom Mode";
            infoHeader.fontSize = 42;
            infoTexts[currentInfo].SetActive(false);
            cInfoTexts[currentInfo].SetActive(true);
        }
        else
        {
            infoHeader.text = "Info";
            infoHeader.fontSize = 64;
            infoTexts[currentInfo].SetActive(true);
            cInfoTexts[currentInfo].SetActive(false);
        }
    }
    public void CloseInfo()
    {
        infoHeader.text = "Info";
        infoHeader.fontSize = 64;
        cInfoTexts[currentInfo].SetActive(false);
        blocker.SetActive(false);
        infoPanel.SetActive(false);
        cInfoButton.SetActive(false);
    }
    public void AckError()
    {
        PlayerPrefs.SetString("ErrorResolved", "true");
        errorPanel.SetActive(false);
        blocker.SetActive(false);
    }
    public void LoadVersusAI()
    {
        int vai = PlayerPrefs.GetInt("versusAI");
        versusAI.value = vai;
    }
    public void LoadClassroomMode()
    {
        int cMode = PlayerPrefs.GetInt("classroomMode");
        classroomMode.value = cMode;
    }
    public void SaveVersusAI()
    {
        PlayerPrefs.SetInt("versusAI", versusAI.value);
    }
    public void SaveClassroomMode()
    {
        Debug.Log(classroomMode.value);
        PlayerPrefs.SetInt("classroomMode", classroomMode.value);
    }
    public void LoadMatchLength()
    {
        int ml = (PlayerPrefs.GetInt("MatchLength") / 10) - 1;
        if (ml < 0) ml = 0;
        matchLengthDD.value = ml;
    }
    public void SaveMatchLength()
    {
        matchLength = (matchLengthDD.value + 1) * 10;
        PlayerPrefs.SetInt("MatchLength", matchLength);
    }
    public void LoadTrialLength()
    {
        int tl = (PlayerPrefs.GetInt("TrialLength") / 10) - 1;
        if (tl < 0) tl = 0;
        trialLengthDD.value = tl;
    }
    public void SaveTrialLength()
    {
        trialLength = (trialLengthDD.value + 1) * 10;
        PlayerPrefs.SetInt("TrialLength", trialLength);
    }
    // Megjeleníti a főmenüt és minden más almenüt elrejt
    public void ShowMainMenu()
    {
        SaveVersusAI();
        SaveClassroomMode();
        SaveMatchLength();
        SaveTrialLength();
        LoadFromPrefs();
        mainMenu.SetActive(true);
        dictMenu.SetActive(false);
        optMenu.SetActive(false);
        gmMenu.SetActive(false);
        WLMenu.SetActive(false);
        trainingModes.SetActive(false);
        singleModes.SetActive(false);
        versusModes.SetActive(false);
        achiMenu.SetActive(false);
    }
    // Megjeleníti az opciókat és minden más almenüt elrejt
    public void ShowOptMenu()
    {
        mainMenu.SetActive(false);
        dictMenu.SetActive(false);
        optMenu.SetActive(true);
        gmMenu.SetActive(false);
        WLMenu.SetActive(false);
    }
    // Megjeleníti a szótármenüt és minden más almenüt elrejt
    public void ShowDictMenu()
    {
        mainMenu.SetActive(false);
        dictMenu.SetActive(true);
        optMenu.SetActive(false);
        gmMenu.SetActive(false);
        WLMenu.SetActive(false);
    }
    // Megjeleníti a játékmód választót és minden más almenüt elrejt, valamint menti a kiválasztást
    public void ShowGMMenu()
    {
        new LearnedCharManager().CheckCharLog();
        if (dh.GetSelected() == null)
        {
            PlayerPrefs.SetString("ErrorText", "No files found in game folder");
            PlayerPrefs.SetString("ErrorResolved", "false");
            Debug.Log("DH No Files Found");
        }
        else
        {
            if (dh.GetSelected().Count == 0)
            {
                PlayerPrefs.SetString("ErrorText", "No files selected");
                PlayerPrefs.SetString("ErrorResolved", "false");
                Debug.Log("DH No Selected");
            }
            else
            {
                SaveToPrefs();
                mainMenu.SetActive(false);
                dictMenu.SetActive(false);
                optMenu.SetActive(false);
                gmMenu.SetActive(true);
                WLMenu.SetActive(false);
            }
        }
    }
    // Megjeleníti a szólistát és minden más almenüt elrejt
    public void ShowWLMenu()
    {
        mainMenu.SetActive(false);
        dictMenu.SetActive(false);
        optMenu.SetActive(false);
        gmMenu.SetActive(false);
        WLMenu.SetActive(true);
    }
    public void ShowAchiMenu()
    {
        ClearAchiList();
        InitAchiList();
        mainMenu.SetActive(false);
        dictMenu.SetActive(false);
        optMenu.SetActive(false);
        gmMenu.SetActive(false);
        WLMenu.SetActive(false);
        achiMenu.SetActive(true);
    }
    // Kilép a programból
    public void ExitGame()
    {
        Application.Quit();
    }
    // Inicializálja a szótármenü elemeit
    private void InitFileList()
    {
        string[] filenames = dh.filenames;
        for(int i = 0; i < filenames.Length; i++)
        {
            GameObject listItem = Instantiate(listItemPrefab);
            listItem.GetComponent<ListItem>().InitListItem(i, filenames[i], dh.selected[i]);
            listItem.transform.SetParent(filenameList.transform);
            listItem.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    // Menti a kiválasztást és újratölti a szólistát
    public void SaveSelected()
    {
        GameObject[] listItems = GameObject.FindGameObjectsWithTag("ListItem");
        List<int> indexes = new List<int>();
        for(int i = 0;i < listItems.Length;i++)
        {
            if (listItems[i].GetComponent<ListItem>().isSelected)
            {
                indexes.Add(listItems[i].GetComponent<ListItem>().GetIndex());
            }
        }
        dh.DeSelectAll();
        foreach (int i in indexes)
        {
            dh.Select(i);
        }
        ClearWordList();
        InitWordList();
        SaveToPrefs();
    }
    // Frissíti a fájlok listáját, kiválasztja mindet és inicializálja a szólistát
    public void RefreshList()
    {
        dh.RefreshFileList();
        for (int i = 0; i < filenameList.transform.childCount; i++)
        {
            Destroy(filenameList.transform.GetChild(i).gameObject);
        }
        InitFileList();
        ClearWordList();
        InitWordList();

    }
    // Törli a korábbi szólista elemeket
    public void ClearWordList()
    {
        for (int i = 0; i < wordList.transform.childCount; i++)
        {
            Destroy(wordList.transform.GetChild(i).gameObject);
        }
    }
    // Inicializálja a szólistát a kiválasztott fájlok tartalmával
    public void InitWordList()
    {
        dh.FillDictionary();
        wordsInDict = dh.dictionary.Keys.Count;
        foreach (string key in dh.dictionary.Keys)
        {
            GameObject wordListItem = Instantiate(wordListItemPrefab);
            wordListItem.GetComponent<WordListItem>().InitWordListItem(key, dh.dictionary[key]);
            wordListItem.transform.SetParent(wordList.transform);
            wordListItem.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
    // Menti a kiválasztást a playerprefsbe
    public void SaveToPrefs()
    {
        List<string> selectedFiles = dh.GetSelected();
        PlayerPrefs.SetInt("SNumber", selectedFiles.Count);
        int i = 0;
        foreach (string file in selectedFiles)
        {
            PlayerPrefs.SetString("SName"+i,file);
            i++;
        }
    }
    // Megjeleníti a training mód kiválasztót
    public void ToTraining()
    {
        singleModes.SetActive(false);
        versusModes.SetActive(false);
        trainingModes.SetActive(true);
    }
    // Tanuló módban indít traininget
    public void startLearning()
    {
        PlayerPrefs.SetInt("TrainingMode", 1);
        SceneManager.LoadScene(1);
    }
    // Gyakorló módban indítja a traininget
    public void StartPractice()
    {
        PlayerPrefs.SetInt("TrainingMode", 2);
        SceneManager.LoadScene(1);
    }
    public void ShowSingleModes()
    {
        trainingModes.SetActive(false);
        versusModes.SetActive(false);
        singleModes.SetActive(true);
    }
    public void StartTimeTrial()
    {
        SceneManager.LoadScene(3);
    }
    public void StartMatch()
    {
        LoadFromPrefs();
        dh.FillDictionary();
        int wordCount = dh.dictionary.Keys.Count;
        if (wordCount < 3)
        {
            PlayerPrefs.SetString("ErrorText", "Select a file with at least 3 characters");
            PlayerPrefs.SetString("ErrorResolved", "false");
            Debug.Log("Match not enough chars");
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }
    public void ShowVersusModes()
    {
        trainingModes.SetActive(false);
        singleModes.SetActive(false);
        versusModes.SetActive(true);
    }
    public void StartTTT()
    {
        LoadFromPrefs();
        dh.FillDictionary();
        int wordCount = dh.dictionary.Keys.Count;
        if (wordCount < 9)
        {
            PlayerPrefs.SetString("ErrorText", "Select a file with at least 9 characters");
            PlayerPrefs.SetString("ErrorResolved", "false");
            Debug.Log("TTT not enough chars");
        }
        else
        {
            Debug.Log("Starting TTT");
            SceneManager.LoadScene(5);
        }
    }
    public void StartConnect4()
    {
        LoadFromPrefs();
        dh.FillDictionary();
        int wordCount = dh.dictionary.Keys.Count;
        if(wordCount < 8)
        {
            PlayerPrefs.SetString("ErrorText", "Select a file with at least 8 characters");
            PlayerPrefs.SetString("ErrorResolved", "false");
            Debug.Log("C4 not enough chars");
        }
        else
        {
            Debug.Log("Starting connect4");
            SceneManager.LoadScene(4);
        }
    }
    private void InitAchiList()
    {
        foreach(string name in achiNames)
        {
            string ppName = String.Concat(name.Where(c => !Char.IsWhiteSpace(c)));
            int value = PlayerPrefs.GetInt(ppName);
            GameObject achiListItem = Instantiate(achiListItemPrefab);
            achiListItem.GetComponent<AchiItem>().Init(name, value.ToString());
            achiListItem.transform.SetParent(achiList.transform);
            achiListItem.transform.localScale = Vector3.one;
        }
    }
    private void ClearAchiList()
    {
        for (int i = 0; i < achiList.transform.childCount; i++)
        {
            Destroy(achiList.transform.GetChild(i).gameObject);
        }
        new LearnedCharManager().Reset();
    }
    public void ResetAchiList()
    {
        foreach (string name in achiNames)
        {
            string ppName = String.Concat(name.Where(c => !Char.IsWhiteSpace(c)));
            Debug.Log(ppName);
            PlayerPrefs.SetInt(ppName, 0);
        }
        ackPanel.SetActive(true);
    }
    public void ConfirmReset()
    {
        ackPanel.SetActive(false);
        ClearAchiList();
        InitAchiList();
    }
    public void CancelReset()
    {
        ackPanel.SetActive(false);
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
    // Betölti a playerprefsbe mentett kiválasztást
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
}
