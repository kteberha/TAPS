using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidHome : MonoBehaviour
{
    [System.Serializable]
    public class pointsAtTime
    {
        public float time; //in seconds
        public int points;
    }

    public pointsAtTime[] deliverPointsAtTime;
    public float currentTime = 0f;
    public float maxTime = 60f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTime < maxTime)
            currentTime += Time.deltaTime;
    }

    public void Deliver()
    {
        int pointsToAward = 1;
        foreach (pointsAtTime p in deliverPointsAtTime)
        {
            if (p.time <= currentTime)
                pointsToAward = p.points;
        }

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().packagesDelivered += 1;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().points += pointsToAward;

        currentTime = 0f;
    }
}
