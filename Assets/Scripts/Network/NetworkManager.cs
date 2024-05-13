using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Text;
using NativeWebSocket;
using System.Threading.Tasks;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public WebSocket websocket;
    private bool attemptingReconnect = false;
    public bool cardsReceived = false;
    public bool dungeonsReceived = false;
    public string dungeon;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    void Update()
    {
     
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
        #endif
        if (!attemptingReconnect && (websocket == null || websocket.State == WebSocketState.Closed))
        {
            AttemptReconnect();
        }
    }

    void Start()
    {
        InitializeWebSocket();
    }

    IEnumerator InitializeWebSocket()
    {
        //websocket = new WebSocket("ws://10.0.2.2:3000");
        websocket = new WebSocket("ws://localhost:3000");
        //websocket = new WebSocket("ws://192.168.1.3:3000");

        websocket.OnOpen += () => StartCoroutine(OnWebSocketOpen());
        websocket.OnError += OnWebSocketError;
        websocket.OnClose += OnWebSocketClose;
        websocket.OnMessage += OnWebSocketMessage;

        yield return StartCoroutine(ConnectWebSocket());
    }

    IEnumerator ConnectWebSocket()
    {
        Task connectTask = websocket.Connect();
        while (!connectTask.IsCompleted)
            yield return null;

        if (connectTask.Exception != null)
            Debug.LogError("WebSocket connect error: " + connectTask.Exception.InnerException);
    }

    IEnumerator SendWebSocketMessage(string message)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            Task sendTask = websocket.SendText(message);
            Debug.Log(message);
            while (!sendTask.IsCompleted)
                yield return null;

            if (sendTask.IsFaulted)
                Debug.LogError("Send message failed: " + sendTask.Exception.InnerException);
        }
        else
        {
            Debug.LogError("WebSocket is closed or not initialized.");
        }
    }
    public IEnumerator EndBattle(string token)
    {
        yield return StartCoroutine(SendWebSocketMessage("{\"type\":\"endBattle\",\"token\":\"" + token + "\"}"));
    }

    public IEnumerator GetCardsAsync(string token)
    {
        yield return StartCoroutine(SendWebSocketMessage("{\"type\":\"getCards\",\"token\":\"" + token + "\"}"));
    }

    public IEnumerator GetPlayerCards(string token)
    {
        Debug.Log("Player Id fetching data: " + token);
        yield return StartCoroutine(SendWebSocketMessage("{\"type\":\"getCards\",\"token\":\"" + token + "\"}"));
    }


    public IEnumerator GetDungeons(string token)
    {
        yield return StartCoroutine(SendWebSocketMessage("{\"type\":\"getDungeons\",\"token\":\"" + token + "\"}"));
    }

    public IEnumerator CompleteDungeons(string dungeonName, string token, int level)
    {
        yield return StartCoroutine(SendWebSocketMessage("{\"type\":\"completeDungeon\",\"token\":\"" + token + "\",\"dungeonName\":\"" + dungeonName + "\",\"level\":\"" + level + "\"}"));
    }
    public IEnumerator RegisterUser(string userId, string password)
    {
        Debug.Log("Registering user " + userId + " for player " + password);
        yield return StartCoroutine(SendWebSocketMessage("{\"type\":\"registerUser\",\"userId\":\"" + userId + "\",\"password\":\"" + password + "\"}"));
    }

    public IEnumerator LoginUser(string userId, string password)
    {
        Debug.Log("Login user " + userId + " for player " + password);
        yield return StartCoroutine(SendWebSocketMessage("{\"type\":\"loginUser\",\"userId\":\"" + userId + "\",\"password\":\"" + password + "\"}"));
    }

    IEnumerator OnWebSocketOpen()
    {
        yield return StartCoroutine(SendWebSocketMessage("{\"type\":\"HELLO\"}"));
        attemptingReconnect = false;
    }
    void AttemptReconnect()
    {
        if (!attemptingReconnect)
        {
            attemptingReconnect = true;
            StartCoroutine(InitializeWebSocket());
        }
    }

    void OnWebSocketMessage(byte[] bytes)
    {
        if (bytes != null)
        {
            var message = Encoding.UTF8.GetString(bytes);
            if (!string.IsNullOrEmpty(message))
            {
                Debug.Log(message);
                if (NetworkController.instance == null)
                {
                    //Debug.LogError("Network controller is empty");
                    return;
                }
                NetworkController.instance.ProcessWebSockerMessage(message);
                Debug.Log("Received from server: " + message);
            }
        }
    }

    void OnWebSocketClose(WebSocketCloseCode closeCode)
    {
        Debug.Log($"WebSocket closed with code: {closeCode}");
        StartCoroutine(HandleWebSocketClose());
    }

    IEnumerator HandleWebSocketClose()
    {
        Debug.Log("Connection to WebSocket server has been closed.");
        if (websocket != null)
        {
            Debug.Log("Attempting to reconnect...");
            yield return new WaitForSeconds(5);
            StartCoroutine(InitializeWebSocket());
        }
        else
        {
            Debug.Log("Not attempting to reconnect.");
        }
    }

    void OnWebSocketError(string errorMsg)
    {
        Debug.LogError("WebSocket Error: " + errorMsg);
        StartCoroutine(HandleWebSocketError());
    }

    IEnumerator HandleWebSocketError()
    {
        int retryCount = 0;
        int backoffTime = 5;
        while (true && retryCount < 60)
        {
            Debug.Log("Handling WebSocket error. Preparing to reconnect...");
            yield return new WaitForSeconds(backoffTime);
            retryCount++;
            backoffTime *= 2;
            Debug.Log($"Attempting to reconnect after error (attempt {retryCount})...");
            StartCoroutine(InitializeWebSocket());
            if (retryCount >= 5)
            {
                Debug.LogError("Reached maximum retry attempts. Aborting reconnection.");
                break;
            }
        }
    }

    // Helper method for conversion, if necessary
    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
            websocket = null;
        }
    }

    internal void SetDungeonEnemyCards(Dungeon selectedDungeon)
    {
        dungeon = selectedDungeon.name;
    }

    public IEnumerator SendCardReward(string token, string rewardCard)
    {
        yield return StartCoroutine(SendWebSocketMessage("{\"type\":\"rewardCard\",\"token\":\"" + token + "\",\"card\":\"" + rewardCard + "\",\"level\":\"" + 1 + "\"}"));
    }

    internal IEnumerator SavePlayerCards(string token, List<Card> selectedCards)
    {
        List<string> cardSOList = new();

        foreach (Card card in selectedCards)
        {
            cardSOList.Add(card.cardSO.cardName);
        }
        CardList cardList = new CardList { cards = cardSOList };
        string cardsJson = JsonUtility.ToJson(cardList);
        string jsonMessage = "{\"type\":\"savePlayerDeck\",\"token\":\"" + token + "\",\"cards\":" + cardsJson + "}";
        yield return StartCoroutine(SendWebSocketMessage(jsonMessage));
        Debug.Log("Saving player deck: " + jsonMessage);
    }
}