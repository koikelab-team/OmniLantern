using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class systemTime : MonoBehaviour {


  public Text text;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
    //text = GetComponent<TextMesh>();
    //text.text = System.DateTime.Now.Second.ToString();
    text.text = System.DateTime.Now.Hour.ToString()+":" +System.DateTime.Now.Minute.ToString()+":" + System.DateTime.Now.Second.ToString();

  }
}
