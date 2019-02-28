using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(this);
	}
	
	// Update is called once per frame
	void Update () {
        Destroy(this);
        //if (Time.deltaTime > 0.2f)
        //{
        //    Destroy(this);
        //}
    }
}
