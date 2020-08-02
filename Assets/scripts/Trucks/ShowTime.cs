using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTime : MonoBehaviour {

  TextMesh text;
  string Hour;
  string Minute;
  string Second;

	// Use this for initialization
	void Start () {
    text = GetComponent<TextMesh>();

  }
	
	// Update is called once per frame
	void Update () {
    Hour = System.DateTime.Now.Hour.ToString();
    Minute = System.DateTime.Now.Minute.ToString();
    Second = System.DateTime.Now.Second.ToString();

    //if (System.DateTime.Now.Hour < 10) {
    //  Hour = "0" + System.DateTime.Now.Hour.ToString();
    //}

    //if (System.DateTime.Now.Minute < 10) {
    //  Minute = "0" + System.DateTime.Now.Minute.ToString();
    //}

    //if (System.DateTime.Now.Second < 10) {
    //  Second = "0" + System.DateTime.Now.Second.ToString();
    //}


    text.text = Hour + ":" + Minute + ":" + Second;
  }
}
