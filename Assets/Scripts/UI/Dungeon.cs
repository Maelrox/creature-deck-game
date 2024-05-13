using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dungeon : MonoBehaviour
{
    public DungeonScriptableObject dungeonSO;
    public TMP_Text textDungeonName;
    public Image easy;
    public Image medium;
    public Image hard;

    public string dungeonName;
    public bool easyCompleted;
    public bool mediumCompleted;
    public bool hardCompleted;
    public int index;
    public int level;

    void Start()
    {
        SetupDungeon();

    }

    private void SetupDungeon()
    {
        if (easyCompleted) easy.gameObject.SetActive(true);
        if (mediumCompleted) medium.gameObject.SetActive(true);
        if (hardCompleted) hard.gameObject.SetActive(true);
        textDungeonName.text = dungeonName;
    }

    private void InitializeForTesting()
    {
        dungeonName = "Dark Caverns";
        easyCompleted = true;
        mediumCompleted = false;
        hardCompleted = false;
        index = 3;
        level = 10;
    }


    void Update()
    {
        
    }

}
