using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
