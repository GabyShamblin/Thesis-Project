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

  //! Movement visualization setting
  /*!
    f = Static
    t = Animated
  */
  public static bool animated = false;

  //! Line visualization setting
  /*!
    f = One hand
    t = Two hand
  */
  public static bool bimanuel = false;

  //! How much to offset the controller
  public static float ghostOffset = 0f;

  //! The current frame
  public static int currFrame = 0;

  //! The current set of movements
  /*!
    0 = Position
    1 = Rotation
    2 = Mixed
  */
  public static int currSet = 0;

  // Absolute correct path for user to follow, only for visualization
  // Movement x Hand
  public static List<List<Hand>> traces = new List<List<Hand>>();
  // Saved user movements
  public static List<Hand> userHands = new List<Hand>();
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