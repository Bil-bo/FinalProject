using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> Screens = new List<GameObject>();
    private GameObject CurrentScreen;

    private void Start()
    {
        CurrentScreen = Screens[0];
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }


    public void ChangeScreen(int screen)
    {
        CurrentScreen.SetActive(false);
        Screens[screen].SetActive(true);
        CurrentScreen = Screens[screen];
    }



    public void QuitGame()
    {


    #if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    #else
        Application.Quit();
    #endif
    }

}
