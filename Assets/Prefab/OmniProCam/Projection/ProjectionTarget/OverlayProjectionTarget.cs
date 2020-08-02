/************************************************************************
  OverlayProjectionTarget: MlioLightっぽいプロジェクションをするやつ
  Created on 2019/09/21 by SATOToshiki
 ************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayProjectionTarget : ProjectionTarget {

  // Update is called once per frame
  void Update() {

    /* スケールの微調整 */
    if (!Input.GetKey(KeyCode.O)) {

      /* 縦スケールの微調整 */
      if (Input.GetKey(KeyCode.E)) {
        scale.y += 0.1f * Time.deltaTime;
        transform.localScale = scale;
      } else if (Input.GetKey(KeyCode.D)) {
        scale.y -= 0.1f * Time.deltaTime;
        transform.localScale = scale;
      }
      /* 縦スケールの微調整 */
      if (Input.GetKey(KeyCode.F)) {
        scale.x += 0.1f * Time.deltaTime;
        transform.localScale = scale;
      } else if (Input.GetKey(KeyCode.A)) {
        scale.x -= 0.1f * Time.deltaTime;
        transform.localScale = scale;
      }

      /* オフセットの設定 */
    } else {

      /* 縦オフセットの微調整 */
      if (Input.GetKey(KeyCode.E)) {
        offset.y += 0.05f * Time.deltaTime;
        transform.position = new Vector3(offset.x, 0.0f, offset.y);
      } else if (Input.GetKey(KeyCode.D)) {
        offset.y -= 0.05f * Time.deltaTime;
        transform.position = new Vector3(offset.x, 0.0f, offset.y);
      }

      /* 横オフセットの微調整 */
      if (Input.GetKey(KeyCode.F)) {
        offset.x += 0.05f * Time.deltaTime;
        transform.position = new Vector3(offset.x, 0.0f, offset.y);
      } else if (Input.GetKey(KeyCode.A)) {
        offset.x -= 0.05f * Time.deltaTime;
        transform.position = new Vector3(offset.x, 0.0f, offset.y);
      }
    }

    /* 縦オフセットの微調整 */
    if (Input.GetKey(KeyCode.E)) {
      offset.y += 0.05f * Time.deltaTime;
      transform.position = new Vector3(offset.x, 0.0f, offset.y);
    }

    /* 2値化閾値 */
    if (Input.GetKey(KeyCode.UpArrow)) {
      binarizingThreshold += 1.0f;
      if (binarizingThreshold >= 255.0f) {
        binarizingThreshold = 255;
      }
      LibOmniProCam.setBinarizingThreshold(binarizingThreshold);
      print("BinarizingThreshold: " + binarizingThreshold);
    } else if (Input.GetKey(KeyCode.DownArrow)) {
      binarizingThreshold -= 1.0f;
      if (binarizingThreshold <= 0.0f) {
        binarizingThreshold = 0;
      }
      LibOmniProCam.setBinarizingThreshold(binarizingThreshold);
      print("BinarizingThreshold: " + binarizingThreshold);
    }

    ///* 高さ */
    //if (Input.GetKey(KeyCode.PageUp)) {
    //  transform.Translate(0.0f, 0.1f * Time.deltaTime, 0.0f);
    //} else if (Input.GetKey(KeyCode.PageDown)) {
    //  transform.Translate(0.0f, -0.1f * Time.deltaTime, 0.0f);
    //}

    /* 書き出し */
    if (Input.GetKeyDown(KeyCode.Alpha0)) {
      if (omniProCamDeviceManagerPtr.getCameraTextureType() == CameraTextureType.CAMERA_TEXTURE_TYPE_BINARIZED) {
        omniProCamDeviceManagerPtr.setCameraTextureType(CameraTextureType.CAMERA_TEXTURE_TYPE_SRC);
        print("CameraTextureType: Src");
      } else {
        omniProCamDeviceManagerPtr.setCameraTextureType(CameraTextureType.CAMERA_TEXTURE_TYPE_BINARIZED);
        print("CameraTextureType: Binarized");
      }
    }

    OmniProCamData[] ptr = omniProCamDeviceManagerPtr.getOmniProCamDataArrayPtr();
    foreach (OmniProCamData i in ptr) {
      if (i.id != 0) {
      }
    }

    /* 投影 */
    if (omniProCamDeviceManagerPtr.getCameraTextureType() == CameraTextureType.CAMERA_TEXTURE_TYPE_BINARIZED) {
      meshRenderer.material.SetTexture("_AlphaTex0", omniProCamDeviceManagerPtr.getBinarizedCameraImageTexturePtr().texture);
    } else {
      meshRenderer.material.SetTexture("_AlphaTex0", omniProCamDeviceManagerPtr.getSrcCameraImageTexturePtr().texture);
    }
  }

}
