using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotation1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    this.transform.Rotate(Vector3.up, 25 * Time.deltaTime, Space.Self);
		
	}
}
