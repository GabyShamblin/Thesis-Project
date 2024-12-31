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

  [Tooltip("How many frames one hand is allowed to be ahead of the other before they are considered out of sync and the gesture is marked as failed")]
  //! [Input] Frame difference allowed between the hands
  /*!
    How many frames one hand is allowed to be ahead of the other before they are considered out of sync and the gesture is marked as failed
  */
  [SerializeField] private int frameAllowance = 5;

  //! Video controller
  private VideoController video;
  //! Whether the gesture has been failed
  private bool fail = false;

  void Start()
  {
    video = this.GetComponent<VideoController>();
  }

  //! Move hands back to the start of the current gesture. Triggered by rewind.
  public void ResetHands() {
    for (int i = 0; i < hands.Length; i++) {
      hands[i].ResetHand();
    }
  }
}
