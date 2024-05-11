using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection.Emit;

public class UIDamageIndicator : MonoBehaviour
{

    public TMP_Text damageText;

    public float moveSpeed;
    public float lifeTime = 3f;

    private RectTransform myRect;
    void Start()
    {
        Destroy(gameObject, lifeTime);
        myRect = GetComponent<RectTransform>();
    }
    
    void Update()
    {
        myRect.anchoredPosition += new Vector2(0f, moveSpeed * Time.deltaTime); 
    }
}
