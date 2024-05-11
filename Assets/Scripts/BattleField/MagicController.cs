using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicController : MonoBehaviour
{
    public static MagicController instance;
    public enum MagicType
    {
        Poison,
        Ice,
        Fire,
        Spirit,
        Earth,
        Assassin
    }

    private Dictionary<MagicType, List<DoTEffect>> activeDoTEffects = new Dictionary<MagicType, List<DoTEffect>>();


    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (MagicType type in Enum.GetValues(typeof(MagicType)))
        {
            activeDoTEffects[type] = new List<DoTEffect>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MagicDamage(Card card)
    {
        foreach (MagicType magicType in card.cardSO.magicTypes)
        {
            if (UnityEngine.Random.value < 0.99f)
            {
                int power = GetPowerByType(magicType, card.cardSO);
                ApplyDamageOverTime(power, magicType, card, 2);
            }
            else
            {
            }
        }
    }

    private int GetPowerByType(MagicType type, CardScriptableObject cardSO)
    {
        return type switch
        {
            MagicType.Poison => cardSO.poisonPower,
            MagicType.Ice => cardSO.icePower,
            MagicType.Fire => cardSO.firePower,
            MagicType.Spirit => cardSO.spiritPower,
            MagicType.Earth => cardSO.earthPower,
            MagicType.Assassin => cardSO.assasinPower,
            _ => 0
        };
    }
    private void ApplyDamageOverTime(int power, MagicType type, Card card, int duration)
    {
        if (power > 0)
        {
            activeDoTEffects[type].Add(new DoTEffect { Card = card, RemainingTurns = duration, Power = power });
            EffectController.instance.InvokeMagicEffect(type, card);
        }
    }

    public void NextTurn()
    {
        foreach (var activeEffect in activeDoTEffects)
        {
            List<DoTEffect> effects = activeEffect.Value;
            for (int i = effects.Count - 1; i >= 0; i--)
            {
                DoTEffect effect = effects[i];
                effect.RemainingTurns--;
                effect.Card.DamageCard(effect.Power);
                if (effect.RemainingTurns <= 0)
                {
                    EffectController.instance.RemoveMagicEffect(activeEffect.Key, effect.Card);
                    effects.RemoveAt(i);
                }
            }
        }
    }

    private IEnumerator DamageOverTime(int power, Card card)
    {
        int initialHealth = card.currentHealth;
        StartCoroutine(EffectController.instance.ShowClaw(card));
        yield return StartCoroutine(Utilities.WaitForAnimation(card.animator));
        bool cardDead = card.DamageCard(power);
        if (cardDead)
        {
            StartCoroutine(card.SendToCemetery());
            CardPointsController.instance.UpdateAfterDead(card.isPlayer);
        }
        StartCoroutine(card.UpdateDamage(initialHealth));
        yield return new WaitForSeconds(0.2f); 
    }

    internal class DoTEffect
    {
        public Card Card { get; internal set; }
        public int RemainingTurns { get; internal set; }
        public int Power { get; internal set; }
    }

}
