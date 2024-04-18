using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitch : MonoBehaviour
{

    [SerializeField]
    private SceneIndex Scene;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EventManager.Broadcast(new SceneChangeEvent() { Stage = Scene });
        }
    }

}
