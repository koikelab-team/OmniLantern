/****************************************************
    ProjectionTarget Plane作成  
    Created on 2018/09/04 by SATOToshiki
 ****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FisheyeProjectionPlaneMesh : MonoBehaviour {

  public int width = 4;
  public int height = 4;

  public float size = 1.0f;

  /* 焦点距離(正規化画像平面に投影するのでf=1.0でOK) */
  public float focalLength = 1.0f;

  /* 内部パラメータ */
  public float fx = 1.0f;
  public float fy = 1.0f;
  public float cx = 0.0f;
  public float cy = 0.0f;

  protected Mesh mesh;

  /* 歪み係数 */
  public float k1, k2, k3, k4;

  public enum PlaneType{
    TOP, LEFT, RIGHT, UP, Down,
  }

  public PlaneType planeType = PlaneType.TOP;

  // Use this for initialization
  void Start () {

    /* 板メッシュを作成 */
    Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];

    for (int j = 0; j < height + 1; j++) {
      for (int i = 0; i < width + 1; i++) {
        float x = -size / 2 + i * (size / width);
        float y = -size / 2 + j * (size / height);
        switch (planeType) {
          case PlaneType.TOP:
            vertices[j * (width + 1) + i] = fishEyeProject2XZPlane(new Vector3(x, size / 2.0f, y));
            break;
          case PlaneType.LEFT:
            vertices[j * (width + 1) + i] = fishEyeProject2XZPlane(new Vector3(-size / 2.0f, -y, x));
            break;
          case PlaneType.RIGHT:
            vertices[j * (width + 1) + i] = fishEyeProject2XZPlane(new Vector3(size / 2.0f, y, -x));
            break;
          case PlaneType.UP:
            vertices[j * (width + 1) + i] = fishEyeProject2XZPlane(new Vector3(-x, y, -size / 2.0f));
            break;
          case PlaneType.Down:
            vertices[j * (width + 1) + i] = fishEyeProject2XZPlane(new Vector3(x, y, size / 2.0f));
            break;
        } // switch
      }
    }

    /* UVの割り当て */
    Vector2[] uv = new Vector2[(width + 1) * (height + 1)];
    for (int j = 0; j < height + 1; j++) {
      for (int i = 0; i < width + 1; i++) {
        uv[j * (width + 1) + i] = new Vector2(i * (1.0f / width), j * (1.0f / height));
      }
    }

    /* 三角形メッシュのインデックス作成 */
    int[] indices = new int[(width * height) * 3 * 2];
    int index = 0;
    for (int j = 0; j < height; j++) {
      for (int i = 0; i < width; i++) {
        int v0 = j        * (width + 1) + i;
        int v1 = (j + 1)  * (width + 1) + i;
        int v2 = j        * (width + 1) + i + 1;
        int v3 = (j + 1)  * (width + 1) + i;
        int v4 = (j + 1)  * (width + 1) + i + 1;
        int v5 = j        * (width + 1) + i + 1;
        indices[index * 6 + 0] = v0;
        indices[index * 6 + 1] = v1;
        indices[index * 6 + 2] = v2;
        indices[index * 6 + 3] = v3;
        indices[index * 6 + 4] = v4;
        indices[index * 6 + 5] = v5;
        index++;
      }
    }

    /* Mesh登録 */
    mesh = new Mesh();
    mesh.vertices = vertices;
    mesh.triangles = indices;
    mesh.uv = uv;
    gameObject.GetComponent<MeshFilter>().sharedMesh = mesh;
  }

  /* 投影後の座標を求める */
  protected Vector3 fishEyeProject2XYPlane(Vector3 src) {
    src.z += focalLength;
    Vector3 srcNormalized = src.normalized;
    float length = Mathf.Sqrt(srcNormalized.x * srcNormalized.x + srcNormalized.y * srcNormalized.y);
    float theta = Mathf.Atan2(length, srcNormalized.z);
    float theta2 = theta * theta;       // theta^2
    float theta4 = theta2 * theta2;     // theta^4
    float theta6 = theta4 * theta2;     // theta^6
    float theta8 = theta4 * theta4;     // theta^8
    float thetaD = theta * (1.0f + k1 * theta2 + k2 * theta4 + k3 * theta6 + k4 * theta8);
    float r = focalLength * Mathf.Tan(theta);
    return new Vector3(thetaD / r * (focalLength * src.x) / src.z, thetaD / r * (focalLength * src.y) / src.z, 0.0f);
  } // fishEyeProjectToXYPlane

  /* 投影後の座標を求める */
  protected Vector3 fishEyeProject2XZPlane(Vector3 src) {
    src.y += focalLength;
    Vector3 srcNormalized = src.normalized;
    float length = Mathf.Sqrt(srcNormalized.x * srcNormalized.x + srcNormalized.z * srcNormalized.z);
    float theta = Mathf.Atan2(length, srcNormalized.y);
    float theta2 = theta * theta;       // theta^2
    float theta4 = theta2 * theta2;     // theta^4
    float theta6 = theta4 * theta2;     // theta^6
    float theta8 = theta4 * theta4;     // theta^8
    float thetaD = theta * (1.0f + k1 * theta2 + k2 * theta4 + k3 * theta6 + k4 * theta8);
    float r = focalLength * Mathf.Tan(theta);
    return new Vector3(
      thetaD / r * (focalLength * src.x) / src.y * fx + cx, 
      0.0f, 
      thetaD / r * (focalLength * src.z) / src.y * fy + cy
    );
  } // fishEyeProject2XZPlane

}
