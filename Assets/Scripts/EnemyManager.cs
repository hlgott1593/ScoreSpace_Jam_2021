﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyManager : MonoBehaviour
{
	private GameManager gameManager;
	private List<Transform> enemySpawners;
	[SerializeField] private bool AllowDebugControls;

	[Header("Spawn Settings")]
	[SerializeField] private float spawnRate;
	private float spawnTimer;

	[Header("Prefabs")]
	[SerializeField] private GameObject enemyPrefab;

	private List<Enemy> enemies;

	/// <summary>
	/// Let's us know how many enemies there are.
	/// Subtracting 1 because the spawners are nested under here too.
	/// </summary>
	public int EnemyCount
	{
		get
		{
			return transform.childCount - 1;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		gameManager = transform.parent.GetComponent<GameManager>();
		enemies = new List<Enemy>();
		LoadSpawners();
	}

	/// <summary>
	/// Reads in all enemy spawner game objects configured on the child.
	/// </summary>
	private void LoadSpawners()
	{
		enemySpawners = new List<Transform>();
		Transform spawnersParent = transform.Find("EnemySpawners");
		foreach (Transform spawner in spawnersParent)
		{
			enemySpawners.Add(spawner);
		}
	}

	// Update is called once per frame
	void Update()
	{
		UpdateSpawnEnemies();
		if (AllowDebugControls && Keyboard.current.spaceKey.wasPressedThisFrame) DebugKillOldest();
	}

	private void DebugKillOldest() {
		if (enemies.Count <= 0) return;
		enemies[0].Kill();
		enemies.RemoveAt(0);
	}

	/// <summary>
	/// Will churn out enemies at the configured rate.
	/// </summary>
	private void UpdateSpawnEnemies()
	{
		spawnTimer -= Time.deltaTime;
		if (spawnTimer <= 0f && EnemyCount < 5)
		{
			SpawnEnemy();
			spawnTimer = spawnRate;
		}
	}

	/// <summary>
	/// Will spawn an enemy at a random spawner.
	/// </summary>
	private void SpawnEnemy()
	{
		Transform spawner = enemySpawners[Random.Range(0, enemySpawners.Count)];
		Enemy enemy = Instantiate(enemyPrefab, transform).GetComponent<Enemy>();
		enemy.transform.position = spawner.position;
		enemy.GameManager = gameManager;
		enemies.Add(enemy);
	}
}
