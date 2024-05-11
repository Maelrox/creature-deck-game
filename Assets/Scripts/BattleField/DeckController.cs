using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static NetworkManager;
using static Utilities;

public class DeckController : MonoBehaviour
{
    public static DeckController instance;

    public int drawCardCost = 2;

    public float waitBetweenDrawingCards = .25f;

    void Awake()
    {
        instance = this;
    }

    private List<CardScriptableObject> activeCards = new();

    public List<CardScriptableObject> enemyDeckToUse = new();
    private List<CardScriptableObject> enemyActiveCards = new();

    private List<CardScriptableObject> cemetery = new();
    public List<CardScriptableObject> enemyCemetery = new();

    public Card cardToSpawn;
    public Thumbnail thumbnail;

    public Card enemyCard;

    public void SetupDeck()
    {
        activeCards.Clear();
        List<CardScriptableObject> tempDeck = new();
        tempDeck.AddRange(SessionManager.instance.deckCards);
        int iterations = 0;
        while (tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            activeCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);
            iterations++;
        }
    }

    public void SetupEnemyDeck()
    {
        enemyActiveCards.Clear();
        List<CardScriptableObject> tempDeck = new();
        tempDeck.AddRange(enemyDeckToUse);
        int iterations = 0;
        while (tempDeck.Count > 0 && iterations < 500)
        {
            int selected = Random.Range(0, tempDeck.Count);
            enemyActiveCards.Add(tempDeck[selected]);
            tempDeck.RemoveAt(selected);
            iterations++;
        }
    }

    public void DrawCardToHand(bool isPlayer)
    {
        if (activeCards.Count == 0 && isPlayer)
        {
            return;
        }
        if (enemyActiveCards.Count == 0 && !isPlayer)
        {
            return;
        }
        Card newCard;
        if (isPlayer)
        {
            newCard = Instantiate(cardToSpawn, transform.position, transform.rotation);
            newCard.cardSO = activeCards[0];
            activeCards.RemoveAt(0);
        }
        else
        {
            newCard = Instantiate(enemyCard, transform.position, transform.rotation);
            newCard.cardSO = enemyActiveCards[0];
            enemyActiveCards.RemoveAt(0);
        }
        Thumbnail newThumbnail = Instantiate(thumbnail, transform.position, transform.rotation);
        newCard.SetupCard();
        newThumbnail.SetupThumbnail(newCard);
        HandController.instance.AddCardToHand(newCard, newThumbnail);
        AudioManager.instance.PlaySFX(3);
    }

    public void DrawMultipleCards(int amountToDraw, bool isPlayer) {
        StartCoroutine(DrawMultipleCo(amountToDraw, isPlayer));
    }

    IEnumerator DrawMultipleCo(int amountToDraw, bool isPlayer) {
        for (int i = 0; i < amountToDraw; i++)
        {
            DrawCardToHand(isPlayer);
            yield return new WaitForSeconds(waitBetweenDrawingCards);
        }
    }

    public void SetPlayableCards(string message)
    {
        Debug.Log(message);
        CardMessage cardMessage = JsonUtility.FromJson<CardMessage>(message);
        foreach (CardServer cardInServer in cardMessage.data)
        {
            CardScriptableObject storedCard = CardScriptableManager.instance.LoadCardByName(cardInServer.cardId);
            CardScriptableManager.instance.ConfigureStatsByXp(storedCard, cardInServer.xp);
            SessionManager.instance.deckCards.Add(storedCard);
        }
        SetupDeck();
        SetDungeonEnemyCards();
    }

    public void SetDungeonEnemyCards()
    {
        DungeonScriptableObject dungeon = SessionManager.instance.activeDungeon.dungeonSO;
        foreach (CardScriptableObject card in dungeon.cards)
        {
            CardScriptableManager.instance.ConfigureStatsByXp(card, (dungeon.level * 5));
            enemyDeckToUse.Add(card);
        }
        SetupEnemyDeck();
    }


}
