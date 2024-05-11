using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScriptableManager : MonoBehaviour
{
    private const string CardsPath = "Card/"; // Path within the Resources folder
    private const string DungeonsPath = "Dungeons/"; // Path within the Resources folder

    public static CardScriptableManager instance;

    void Awake()
    {
        instance = this;
    }

    public CardScriptableObject LoadCardByName(string cardName)
    {
        CardScriptableObject card = Resources.Load<CardScriptableObject>(CardsPath + cardName);
        if (card == null)
        {
            Debug.LogError("No card found with the name: " + cardName);
            return null;
        }
        return card;
    }

    public DungeonScriptableObject LoadDungeonByName(string dungeonName)
    {
        DungeonScriptableObject card = Resources.Load<DungeonScriptableObject>(DungeonsPath + dungeonName);
        if (card == null)
        {
            Debug.LogError("No dungeon found with the name: " + dungeonName);
            return null;
        }
        return card;
    }

    public void ConfigureStatsByXp(CardScriptableObject card, int xp)
    {
        int level = xp / 1000;
        int levelAttackModifier = 10 * level;
        int healthModifier = 10 * level;
        card.attackPower += levelAttackModifier;
        card.currentHealth += healthModifier;
    }
}
