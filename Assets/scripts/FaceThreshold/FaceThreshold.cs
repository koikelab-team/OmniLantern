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

public class FaceThreshold : MonoBehaviour {
  public GameObject ProjectionImage;

  protected Mat src;
	// Use this for initialization
	void Start () {
    //ProjectionMat = new Mat();
    //ProjectionImage.SetActive(true);

    //Texture texToProcess = ProjectionImage.GetComponent<Renderer>().material.mainTexture;
    ////Debug.Log(texToProcess);
    //Texture2D Tex2dImg = new Texture2D(texToProcess.width, texToProcess.height);
    //Utils.textureToTexture2D(texToProcess, Tex2dImg);

    ////convert the tex2d to mat
    //ProjectionMat = new Mat(texToProcess.height, texToProcess.width, CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
    //Utils.texture2DToMat(Tex2dImg, ProjectionMat);

    src = OpenCVForUnity.ImgcodecsModule.Imgcodecs.imread(Application.streamingAssetsPath + "/faceThreshold.jpg",1);
  }

  // Update is called once per frame
  public Mat Processing(Mat img){
    Mat mask = new Mat();
    Core.bitwise_not(img, mask);
    Imgproc.resize(src, src, new Size(mask.cols()/2,mask.rows()/2));
    Imgproc.resize(mask, mask, new Size(src.cols(),src.rows()));

    Mat kernel = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(5, 5));
    Imgproc.morphologyEx(mask, mask, Imgproc.MORPH_OPEN, kernel);
    Imgproc.morphologyEx(mask, mask, Imgproc.MORPH_CLOSE, kernel);


    Mat img_masked = new Mat(src.rows(), src.cols(), CvType.CV_8UC4, new Scalar(0, 0, 0, 255));
    //Debug.Log(mask.size());
    //Debug.Log(mask.size());
    //Debug.Log(src.get(1,1).Length);
    //Debug.Log(img_masked.size());

    Core.bitwise_or(src, img_masked, mask);
    for (int i = 0; i < img_masked.cols(); i++) {
      for (int j = 0; j < img_masked.rows(); j++) {
        if (mask.get(j, i)[0] < 10) {
          double[] ch = src.get(j, i);
          img_masked.put(j, i, ch[0], ch[1], ch[2], 1);
          //img_masked.put(j, i, 100, 0, 0, 1);
        } else {

          img_masked.put(j, i, 0, 0, 0, 1);
        }
      }
    }

    return img_masked;
  }
}
