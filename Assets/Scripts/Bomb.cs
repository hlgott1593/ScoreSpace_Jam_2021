using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
	[SerializeField] private float explodeTime;
	private float timer;

	// Start is called before the first frame update
	void Start()
	{
		timer = explodeTime;
	}

	// Update is called once per frame
	void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			Destroy(gameObject);
		}
	}
}
