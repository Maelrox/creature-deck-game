using NativeWebSocket;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


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

        // Disable SSL certificate validation
        System.Net.ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
    }

    public static bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
        {
            return true;   // Is valid
        }

        Debug.Log("Certificate error: " + sslPolicyErrors);
        return true;   // For development, allow any certificate
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
        Debug.Log("Initializing WebSocket connection...");

        //websocket = new WebSocket("wss://localhost:3000");
        websocket = new WebSocket("wss://creature-deck.mooo.com:3000");
        websocket.OnOpen += () => StartCoroutine(OnWebSocketOpen());
        websocket.OnError += (e) => {
            Debug.LogError($"WebSocket Error: {e}");
            StartCoroutine(HandleWebSocketError());
        };
        websocket.OnClose += (e) => {
            Debug.Log($"WebSocket closed with code: {e}");
            StartCoroutine(HandleWebSocketClose());
        };
        websocket.OnMessage += OnWebSocketMessage;

        yield return StartCoroutine(ConnectWebSocket());
    }

    IEnumerator ConnectWebSocket()
    {
        Debug.Log("Connecting to WebSocket server...");

        Task connectTask = websocket.Connect();
        while (!connectTask.IsCompleted)
            yield return null;

        if (connectTask.Exception != null)
        {
            Debug.LogError("WebSocket connect error: " + connectTask.Exception.InnerException);
        }
        else
        {
            Debug.Log("WebSocket connected successfully.");
        }
    }

    IEnumerator SendWebSocketMessage(string message)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            Task sendTask = websocket.SendText(message);
            Debug.Log("Sending message: " + message);
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
        Debug.Log("WebSocket connection opened.");
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
                Debug.Log("Received from server: " + message);
                if (NetworkController.instance == null)
                {
                    //Debug.LogError("Network controller is empty");
                    return;
                }
                NetworkController.instance.ProcessWebSockerMessage(message);
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