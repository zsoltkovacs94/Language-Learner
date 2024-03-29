using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NoLagVideoPlayer : MonoBehaviour
{
    public Sprite[] frames;
    public float speed = 1;
    int currentFrame = 0;
    RectTransform rectTransform;
    RectTransform bgOverRT;
    public GameObject BGOver;
    public GameObject canvas;
    float screenScale;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        bgOverRT = BGOver.GetComponent<RectTransform>();
        StartCoroutine(ShowOtherFrame());
    }
    void Update()
    {
        screenScale = canvas.transform.localScale.x;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height / screenScale);
        bgOverRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height / screenScale);
    }
    IEnumerator ShowOtherFrame()
    {
        while (true)
        {
            yield return new WaitForSeconds(1/(frames.Length*speed));
            gameObject.GetComponent<Image>().sprite = frames[currentFrame];
            currentFrame++;
            if(currentFrame >= frames.Length)
            {
                currentFrame -= frames.Length;
            }
        }
    }
}
