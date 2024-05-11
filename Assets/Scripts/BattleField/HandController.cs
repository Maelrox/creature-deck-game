using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour
{
    public static HandController instance;

    void Awake()
    {
        instance = this;
    }

    public List<Card> heldCards;
    public List<Thumbnail> heldCardThumbnails;

    public List<Card> heldEnemyCards;
    public List<Thumbnail> heldEnemyCardThumbnails;

    public Transform minPos, maxPos;
    public Transform enemyMinPos, enemyMaxPos;

    public List<Vector3> cardPositions = new();
    public List<Vector3> enemyCardPositions = new();

    void Start()
    {
        PositionCardsInHand();
    }

    // Unique methdo to remove a card from the hand
    public void RemoveCardFromHand(Card cardToRemove)
    {
        if (cardToRemove.isPlayer)
        {
            if (heldCards[cardToRemove.handPosition] == cardToRemove)
            {
                heldCardThumbnails[cardToRemove.handPosition].gameObject.SetActive(false);
                heldCards.RemoveAt(cardToRemove.handPosition);
                heldCardThumbnails.RemoveAt(cardToRemove.handPosition);
                cardToRemove.gameObject.SetActive(true);
            }

        }
        else
        {
            if (cardToRemove.handEnemyPosition < heldEnemyCards.Count && heldEnemyCards[cardToRemove.handEnemyPosition] == cardToRemove)
            {
                heldEnemyCardThumbnails[cardToRemove.handEnemyPosition].gameObject.SetActive(false);
                heldEnemyCards.RemoveAt(cardToRemove.handEnemyPosition);
                heldEnemyCardThumbnails.RemoveAt(cardToRemove.handEnemyPosition);
                cardToRemove.gameObject.SetActive(true);
            }
        }
        PositionCardsInHand();

    }


    public void AddCardToHand(Card cartToAdd, Thumbnail thumbnail)
    {
        if (cartToAdd.isPlayer)
        {
            heldCards.Add(cartToAdd);
            heldCardThumbnails.Add(thumbnail);
        }
        else
        {
            heldEnemyCards.Add(cartToAdd);
            heldEnemyCardThumbnails.Add(thumbnail);
        }
        PositionCardsInHand();
    }

    public void PositionCardsInHand()
    {
        Vector3 distanceBetweenPoints = Vector3.zero;
        cardPositions.Clear();
        enemyCardPositions.Clear();

        if (heldCards.Count > 1)
        {
            float cardWidth = heldCards[0].GetComponent<BoxCollider>().size.x;
            distanceBetweenPoints = (maxPos.position - minPos.position).normalized * cardWidth;
        }

        Vector3 currentPos = enemyMinPos.position;
        for (int i = 0; i < heldEnemyCards.Count; i++)
        {
            enemyCardPositions.Add(currentPos);
            heldEnemyCardThumbnails[i].MoveToPoint(currentPos, enemyMinPos.rotation);
            heldEnemyCards[i].inHand = true;
            heldEnemyCards[i].handEnemyPosition = i;
            heldEnemyCards[i].gameObject.SetActive(false);
            currentPos += distanceBetweenPoints;
        }


        currentPos = minPos.position;
        for (int i = 0; i < heldCards.Count; i++)
        {
            cardPositions.Add(currentPos);
            heldCardThumbnails[i].MoveToPoint(currentPos, minPos.rotation);
            heldCards[i].inHand = true;
            heldCards[i].handPosition = i;
            heldCards[i].gameObject.SetActive(false);
            currentPos += distanceBetweenPoints;

        }
    }

}
