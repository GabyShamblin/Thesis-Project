using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconColorChange : MonoBehaviour
{
  [SerializeField] Renderer controllerRender;
  [SerializeField] Color color1;
  [SerializeField] Color color2;

  void Start() {
    if (controllerRender == null) {
      controllerRender = GetComponent<Renderer>();
      if (controllerRender == null) {
        Debug.LogError(gameObject.name + " renderer not set!");
      }
    }
  }

  public void UpdateColor(float blend) {
    if (blend < 0 || blend > 1) { return; }
    controllerRender.material.color = Color.Lerp(color1, color2, blend);
  }
}
