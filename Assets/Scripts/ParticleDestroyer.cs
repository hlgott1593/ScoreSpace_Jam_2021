using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
	private ParticleSystem mySystem;

	// Start is called before the first frame update
	void Start()
	{
		mySystem = GetComponent<ParticleSystem>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!mySystem.isPlaying)
		{
			Destroy(gameObject);
		}
	}
}
