using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public static BattleController instance;
    public enum TurnOrder { playerActive, playerCardAttacks, enemyActive, enemyCardAttacks }
    public TurnOrder currentPhase;
    public int startingCardsAmount = 5, cardsToDrawPerTurn = 2;
    public int playerHealth, enemyHealth, currentTurn;
    public Transform discardPoint;
    public bool battleEnded;
    public float resultScreenDelayTime = 1f;
    public List<CardScriptableObject> playerCards;
    public List<CardScriptableObject> enemyCards;

    [Range(0f,1f)]
    public float enemyFirstChance = .5f;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(WaitForCards());
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            
        }
    }

    IEnumerator WaitForCards()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(NetworkManager.instance.GetCardsAsync("Morgox"));
        yield return new WaitUntil(() => NetworkManager.instance.cardsReceived);
        SetBattle();
    }

    private void SetBattle()
    {
        DeckController.instance.DrawMultipleCards(startingCardsAmount, true);
        DeckController.instance.DrawMultipleCards(startingCardsAmount, false);
        UIController.instance.SetPlayerHealthText(playerHealth);
        UIController.instance.healthBar.SetMaxHealth(playerHealth);
        UIController.instance.enemyHealthBar.SetMaxHealth(enemyHealth);
        UIController.instance.SetEnemyHealthText(enemyHealth);
        if (Random.value > enemyFirstChance)
        {
            currentPhase = TurnOrder.playerCardAttacks;
            AdvanceTurn();
        }
        AudioManager.instance.PlayBGM();
    }

    public void AdvanceTurn()
    {
        if (!battleEnded)
        {
            currentTurn++;
            MagicController.instance.NextTurn();
            currentPhase++;
            if ((int)currentPhase >= System.Enum.GetValues(typeof(TurnOrder)).Length)
            {
                currentPhase = 0;
            }
            switch (currentPhase)
            {
                case TurnOrder.playerActive:
                    Button btn = UIController.instance.endTurnButton.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.interactable = true;
                    }
                    UIController.instance.drawCardButton.SetActive(true);
                    DeckController.instance.DrawMultipleCards(cardsToDrawPerTurn, true);
                    break;
                case TurnOrder.playerCardAttacks:
                    CardPointsController.instance.PlayerAttack();
                    DecreaseCardTurns();
                    break;
                case TurnOrder.enemyActive:
                    EnemyController.instance.StartAction();
                    break;
                case TurnOrder.enemyCardAttacks:
                    CardPointsController.instance.EnemyAttack();
                    DecreaseCardTurns();
                    DeckController.instance.DrawMultipleCards(cardsToDrawPerTurn, false);
                    break;
            }
        }
    }

    public void EndPlayerTurn()
    {
        Button btn = UIController.instance.endTurnButton.GetComponent<Button>();
        if (btn != null)
        {
            btn.interactable = false; 
        }
        UIController.instance.drawCardButton.SetActive(false);
        AdvanceTurn();
    }

    public void DamagePlayer(int damageAmount)
    {
        if (playerHealth > 0 || !battleEnded)
        {
            playerHealth -= damageAmount;
            if (playerHealth <= 0)
            {
                playerHealth = 0;
                EndBattle();
            }
            UIController.instance.SetPlayerHealthText(playerHealth);
            AudioManager.instance.PlaySFX(6);
        }
    }

    public void DamageEnemy(int damageAmount)
    {
        if (enemyHealth > 0 || !battleEnded)
        {
            enemyHealth -= damageAmount;
            if (enemyHealth <= 0)
            {
                enemyHealth = 0;
                EndBattle();
            }
            UIController.instance.SetEnemyHealthText(enemyHealth);
        }
        AudioManager.instance.PlaySFX(5);
    }

    void EndBattle()
    {
        battleEnded = true;
        if (enemyHealth <= 0) {
            UIController.instance.battleResultText.text = "YOU WON!";
            foreach (CardPlacePoint point in CardPointsController.instance.enemyCardPoints) {
                if (point.activeCard != null) point.activeCard.MoveToPoint(discardPoint.position, point.activeCard.transform.rotation);
            }
            StartCoroutine(NetworkManager.instance.EndBattle("Morgox"));
            StartCoroutine(NetworkManager.instance.CompleteDungeons(SessionManager.instance.activeDungeon.dungeonName, "Morgox", SessionManager.instance.selectedLevel + 1));
            float probability = Random.Range(0f, 1f);
            if (probability > 0.1f)
            {
                SessionManager.instance.RewardCardToPlayer();
            }
        }
        else {
             foreach (CardPlacePoint point in CardPointsController.instance.playerCardPoints) {
                if (point.activeCard != null) point.activeCard.MoveToPoint(discardPoint.position, point.activeCard.transform.rotation);
             }
            UIController.instance.battleResultText.text = "YOU LOST!";
        }
        StartCoroutine(ShowResultsCo());
    }

    IEnumerator ShowResultsCo()
    {
        yield return new WaitForSeconds(resultScreenDelayTime);
        UIController.instance.battleEndScreen.SetActive(true);
    }

    public void DecreaseCardTurns()
    {
        var cardThumbnailPairs = new List<(Card card, List<Thumbnail> thumbnails)>();
        cardThumbnailPairs.AddRange(HandController.instance.heldEnemyCards.Select(card => (card, HandController.instance.heldEnemyCardThumbnails)));
        cardThumbnailPairs.AddRange(HandController.instance.heldCards.Select(card => (card, HandController.instance.heldCardThumbnails)));
        foreach (var (card, thumbnails) in cardThumbnailPairs)
        {
            HandleCardTurn(card, thumbnails);
        }
    }

    private void HandleCardTurn(Card card, List<Thumbnail> thumbnails)
    {
        card.turnToAttack = Mathf.Max(0, card.turnToAttack - 1);
        Thumbnail thumbnail = thumbnails.FirstOrDefault(t => t.card == card);
        if (thumbnail != null) thumbnail.turnText.text = card.turnToAttack.ToString();
    }

}
