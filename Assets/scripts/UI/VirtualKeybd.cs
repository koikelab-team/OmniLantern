using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class VirtualKeybd : MonoBehaviour {

  [DllImport("user32.dll", EntryPoint = "keybd_event")]

  public static extern void Keybd_event(
    byte bvk, //key value
    byte bScan, // 0
    int dwFlags, // 0 down, 1 keep, 2 up
    int dwEntraInfo //0
    );

	// Use this for initialization
	void Start () {
    //Keybd_event(27, 0, 0, 0);
	}
	
}
