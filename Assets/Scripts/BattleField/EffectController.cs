using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static MagicController;

public class EffectController : MonoBehaviour
{
    public static EffectController instance;

    public GameObject clawWrapper;
    public VisualEffect clawComponent;
    public ParticleSystem poisonParticles;

    private Dictionary<Card, ParticleSystem> poisonEffects = new Dictionary<Card, ParticleSystem>();
    private Dictionary<MagicType, Action<Card>> magicMethods;
    private Dictionary<MagicType, Action<Card>> magicRemoveMethods;


    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeMagicMethods();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    internal void InitializeMagicMethods()
    {
        magicMethods = new Dictionary<MagicType, Action<Card>>
        {
            { MagicType.Poison, ApplyPoisonEffect }
        };
        magicRemoveMethods = new Dictionary<MagicType, Action<Card>>
        {
            { MagicType.Poison, RemovePoisonEffect }
        };
    }

    public IEnumerator ShowClaw(Card card)
    {
        CardPlacePoint selectedPoint = Utilities.GetCardPoint(card);

        clawWrapper.transform.position = selectedPoint.transform.position;
        clawComponent.Play();
        yield return new WaitForSeconds(0.5f); // Wait for the effect to complete
    }

    public void ApplyPoisonEffect(Card card)
    {
        if (poisonEffects.ContainsKey(card))
        {
            return;
        }
        CardPlacePoint selectedPoint = Utilities.GetCardPoint(card);
        ParticleSystem clone = Instantiate(poisonParticles, selectedPoint.transform.position, Quaternion.identity);
        clone.gameObject.SetActive(true);
        poisonEffects[card] = clone;
    }

    public void RemovePoisonEffect(Card card)
    {
        if (poisonEffects.TryGetValue(card, out ParticleSystem effect))
        {
            Destroy(effect.gameObject);
            poisonEffects.Remove(card);
        }
    }

    public void InvokeMagicEffect(MagicType type, Card card)
    {
        if (magicMethods.ContainsKey(type))
        {
            magicMethods[type](card);
        }
        else
        {
        }
    }

    internal void RemoveMagicEffect(MagicType type, Card card)
    {
        if (magicRemoveMethods.ContainsKey(type))
        {
            magicRemoveMethods[type](card);
        }
        else
        {
        }
    }
}
