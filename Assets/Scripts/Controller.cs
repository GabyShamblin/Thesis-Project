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

  // TODO: Add stuff to this to update everything. (Also rename to just controller, no video anymore)
  void Update() 
  {
  }

  //! Move video to next gesture. Triggered by moving hands to correct positions or clicking right arrow key.
  public void Forward() {
    Debug.Log("Forward");
    lineLogic.ResetLines();
    Globals.paused = false;

    if (Globals.trial % 4 == 0) {
      Globals.vis[0] = true;
    }

    if (Globals.trial % 2 == 0) {
      Globals.vis[1] = true;
    } else {
      Globals.vis[1] = false;
    }

    Globals.vis[2] = !Globals.vis[2];

    Globals.trial += 1;
  }

  //! Replay current gesture. Triggered by geting hand positions wrong or clicking left arrow key.
  public IEnumerator Rewind() {
    Globals.start = false;
    // audioFeedback.PlayOneShot(failAudio);
    yield return StartCoroutine(lineLogic.ReplayHands());

    Debug.Log("Rewind");
    lineLogic.ResetLines();
    for ( int i = 0; i < 2; i++) {
      Globals.armCheck[i] = 0;
    }
  }
}
