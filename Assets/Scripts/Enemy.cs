using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
	[SerializeField] private int scoreGranted;
	public GameManager GameManager;

	[Header("Movement Settings")]
	[SerializeField] private float moveSpeed;
	public float Damage;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		SimpleMovement();
	}

	/// <summary>
	/// Simply move towards the center of the screen
	/// </summary>
	private void SimpleMovement()
	{
		Vector3 toCenter = Vector3.zero - transform.position;
		if (toCenter.magnitude > 3f)
		{
			transform.Translate(toCenter.normalized * moveSpeed * Time.deltaTime);
		}
	}

	/// <summary>
	/// Destroys the enemy and increases score.
	/// </summary>
	private void Kill()
	{
		GameManager.IncreaseScore(scoreGranted);
		Destroy(gameObject);
	}

	/// <summary>
	/// Blow up after colliding with a bomb.
	/// </summary>
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Bomb"))
		{
			Kill();
			Destroy(other.gameObject);
		}
	}
}
