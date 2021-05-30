﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attack 0 = Normal, Attack 1 = Spawn Projectiles
public class DemonBoss : Enemy
{
    private GameObject bossRoom;
    public GameObject projectile;
    public GameObject tileObj;

    public float SPAWN_PROJECTILE_COOLDOWN;
    public float SPAWN_PROJECTILE_TIMER;
    public float SPAWN_TILES_COOLDOWN;
    public float SPAWN_TILES_TIMER;

    public float closeAttackRange;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        bossRoom = GameObject.FindGameObjectWithTag("BossRoom");
        canBeKnockedBack = false;
	}
	
	// Update is called once per frame
	public override void Update ()
    {
        base.Update();

        if (SPAWN_PROJECTILE_TIMER >= 0)
            SPAWN_PROJECTILE_TIMER -= Time.deltaTime;

        if (SPAWN_TILES_TIMER >= 0)
            SPAWN_TILES_TIMER -= Time.deltaTime;
	}

    /*
     * Simply checks the distance from player to enemy
     */ 
    public override void DistanceChecking()
    {
        distance = Vector2.Distance(player.position, transform.position);
        if (distance > followRange)
        {
            state = State.Idle;
        }
        else if (distance <= followRange && distance > attackRange)
        {
            pathfinding.target = GameManagerSingleton.instance.player.transform;
            state = State.Moving;
        }
        else if (distance <= attackRange && (ATTACK_TIMER <= 0 ||  SPAWN_PROJECTILE_TIMER <= 0))
        {
            state = State.Attacking;
        }
    }

    /*
     * Decides the state that the enemy should be in
     */ 
    public override void StateDecision()
    {
        float dist = Vector2.Distance(player.transform.position, transform.position);
        switch (state)
        {
            case State.Idle:
                break;
            case State.Moving:
                FollowTarget(player);
                break;
            case State.Attacking:
                if (ATTACK_TIMER <= 0)
                {
                    // if close and timers 0, choose random so it's not always the same
                    if(dist <= closeAttackRange && (SPAWN_TILES_TIMER <= 0 && SPAWN_PROJECTILE_TIMER <= 0))
                    {
                        attackChosen = Random.Range(0, NumberOfAttacks);
                    }
                    else if (dist <= attackRange && dist >= closeAttackRange)
                    {
                        //Debug.Log("Range for projectile/tiles");
                        if (SPAWN_PROJECTILE_TIMER <= 0)
                            attackChosen = 1;
                        else if (SPAWN_TILES_TIMER <= 0)
                            attackChosen = 2;
                    }
                    else if (dist <= closeAttackRange)
                    {
                        //Debug.Log("range for melee");
                        attackChosen = 0;
                    }

                    Attack(attackChosen);
                }
                break;
        }
    }

    /*
     * Choose attack based on the attackChosen int (chosen in StateDecision())
     */ 
    public override void ChooseAttack(float timeDelay, int attackChosen)
    {
        base.ChooseAttack(timeDelay, attackChosen);
        if (attackChosen == 1 && SPAWN_PROJECTILE_TIMER <= 0)
        {
            StartCoroutine(SpawnProjectiles(25, 1)); // do with delay to match animation
        }
        else if(attackChosen == 2 && SPAWN_TILES_TIMER <= 0)
        {
            SpawnHarmfulTiles();
        }
        else if(attackChosen == 0)
        {
        }
    }

    /*
     * Method to get all tiles following a pattern
     */
    List<Transform> GetTilePattern()
    {
        List<Transform> tilesToUse = new List<Transform>();
        foreach (Transform tile in bossRoom.GetComponent<Tiles>().tiles)
        {
            if (tile.transform.position.y % 3 == 0)
                tilesToUse.Add(tile);
        }

        return tilesToUse;

    }

    /*
     * Spawn harmful tiles using the above method's returned tile pattern
     */ 
    void SpawnHarmfulTiles()
    {
        List<Transform> besideTiles = GetTilePattern();

        foreach (Transform tile in besideTiles)
            Instantiate(tileObj, tile.position, Quaternion.identity);

        SoundManager.instance.PlayRandomOneShotAtPoint(1, transform.position, SoundManager.instance.spawnTileSound);

        SPAWN_TILES_TIMER = SPAWN_TILES_COOLDOWN;
    }

    /*
     * Get position around radius via the angle in which to spawn a projectile
     */ 
    Vector2 CreateRingOfProjectiles(Vector2 center, float radius, int angle)
    {
        float ang = angle;
        Vector2 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    /*
     * Spawn projectiles around the boss
     */ 
    IEnumerator SpawnProjectiles(int numProjectiles, float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < numProjectiles; i++)
        {
            int angle = 360 / numProjectiles * i; // change the angle every interation to get it in a circle
            Vector2 pos = CreateRingOfProjectiles(transform.position, 1f, angle);
            GameObject obj = Instantiate(projectile, pos, Quaternion.identity);

            // Add force relative to the projectile position in opposite direction from the boss
            Vector2 dir = (obj.transform.position - gameObject.transform.position).normalized;

            obj.GetComponent<Rigidbody2D>().AddForce(dir * 200);
            SoundManager.instance.PlayRandomOneShotAtPoint(0.4f, transform.position, SoundManager.instance.demonProjectileSounds);
        }

        SPAWN_PROJECTILE_TIMER = SPAWN_PROJECTILE_COOLDOWN;
    }

}
