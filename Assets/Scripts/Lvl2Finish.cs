using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lvl2Finish : MonoBehaviour
{
    //Name of Player component
    string playerName = "Player";

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Load scene only if colliding object is a player
        if (collision.gameObject.name == playerName)
        {
            //Load next scene by name
            SceneManager.LoadScene("Transition-Graveyard");
        }

    }
}
