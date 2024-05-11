using System;
using System.Collections;
using UnityEngine;

public class CardPointsController : MonoBehaviour
{
    public static CardPointsController instance;
    public CardPlacePoint[] playerCardPoints, enemyCardPoints;
    public float timeBetweenAttacks = .25f;
    public float timeBetweenDropTime = 0.8f;
    public float waitTimeForDamage = 0.8f;

    public void Awake()
    {
        instance = this;
    }

    public void PlayerAttack()
    {
        StartCoroutine(HandleAttackSequence(playerCardPoints, true));
    }

    public void EnemyAttack()
    {
        StartCoroutine(HandleAttackSequence(enemyCardPoints, false));
    }

    private IEnumerator Attack(CardPlacePoint attacker, CardPlacePoint defender, bool isPlayer)
    {
        attacker.activeCard.animator.SetTrigger(isPlayer ? "Attack" : "AttackShort");
        AudioManager.instance.PlayHit();

        if (defender.activeCard != null)
        {
            UIController.instance.DisplayAttackDamage(attacker.activeCard.attackPower, true, defender.activeCard);
            int defenderInitialHealth = defender.activeCard.currentHealth;
            defender.activeCard.animator.SetTrigger("Hurt");
            StartCoroutine(EffectController.instance.ShowClaw(attacker.activeCard));
            yield return StartCoroutine(Utilities.WaitForAnimation(attacker.activeCard.animator));
            bool cardDead = defender.activeCard.DamageCard(attacker.activeCard.attackPower);
            MagicController.instance.MagicDamage(defender.activeCard);
            StartCoroutine(defender.activeCard.UpdateDamage(defenderInitialHealth));
            yield return new WaitForSeconds(waitTimeForDamage); //Wait for damage
            if (cardDead)
            {
                StartCoroutine(defender.activeCard.SendToCemetery());
                UpdateAfterDead(isPlayer);
            }
        }
        else
        {
            UIController.instance.DisplayAttackDamage(attacker.activeCard.attackPower, false, null);
            if (isPlayer)
            {
                BattleController.instance.DamageEnemy(attacker.activeCard.attackPower);
            }
            else
            {
                BattleController.instance.DamagePlayer(attacker.activeCard.attackPower);
            }
            //UIController.instance.claw.transform.position = attacker.activeCard.transform.position;
        }
        yield return new WaitForSeconds(timeBetweenAttacks); //Wait for damage
    }

    private IEnumerator HandleAttackSequence(CardPlacePoint[] cardPlacePoints, bool isPlayer)
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        for (int i = 0; i < cardPlacePoints.Length; i++)
        {
            if (cardPlacePoints[i] != null && cardPlacePoints[i].activeCard != null)
            {
                yield return StartCoroutine(Attack(cardPlacePoints[i], isPlayer ? enemyCardPoints[i] : playerCardPoints[i], isPlayer));
            }
            if (BattleController.instance.battleEnded)
            {
                break;
            }
        }
        yield return new WaitForSeconds(0.3f);  // Wait after all attacks are done
        BattleController.instance.AdvanceTurn();
    }

    public void UpdateAfterDead(bool isPlayer)
    {
        int openedPoint = Utilities.GetNextPointIndex(isPlayer);
        int currentPoint = Utilities.GetCurrentPointIndex(isPlayer);
        if (currentPoint > openedPoint)
        {
            CardPlacePoint openPoint = Utilities.GetNextPoint(isPlayer);
            CardPlacePoint pointToMove = Utilities.GetLastPointOccupied(isPlayer);
            Card currentCard = pointToMove.activeCard;
            currentCard.MoveToPoint(openPoint.transform.position, Quaternion.identity);
            openPoint.activeCard = currentCard;
            pointToMove.activeCard = null;
        }
        //TODO: Implement cemetery
    }

    //Unique method to place a card on board
    public void PlaceCard(CardPlacePoint selectedPoint)
    {
        StartCoroutine(PlaceCardCO(selectedPoint));
    }

    public IEnumerator PlaceCardCO(CardPlacePoint selectedPoint)
    {
        selectedPoint.activeCard.gameObject.SetActive(true);
        selectedPoint.activeCard.MoveToPoint(selectedPoint.transform.position, Quaternion.identity);
        selectedPoint.activeCard.inHand = false;
        yield return new WaitForSeconds(timeBetweenDropTime);
        HandController.instance.RemoveCardFromHand(selectedPoint.activeCard);
        AudioManager.instance.PlaySFX(4);
    }

    public int FindIndexOfCardPlacePoint(Card cardToFind)
    {
        CardPlacePoint[] cardPoints = cardToFind.isPlayer ? playerCardPoints :enemyCardPoints;
        for (int i = 0; i < cardPoints.Length; i++)
        {
            if (cardPoints[i].activeCard == cardToFind)
            {
                return i;
            }
        }
        return -1;
    }

    public Card GetCardFromCardPlacePoint(int index)
    {
        return playerCardPoints[index].activeCard;
    }
}
