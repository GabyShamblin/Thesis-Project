using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconColorChange : MonoBehaviour
{
  [SerializeField] Renderer renderer;
  [SerializeField] Color color1;
  [SerializeField] Color color2;

  void Start() {
    if (renderer == null) {
      renderer = GetComponent<Renderer>();
      if (renderer == null) {
        Debug.LogError("Object renderer not set!");
      }
    }
  }

  public void UpdateColor(float blend) {
    if (blend < 0 || blend > 1) { return; }
    renderer.material.color = Color.Lerp(color1, color2, blend);
  }
}
