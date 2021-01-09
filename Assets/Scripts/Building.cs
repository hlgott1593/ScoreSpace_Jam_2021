using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
	[SerializeField] private Text healthText;

	public float Health;

	// Start is called before the first frame update
	void Start()
	{
		healthText.text = "" + (int)Health;
	}

	// Update is called once per frame
	void Update()
	{
		healthText.text = "" + (int)Health;
		if (Health <= 0f)
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Explosions will damage buildings.
	/// </summary>
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Explosion"))
		{
			Explosion explosion = other.GetComponent<Explosion>();
			Health -= explosion.Damage;
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("Enemy"))
		{
			Enemy enemy = other.gameObject.GetComponent<Enemy>();
			Health -= enemy.Damage * Time.deltaTime;
		}
	}

	/// <summary>
	/// When an enemy is overlapping the building, apply damage
	/// </summary>
	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			Health -= enemy.Damage * Time.deltaTime;
		}
	}
}
