﻿
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

public class ImageProcessLocation : ProjectionTarget {

  protected Mat MatToProcess;
  protected Texture TexToProcess;


  // for using the web camera
  //please choose false if you use the normal camera
  public bool UseIRCamera = false;
  // protected new OmniProCamDeviceManager omniProCamDeviceManagerPtr;
  public MarkerBasedLocation markerLocation;

  void Start() {

    omniProCamDeviceManagerPtr = GameObject.Find("OmniProCam/LibOmniProCamManager").GetComponent<OmniProCamDeviceManager>();


  }

  void Update() {


    //convert from tex to tex2d
    if (omniProCamDeviceManagerPtr.getCameraTextureType() == CameraTextureType.CAMERA_TEXTURE_TYPE_BINARIZED) {
      TexToProcess = omniProCamDeviceManagerPtr.getBinarizedCameraImageTexturePtr().texture;
    } else if (omniProCamDeviceManagerPtr.getCameraTextureType() == CameraTextureType.CAMERA_TEXTURE_TYPE_SRC) {
      TexToProcess = omniProCamDeviceManagerPtr.getSrcCameraImageTexturePtr().texture;
    }

    Texture2D Tex2dImg = new Texture2D(TexToProcess.width, TexToProcess.height);
    Utils.textureToTexture2D(TexToProcess, Tex2dImg);

    //convert the tex2d to mat
    MatToProcess = new Mat(TexToProcess.height, TexToProcess.width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
    Utils.texture2DToMat(Tex2dImg, MatToProcess);





    //to gary image
    Imgproc.cvtColor(MatToProcess, MatToProcess, 6);
    Mat resultMat = new Mat();
    MatToProcess.copyTo(resultMat);

    //processing
    resultMat = new Mat(resultMat.rows(), resultMat.cols(), CvType.CV_8UC1, new Scalar(0, 0, 0));

    
    resultMat = markerLocation.MarkerProcessing(MatToProcess);
    //Core.flip(resultMat, resultMat, -1);


    // convert to texture and show
    Texture2D ResultTex = new Texture2D(resultMat.cols(), resultMat.rows(), TextureFormat.RGB24, false);
    Utils.matToTexture2D(resultMat, ResultTex);

    //show the image
    gameObject.GetComponent<Renderer>().material.mainTexture = ResultTex;

  }

}


