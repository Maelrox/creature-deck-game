using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dungeon", menuName = "Dungeon", order = 2)]
public class DungeonScriptableObject : ScriptableObject
{
    public string dungeonName;
    public bool easyCompleted;
    public bool mediumCompleted;
    public bool hardCompleted;
    public int level;
    public int index;
    public List<CardScriptableObject> cards; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
