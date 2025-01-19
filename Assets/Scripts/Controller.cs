using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

//! Controls the videos, lines, and audio
public class Controller : MonoBehaviour
{
  //! Line logic
  private LineLogic lineLogic;
  //! Export data
  private ExportData exportData;
  //! Whether all gestures have been finished
  private bool finished = false;

  void Start()
  {
    lineLogic = GetComponent<LineLogic>();
    exportData = GetComponent<ExportData>();
  }

  //! Start video player. Triggered by import data.
  public void StartTracing() {
    lineLogic.StartLines();
    Globals.start = true;
    
    // StartCoroutine(PlayMovement());
  }

  private IEnumerator PlayMovement() {
    for (int frame = 0; frame < Globals.traces[Globals.move][0].Positions.Count; frame++) {
      lineLogic.UpdateLines(frame);
      yield return new WaitForSeconds(0.01f);
    }
  }

  // TODO: Add stuff to this to update everything.
  void Update() 
  {
    if (Globals.paused) {
      if (Input.GetKeyDown("up")) {
        Debug.Log("Play key hit");
        StartCoroutine(PlayMovement());
      }
      if (Input.GetKeyDown("right")) {
        Debug.Log("Forward key hit");
        Forward();
      }
      else if (Input.GetKeyDown("left")) {
        Debug.Log("Rewind key hit");
        StartCoroutine(Rewind());
      }
    }
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
