using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrkSpawner : MonoBehaviour
{
    public GameObject Ork;
    public int SpawnCycle;
    public int SpawnAmount;
    public float AreaSizeX;
    public float AreaSizeZ;
    float timer;
    int OrkCounter;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        OrkCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= SpawnCycle)
        {
            timer = 0;
            Spawn();
        }
    }

    private void Spawn()
    {
        for(int i =0; i < SpawnAmount; i++)
        {
            OrkCounter++;
               GameObject tmpOrk = Instantiate(Ork);
            tmpOrk.transform.position = new Vector3(UnityEngine.Random.Range(-AreaSizeX, AreaSizeX), 1, UnityEngine.Random.Range(-AreaSizeZ, AreaSizeZ));
            tmpOrk.name = "Ork " + OrkCounter;
        }
    }
}
