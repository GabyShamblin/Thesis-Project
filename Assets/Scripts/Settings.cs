using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/********************************************************************************************
  \mainpage
  \section intro Introduction
    Intro

  \section ref References
    Jigsaw dataset: https://cirl.lcsr.jhu.edu/research/hmm/datasets/jigsaws_release/
********************************************************************************************/

//! Global variables
/*!
  Stores the game states, hand states, machine data, and gesture data.
*/
public static class Globals
{
  public static bool start = false;
  public static bool paused = true;



  // 0: No offset,  1: Offset/no offset
  // 0: Continuous, 1: Keyframe
  // 0: Unimanuel,  1: Mirror Bimanuel,  2: Async Bimanuel
  public static int[] vis = new int[3];

  // Total 12 (0-11)
  public static int trial = -1;
  // Store all remaining trials. Are randomly removed in Controller.Forward.
  public static List<int> leftover = new List<int>();

  //! The current set of movements. Not from the actual jigsaws, just me mimicing it.
  /*!
    0 = Knot tying
    1 = Needle passing
    2 = Suturing
  */
  public static int move = 0;
  //! How many attempts was made for a movement
  public static int moveAttempt = 0;

  //! How much to offset the controller
  public static float ghostOffset = 0.5f;



  //! Distance allowance
  public static float distAllow;

  //! Angle allowance
  public static float angleAllow;

  //! The current frame
  public static int currFrame = 0;

  // What frame each hand is at
  public static int[] armCheck = new int[2];

  // Absolute correct path for user to follow, only for visualization
  // Movement x Hand
  public static List<List<Hand>> traces;

  // Saved user movements
  public static List<Hand> userHands;
}

//! Each arm has a list of data per frame of video
public class Hand
{
  //! User hand position for feedback, temporary and gets saved to user info
  public List<Vector3> Positions { get; set; }
  //! User hand rotation for feedback, temporary and gets saved to user info
  public List<Quaternion> Rotations { get; set; }
  //! Timestamp if the movement
  public List<float> Timestamps { get; set; }

  public Hand() {
    Positions = new List<Vector3>();
    Rotations = new List<Quaternion>();
    Timestamps = new List<float>();
  }

  public override string ToString() {
    string output = "";
    for (int i = 0; i < Positions.Count; i++) {
      // Check if each 
      if (Positions[i] == null) {
        output += Vector3.zero.ToString("F8");
      } else {
        output += Positions[i].ToString("F8");
      }
      output += " ";

      if (Rotations[i] == null) {
        output += Vector3.zero.ToString("F8");
      } else {
        output += Rotations[i].ToString("F8");
      }
      output += " ";
    }

    return output;
  }
}