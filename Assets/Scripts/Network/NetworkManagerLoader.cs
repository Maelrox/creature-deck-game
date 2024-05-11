using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManagerLoader : MonoBehaviour
{
    public NetworkManager network;

    private void Awake() {
        if (FindFirstObjectByType<NetworkManager>() == null) {
            NetworkManager.instance = Instantiate(network);
            DontDestroyOnLoad(NetworkManager.instance.gameObject);
        }
    }
}
