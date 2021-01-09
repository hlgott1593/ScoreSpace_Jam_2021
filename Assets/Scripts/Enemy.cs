using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
	private Transform mySprite;
	[SerializeField] private int scoreGranted;
	public GameManager GameManager;
	[SerializeField] private Text healthText;

	public float Health;
	public float Damage;

	[Header("Movement Settings")]
	[SerializeField] private float orbitRange;
	[SerializeField] private float attackRange;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float spriteRotateSpeed;
	private bool attacking;
	private float behaviorTimer;

	[Header("Pefabs")]
	[SerializeField] private GameObject DeathExplosion;

	[Header("Drops")] [SerializeField] private List<Drop> drops;
	
	
	void Start()
	{
		healthText.text = "" + (int)Health;
		mySprite = transform.Find("Sprite");
	}
	
	void Update()
	{
		OrbitalMovement();
		UpdateSpinEffect();
	}

	/// <summary>
	/// Simply move towards the center of the screen
	/// </summary>
	private void SimpleMovement()
	{
		Vector3 toCenter = Vector3.zero - transform.position;
		if (toCenter.magnitude > 2f)
		{
			transform.Translate(toCenter.normalized * moveSpeed * Time.deltaTime);
		}
	}

	/// <summary>
	/// Move into a set range orbiting around, randomly going in and out to damage buildings.
	/// </summary>
	private void OrbitalMovement()
	{
		// Calculate some info
		Vector3 toCenter = Vector3.zero - transform.position;
		float distance = toCenter.magnitude;
		float range = attacking ? attackRange : orbitRange;

		// Move to the correct range, and orbit when in range
		if (distance > range + 0.1f)
		{
			transform.Translate(toCenter.normalized * moveSpeed * Time.deltaTime);
		}
		else if (distance < range - 0.1f)
		{
			transform.Translate(toCenter.normalized * -moveSpeed * Time.deltaTime);
		}
		else
		{
			Vector3 orbitDirection = Vector2.Perpendicular(toCenter);
			transform.Translate(orbitDirection.normalized * moveSpeed * Time.deltaTime);
		}

		// Every so often, change behavior
		behaviorTimer -= Time.deltaTime;
		if (behaviorTimer <= 0f)
		{
			behaviorTimer = 1f;
			if (Random.Range(0,3) == 0)
			{
				attacking = !attacking;
			}
		}
	}

	/// <summary>
	/// Simple spin effect on the main renderer
	/// </summary>
	private void UpdateSpinEffect()
	{
		mySprite.transform.Rotate(0f, 0f, spriteRotateSpeed * Time.deltaTime);
	}

	/// <summary>
	/// Destroys the enemy and increases score.
	/// </summary>
	public void Kill()
	{
		GameManager.IncreaseScore(scoreGranted);
		Instantiate(DeathExplosion, transform.position, Quaternion.identity);
		HandleDrop();
		Destroy(gameObject);
	}

	private void HandleDrop() {
		// return;
		// var roll = Random.Range(0, 100);
		// if (roll < 75) return;
		var drop = drops[0];
		Instantiate(drop, transform.position, Quaternion.identity);
	}

	/// <summary>
	/// Blow up after colliding with a bomb.
	/// </summary>
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Bomb"))
		{
			Bomb bomb = other.GetComponent<Bomb>();
			if (bomb.EarlyCheck)
			{
				bomb.MyExplodeCallback += Kill;
				bomb.Explode();
			}
		}

		if (other.CompareTag("Explosion"))
		{
			Explosion explosion = other.GetComponent<Explosion>();
			Health -= explosion.Damage;
			healthText.text = "" + (int)Health;
			if (Health <= 0)
			{
				Kill();
			}
		}
	}
}
