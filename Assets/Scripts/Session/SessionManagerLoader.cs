using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManagerLoader : MonoBehaviour
{
    public SessionManager session;

    private void Awake()
    {
        if (FindFirstObjectByType<SessionManager>() == null)
        {
            SessionManager.instance = Instantiate(session);
            DontDestroyOnLoad(SessionManager.instance.gameObject);
        }
    }
}
