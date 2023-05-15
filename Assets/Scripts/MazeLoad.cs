using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MazeLoad : MonoBehaviour
{
    // Start when enabled
    void OnEnable()
    {   
        // Replace "SceneName" with the actual name of the scene
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
