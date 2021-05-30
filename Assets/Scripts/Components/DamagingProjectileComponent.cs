﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingProjectileComponent : MonoBehaviour, IDamageDealer
{

    public int customDamage;
    public int Damage { get; set; }

    private float flashDelay = 0.25f;
    private CircleCollider2D col;
    private SpriteRenderer spr;

    void Awake()
    {
        Damage = customDamage;
    }

    void Start()
    {
        col = GetComponent<CircleCollider2D>();
        spr = GetComponent<SpriteRenderer>();

        //StartCoroutine(Flash(flashDelay));
    }

    IEnumerator Flash(float x)
    {
        col.enabled = false;
        for (int i = 0; i < 4; i++)
        {
            spr.enabled = false;
            yield return new WaitForSeconds(x);
            spr.enabled = true;
            yield return new WaitForSeconds(x);
        }
        col.enabled = true;
    }
}
