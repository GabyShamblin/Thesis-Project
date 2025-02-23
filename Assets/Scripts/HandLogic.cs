using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! How all hands are controlled
public class HandLogic : MonoBehaviour
{
  [Tooltip("The hands")]
  //! [Input List] The list of hands to use
  public HandTrack[] hands;

  [Tooltip("The distance differece allowed to move on to the next frame")]
  //! [Input] The distance differece allowed to move on to the next frame
  public float distanceAllowance = 0.025f;

  
  [Tooltip("The angle difference allowed to move on to the next frame")]
  //! [Input] The angle difference allowed to move on to the next frame
  public float angleAllowance = 20f;

  [Tooltip("How many frames one hand is allowed to be ahead of the other before they are considered out of sync and the gesture is marked as failed")]
  //! [Input] Frame difference allowed between the hands
  /*!
    How many frames one hand is allowed to be ahead of the other before they are considered out of sync and the gesture is marked as failed
  */
  [SerializeField] private int frameAllowance = 5;

  //! Controller
  private Controller control;

  //! Whether the gesture has been finished
  private bool cont = true;
  //! Whether the gesture has been failed
  private bool fail = false;

  void Start() 
  {
    control = this.GetComponent<Controller>();

    Globals.distAllow = distanceAllowance;
    Globals.angleAllow = angleAllowance;

    Globals.userHands = new List<Hand>();
    Globals.userHands.Add(new Hand());
    Globals.userHands.Add(new Hand());

    // bool hasArms = false;
    // for (int i = 0; i < Globals.armCheck.Length; i++) {
    //   if (Globals.armCheck[i] == 0) {
    //     Debug.LogWarning("Arm " + i + " is not enabled");
    //   } else {
    //     hasArms = true;
    //   }
    // }

    if (hands == null || hands.Length == 0) {
      Debug.LogError("Arms are not set and WILL cause problems");
    }
  }

  void Update() {
    if (Globals.start && Globals.paused) {
      if (Globals.vis[2] == 0) {
        if (hands[1].currFrame < Globals.traces[Globals.move][1].Positions.Count-1) {
          cont = false;
        } 

      } else {
        // If the arms are out of sync, fail and restart gesture once finished
        if (Math.Abs(hands[0].currFrame - hands[1].currFrame) > frameAllowance && !fail) {
          fail = true;
          Debug.Log("Fail gesture: " + hands[0].currFrame + "-" + hands[1].currFrame + "=" + Math.Abs(hands[0].currFrame - hands[1].currFrame) + " > " + frameAllowance);
        }

        // Check if all arms are finished
        for (int i = 0; i < hands.Length; i++) {
          if (hands[i].currFrame < Globals.traces[Globals.move][i].Positions.Count-1) {
            cont = false;
            // Debug.Log(i + ": " + hands[i].currFrame + " >= " + (Globals.traces[Globals.move][i].Positions.Count-1));
          } 
        }
      }

      // If got to the end of the gesture, continue to next gesture (or restart)
      if (cont) {
        if (fail) { StartCoroutine(control.Rewind()); }
        else { control.Forward(); }
        fail = false;
      } else {
        cont = true;
      }
    }
  }

  //! Move hands back to the start of the current gesture. Triggered by rewind.
  public void ResetHands() {
    for (int i = 0; i < hands.Length; i++) {
      hands[i].ResetHand();
    }
  }
}
