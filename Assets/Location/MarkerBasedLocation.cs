﻿using UnityEngine;
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


//namespace OpenCVForUnityExample {
public class MarkerBasedLocation : MonoBehaviour {
  public float markerLengthStandard = 20; //pixel

  //a timer for undisplay the object
  public float waitTime = 5.0f;
  private Dictionary<int, float> Timer;


  //relationship between marker id & gameobject
  public Dictionary<int, string> idDict;

  //marker type
  public OpenCVForUnityExample.ArUcoWebCamTextureExample.MarkerType markerType = OpenCVForUnityExample.ArUcoWebCamTextureExample.MarkerType.CanonicalMarker;
  //marker dict
  public OpenCVForUnityExample.ArUcoWebCamTextureExample.ArUcoDictionary dictionaryId = OpenCVForUnityExample.ArUcoWebCamTextureExample.ArUcoDictionary.DICT_4X4_50;

  public bool showRejectedCorners = false;
  //public Toggle showRejectedCornersToggle;
  public bool applyEstimationPose = true;

  [Space(10)]

  //marker size, meter
  public float markerLength = 0.1f;

  //Game Object to project
  public GameObject arGameObject;
  public Camera arCamera;
  private GameObject arChildObject;

  [Space(10)]

  //protected ShadeTouch shadeTouch;

  public bool enableLowPassFilter;
  //public Toggle enableLowPassFilterToggle;


  /// <summary>
  /// The position low pass. (Value in meters)
  /// </summary>
  public float positionLowPass = 0.005f;

  /// <summary>
  /// The rotation low pass. (Value in degrees)
  /// </summary>
  public float rotationLowPass = 2f;

  /// <summary>
  /// The old pose data.
  /// </summary>
  PoseData oldPoseData;

  /// <summary>
  /// The cameraparam matrix.
  /// </summary>
  Mat camMatrix;

  /// <summary>
  /// The distortion coeffs.
  /// </summary>
  MatOfDouble distCoeffs;

  /// <summary>
  /// The transformation matrix for AR.
  /// </summary>
  Matrix4x4 ARM;

  /// <summary>
  /// The identifiers.
  /// </summary>
  Mat ids;

  // id in the data of list
  List<int> list_ids;

  /// <summary>
  /// The corners.
  /// </summary>
  List<Mat> corners;

  /// <summary>
  /// The rejected corners.
  /// </summary>
  List<Mat> rejectedCorners;

  /// <summary>
  /// The rvecs.
  /// </summary>
  Mat rvecs;

  /// <summary>
  /// The tvecs.
  /// </summary>
  Mat tvecs;

  /// <summary>
  /// The rot mat.
  /// </summary>
  Mat rotMat;

  /// <summary>
  /// The detector parameters.
  /// </summary>
  DetectorParameters detectorParams;

  /// <summary>
  /// The dictionary.
  /// </summary>
  Dictionary dictionary;

  Mat rvec;
  Mat tvec;
  Mat recoveredIdxs;

  //Size of the image
  int Imgwidth = 760;
  int Imgheight = 760;

  // for changing from imagepixel to world
  public float Margin = 0.50f;


  // Use this for initialization
  void Start() {
    //showRejectedCornersToggle.isOn = showRejectedCorners;
    //enableLowPassFilterToggle.isOn = enableLowPassFilter;

    CameraInit(Imgheight, Imgwidth);

    // id & gameobject
    idDict = new Dictionary<int, string>();
    idDict.Add(7, "ARGameObject1");
    idDict.Add(10, "ARGameObject2");
    idDict.Add(15, "ARGameObject3");
    idDict.Add(17, "ARGameObject4");

    Timer = new Dictionary<int, float>();
    Timer.Add(7, 0.0f);
    Timer.Add(10, 0.0f);
    Timer.Add(15, 0.0f);
    Timer.Add(17, 0.0f);


    //init shade touch
    //shadeTouch = new ShadeTouch();
  }



  private void CameraInit(int height, int width) {

    double fx;
    double fy;
    double cx;
    double cy;

    int max_d = (int)Mathf.Max(width, height);
    fx = 3.1145896e+02;
    fy = 3.0654921e+02;
    cx = 4.0354616e+02;
    cy = 4.0661785e+02;

    camMatrix = new Mat(3, 3, CvType.CV_64FC1);
    camMatrix.put(0, 0, fx);
    camMatrix.put(0, 1, 0);
    camMatrix.put(0, 2, cx);
    camMatrix.put(1, 0, 0);
    camMatrix.put(1, 1, fy);
    camMatrix.put(1, 2, cy);
    camMatrix.put(2, 0, 0);
    camMatrix.put(2, 1, 0);
    camMatrix.put(2, 2, 1.0f);

    distCoeffs = new MatOfDouble(-2.971244e-01, 8.356048e-02, -4.74639e-03, 8.1501643e-05, -9.992362e-03);




    Debug.Log("Import the calibration data.");



    float imageSizeScale = 1.0f;
    float widthScale = (float)Screen.width / width;
    float heightScale = (float)Screen.height / height;
    //if (widthScale < heightScale) {
    //  Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
    //  imageSizeScale = (float)Screen.height / (float)Screen.width;
    //} else {
    //  Camera.main.orthographicSize = height / 2;
    //}



    // calibration camera matrix values.
    Size imageSize = new Size(width * imageSizeScale, height * imageSizeScale);
    double apertureWidth = 0;
    double apertureHeight = 0;
    double[] fovx = new double[1];
    double[] fovy = new double[1];
    double[] focalLength = new double[1];
    Point principalPoint = new Point(0, 0);
    double[] aspectratio = new double[1];

    Calib3d.calibrationMatrixValues(camMatrix, imageSize, apertureWidth, apertureHeight, fovx, fovy, focalLength, principalPoint, aspectratio);

    Debug.Log("imageSize " + imageSize.ToString());
    Debug.Log("apertureWidth " + apertureWidth);
    Debug.Log("apertureHeight " + apertureHeight);
    Debug.Log("fovx " + fovx[0]);
    Debug.Log("fovy " + fovy[0]);
    Debug.Log("focalLength " + focalLength[0]);
    Debug.Log("principalPoint " + principalPoint.ToString());
    Debug.Log("aspectratio " + aspectratio[0]);


    // To convert the difference of the FOV value of the OpenCV and Unity. 
    double fovXScale = (2.0 * Mathf.Atan((float)(imageSize.width / (2.0 * fx)))) / (Mathf.Atan2((float)cx, (float)fx) + Mathf.Atan2((float)(imageSize.width - cx), (float)fx));
    double fovYScale = (2.0 * Mathf.Atan((float)(imageSize.height / (2.0 * fy)))) / (Mathf.Atan2((float)cy, (float)fy) + Mathf.Atan2((float)(imageSize.height - cy), (float)fy));

    Debug.Log("fovXScale " + fovXScale);
    Debug.Log("fovYScale " + fovYScale);


    // Adjust Unity Camera FOV https://github.com/opencv/opencv/commit/8ed1945ccd52501f5ab22bdec6aa1f91f1e2cfd4
    if (widthScale < heightScale) {
      arCamera.fieldOfView = (float)(fovx[0] * fovXScale);
    } else {
      arCamera.fieldOfView = (float)(fovy[0] * fovYScale);
    }
    // Display objects near the camera.
    arCamera.nearClipPlane = 0.01f;


    //Init all of the varibles
    ids = new Mat();
    list_ids = new List<int>();
    corners = new List<Mat>();
    rejectedCorners = new List<Mat>();
    rvecs = new Mat();
    tvecs = new Mat();
    rotMat = new Mat(3, 3, CvType.CV_64FC1);


    detectorParams = DetectorParameters.create();
    dictionary = Aruco.getPredefinedDictionary((int)dictionaryId);

    rvec = new Mat();
    tvec = new Mat();
    recoveredIdxs = new Mat();

  }


  public Mat MarkerProcessing(Mat MatImg) {

    Mat resultMat = new Mat();
    Imgproc.cvtColor(MatImg, resultMat, Imgproc.COLOR_RGBA2RGB);
    Aruco.detectMarkers(resultMat, dictionary, corners, ids, detectorParams, rejectedCorners, camMatrix, distCoeffs);

    // add the time to each marker
    // it is not premitted to change value in foreach
    //List<int> temp_list = new List<int>();
    //temp_list.AddRange(Timer.Keys);
    //foreach (int key in temp_list) {
    //  Timer[key] = Timer[key] + Time.deltaTime;
    //  if (Timer[key] > waitTime) {
    //    GameObject parentObject = GameObject.Find("ARGameObjects");
    //    GameObject HidearGameObject = parentObject.transform.Find(idDict[key]).gameObject;
    //    HidearGameObject.SetActive(false);
    //    //Timer[key] = 0.0f;
    //  }
    //}


    if (ids.total() > 0) {
      OpenCVForUnity.UtilsModule.Converters.Mat_to_vector_int(ids, list_ids);

      // draw markers.
      Aruco.drawDetectedMarkers(resultMat, corners, ids, new Scalar(0, 255, 0));
      //Debug.Log("draw!!");
      // estimate pose.
      if (applyEstimationPose) {
        EstimatePoseCanonicalMarker(resultMat);
      }

      if (showRejectedCorners && rejectedCorners.Count > 0)
        Aruco.drawDetectedMarkers(resultMat, rejectedCorners, new Mat(), new Scalar(255, 0, 0));
    }
    //Mat BlackImage = new Mat(resultMat.rows(), resultMat.cols(), CvType.CV_8UC1, new Scalar(0, 0, 0));
    //Mat BlackImage = new Mat(resultMat.rows(), resultMat.cols(), CvType.CV_8UC1, new Scalar(255, 255, 255));
    //return BlackImage;
    return resultMat;
    //Imgproc.circle(resultMat, new Point(380, 380), (int)(380 * (170.0 / 180.0)), new Scalar(255,255,255), 2);
    //Imgproc.circle(resultMat, new Point(380, 380), (int)(380 * (160.0 / 180.0)), new Scalar(255, 255, 255), 2);
    //Imgproc.circle(resultMat, new Point(380, 380), (int)(380 * (150.0 / 180.0)), new Scalar(255, 255, 255), 2);
    //Debug.Log(150.0 / 180.0 * 350.0);


  }


  private void EstimatePoseCanonicalMarker(Mat Img) {
    Aruco.estimatePoseSingleMarkers(corners, markerLength, camMatrix, distCoeffs, rvecs, tvecs);
    for (int i = 0; i < ids.total(); i++) {
      using (Mat rvec = new Mat(rvecs, new OpenCVForUnity.CoreModule.Rect(0, i, 1, 1)))
      using (Mat tvec = new Mat(tvecs, new OpenCVForUnity.CoreModule.Rect(0, i, 1, 1))) {
        // In this example we are processing with RGB color image, so Axis-color correspondences are X: blue, Y: green, Z: red. (Usually X: red, Y: green, Z: blue)
        Aruco.drawAxis(Img, camMatrix, distCoeffs, rvec, tvec, markerLength * 0.5f);

        //Update the AR object depend on marker id

        //Based on the code in OpenCv for Unity
        //UpdateARObjectTransform(rvec, tvec, list_ids[i]);
        UpdateARObjectTransform_2(rvec, tvec);
        //Based on the code By Kyo
        //UpdateObjectCenterAndSize(corners, list_ids[i], i);
      }
    }
  }

  private void UpdateObjectCenterAndSize(List<Mat> corners, int Markerid, int i) {
    if (idDict.ContainsKey(Markerid)) {
      GameObject parentObject = GameObject.Find("ARGameObjects");
      //arGameObject = GameObject.Find(idDict[Markerid]);
      arGameObject = parentObject.transform.Find(idDict[Markerid]).gameObject;
      arGameObject.SetActive(true);
      Timer[Markerid] = 0.0f;
    }


    double[] tl;
    double[] br;
    tl = corners[i].get(0, 0);
    br = corners[i].get(0, 2);

    float centerPoint_x = (float)((tl[0] + br[0]) / 2);
    float centerPoint_y = (float)((tl[1] + br[1]) / 2);

    // change the size of object with the distance of marker(donot change now)
    float ImageMarkerLength = Mathf.Abs((float)br[0] - (float)tl[0]);
    float ChangeScale = 1;
    //ImageMarkerLength / markerLengthStandard;
    Vector3 ScaleChange = new Vector3(ChangeScale, 1.0f, ChangeScale);
    Vector3 Currentpose = ImgPositionToTransform((float)centerPoint_x, (float)centerPoint_y);

    //donot change the position
    //float angle = 0;
    float angle = 0.0f;
    //shadeTouch.RotateProjectionContect((float)centerPoint_x, (float)centerPoint_y);

    arGameObject.transform.localScale = ScaleChange;
    arGameObject.transform.position = Currentpose;
    arGameObject.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));


  }


  private void UpdateARObjectTransform(Mat rvec, Mat tvec, int id) {

    if (idDict.ContainsKey(id)) {
      GameObject parentObject = GameObject.Find("ARGameObjects");
      //arGameObject = GameObject.Find(idDict[Markerid]);
      arGameObject = parentObject.transform.Find(idDict[id]).gameObject;
      arGameObject.SetActive(true);
    }

    // Convert to unity pose data.
    double[] rvecArr = new double[3];
    rvec.get(0, 0, rvecArr);
    double[] tvecArr = new double[3];
    tvec.get(0, 0, tvecArr);

    //arGameObject.transform.position = new Vector3((float)tvecArr[0], (float)tvecArr[1], (float)tvecArr[2]);
    PoseData poseData = ARUtils.ConvertRvecTvecToPoseData(rvecArr, tvecArr);

    // Changes in pos/rot below these thresholds are ignored.
    if (enableLowPassFilter) {
      ARUtils.LowpassPoseData(ref oldPoseData, ref poseData, positionLowPass, rotationLowPass);
    }
    oldPoseData = poseData;

    // Convert to transform matrix.
    ARM = ARUtils.ConvertPoseDataToMatrix(ref poseData, true, true);


    ARM = arCamera.transform.localToWorldMatrix * ARM;
    ARUtils.SetTransformFromMatrix(arGameObject.transform, ref ARM);


    //Only move and do not rotate
    Vector3 rawRotation = arGameObject.transform.rotation.eulerAngles;
    arGameObject.transform.rotation = Quaternion.Euler(new Vector3(0, adjustRotation(rawRotation[1]), 0));
  }

  private void UpdateARObjectTransform_2(Mat rvec, Mat tvec) {
    // Convert to unity pose data.
    double[] rvecArr = new double[3];
    rvec.get(0, 0, rvecArr);
    double[] tvecArr = new double[3];
    tvec.get(0, 0, tvecArr);
    PoseData poseData = ARUtils.ConvertRvecTvecToPoseData(rvecArr, tvecArr);

    // Changes in pos/rot below these thresholds are ignored.
    if (enableLowPassFilter) {
      ARUtils.LowpassPoseData(ref oldPoseData, ref poseData, positionLowPass, rotationLowPass);
    }
    oldPoseData = poseData;

    // Convert to transform matrix.
    ARM = ARUtils.ConvertPoseDataToMatrix(ref poseData, true, true);

    //if (shouldMoveARCamera) {

    //  ARM = arGameObject.transform.localToWorldMatrix * ARM.inverse;

    //  ARUtils.SetTransformFromMatrix(arCamera.transform, ref ARM);

    //} else {

      ARM = arCamera.transform.localToWorldMatrix * ARM;

      ARUtils.SetTransformFromMatrix(arGameObject.transform, ref ARM);
    //}
  }


  // divide the circle into 12 piece
  private float adjustRotation(float raw) {
    raw = (raw + 360) % 360;
    return ((int)raw / 30) * 30 + 15;

  }



  //width 752 height 760
  //origin point on br now, width is x axis and height is y(with object rotate 180 degree and camera do not)
  public Vector3 ImgPositionToTransform(float x, float z) {


    x = -(x - Imgwidth / 2) / Imgwidth * Margin;
    z = -(z - Imgheight / 2) / Imgheight * Margin;

    return new Vector3(x, 0, z);
  }
}