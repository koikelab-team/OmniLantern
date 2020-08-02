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

public class ShadeTouch : MonoBehaviour {
  public enum TouchType {
    OneFinger,
    MultiFinger
  }
  public TouchType touchType;

  //things to project
  protected GameObject TouchObject;

  [Space(10)]

  public float ShadeThreshold = 50;
  public float MinArea = 5000;
  protected double MaxArea = 0;
  public double MaxHull = 0.5;
  public double MinArea_thres = 0;
  public double MaxArea_thres = 2000;
  //protected Mat img_delta;

  [Space(10)]

  // for changing from imagepixel to world
  public float width = 752;
  public float height = 760;
  public float Margin = 0.50f;

  protected Vector3 OldPosedata;

  private void Start() {
    GameObject parentObject = GameObject.Find("TouchGameObjects");
    TouchObject = parentObject.transform.Find("ShowTime").gameObject;
    TouchObject.SetActive(false);
  }

  public Mat TouchProcessing (Mat img, Mat pre_frame) {
    TouchObject.SetActive(true);

    Mat img_delta = new Mat();
    List<MatOfPoint> contours = new List<MatOfPoint>();
    Mat hierarchy = new Mat();
    MatOfPoint2f contour_2f = new MatOfPoint2f();
    OpenCVForUnity.CoreModule.Rect BoundingRect = new OpenCVForUnity.CoreModule.Rect();
    
    float hullRatio = 0;

    // remove background & threshold
    Core.absdiff(img,pre_frame,img_delta);
    Imgproc.threshold(img_delta,img_delta,ShadeThreshold,255,Imgproc.THRESH_BINARY);
    
    //reduce noise
    Mat kernel = Imgproc.getStructuringElement(Imgproc.MORPH_RECT,new Size(5,5));
    Imgproc.morphologyEx(img_delta,img_delta,Imgproc.MORPH_OPEN,kernel);
    Imgproc.morphologyEx(img_delta,img_delta,Imgproc.MORPH_CLOSE,kernel);
    //Imgproc.blur(img_delta,img_delta,new Size(5,5));

    // RETR_EXTERNAL & CHAIN_APPROX_SIMPLE
    Imgproc.findContours(img_delta,contours,hierarchy,0,2);
    //List<MatOfPoint> hulls = new List<MatOfPoint>();
    List<MatOfInt> hulls = new List<MatOfInt>();
    List<MatOfPoint> hulls_point = new List<MatOfPoint>();

    //for store bounding rect
    List<OpenCVForUnity.CoreModule.Rect> TouchRect = new List<OpenCVForUnity.CoreModule.Rect>();

    for (int i =0; i < contours.Count; i++) {
      hulls.Add(new MatOfInt());
    }


    // Only for max touching
    if(touchType == TouchType.OneFinger){
      int maxArea_i = -1;
      for(int i = 0; i < contours.Count; i++){
        double area = Imgproc.contourArea(contours[i]);
        if(area > MaxArea && area > MinArea_thres && area < MaxArea_thres) {
          MaxArea = area;
          maxArea_i = i;
        }
      }
      MaxArea = 0;
      if (maxArea_i > -1){
        Debug.Log(Imgproc.contourArea(contours[maxArea_i]));
        contours[maxArea_i].convertTo(contour_2f, CvType.CV_32F);
        BoundingRect = Imgproc.boundingRect((Mat)contour_2f);
        TouchRect.Add(BoundingRect);
        Imgproc.rectangle(img_delta,BoundingRect, new Scalar(255, 255, 255));
      }
    }

    // for detecting all of the touch
    if (touchType == TouchType.MultiFinger){
      for (int i = 0; i < contours.Count; i++) {
        double area = Imgproc.contourArea(contours[i]);
        //filter by size of contour
        if (area > MinArea_thres) {
          Imgproc.convexHull(contours[i], hulls[i], false);
          MatOfPoint hull_point = new MatOfPoint();

          // data type is different
          hull_point = convertIndexToPoint(hulls[i], contours[i]);

          //calculate convex ratio
          hullRatio = (float)Imgproc.contourArea(contours[i]) / (float)Imgproc.contourArea(hull_point);
          if (hullRatio > MaxHull) {
            // data type is different(point & point2f)
            contours[i].convertTo(contour_2f, CvType.CV_32F);
            BoundingRect = Imgproc.boundingRect((Mat)contour_2f);
            TouchRect.Add(BoundingRect);
            Imgproc.rectangle(img_delta, BoundingRect, new Scalar(255, 255, 255));
          }
        }
      }
    }

    for (int i = 0; i < TouchRect.Count; i++) {
      Point tl = TouchRect[i].tl();
      Point br = TouchRect[i].br();
      Point centerRect = new Point((tl.x + br.x) / 2, (tl.y + br.y) / 2);

      Vector3 CurrentPos = ImgPositionToTransform((float)centerRect.x, (float)centerRect.y);
      TouchObject.transform.position = CurrentPos;
    }

    Imgproc.circle(img_delta, new Point(0, 0), 5, new Scalar(255, 255, 255));
    Imgproc.circle(img_delta, new Point(200, 0), 5, new Scalar(255, 255, 255));

    Mat BlackImage = new Mat(img_delta.rows(), img_delta.cols(), CvType.CV_8UC1, new Scalar(0, 0, 0));
    //return BlackImage;
    return img_delta;
	}


  protected static MatOfPoint convertIndexToPoint(MatOfInt index, MatOfPoint contour){
    Point[] arrPoint = contour.toArray();
    int[] arrIndex = index.toArray();
    Point[] arrResult = new Point[arrIndex.Length];

    for(int i = 0; i< arrIndex.Length;i++){
      arrResult[i] = arrPoint[arrIndex[i]];
    }

    MatOfPoint hull = new MatOfPoint();
    hull.fromArray(arrResult);
    return hull;
    
  }

  //width 752 height 760
  //origin point on br now, width is x axis and height is y(with object rotate 180 degree and camera do not)
  public Vector3 ImgPositionToTransform(float x, float z) {


    x = -(x - width/2)/width * Margin;
    z = -(z - height / 2) / height * Margin;

    return new Vector3(x,0,z);
  }

  public float RotateProjectionContect(float x, float z){
    float angle = 0;
    angle = Mathf.Atan((width / 2 - x) / (height / 2 -z));
    angle = angle * 180 / Mathf.PI;
    if (z > height / 2){
      angle = angle +180;
    }
    return angle;

  }

}
