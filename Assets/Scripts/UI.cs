using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    private Text lives;
    private Text fruit;
    private CharacterController2D player;


    // Start is called before the first frame update
    void Start()
    {
        lives = GameObject.Find("Lives_Text").GetComponent<Text>();
        fruit = GameObject.Find("Fruits").GetComponent<Text>();

        player = GameObject.Find("Player").GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        lives.text = $"x{player.Lives.ToString()}";

        fruit.text = $"Items: {player.ItemsCount.ToString()}";
    }
}
