using UnityEngine;

public class KeepBetweenScenes : MonoBehaviour
{
    private void Start()
    {
        Debug.Log(gameObject.tag);
        GameObject[] objects = GameObject.FindGameObjectsWithTag(gameObject.tag);
        if (objects.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
}
