using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackagePooler : MonoBehaviour
{
    [System.Serializable]
    public class PackagePool
    {
        public int type;
        public GameObject prefab;
        public int size;
    }

    public List<PackagePool> packagePools;
    public Dictionary<int, Queue<GameObject>> poolDictionary;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<int, Queue<GameObject>>();

        foreach(PackagePool packPool in packagePools)
        {
            Queue<GameObject> packageQueue = new Queue<GameObject>();

            for (int i = 0; i < packPool.size; i++)
            {
                GameObject obj = Instantiate(packPool.prefab);
                obj.SetActive(false);
                packageQueue.Enqueue(obj);
            }

            poolDictionary.Add(packPool.type, packageQueue);

        }
    }

    public GameObject SpawnFromPool(int _type, Vector3 _pos, Quaternion _rot)
    {
        if(!poolDictionary.ContainsKey(_type))
        {
            Debug.LogWarning("Pool with type " + _type + " doesn't exist");
            return null;
        }

        GameObject objToSpawn = poolDictionary[_type].Dequeue();

        objToSpawn.SetActive(true);
        objToSpawn.transform.position = _pos;
        objToSpawn.transform.rotation = _rot;

        return objToSpawn;
    }

}
