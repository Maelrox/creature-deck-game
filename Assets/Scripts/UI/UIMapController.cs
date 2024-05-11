using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Linq;

using static Utilities;
public class UIMapController : MonoBehaviour
{

    public static UIMapController instance;

    public Image image;
    public string dungeonName; //Mapname

    public List<DungeonScriptableObject> dungeons;
    public Dungeon dungeonToSpawn;
    private const string DungeonsPath = "Dungeons/";
    public string selectedDungeon;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForDungeons());
    }

    IEnumerator WaitForDungeons()
    {
        yield return new WaitForSeconds(0.7f);
        Debug.Log("Fetching dungeons...");
        StartCoroutine(NetworkManager.instance.GetDungeonsAsync("Morgox")); 
        yield return new WaitUntil(() => NetworkManager.instance.dungeonsReceived);
        Debug.Log("Dungones received...");
    }

    public void ConfirmModal(Dungeon dungeon)
    {
        SessionManager.instance.activeDungeon = dungeon;
        SessionManager.instance.selectedLevel = dungeon.level;
        UIDungeonController.instance.OpenModal(dungeon);
    }

    public void CreateDungeon(DungeonScriptableObject storedDungeon)
    {
        GameObject holder = GameObject.Find("Dungeon Holder " + storedDungeon.index);
        if (holder != null)
        {
            Dungeon dungeon = Instantiate(dungeonToSpawn, transform.position, transform.rotation, holder.transform);
            if (storedDungeon.level >= 1) dungeon.easyCompleted = true;
            if (storedDungeon.level >= 2) dungeon.mediumCompleted = true;
            if (storedDungeon.level >= 3) dungeon.hardCompleted = true;

            dungeon.dungeonName = storedDungeon.dungeonName;
            dungeon.index = storedDungeon.index;
            dungeon.level = storedDungeon.level;
            dungeon.transform.localPosition = Vector3.zero;
            dungeon.gameObject.SetActive(true);
            dungeon.dungeonSO = storedDungeon;
        }
        else
        {
            Debug.LogError("Dungeon Holder " + storedDungeon.index + " not found!");
        }
    }

    public void SetPlayableDungeons(string message)
    {
        Debug.Log("Setting playable dungeons");
        int currentIndex = 0;
        DungeonMessage dungeonMessage = JsonUtility.FromJson<DungeonMessage>(message);
        foreach (DungeonServer dungeonInServer in dungeonMessage.data)
        {
            DungeonScriptableObject storedDungeon = LoadDungeonByName(dungeonInServer.dungeon);
            storedDungeon.level = dungeonInServer.level;
            currentIndex = storedDungeon.index;
            CreateDungeon(storedDungeon);
        }
        if (dungeonMessage.data == null || dungeonMessage.data.Count == 0)
        {
            CreateDungeon(LoadDungeonByName("Rock Mountains"));
        }
        if (dungeons.Count > 0 && currentIndex > 0)
        {
            ShowNextDungeon(currentIndex);
        }

    }

    private void ShowNextDungeon(int index)
    {
        DungeonScriptableObject nextDungeon = dungeons.OrderBy(d => d.index).FirstOrDefault(d => d.index > index);
        if (nextDungeon != null)
        {
            DungeonScriptableObject storedDungeon = LoadDungeonByName(nextDungeon.dungeonName);
            CreateDungeon(storedDungeon);
        }
    }

    public DungeonScriptableObject LoadDungeonByName(string dungeonToLoad)
    {
        DungeonScriptableObject dungeon = Resources.Load<DungeonScriptableObject>(DungeonsPath + dungeonToLoad);
        if (dungeon == null)
        {
            Debug.LogError("No dungeon found with the name: " + dungeonToLoad);
            return null;
        }
        return dungeon;
    }

    public void CompleteDungeon(string playerId, string dungeonName, int level)
    {   

    }
}
