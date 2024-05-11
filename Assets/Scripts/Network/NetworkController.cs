using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviour
{
    public static NetworkController instance;
    public static string MAIN_MENU = "menu";

    void Awake()
    {
        instance = this;
    }
    public  void ProcessWebSockerMessage(string message)
    {
        if (message.Contains("getCards"))
        {
            DeckSelectController.instance.SetPlayableCards(message);
            NetworkManager.instance.cardsReceived = true;
        }
        if (message.Contains("getDungeons"))
        {
            UIMapController.instance.SetPlayableDungeons(message);
            NetworkManager.instance.dungeonsReceived = true;
        }
        if (message.Contains("registerUserResponse"))
        {
            MainMenuController.instance.ShowRegisterResponse(message);
            NetworkManager.instance.dungeonsReceived = true;
        }
        if (message.Contains("loginUserResponse"))
        {
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(message);
            if (response != null && !string.IsNullOrEmpty(response.data.token))
            {
                Debug.Log(response.data.token);
                SessionManager.instance.playerId = MainMenuController.instance.loginUserId.text;
                SessionManager.instance.token = response.data.token;
                MainMenuController.instance.StartGame();
                NetworkManager.instance.dungeonsReceived = true;
            } else
            {
                Debug.Log(response);
            }

        }
        if (message.Contains("authenticationFailure"))
        {
            SceneManager.LoadScene(MAIN_MENU);
        }
    }

    public void SendWebSocketMessage(string message)
    {
        if (message.Contains("endBattle"))
        {
            StartCoroutine(NetworkManager.instance.EndBattle("Morgox"));
        }
        if (message.Contains("endDungeon"))
        {
            StartCoroutine(NetworkManager.instance.CompleteDungeons("Dungeon 1", "Morgox", 1));
        }
    }
}
