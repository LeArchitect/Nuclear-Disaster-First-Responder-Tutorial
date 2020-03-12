using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChangerScript : MonoBehaviour
{
    public static GameObject opening;
    public static GameObject gameRoom;

    public GameObject openingScene;
    public GameObject gameRoomScene;

    // Start is called before the first frame update
    void Start()
    {
        opening = openingScene = GameObject.Find("Opening");
        gameRoom = gameRoomScene = GameObject.Find("GameRoom");
        SceneChanger(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void SceneChanger(int sceneNumber)
    {
        switch (sceneNumber)
        {
            case 0:
                opening.SetActive(true);
                gameRoom.SetActive(false);
                break;
            case 1:
                opening.SetActive(false);
                gameRoom.SetActive(true);
                break;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
