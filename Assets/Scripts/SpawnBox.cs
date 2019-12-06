using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBox : MonoBehaviour
{
    private float timeSinceSpawn = 0f;
    public float spawnRate = 15f;

    public GameObject box;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceSpawn += Time.deltaTime;
        if (timeSinceSpawn >= spawnRate)
        {
            if (box != null)
                Instantiate(box, new Vector3(this.transform.position.x + Random.Range(-25, 25), this.transform.position.y + Random.Range(-15, 30), 12.35f), new Quaternion());
            timeSinceSpawn = 0;
        }
    }
}
