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
  //! Whether the gesture has been failed
  private bool fail = false;

  void Start() 
  {
    control = this.GetComponent<Controller>();

    Globals.distAllow = distanceAllowance;
    Globals.angleAllow = angleAllowance;

    Globals.userHands = new List<Hand>(2);
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

  void Update()
  {
    if (Globals.start && Globals.paused) {
      // Check if all hands are in the correct positions
      bool cont = true;
      int smallest = 100000000;

      // If the arms are out of sync, fail and restart gesture once finished
      if (Math.Abs(hands[0].currFrame - hands[1].currFrame) > frameAllowance && !fail) {
        fail = true;
      }

      for (int i = 0; i < hands.Length; i++) {
        // Check if all arms are finished
        if (hands[i].currFrame < Globals.traces[Globals.move].Count-1) {
          cont = false;
        }
        
        if (hands[i].currFrame < smallest) {
          smallest = hands[i].currFrame;
        }
      }

      // If got to the end of the gesture, continue to next gesture (or restart)
      if (cont) {
        if (fail) { StartCoroutine(control.Rewind()); }
        else { control.Forward(); }
        
        fail = false;
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
