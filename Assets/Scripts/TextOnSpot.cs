using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextOnSpot : MonoBehaviour
{
    public string DisplayText;
	public int DisplayPoints;
	public TextMeshProUGUI TextPrefab;
	public float Speed;
	public float DestroyAfter;
	private float Timer;

	private int textSize = 180;

	// Use this for initialization
	void Start () {
		Timer = DestroyAfter;
	}
	
	// Update is called once per frame
	void Update () {
		TextPrefab.fontSize = textSize;
		Timer -= Time.deltaTime;
		if(Timer < 0) {
			Destroy(gameObject);
		}

		if(Speed > 0) {
			transform.Translate(Vector3.up * Speed * Time.deltaTime, Space.World);
		}
	}

    public void SetFontSize(int textSize)
    {
        this.textSize = textSize;
    }

    public void SetColor(Color color)
    {
        color.a = 1f;
        TextPrefab.color = color;
    }
}
