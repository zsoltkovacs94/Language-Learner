using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip completedSound;
    AudioSource source;
    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    private void Update()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("Button");
        foreach (GameObject button in buttons)
        {
            button.GetComponent<Button>().onClick.AddListener(PlayClick);
        }
    }
    public void PlayClick()
    {
        source.clip = clickSound;
        source.time = 0.24f;
        source.Play();
    }
    public void PlayCorrect()
    {
        source.clip = correctSound;
        source.Play();
    }
    public void PlayWrong()
    {
        source.clip = wrongSound;
        source.Play();
    }
    public void PlayCompleted()
    {
        source.clip = completedSound;
        source.Play();
    }
}
