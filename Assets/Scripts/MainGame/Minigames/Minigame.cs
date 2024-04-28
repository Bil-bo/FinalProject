using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


// Abstract class for minigames
public abstract class Minigame : MonoBehaviour
{
    public Action<bool> completed; // The callback for if a minigame has been completed successfully or not

}
