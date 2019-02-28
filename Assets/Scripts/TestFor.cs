using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (j > 5)
                {
                    print("j" + j);
                    break;
                }
               
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
