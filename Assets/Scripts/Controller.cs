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



  void Start() {
    lineLogic = GetComponent<LineLogic>();
    exportData = GetComponent<ExportData>();
  }

  //! Start video player. Triggered by import data.
  public void StartTracing() {
    lineLogic.StartLines();
    Globals.start = true;
    
    exportData.StartData();
    // StartCoroutine(PlayMovement());
  }

  private IEnumerator PlayMovement() {
    string debug = Globals.trial + ": ";
    if (Globals.vis[0] == 1) {
      debug += "Offset, ";
    } else {
      debug += "In-place, ";
    }
    if (Globals.vis[1] == 1) {
      debug += "Keyframe, ";
    } else {
      debug += "Continuous, ";
    }
    if (Globals.vis[2] == 0) {
      debug += "Unimanuel, ";
    } else if (Globals.vis[2] == 1) {
      debug += "Mirror Bimanuel, ";
    } else {
      debug += "Async Bimanuel, ";
    }
    debug += "" + Globals.move;
    Debug.Log(debug);
    for (int frame = 0; frame < Globals.traces[Globals.move][0].Positions.Count; frame++) {
      lineLogic.UpdateLines(frame);
      yield return new WaitForSeconds(0.01f);
    }
    Globals.paused = true;
  }

  void Update() 
  {
    if (Globals.paused) {
      if (Input.GetKeyDown("up")) {
        StartCoroutine(PlayMovement());
      }
      // else if (Input.GetKeyDown("down")) {
      //   lineLogic.StartLines();
      // }

      if (Input.GetKeyDown("right")) {
        Forward();
      }
      else if (Input.GetKeyDown("left")) {
        // StartCoroutine(Rewind());
      }
    }
  }

  //! Move video to next gesture. Triggered by moving hands to correct positions or clicking right arrow key.
  public void Forward() {
    // Debug.Log("Forward");
    lineLogic.ResetLines();
    Globals.paused = false;

    if (Globals.leftover.Count <= 0) {
      exportData.WriteData();
      return;
    }

    if (Globals.move < 2) {
      // Debug.Log("Controller: Next movement");
      Globals.move++;
    } else {
      // Randomly get next trial number
      int index = Random.Range(0, Globals.leftover.Count);
      Globals.trial = Globals.leftover[index];
      Globals.leftover.RemoveAt(index);
      Debug.Log("Controller: Next visualization");

      // First half of trials have no offset
      // Second half are offset
      if (Globals.trial < 6) {
        Globals.vis[0] = 0;
      } else {
        Globals.vis[0] = 1;
      }

      // Trials 0-2 and 6-8 are continuous animated
      // Trials 3-5 and 9-11 are keyframe animated
      if (Globals.trial < 3 || (Globals.trial >= 6 && Globals.trial < 9)) {
        Globals.vis[1] = 0;
      } else {
        Globals.vis[1] = 1;
      }

      // Trials 2,5,8,11 are async bimanuel
      // Trials 1,4,7,10 are sync bimanuel
      // Trials 0,3,6,9 are unimanuel
      if (Globals.trial+1 % 3 == 0) {
        Globals.vis[2] = 2;
      }
      else if (Globals.trial % 3 == 0) {
        Globals.vis[2] = 0;
      } 
      else {
        Globals.vis[2] = 1;
      }

      lineLogic.UpdateVis();

      Globals.move = 0;
    }
    StartCoroutine(PlayMovement());
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
