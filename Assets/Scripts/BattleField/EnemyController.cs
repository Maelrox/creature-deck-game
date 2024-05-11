using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class EnemyController : MonoBehaviour
{
    public static EnemyController instance;

    public List<CardScriptableObject> deckToUse = new();
    private List<CardScriptableObject> activeCards = new();
    private HandController theHC;

    public Card cardToSpawn;
    public CardPlacePoint assignedPlaced;
    public Thumbnail thumbnail;

    public Transform cardSpawnPoint;

    public enum AIType { placeFromDeck, handRandomPlace, handDefensive, handAttacking }
    public AIType enemyAIType;

    private List<CardScriptableObject> cardsInHand = new List<CardScriptableObject>();
    public int startHandSize;

    public int costPerDraw = 0;

    void Awake(){
        instance = this;
    }

    private void Start()
    {
        theHC = FindFirstObjectByType<HandController>();
    }

    public void StartAction() {
        StartCoroutine(EnemyActionCo());
    }

    /**
     * Couroutine for Enemy Actions
     **/
    IEnumerator EnemyActionCo() {
        switch(enemyAIType) {
            case AIType.placeFromDeck:
                PlayCard();
                break;
            case AIType.handDefensive:
                break;
            case AIType.handAttacking:
                break;
        }
        yield return new WaitForSeconds(.5f);
        BattleController.instance.AdvanceTurn();
    }

    public void PlayCard()
    {
        StartCoroutine(PlayCardCo());
    }

    IEnumerator PlayCardCo()
    {
        yield return new WaitForSeconds(0.5f);
        List<Card> enemyCards = HandController.instance.heldEnemyCards;
        List<Card> readyToAttackCards = enemyCards.Where(card => card.turnToAttack == 0).ToList();
        if (readyToAttackCards.Count > 0)
        {
            foreach (Card card in readyToAttackCards)
            {
                CardPlacePoint selectedPoint = Utilities.GetNextPoint(card.isPlayer);
                card.isPlayer = false;
                card.assignedPlaced = selectedPoint;
                selectedPoint.activeCard = card;
                CardPointsController.instance.PlaceCard(selectedPoint);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

}
