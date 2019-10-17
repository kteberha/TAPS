using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour
{
    private float noCollisionTime = 0f;
    private List<GameObject> _heldPackages;
    private LineRenderer lineRend;

    // Start is called before the first frame update
    void Start()
    {
        _heldPackages = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().heldPackages;
        lineRend = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().lineRenderer;
    }

    // Update is called once per frame
    void Update()
    {
        noCollisionTime -= Time.deltaTime;
        if (noCollisionTime > 0)
        {
            GetComponent<BoxCollider2D>().usedByEffector = true;
        }
        else
        {
            GetComponent<BoxCollider2D>().usedByEffector = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            //check if the package is in the inventory to be removed
            if(_heldPackages.IndexOf(gameObject) != -1)
            {
                for(int i = _heldPackages.IndexOf(gameObject) + 1; i < _heldPackages.Count - 1; i++)
                {
                    Debug.Log("Package destroyed index: " + i);
                    Debug.Log("Package destroyed: " + gameObject);
                    //_heldPackages.RemoveAt(_heldPackages.IndexOf(gameObject));
                    //_heldPackages.RemoveRange(i, _heldPackages.Count);
                }
                
            }
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().packagesDelivered += 1;
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            //check if the package is in the inventory to be removed
            if (_heldPackages.IndexOf(gameObject) != -1)
            {
                _heldPackages.Remove(_heldPackages[_heldPackages.IndexOf(gameObject)]);
            }
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().packagesDelivered += 1;
            Destroy(gameObject);
        }
    }

    public void DoNotCollideWithPlayer(float duration)
    {
        noCollisionTime = duration;
    }
}
