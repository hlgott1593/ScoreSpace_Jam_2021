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
		if (Health <= 0f)
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Explosions will damage buildings.
	/// </summary>
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Explosion"))
		{
			Explosion explosion = other.GetComponent<Explosion>();
			Health -= explosion.Damage;
			healthText.text = "" + (int)Health;
		}
	}

	/// <summary>
	/// When an enemy is overlapping the building, apply damage
	/// </summary>
	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("Enemy"))
		{
			Enemy enemy = other.GetComponent<Enemy>();
			Health -= enemy.Damage * Time.deltaTime;
			healthText.text = "" + (int)Health;
		}
	}
}
