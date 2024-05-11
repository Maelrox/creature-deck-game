using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager instance;

    public string playerId;
    public string playerName;
    public Dungeon activeDungeon;
    public List<CardScriptableObject> deckCards = new();
    public List<CardScriptableObject> gameCards;
    public string cardReceived;
    public int selectedLevel;
    public string token;

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

    private void Start()
    {

    }

    public void RewardCardToPlayer()
    {
        if (deckCards.Count >= gameCards.Count)
        {
            Debug.Log("No more unique reward cards available.");
        }
        CardScriptableObject rewardCard = new CardScriptableObject();
        HashSet<int> triedIndices = new HashSet<int>();
        while (triedIndices.Count < gameCards.Count)
        {
            int randomIndex = UnityEngine.Random.Range(0, gameCards.Count);
            if (triedIndices.Contains(randomIndex))
                continue;

            triedIndices.Add(randomIndex);
            rewardCard = gameCards[randomIndex];

            if (!deckCards.Contains(rewardCard))
            {
                deckCards.Add(rewardCard);
                Debug.Log("Reward Card added: " + rewardCard.name);
                StartCoroutine(NetworkManager.instance.SendCardReward("Morgox", rewardCard.name));
                cardReceived = rewardCard.name;
                break;
            }
        }
        Debug.Log("All cards have been rewarded already. No more unique cards available.");
    }
}
