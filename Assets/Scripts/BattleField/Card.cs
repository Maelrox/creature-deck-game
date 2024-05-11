using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class Card : MonoBehaviour
{
    public CardScriptableObject cardSO;

    public bool isPlayer;
    public int cardId, xp;
    public int currentHealth, attackPower, turnToAttack;

    public TMP_Text healthText, attackText, costText, nameText, descriptionText, loreText;

    public Image characterArt, bgArt;
    public UIDamageIndicator damageCardText;

    private Vector3 targetPoint;
    private Quaternion targetRotation;
    private float moveSpeed = 5f;
    private float rotateSpeed = 540f;
    public bool inHand;
    public int handPosition;
    public int handEnemyPosition;

    private HandController theHC;
    private Collider theCol;
    public LayerMask whatIsDesktop, whatIsPlacement;
    public bool justPressed;

    public CardPlacePoint assignedPlaced;

    public Animator animator;
    public bool taken = false;
    public bool isDeckSelect = false;

    // Start is called before the first frame update
    void Start()
    {
        if (targetPoint == Vector3.zero) {
            targetPoint = transform.position;
            targetRotation = transform.rotation;
        }
        SetupCard();
        theHC = FindFirstObjectByType<HandController>();
        theCol = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDeckSelect)
        {
            transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    public void SetupCard() {
        currentHealth = cardSO.currentHealth;
        turnToAttack = cardSO.turnsToAttack;
        attackPower = cardSO.attackPower;
        descriptionText.text = cardSO.actionDescription;
        loreText.text = cardSO.cardLore;
        nameText.text = cardSO.cardName;
        characterArt.sprite = cardSO.characterSprite;
        bgArt.sprite = cardSO.bgSprite;
        UpdateCardDisplay();
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotation) {
        targetPoint = pointToMoveTo;
        targetRotation = rotation;
    }

    public void ReturnToHand() {
        theCol.enabled = true;
        MoveToPoint(theHC.cardPositions[handPosition], theHC.minPos.rotation);
    }

    public bool DamageCard(int damageAmount) {
        currentHealth -= damageAmount;
        if (currentHealth <= 0) {
            currentHealth = 0;
            
        }
        else {
            AudioManager.instance.PlaySFX(1);
        }
        return currentHealth <= 0;

    }

    public IEnumerator SendToCemetery()
    {
        Utilities.RemoveCardPoint(this);
        MoveToPoint(BattleController.instance.discardPoint.position, BattleController.instance.discardPoint.rotation);
        animator.SetTrigger("Jump");
        AudioManager.instance.PlaySFX(2);
        yield return new WaitForSeconds(0.7f);
        assignedPlaced.activeCard = null;
        Destroy(gameObject, 1f);
    }

    public void UpdateCardDisplay() {
        healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
    }
    public IEnumerator UpdateDamage(int initialHealth)
    {
        float duration = 0.6f;
        float counter = duration;
        while (counter > 0)
        {
            float interpolatedValue = Mathf.Lerp(initialHealth, currentHealth, 1 - (counter / duration));
            healthText.text = Mathf.CeilToInt(interpolatedValue).ToString();
            counter -= Time.deltaTime;
            yield return null;
        }
        healthText.text = currentHealth.ToString();
        attackText.text = attackPower.ToString();
    }


}
