using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIDungeonController : MonoBehaviour
{
    public static UIDungeonController instance;

    public TMP_Text dungeonName;
    public TMP_Text easyXP;
    public TMP_Text mediumXP;
    public TMP_Text hardXP;
    public string selectedDungeon;
    public int selectedLevel;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (SessionManager.instance.cardReceived != null && SessionManager.instance.cardReceived.Length>0)
        {
            OpenRewardModal(SessionManager.instance.cardReceived);
            //OpenRewardModal("lizard");
        }
    }

    public void OpenModal(Dungeon dungeon)
    {
        gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.SetActive(true);
        selectedDungeon = dungeon.dungeonName;
        this.dungeonName.text = dungeon.dungeonName;
        this.easyXP.text = 500 + " XP";
        this.mediumXP.text = 1000 + " XP";
        this.hardXP.text = 2000 + " XP";
        gameObject.transform.GetChild(0).gameObject.SetActive(true);

    }

    public void OpenRewardModal(string cardName)
    {
        this.dungeonName.text = cardName;
        this.easyXP.text = "Card found";
        this.mediumXP.text = "";
        this.hardXP.text = "";
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.SetActive(false);

    }

    public void CloseModal()
    {
        this.dungeonName.text = "";
        this.easyXP.text = "";
        this.mediumXP.text = "";
        this.hardXP.text = "";
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void GoToBattle()
    {
        SceneManager.LoadScene("Battle Field");
    }
}
