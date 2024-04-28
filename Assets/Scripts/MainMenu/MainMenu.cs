using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Holds the behaviours for buttons in the main menu
public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> Screens = new List<GameObject>();
    private GameObject CurrentScreen;

    private void Start()
    {
        CurrentScreen = Screens[0];
    }

    // Start button behaviour to change the loaded scene to the maingame
    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    // For next and back buttons, switch the currently active canvas to the indexed one
    public void ChangeScreen(int screen)
    {
        CurrentScreen.SetActive(false);
        Screens[screen].SetActive(true);
        CurrentScreen = Screens[screen];
    }



    public void QuitGame()
    {

        //In the unity editor, quitting is slightly different
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
