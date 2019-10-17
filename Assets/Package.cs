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
                int removeRange = 0;

                //loop through all packages starting at the end of the rope up to and including the triggered package
                for (int i = _heldPackages.Count - 1; i >= _heldPackages.IndexOf(gameObject); i--)
                {
                    //destroy the spring arm of this object
                    Destroy(_heldPackages[_heldPackages.IndexOf(gameObject)].GetComponent<SpringJoint2D>());
                    
                    //increase integer that is used to determine how many objects to remove from package list
                    removeRange++; 
                }

                // remove all packages starting from triggered object and those behind it
                _heldPackages.RemoveRange(_heldPackages.IndexOf(gameObject), removeRange);
            }

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().packagesDelivered += 1;
            Destroy(gameObject);
        }
    }

    public void DoNotCollideWithPlayer(float duration)
    {
        noCollisionTime = duration;
    }

    //for testing
    private void OnMouseDown()
    {
        if (_heldPackages.IndexOf(gameObject) != -1)
        {
            int removeRange = 0;

            for (int i = _heldPackages.Count - 1; i >= _heldPackages.IndexOf(gameObject); i--)
            {
                Destroy(_heldPackages[_heldPackages.IndexOf(gameObject)].GetComponent<SpringJoint2D>());//destroy the spring arm of this object
                removeRange++; //increase integer that is used to determine how many objects to remove from package list
            }
            
            // remove all packages starting from triggered object and those behind it
            _heldPackages.RemoveRange(_heldPackages.IndexOf(gameObject), removeRange);
            Destroy(gameObject);
        }
    }
}
