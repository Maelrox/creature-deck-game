using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Thumbnail : MonoBehaviour
{
    public TMP_Text healthText, attackText, turnText;
    public GameObject thumbnailArt;

    private HandController theHC;
    private Collider theCol;
    public CardPlacePoint assignedPlaced;
    public Card card;

    public LayerMask whatIsDesktop, whatIsPlacement;
    private Vector3 targetPoint;
    private Quaternion targetRotation;

    private float moveSpeed = 5f;
    private float rotateSpeed = 540f;
    private bool isSelected;
    public bool justPressed;
    public bool inHand;

    void Start()
    {
        theHC = FindFirstObjectByType<HandController>();
        theCol = GetComponent<Collider>();
    }

    void Update()
    {
        //transform.position = targetPoint;
        transform.position = Vector3.Lerp(transform.position, targetPoint, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        if (isSelected && BattleController.instance.battleEnded == false && Time.timeScale != 0f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, whatIsDesktop))
            {
                MoveToPoint(hit.point + new Vector3(0f, 2f, 0f), Quaternion.identity);
            }

        }
    }

    public void SetupThumbnail(Card card)
    {
        this.card = card;
        attackText.text = card.cardSO.attackPower.ToString();
        healthText.text = card.cardSO.currentHealth.ToString();
        turnText.text = card.turnToAttack.ToString();
        thumbnailArt.gameObject.GetComponent<Image>().sprite = card.cardSO.characterSprite;
    }

    public void ReturnToHand()
    {
        isSelected = false;
        theCol.enabled = true;
        MoveToPoint(theHC.cardPositions[card.handPosition], theHC.minPos.rotation);
    }


    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotation)
    {
        targetPoint = pointToMoveTo;
        targetRotation = rotation;
    }

    public void OnMouseOver()
    {
        if (card.isPlayer)
        {
            MoveToPoint(theHC.cardPositions[card.handPosition] + new Vector3(0f, 0.1f, .1f), Quaternion.identity);
        }
    }

    public void OnMouseExit()
    {
        if (card.isPlayer)
        {
            MoveToPoint(theHC.cardPositions[card.handPosition], theHC.minPos.rotation);
        }
    }

    public void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && !BattleController.instance.battleEnded && card.isPlayer && !justPressed)
        {
            justPressed = true;
            if (BattleController.instance.currentPhase == BattleController.TurnOrder.playerActive && card.isPlayer && card.turnToAttack == 0 && !card.taken)
            {
                card.taken = true;
                CardPlacePoint selectedPoint = Utilities.GetNextPoint(card.isPlayer);
                card.isPlayer = true;
                selectedPoint.activeCard = card;
                card.assignedPlaced = selectedPoint;
                assignedPlaced = selectedPoint;
                CardPointsController.instance.PlaceCard(selectedPoint);
            }
            else
            {
                ReturnToHand();
            }
            justPressed = false;
        }
    }
}
