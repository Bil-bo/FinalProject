using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// Class that stores the behaviour for buttons in the menus
public class GameMenu : MonoBehaviour
{
    public void Continue()
    {
        FindFirstObjectByType<HUD>().GetComponent<HUD>().OnPause();
        gameObject.SetActive(false);
    }

    public void Restart() 
    {
        EventManager.Broadcast(new RestartEvent());

        gameObject.SetActive(false);
        
    }

    public void Menu()
    {
        SceneManager.LoadScene("MainMenu");

    }

}
