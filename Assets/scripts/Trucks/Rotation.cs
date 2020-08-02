using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour {

  float y;
  float z;
  public float speed = 25f;

	// Use this for initialization
	void Start () {
    z = this.transform.eulerAngles.z;
		
	}
	
	// Update is called once per frame
	void Update () {
    y -= Time.deltaTime * speed;
    transform.rotation = Quaternion.Euler(0, y, 0);
    this.transform.localScale = new Vector3(1, 1, 1);
    this.transform.localPosition = new Vector3(0, 0, 0);

	}
}
