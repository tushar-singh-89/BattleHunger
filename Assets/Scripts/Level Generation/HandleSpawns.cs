﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSpawns : MonoBehaviour 
{

	public List<Transform> spawnPoints;
	public List<GameObject> objects;

    // Add spawnpoints from prefab
	void Start () 
	{
		foreach (Transform child in transform)
		{
			if (child.name == "SpawnPoints")
			{
				foreach(Transform spawnPoint in child)
					spawnPoints.Add(spawnPoint);
			}
		}

		SpawnObjects();
	}

    /*
     * Spawn random number of objects based on size of the spawn points in a room prefab object 
     */ 
	void SpawnObjects()
	{
        // 2 to get more population of interaction
		int numObjToSpawn = Random.Range (Random.Range(2, spawnPoints.Count), spawnPoints.Count); // random between 1 and spawnpoints size
		//Debug.Log (numObjToSpawn);
		for (int i = 0; i < numObjToSpawn; i++) 
		{
			int chosenPoint = Random.Range (0, spawnPoints.Count); // save this to remove it from the list later
			Instantiate (objects[Random.Range (0, objects.Count)], spawnPoints[chosenPoint].transform.position, Quaternion.identity);
			spawnPoints.Remove (spawnPoints [chosenPoint]);
		}

	}

}
