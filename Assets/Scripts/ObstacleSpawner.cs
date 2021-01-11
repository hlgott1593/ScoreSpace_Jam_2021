using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] public GameObject obstacle;

    [SerializeField] public int spawnRange;
    [SerializeField] public float astroidSpeed;
    [SerializeField] public float astroidOffsetRange;
    [SerializeField] public float intensity;
    [SerializeField] public int destroyAfterSec;

    [SerializeField] public Sprite[] astroids;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnAstroids());
    }

    private IEnumerator SpawnAstroids()
    {
        float duration = 0;
        while (true)
        {
            if (duration > 1 / intensity)
            {
                SpawnAstroid();
                duration = 0;
            }
            else
            {
                duration += Time.deltaTime;
            }
            yield return null;
        }
    }

    private void SpawnAstroid()
    {
        Obstacle astroid = Instantiate(obstacle, transform).GetComponent<Obstacle>();
        // Set Sprite
        astroid.spriteRenderer.sprite = astroids[Random.Range(0, astroids.Length)];
        float radians = Random.Range(0, 360) * (Mathf.PI / 180);
        Vector3 location = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0) * spawnRange;
        // set values
        astroid.transform.position = location;
        // add force
        Vector3 end = new Vector2(
            Random.Range(-astroidOffsetRange, astroidOffsetRange),
            Random.Range(-astroidOffsetRange, astroidOffsetRange)
        );
        astroid.GetComponent<Rigidbody2D>().AddForce((end - astroid.transform.position) * astroidSpeed);
        StartCoroutine(DestoryAstroid(astroid));
        
    }

    private IEnumerator DestoryAstroid(Obstacle astroid)
    {
        yield return new WaitForSeconds(destroyAfterSec);
        if (astroid != null)
        {
            Destroy(astroid.gameObject);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
