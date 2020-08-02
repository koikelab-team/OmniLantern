using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonControl : MonoBehaviour {
  AsyncOperation async;

  private void Awake() {
    //StartCoroutine(LoadScene("HelloOmniProCam"));
  }

  IEnumerator LoadScene(string sceneName) {
    yield return null;
    async = SceneManager.LoadSceneAsync(sceneName);
    async.allowSceneActivation = false;
    yield return null;
  }

  public void Button1() {
    Debug.Log("aaaa");
    //async.allowSceneActivation = true;
    //SceneManager.LoadScene("Mode_1_Searching");

  }

  public void Button2() {
    Debug.Log("bbbb");
    //SceneManager.LoadScene("Mode_2_Place");
  }

  public void Button3() {
    Debug.Log("cccc");
    //SceneManager.LoadScene("Mode_3_Game");
  }

  public void Button4() {
    //Debug.Log("dddd");
    SceneManager.LoadScene("LampShade");
  }
}
