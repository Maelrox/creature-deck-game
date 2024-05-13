using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NativeWebSocket;
using static Utilities;
using UnityEngine.EventSystems;

public class DeckSelectController : MonoBehaviour
{
    public Transform bottomCarouselStartingPoint;
    public Transform bottomContainer;

    public Transform upperCarouselStartingPoint;
    public Transform upperContainer;

    public float cardSpacing = 0.1f;
    public float scrollSpeed = 5.0f;
    private int selectedIndex = 0;
    private int deckCapacity = 10;

    public GameObject spinner;
    public Card cardToSpawn;

    public List<Card> availableCards;
    public List<Card> selectedCards;


    public static DeckSelectController instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Utilities.DevLogin();
        StartCoroutine(GetPlayerCards());
    }

    private IEnumerator GetPlayerCards()
    {
        yield return new WaitUntil(() => NetworkManager.instance.websocket != null && NetworkManager.instance.websocket.State == WebSocketState.Open);
        spinner.SetActive(true);
        StartCoroutine(NetworkManager.instance.GetPlayerCards(SessionManager.instance.token));
        yield return new WaitUntil(() => NetworkManager.instance.cardsReceived);
        spinner.SetActive(false);
        DrawCardToCarousel();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Scroll(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Scroll(-1);
        }
    }

    private void Scroll(int direction)
    {
        int newIndex = selectedIndex + direction;
        if (newIndex < 0 || newIndex >= availableCards.Count) return;
        selectedIndex = newIndex;
        PositionBottomCards();
    }

    private void PositionBottomCards()
    {
        float baseOffset = cardSpacing;

        for (int i = 0; i < availableCards.Count; i++)
        {
            float offset = (i - selectedIndex) * baseOffset;
            Vector3 targetPosition = bottomCarouselStartingPoint.position + new Vector3(offset, 0, 0);
            StartCoroutine(SmoothMove(availableCards[i].transform, availableCards[i].transform.position, targetPosition));
        }
    }

    private IEnumerator SmoothMove(Transform cardTransform, Vector3 startPosition, Vector3 endPosition)
    {
        float duration = 0.5f;
        float elapsed = 0;
        while (elapsed < duration)
        {
            cardTransform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            cardTransform.rotation = Quaternion.identity;
            elapsed += Time.deltaTime;
            yield return null;
        }
        cardTransform.position = endPosition;

    }

    public void DrawCardToCarousel()
    {
        Transform parentObject = bottomContainer.gameObject.transform;

        foreach (CardScriptableObject cardSO in SessionManager.instance.deckCards)
        {
            Card newCard = Instantiate(cardToSpawn, bottomCarouselStartingPoint.localPosition, parentObject.rotation, parentObject).GetComponent<Card>();
            newCard.cardSO = cardSO;
            newCard.isDeckSelect = true;
            newCard.gameObject.SetActive(true);
            newCard.SetupCard();
            AudioManager.instance.PlaySFX(3);
            availableCards.Add(newCard);
            AddTrigger(newCard);
        }
        PositionBottomCards();
    }

    private void AddTrigger(Card newCard)
    {
        EventTrigger trigger = newCard.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnCardClicked((PointerEventData)data, newCard); });
        trigger.triggers.Add(entry);
    }

    public void SetPlayableCards(string message)
    {
        CardMessage cardMessage = JsonUtility.FromJson<CardMessage>(message);
        foreach (CardServer cardInServer in cardMessage.data)
        {
            CardScriptableObject storedCard = CardScriptableManager.instance.LoadCardByName(cardInServer.cardId);
            CardScriptableManager.instance.ConfigureStatsByXp(storedCard, cardInServer.xp);
            SessionManager.instance.deckCards.Add(storedCard);
        }
    }

    void OnCardClicked(PointerEventData data, Card selectedCard)
    {
        if (selectedCards.Count < deckCapacity)
        {
            float offset = (selectedCards.Count) * cardSpacing;
            Vector3 targetPosition = upperCarouselStartingPoint.position + new Vector3(offset, 0, 0);
            StartCoroutine(SmoothMove(selectedCard.transform, selectedCard.transform.position, targetPosition));
            availableCards.Remove(selectedCard);
            selectedCards.Add(selectedCard);
            PositionBottomCards();
        } else
        {
            Debug.Log("Max card deck reached ");
        }
    }
    
    public void SaveDeck()
    {
        StartCoroutine(NetworkManager.instance.SavePlayerCards(SessionManager.instance.token, selectedCards));
    }

}