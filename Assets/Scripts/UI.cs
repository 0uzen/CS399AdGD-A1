using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    private Text lives;
    private Text fruit;
    private CharacterController2D player;

    [SerializeField]
    protected GameObject inGameMenu;

    private bool menuPaused = false;


    // Start is called before the first frame update
    void Start()
    {
        lives = GameObject.Find("Lives_Text").GetComponent<Text>();
        fruit = GameObject.Find("Fruits").GetComponent<Text>();

        player = GameObject.Find("Player").GetComponent<CharacterController2D>();

        //Make in-game menu invisible
        inGameMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        lives.text = $"x{player.Lives.ToString()}";

        fruit.text = $"Items: {player.ItemsCount.ToString()}";

        //On escape key press
        OnEscPress();
    }

    void OnEscPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuPaused)
            {
                //Resume game
                Resume();
            }
            else
            {
                //Pause game
                PauseMenu();
            }
        }
    }

    void PauseMenu()
    {
        //Make in-game menu visible
        inGameMenu.SetActive(true);

        //Pause game
        Time.timeScale = 0;

        menuPaused = true;
    }

    public void Resume()
    {
        //Make in-game menu invisible
        inGameMenu.SetActive(false);

        //Resume game
        Time.timeScale = 1;

        menuPaused = false;
    }

    public void ReturnToTitle()
    {
        //Load the title screen
        SceneManager.LoadScene("TitleScreen");
    }
}
