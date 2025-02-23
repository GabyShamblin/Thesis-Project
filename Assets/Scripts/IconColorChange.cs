using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconColorChange : MonoBehaviour
{
  [SerializeField] Renderer controllerRender;
  [SerializeField] Color[] colors;
  private int len = 0;
  private float interval = 0f;

  void Start() {
    if (controllerRender == null) {
      controllerRender = GetComponent<Renderer>();
      if (controllerRender == null) {
        Debug.LogError(gameObject.name + " renderer not set!");
      }
    }

    len = colors.Length - 1;
    interval = 1f / (float)len;
  }

  public void UpdateColor(float blend) {
    if (blend < 0 || blend > 1) { return; }
    len = colors.Length - 1;
    interval = 1f / (float)len;
    
    for (int i = 1; i < colors.Length; i++) {
      if (blend >= (interval * (i-1)) && blend < (interval * i)) {
        // Debug.Log((interval * (i-1)) + " < " + blend + " < " + (interval * i));
        controllerRender.material.color = Color.Lerp(colors[i-1], colors[i], blend*len-(i-1));
        return;
      }
    }
  }
}
