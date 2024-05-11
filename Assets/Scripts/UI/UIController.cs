using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    public static UIController instance;
    public TMP_Text playerHealthText, enemyHealthText;
    public GameObject manaWarning;
    public float manaWarningTime;

    public GameObject drawCardButton, endTurnButton;

    public GameObject battleEndScreen;

    public TMP_Text battleResultText;

    public string mainMenuScene, battleSelectScene;

    public GameObject pauseScreen;
    public GameObject strikeEffect;
    public GameObject claw;

    public GameObject point1, point2, point3, point4, point5;
    public UIDamageIndicator damageIndicator;

    [SerializeField] 
    public HealthBar healthBar;

    [SerializeField]
    public HealthBar enemyHealthBar;

    private void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseUnpause();
        }
    }

    public void SetPlayerHealthText(int healthAmount) {
        playerHealthText.text = "Player Health: " + healthAmount;
        healthBar.SetHealth(healthAmount);
    }

    public void SetEnemyHealthText(int healthAmount) {
        enemyHealthText.text = "Enemy Health: " + healthAmount;
        enemyHealthBar.SetHealth(healthAmount);
    }


    public void ShowManaWarning() {
        manaWarning.SetActive(true);//REUSE LIKE SEEING CARD STATS
    }

    public void PassTurn() {
        BattleController.instance.AdvanceTurn();
    }

    public void EndPlayerTurn() {
        BattleController.instance.EndPlayerTurn();
        AudioManager.instance.PlaySFX(0);
    }
    
    public void MainMenu() {
        SceneManager.LoadScene(mainMenuScene);
        AudioManager.instance.PlaySFX(0);
        Time.timeScale = 1f;
    }
    
    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
        AudioManager.instance.PlaySFX(0);
    }

    public void ChooseNewBattle() {
        SceneManager.LoadScene(battleSelectScene);
        Time.timeScale = 1f;
        AudioManager.instance.PlaySFX(0);
    }
    
    public void PauseUnpause() {
        if(!pauseScreen.activeSelf) {
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
        } else {
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
        }
        AudioManager.instance.PlaySFX(0);
    }


    public void DisplayAttackDamage(int damage, bool targetIsCard, Card cardToFind)
    {
        UIDamageIndicator damageClone;

        if (targetIsCard)
        {
            int index = CardPointsController.instance.FindIndexOfCardPlacePoint(cardToFind);
            Vector3 newPosition = damageIndicator.transform.position;
            newPosition.x += (index * 250);
            damageClone = Instantiate(damageIndicator, newPosition, Quaternion.identity, damageIndicator.transform.parent);
        }
        else
        {
            damageClone = Instantiate(damageIndicator, damageIndicator.transform.parent); //TODO: Move to player health
        }
        damageClone.damageText.text = (damage * -1).ToString();
        damageClone.gameObject.SetActive(true);
    }


}
