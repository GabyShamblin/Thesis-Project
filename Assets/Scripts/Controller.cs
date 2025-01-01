using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

//! Controls the videos, lines, and audio
public class VideoController : MonoBehaviour
{
  [Tooltip("The screen containing all UI aspects")]
  //! [Input] The screen containing all UI aspects
  [SerializeField] private GameObject screen;

  //! Line logic
  private LineLogic lineLogic;
  //! Export data
  private ExportData exportData;
  //! Whether all gestures have been finished
  private bool finished = false;

  void Start()
  {
    if (screen == null) {
      Debug.LogError("Screen is not set");
    }
    else if (screen.activeSelf == false) {
      Debug.LogError("Screen is not active or set to correct canvas");
    }
    lineLogic = GetComponent<LineLogic>();
    exportData = GetComponent<ExportData>();
  }

  //! Start video player. Triggered by import data.
  public void StartTraces() {
    lineLogic.StartLines();
    Globals.start = true;
  }

  void Update() 
  {
  }

  //! Move video to next gesture. Triggered by moving hands to correct positions or clicking right arrow key.
  public void Forward() {
    Debug.Log("Forward");
    lineLogic.ResetLines();
    Globals.paused = false;

    if (Globals.animated) {
      if (Globals.ghostOffset > 0) {
        if (Globals.bimanuel) {
          Debug.Log("Task complete");
          Globals.start = false;
        } else {
          Globals.bimanuel = true;
        }
        Globals.ghostOffset = 0;
      } else {
        Globals.ghostOffset = 1.0f;
      }
    } else {
      if (Globals.ghostOffset > 0) {
        if (Globals.bimanuel) {
          Globals.animated = true;
        } else {
          Globals.bimanuel = true;
        }
        Globals.ghostOffset = 0;
      } else {
        Globals.ghostOffset = 1.0f;
      }
    }
  }
}
