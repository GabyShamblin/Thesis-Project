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



  // Offset/no offset, Keyframe/continuous, Manuel/bimanuel
  public static bool[] vis = new bool[3];

  // Total 8
  // Offset switch: 4
  // Frame switch: 2
  // Hands switch: 1
  public static int trial = 1;

  //! The current set of movements
  /*!
    0 = Position
    1 = Rotation
    2 = Mixed
  */
  public static int move = 0;

  //! How much to offset the controller
  public static float ghostOffset = 0.5f;



  //! The current frame
  public static int currFrame = 0;

  // What frame each hand is at
  public static int[] armCheck = new int[2];

  // Absolute correct path for user to follow, only for visualization
  // Movement x Hand
  public static List<List<Hand>> traces;

  // Saved user movements
  public static List<Hand> userHands = new List<Hand>(2);
}

//! Each arm has a list of data per frame of video
public class Hand
{
  //! User hand position for feedback, temporary and gets saved to user info
  public List<Vector3> Positions { get; set; }
  //! User hand rotation for feedback, temporary and gets saved to user info
  public List<Vector3> Rotations { get; set; }
  //! Timestamp if the movement
  public List<float> Timestamps { get; set; }

  public Hand() {
    Positions = new List<Vector3>();
    Rotations = new List<Vector3>();
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