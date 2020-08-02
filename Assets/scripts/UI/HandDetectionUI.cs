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

public class HandDetectionUI : MonoBehaviour {
  public float HandThreshold;
  
  public double MinArea_thres = 3000;
  public double MaxArea_thres = 10000;

  // for adjusting the projection position with marker
  public float MoveParam = 0.08f;
  protected double MaxArea = 0;
  protected Vector3 OldPosedata;

  protected ShadeTouch shadeTouch;

  private Button button_1;
  private Button button_2;
  private Button button_3;
  private Button button_4;

  private void Start() {
    shadeTouch = new ShadeTouch();

    button_1 = GameObject.Find("Button1").GetComponent<Button>();
    button_2 = GameObject.Find("Button2").GetComponent<Button>();
    button_3 = GameObject.Find("Button3").GetComponent<Button>();
    button_4 = GameObject.Find("Button4").GetComponent<Button>();
  }

  
  public Mat HandProcessing(Mat img) {

    // process the image
    Mat resultMat = new Mat();
    Imgproc.threshold(img, resultMat, HandThreshold, 255, Imgproc.THRESH_BINARY);

    Mat kernel = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(10, 10));
    Imgproc.morphologyEx(resultMat, resultMat, Imgproc.MORPH_OPEN, kernel);
    Imgproc.morphologyEx(resultMat, resultMat, Imgproc.MORPH_CLOSE, kernel);

    //button areas
    OpenCVForUnity.CoreModule.Rect RectButton_2 = new OpenCVForUnity.CoreModule.Rect(new Point(180, 400), new Point(360, 550));
    OpenCVForUnity.CoreModule.Rect RectButton_3 = new OpenCVForUnity.CoreModule.Rect(new Point(400, 400), new Point(580, 550));
    OpenCVForUnity.CoreModule.Rect RectButton_1 = new OpenCVForUnity.CoreModule.Rect(new Point(180, 170), new Point(360, 320));
    OpenCVForUnity.CoreModule.Rect RectButton_4 = new OpenCVForUnity.CoreModule.Rect(new Point(400, 170), new Point(580, 320));

    Imgproc.rectangle(resultMat, new Point(180, 400), new Point(360, 550), new Scalar(255, 0, 0));
    Imgproc.rectangle(resultMat, new Point(400, 400), new Point(580, 550), new Scalar(255, 0, 0));
    Imgproc.rectangle(resultMat, new Point(180, 170), new Point(360, 320), new Scalar(255, 0, 0));
    Imgproc.rectangle(resultMat, new Point(400, 170), new Point(580, 320), new Scalar(255, 0, 0));


    //VirtualKeybd.Keybd_event(27, 0, 0, 0);
    //if (Input.GetKeyDown(KeyCode.Escape)) {
    //  button_1.onClick.Invoke();
    //}

    // Find hand areas
    List<MatOfPoint> contours_out = new List<MatOfPoint>();
    List<MatOfPoint> contours_all = new List<MatOfPoint>();
    Mat hierarchy_out = new Mat();
    Mat hierarchy_all = new Mat();


    // RETR_EXER & CHAIN_APPROX_SIMPLE:find the out contour
    Imgproc.findContours(resultMat, contours_out, hierarchy_out, 0, 2);

    // RETR_TREE & CHAIN_APPROX_SIMPLE:find all of the contours
    Imgproc.findContours(resultMat, contours_all, hierarchy_all, 3, 2);

    // if have any inside. 
    if (contours_all.Count > contours_out.Count) {
      for (int i = 0; i < contours_out.Count; i++) {
        Size OutSize = contours_out[i].size();

        for (int j = 0; j < contours_all.Count; j++) {
          Size AllSize = contours_all[j].size();
          if (OutSize == AllSize) {
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
      contours_all[maxArea_i].convertTo(contour_2f, CvType.CV_32F);
      BoundingRect = Imgproc.boundingRect((Mat)contour_2f);
      TouchRect.Add(BoundingRect);
      Imgproc.rectangle(resultMat, BoundingRect, new Scalar(0, 0, 0));
    }

    if(TouchRect.Count == 1) {
      Point tl = TouchRect[0].tl();
      Point br = TouchRect[0].br();
      Point centerRect = new Point((tl.x + br.x) / 2, (tl.y + br.y) / 2);

      if (RectButton_1.contains(centerRect)) {
        VirtualKeybd.Keybd_event(65, 0, 0, 0);
        if (Input.GetKey(KeyCode.A)) {
          button_1.onClick.Invoke();
        }
      }

      else if (RectButton_2.contains(centerRect)) {
        VirtualKeybd.Keybd_event(66, 0, 0, 0);
        if (Input.GetKey(KeyCode.B)) {
          button_2.onClick.Invoke();
        }
      } else if (RectButton_3.contains(centerRect)) {
        VirtualKeybd.Keybd_event(67, 0, 0, 0);
        if (Input.GetKey(KeyCode.C)) {
          button_3.onClick.Invoke();
        }
      } else if (RectButton_4.contains(centerRect)) {
        VirtualKeybd.Keybd_event(68, 0, 0, 0);
        if (Input.GetKey(KeyCode.D)) {
          button_4.onClick.Invoke();
        }
      }
    }

    // make the bg black and don not effect projection
    Mat BlackImage = new Mat(resultMat.rows(), resultMat.cols(), CvType.CV_8UC1, new Scalar(0, 0, 0));
    //return BlackImage;

    return resultMat;
  }

}
