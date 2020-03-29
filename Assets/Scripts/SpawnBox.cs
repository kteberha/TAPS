using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBox : MonoBehaviour
{
    private float timeSinceSpawn = 0f;
    public float spawnRate = 15f;

    private int packageTypeIndex = 0;
    private int spawnPointIndex = 0;

    public GameObject[] packageTypes = new GameObject[3];
    public GameObject[] spawnPoints = new GameObject[4];
    private GameObject package;
    private GameObject spawnPoint;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.state == GAMESTATE.CLOCKEDIN)
        {
            timeSinceSpawn += Time.deltaTime;

            if (timeSinceSpawn >= spawnRate) //check that enough time has passed to spawn new package
            {
                switch (spawnPointIndex)//pick a spawn point for the package to spawn from
                {
                    case (0):
                        spawnPoint = spawnPoints[0];
                        spawnPointIndex++;
                        break;

                    case (1):
                        spawnPoint = spawnPoints[1];
                        spawnPointIndex++;
                        break;

                    case (2):
                        spawnPoint = spawnPoints[2];
                        spawnPointIndex++;
                        break;

                    case (3):
                        spawnPoint = spawnPoints[3];
                        spawnPointIndex = 0;
                        break;

                    default:
                        print("spawn point error");
                        break;
                }

                switch (packageTypeIndex)//select the type of package to spawn
                {
                    case (0):
                        package = packageTypes[0];
                        packageTypeIndex++;
                        break;

                    case (1):
                        package = packageTypes[1];
                        packageTypeIndex++;
                        break;

                    case (2):
                        package = packageTypes[2];
                        packageTypeIndex = 0;
                        break;

                    default:
                        print("package type error");
                        break;
                }

                if (spawnPoint != null && package != null) // spawn selected package at point
                {
                    Instantiate(package, new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z), new Quaternion());
                    //print(package.ToString());
                    //print(spawnPoint.ToString());
                    timeSinceSpawn = 0;
                }
            }
        }
    }
}
