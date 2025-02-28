using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateReal : MonoBehaviour
{
  private float timer = 0f;
  private bool increase = true;

  void Update() {
    if (Globals.waiting) {
      if (timer <= 0) { increase = true; }
      else if (timer >= 1) { increase = false; }

      if (increase) { timer += Time.deltaTime; }
      else { timer -= Time.deltaTime; }

      GetComponent<Renderer>().material.SetFloat("_Outline", Mathf.Clamp(timer, 0f, 3f));
    } else {
      timer = 0f;
    }
  }
}
