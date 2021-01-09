using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
	public delegate void ExplodeCallback();
	public ExplodeCallback MyExplodeCallback;

	[SerializeField] private float explodeTime;
	private float timer;

	public float Damage;

	[Header("Prefabs")]
	[SerializeField] private GameObject explosionPrefab;

	/// <summary>
	/// Checks if the timer has made less than a tenth of a second progress.
	/// Used in enemies to see if the bomb spawned within.
	/// </summary>
	public bool EarlyCheck
	{
		get
		{
			return timer <= 0.1f;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		timer = 0f;
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		if (timer > explodeTime)
		{
			Explode();
		}
	}

	/// <summary>
	/// Detonates the bomb, spawning the explosion.
	/// </summary>
	public void Explode()
	{
		Destroy(gameObject);
		Explosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity).GetComponent<Explosion>();
		explosion.Damage = Damage;
		MyExplodeCallback?.Invoke();
	}
}
