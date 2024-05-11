using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : ScriptableObject
{
    public static void RemoveCardPoint(Card card)
    {
        CardPlacePoint[] cardPoints = card.isPlayer ? CardPointsController.instance.playerCardPoints : CardPointsController.instance.enemyCardPoints;
        foreach (CardPlacePoint point in cardPoints)
        {
            if (point != null && point.activeCard != null && point.activeCard == card)
            {
                point.activeCard = null;
            }
        }
    }

    public static CardPlacePoint GetNextPoint(bool isPlayer)
    {
        CardPlacePoint[] cardPoints = isPlayer ? CardPointsController.instance.playerCardPoints : CardPointsController.instance.enemyCardPoints;
        foreach (CardPlacePoint point in cardPoints)
        {
            if (point.activeCard == null)
            {
                return point;
            }
        }
        return null;
    }

    public static int GetNextPointIndex(bool isPlayer)
    {
        int index = 0;
        CardPlacePoint[] cardPoints = isPlayer ? CardPointsController.instance.playerCardPoints : CardPointsController.instance.enemyCardPoints;
        foreach (CardPlacePoint point in cardPoints)
        {
            index++;
            if (point.activeCard == null)
            {
                return index;
            }
        }
        return index;
    }

    public static int GetCurrentPointIndex(bool isPlayer)
    {
        CardPlacePoint[] cardPoints = isPlayer ? CardPointsController.instance.playerCardPoints : CardPointsController.instance.enemyCardPoints;
        int index = cardPoints.Length;
        for (int i = cardPoints.Length - 1; i >= 0; i--)
        {
            if (cardPoints[i].activeCard != null)
            {
                return index;
            }
            index--;
        }
        return index;
    }

    public static CardPlacePoint GetLastPointOccupied(bool isPlayer)
    {
        CardPlacePoint[] cardPoints = isPlayer ? CardPointsController.instance.playerCardPoints : CardPointsController.instance.enemyCardPoints;
        int index = cardPoints.Length;
        for (int i = cardPoints.Length - 1; i >= 0; i--)
        {
            if (cardPoints[i].activeCard != null)
            {
                return cardPoints[i];
            }
            index--;
        }
        return null;
    }

    public static CardPlacePoint GetCurrentPoint(bool isPlayer)
    {
        CardPlacePoint[] cardPoints = isPlayer ? CardPointsController.instance.playerCardPoints : CardPointsController.instance.enemyCardPoints;
        foreach (CardPlacePoint point in cardPoints)
        {
            if (point.activeCard != null)
            {
                return point;
            }
        }
        return null;
    }

    public static CardPlacePoint GetCardPoint(Card cardToFind)
    {
        CardPlacePoint[] cardPoints = cardToFind.isPlayer ? CardPointsController.instance.playerCardPoints : CardPointsController.instance.enemyCardPoints;
        foreach (CardPlacePoint point in cardPoints)
        {
            if (point.activeCard != null && point.activeCard == cardToFind)
            {
                return point;
            }
        }
        return null;
    }

    public static IEnumerator WaitForAnimation(Animator animator)
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
    }

    internal static void DevLogin()
    {
        SessionManager.instance.token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI2NjNmNmI3YmIyYWJjZjY1YTFmMGNlMmQiLCJpYXQiOjE3MTU0MzIzNDUsImV4cCI6MTcxNTYwNTE0NX0.5UMMsRetpHJgyRtd5YeLDj-CrUQVvUuMo_3hmurcsdE";
    }

    [System.Serializable]
    public class CardMessage
    {
        public string type;
        public List<CardServer> data;
    }


    [System.Serializable]
    public class CardServer
    {
        public string _id;
        public string cardId;
        public string userId;
        public int xp;
    }

    [System.Serializable]
    public class DungeonMessage
    {
        public string type;
        public List<DungeonServer> data;
    }


    [System.Serializable]
    public class DungeonServer
    {
        public string _id;
        public string dungeon;
        public int level;
    }
}
