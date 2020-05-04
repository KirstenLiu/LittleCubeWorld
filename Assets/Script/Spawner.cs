using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public new Transform transform;
    //Variables for spawning the cube
    public GameObject spawnablePrefab; 
    public GameObject spawnedObj;
    //Fixed-time based for first temp design
    public float spawnThreshold = 10f;
    public float spawnAcc = 0f;

    //Variables for fetching controls status
    Controls controls;

    //Start is called before the first frame update
    void Start()
    {
        transform = GetComponent<Transform>();
        spawnAcc = spawnThreshold;
    }

    //Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        spawnAcc += deltaTime;
        
        //Spawn the cube after fixed period of time
        if(spawnAcc >= spawnThreshold || getReleaseControls()==true){
            //Debug.Log("spawning");
            spawnedObj = GameObject.Instantiate(spawnablePrefab, transform.position, new Quaternion());
            controls = spawnedObj.GetComponent<Controls>();
            //Debug.Log("##releaseControls in spawner is get:" + getReleaseControls().ToString());
            spawnAcc = 0;
        }else{
            //Debug.Log("outside spawning");
        }
    }

    bool getReleaseControls(){
        return controls.releaseControls;
    }
}
