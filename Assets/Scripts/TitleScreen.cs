using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TitleScreen : MonoBehaviour
{
    public void LoadLevel()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Exit()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else 
            Application.Quit();
        #endif
    }
}
