using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private bool paused = false;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            paused = !paused;
            if (paused) Time.timeScale = 0.0f;
            else Time.timeScale = 1.0f;
        }
    }
}
