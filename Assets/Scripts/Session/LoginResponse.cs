using UnityEditor;
using UnityEngine;


[System.Serializable]
public class LoginResponse
{
    public Data data;

    [System.Serializable]
    public class Data
    {
        public string token;
    }
}