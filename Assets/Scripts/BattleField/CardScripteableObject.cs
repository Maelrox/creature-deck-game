using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]
public class CardScriptableObject : ScriptableObject
{
    public string cardName;
    [TextArea]
    public string actionDescription, cardLore;
    public int currentHealth, attackPower, turnsToAttack;
    public Sprite characterSprite, bgSprite;

    public MagicController.MagicType[] magicTypes;

    public int poisonPower;
    public int icePower;
    public int firePower;
    public int spiritPower;
    public int earthPower;
    public int assasinPower;
    public int healingPower;
}
