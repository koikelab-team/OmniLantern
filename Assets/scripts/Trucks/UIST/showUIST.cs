using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ArucoModule;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVForUnity.UtilsModule;

public class showUIST : MonoBehaviour {

  public GameObject target;
  public GameObject thisone;

  private float timer = 0.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    Debug.Log(target.activeInHierarchy);
    if (timer < 10) {
      timer += Time.deltaTime;
    }
    else if (!target.activeInHierarchy){

      thisone.SetActive(true);

    }


  }
}
