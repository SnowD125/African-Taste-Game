using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuMusic : MonoBehaviour
{
    AudioSource music;

    void Start()
    {
        music = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main Menu" || scene.name == "LoadingScene" || scene.name == "Food Menu")
        {
            if (!music.isPlaying)
            {
                music.Play();
            }
        }
        else
        {
            music.Stop();
        }
    }
}