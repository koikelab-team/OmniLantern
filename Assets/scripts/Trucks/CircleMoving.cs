using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMoving : MonoBehaviour {

  // Use this for initialization
  public float r = 0.25f;
  public float a;
  public float aSpeed = 25f;

  protected float speed;

  protected float x;
  protected float y;
  protected float rad;

  protected float o;

  private void Awake() {
    o = this.transform.position.x;
    rad = Mathf.PI * a / 180.0f;
    speed = Mathf.PI * aSpeed / 180.0f;
    
  }

  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    rad += speed * Time.deltaTime;

    x = Mathf.Cos(rad) * r;
    y = Mathf.Sin(rad) * r;

    transform.position = new Vector3(x, transform.position.y, y);
	}
}
