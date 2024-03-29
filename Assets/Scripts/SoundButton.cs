using UnityEngine;

public class SoundButton : MonoBehaviour
{
    GameObject soundManager;
    GameObject bgMusic;
    float bgMMaxVolume = 0.25f;
    float bgMIncrement = 0.025f;
    float soundMaxVolume = 1f;
    float soundIncrement = 0.1f;
    public GameObject soundPanel;
    void Start()
    {
        soundManager = GameObject.FindGameObjectWithTag("SoundManager");
        bgMusic = GameObject.FindGameObjectWithTag("BGM");
    }
    private void Update()
    {
        Debug.Log("Sounds " + soundManager.GetComponent<AudioSource>().volume);
        Debug.Log("BGM " + bgMusic.GetComponent<AudioSource>().volume);
    }
    public void SoundDown()
    {
        float soundVolume = soundManager.GetComponent<AudioSource>().volume;
        if (soundVolume > 0f)
        {
            soundManager.GetComponent<AudioSource>().volume = soundVolume - soundIncrement;
        }
        else
        {
            soundManager.GetComponent<AudioSource>().volume = 0f;
        }
        float bgMVolume = bgMusic.GetComponent<AudioSource>().volume;
        if (bgMVolume > 0f)
        {
            bgMusic.GetComponent<AudioSource>().volume = bgMVolume - bgMIncrement;
        }
        else
        {
            bgMusic.GetComponent<AudioSource>().volume = 0f;
        }
    }
    public void SoundUp()
    {
        float soundVolume = soundManager.GetComponent<AudioSource>().volume;
        if (soundVolume < soundMaxVolume)
        {
            soundManager.GetComponent<AudioSource>().volume = soundVolume + soundIncrement;
        }
        else
        {
            soundManager.GetComponent<AudioSource>().volume = soundMaxVolume;
        }
        float bgMVolume = bgMusic.GetComponent<AudioSource>().volume;
        if (bgMVolume < bgMMaxVolume)
        {
            bgMusic.GetComponent<AudioSource>().volume = bgMVolume + bgMIncrement;
        }
        else
        {
            bgMusic.GetComponent<AudioSource>().volume = bgMMaxVolume;
        }
    }
    public void ToggleSP()
    {
        if(soundPanel.activeSelf == true)
        {
            soundPanel.SetActive(false);
        }
        else
        {
            soundPanel.SetActive(true);
        }
    }
}
