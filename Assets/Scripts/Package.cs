﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Package : MonoBehaviour
{
    public float maxSize = 5.5f;
    public float minSize = 2.5f;
    public bool randomRotateAtSpawn = true;

    public float brokenPackagePercent = 0.3f;//odds that a broken package sound will play

    private float noCollisionTime = 0f;
    private float justThrownTimer = 10f;
    private float justThrownClock = 0f;

    private List<GameObject> _heldPackages;
    public LineRenderer lineRend;
    public GameObject invBubble;

    public AudioClip brokenPackageClip;
    public AudioClip normalPackageClip;
    private AudioSource a_Source;

    // Start is called before the first frame update
    void Start()
    {
        _heldPackages = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().heldPackages;
        a_Source = GetComponent<AudioSource>();

        //Random Size
        //float randomSize = Random.Range(minSize, maxSize);
        //this.transform.localScale = new Vector3(randomSize, randomSize, randomSize);

        //Random Rotation at spawn
        if (randomRotateAtSpawn)
        {
            int randomNum = Random.Range(0, 360);
            this.transform.localRotation = Quaternion.Euler(0, 0, randomNum);
        }
    }

    // Update is called once per frame
    void Update()
    {
        justThrownClock -= Time.deltaTime;
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

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Finish") && justThrownClock >= 0 && collision.gameObject.GetComponent<AsteroidHome>().packageOrdered)
        {

            RemovePackages();

            collision.gameObject.GetComponent<AsteroidHome>().Deliver();

            //destroy the package being hit and play audio
            StartCoroutine("DeliverySound");
        }
    }

    public void DoNotCollideWithPlayer(float duration)
    {
        noCollisionTime = duration;
    }

    public void Pickup()
    {
        invBubble.SetActive(true);
        invBubble.transform.DOScale(4, 1).SetEase(DG.Tweening.Ease.OutQuint);
    }

    public void Throw()
    {
        //invBubble.transform.DOScale(6, .5).SetEase(Ease.OutQuint);
        justThrownClock = justThrownTimer;
        invBubble.SetActive(false);
    }

    public void RemovePackages()
    {
        //check if the package is in the inventory to be removed
        if (_heldPackages.IndexOf(gameObject) != -1)
        {
            int removeRange = 0; //local to determine how many packages will be getting removed behind this package

            //start the loop at the end of the inventory and work toward the beginning
            for (int i = _heldPackages.Count - 1; i >= _heldPackages.IndexOf(gameObject); i--)
            {
                Destroy(_heldPackages[_heldPackages.IndexOf(gameObject)].GetComponent<SpringJoint2D>()); //destroy the spring arm of this object
                removeRange++; //increase integer that is used to determine how many objects to remove from package list
                _heldPackages[i].GetComponent<Package>().invBubble.SetActive(false); //set the inventory bubble to inactive
            }

            _heldPackages.RemoveRange(_heldPackages.IndexOf(gameObject), removeRange); // remove all packages starting from triggered object and those behind it from the list

            //update the line renderer's position count
            if (_heldPackages.Count > 0)
            {
                lineRend.positionCount = _heldPackages.Count + 1;
            }
            else
            {
                lineRend.positionCount = 1;
            }
        }
    }

    IEnumerator DeliverySound()
    {
        invBubble.SetActive(false);
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.transform.Find("package").Find("pCube1").GetComponent<MeshRenderer>().enabled = false;
        gameObject.transform.Find("package").GetComponent<BoxCollider>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        float f = Random.value;
        if (f <= brokenPackagePercent)
            a_Source.clip = brokenPackageClip;
        else
            a_Source.clip = normalPackageClip;
        a_Source.Play();
        yield return new WaitForSeconds(a_Source.clip.length);
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        RemovePackages();
    }
}