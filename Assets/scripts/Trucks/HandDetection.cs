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

public class HandDetection : MonoBehaviour {
	public float HandThreshold;
  public GameObject HandObject;

  public double MinArea_thres = 3000;
  public double MaxArea_thres = 10000;

  // for adjusting the projection position with marker
  public float MoveParam = 0.08f;
  protected double MaxArea = 0;
  protected Vector3 OldPosedata;

  protected ShadeTouch shadeTouch;
  protected MarkerDetection markerDetection;

  private void Start() {
    shadeTouch = new ShadeTouch();
    markerDetection = new MarkerDetection();
  }

  /*2 modes:
   if with marker,use the marker for location, and use hand for controlling
   if without marker, use hand for location and controlling*/
  public Mat HandProcessing (Mat img,bool withMarker, GameObject markerObject = null) {
    Mat resultMat = new Mat();
    Imgproc.threshold(img, resultMat, HandThreshold, 255, Imgproc.THRESH_BINARY);

    Mat kernel = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(10, 10));
    Imgproc.morphologyEx(resultMat, resultMat, Imgproc.MORPH_OPEN, kernel);
    Imgproc.morphologyEx(resultMat, resultMat, Imgproc.MORPH_CLOSE, kernel);

    List<MatOfPoint> contours_out = new List<MatOfPoint>();
    List<MatOfPoint> contours_all = new List<MatOfPoint>();
    Mat hierarchy_out = new Mat();
    Mat hierarchy_all = new Mat();

    // RETR_EXER & CHAIN_APPROX_SIMPLE:find the out contour
    Imgproc.findContours(resultMat, contours_out, hierarchy_out, 0, 2);

    // RETR_TREE & CHAIN_APPROX_SIMPLE:find all of the contours
    Imgproc.findContours(resultMat, contours_all, hierarchy_all, 3, 2);

    // if have any inside. 
    if(contours_all.Count > contours_out.Count){
      for (int i =0; i < contours_out.Count; i++){
        Size OutSize = contours_out[i].size();

        for (int j = 0; j < contours_all.Count; j++){
          Size AllSize = contours_all[j].size();
          if (OutSize == AllSize){
            contours_all.RemoveAt(j);
          }
        }
      }
    }


    MatOfPoint2f contour_2f = new MatOfPoint2f();
    OpenCVForUnity.CoreModule.Rect BoundingRect = new OpenCVForUnity.CoreModule.Rect();
    List<OpenCVForUnity.CoreModule.Rect> TouchRect = new List<OpenCVForUnity.CoreModule.Rect>();

    int maxArea_i = -1;
    for (int i = 0; i < contours_all.Count; i++) {
      double area = Imgproc.contourArea(contours_all[i]);
      if (area > MaxArea && area > MinArea_thres && area < MaxArea_thres) {
        MaxArea = area;
        maxArea_i = i;
      }
    }
      MaxArea = 0;
      if (maxArea_i > -1) {
        //Debug.Log(Imgproc.contourArea(contours_all[maxArea_i]));
        contours_all[maxArea_i].convertTo(contour_2f, CvType.CV_32F);
        BoundingRect = Imgproc.boundingRect((Mat)contour_2f);
        TouchRect.Add(BoundingRect);
        Imgproc.rectangle(resultMat, BoundingRect, new Scalar(0, 0, 0));
      }

    for (int i = 0; i < TouchRect.Count; i++) {
      Point tl = TouchRect[i].tl();
      Point br = TouchRect[i].br();
      Point centerRect = new Point((tl.x + br.x) / 2, (tl.y + br.y) / 2);

      //Vector3 CurrentPos = shadeTouch.ImgPositionToTransform((float)centerRect.x, (float)centerRect.y);

      //if have marker, only adjust the position of last marker content
      if(withMarker){
        HandObject = markerObject;
        Vector3 MovePose = shadeTouch.ImgPositionToTransform((float)centerRect.x, (float)centerRect.y);
        Vector3 OldPose = HandObject.transform.position;

        // slow down the moving speed
        Vector3 CurrentPos = new Vector3(OldPose[0] + (MovePose[0] - OldPose[0]) * MoveParam, 0, OldPose[2] + (MovePose[2] - OldPose[2]) * MoveParam);
        HandObject.transform.position = CurrentPos;
      }
      // if don't have marker, using the hand for locating
      else{
        Vector3 CurrentPos = shadeTouch.ImgPositionToTransform((float)centerRect.x, (float)centerRect.y);
        HandObject.transform.position = CurrentPos;
        float angle = shadeTouch.RotateProjectionContect((float)centerRect.x, (float)centerRect.y);
        HandObject.transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
      }
    }

    // make the bg black and don not effect projection
    Mat BlackImage = new Mat(resultMat.rows(), resultMat.cols(), CvType.CV_8UC1, new Scalar(0,0,0));
    //return BlackImage;
    return resultMat;
  }

}
