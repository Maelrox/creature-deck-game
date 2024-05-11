using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsController : MonoBehaviour
{
    public static EffectsController instance;

    private void Awake()
    {
        instance = this;
    }

    private Vector3 targetPoint;
    private Quaternion targetRotation;
    public ParticleSystem lightningParticles;
    public GameObject effects;

    public bool show = false; 
    // Start is called before the first frame update
    void Start()
    {
        if (show)
        {
            effects.SetActive(true);
            lightningParticles.Play();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotation)
    {
        targetPoint = pointToMoveTo;
        targetRotation = rotation;
    }
}
