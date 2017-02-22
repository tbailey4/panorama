using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class position : MonoBehaviour {

    public GameObject terrain;

	// Use this for initialization
	void Start () {
        print (terrain.GetComponent<Terrain>().terrainData.size);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
