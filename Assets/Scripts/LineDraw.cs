using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

//! The logic for the individual icon lines
/*!
  Gameobjects with interchangable icons that show the user where to put their hands.
*/
public class LineDraw : MonoBehaviour
{
  [Tooltip("Prefab containing the 3D icon for hand placement")]
  public GameObject iconPrefab;

  [SerializeField] private GhostHands ghostHand;

  [Tooltip("Parents for the three movement types")]
  public GameObject[] iconParents;

  [Tooltip("Which hand is this (0 = left, 1 = right)")]
  [Range(0, 1)]
  public int lineNum = 0;

  [Tooltip("How much to offset line from the origin")]
  [HideInInspector] public Vector3 offset;

  //! [Input] The debug text for what frame the line is on
  public TMP_Text frameText;

  //! The previous frame activated
  private int prevFrame = 0;
  private int skipFrames = 8;
  private bool offsetMode = false;
  //! The current hand position
  private Vector3 currPos;
  //! The current hand rotation
  private Quaternion currRot;
  //! The icons to use for the hands
  private GameObject[][] icons;

  //! The original hand position
  private Vector3 ogHandPos;

  void Start()
  {
    if (iconPrefab == null) {
      Debug.LogError("Hand " + lineNum + " icon not set.");
      Application.Quit();
    }
    offset = this.transform.position;
  }

  //! Set up icon objects. Triggered by import data.
  /*!
    Create and position all icons for the line tracing
  */
  public void CreateLine() {
    icons = new GameObject[3][];

    for (int move = 0; move < 3; move++) {
      icons[move] = new GameObject[Globals.traces[move][lineNum].Positions.Count];
      // Create all icons and set them not active
      // Create every other icon and gesture beginning and end for downsampling
      for (int i = 0; i < Globals.traces[move][lineNum].Positions.Count; i++) {
        currPos = Globals.traces[move][lineNum].Positions[i];
        currRot = Globals.traces[move][lineNum].Rotations[i];
        // if (lineNum == 1 && move == 2) { Debug.Log("Rotation check " + i + ": " + currRot); }

        // Create point and set to correct position and rotation
        icons[move][i] = Instantiate(iconPrefab, iconParents[move].transform);

        icons[move][i].name += i;
        icons[move][i].transform.localPosition = currPos;
        icons[move][i].transform.localRotation = currRot;
        icons[move][i].SetActive(false);
      }
    }

    // Icons are set based on global (0,0,0), so move the parent to proper hand position afterwards
    if (Globals.vis[0] == 1) {
      transform.localPosition = new Vector3(
        offset.x, 
        offset.y, 
        offset.z + Globals.ghostOffset
      );
    }

    UpdateVis();
  }

  //! Set icons active are line is drawn. Triggered by hand track.
  public void UpdateLine(int frame, bool toggle = true) {
    if (frame < icons[Globals.move].Length && icons[Globals.move][frame] != null) {
      Debug.Log("Update frame " + frame);

      if (Globals.vis[1] == 1) {
        // If keyframe animation, turn off previous frame, then turn on current frame
        if (prevFrame < icons[Globals.move].Length && icons[Globals.move][prevFrame] != null) {
          icons[Globals.move][prevFrame].SetActive(false);
        }
        icons[Globals.move][frame].SetActive(true);
        prevFrame = frame;
      } else {
        // If continuous animation, only turn on every 5th frame to lessen confusion
        if (frame % skipFrames == 0 || frame == Globals.traces[Globals.move][lineNum].Positions.Count-1) {
          icons[Globals.move][frame].SetActive(toggle);
        }
      }
      
      if (frameText != null) {
        frameText.text = "" + frame;
      }

      // Percent of the way through the gesture for color changing
      float blend = (float)frame / (float)Globals.traces[Globals.move][lineNum].Positions.Count;
      IconColorChange render = icons[Globals.move][frame].GetComponent<IconColorChange>();
      render.UpdateColor(blend);
    }
  }

  //! Update the visualization of this line
  public void UpdateVis() {
    if (Globals.vis[0] == 1) {
      if (!offsetMode) {
        transform.position = transform.position + new Vector3(0,0,Globals.ghostOffset);
        offsetMode = true;
        ghostHand.CreateGhostHands();
      }
    } else {
      transform.position = transform.position - new Vector3(0,0,Globals.ghostOffset);
      offsetMode = false;
    }
    offset = transform.position;

    // Manuel = Right
    // Mirror = Left, Mirror Right
    // Async = Left, Right
    if (lineNum == 0) {
      if (Regex.IsMatch(this.gameObject.name, "mirror", RegexOptions.IgnoreCase)) {
        this.gameObject.SetActive(Globals.vis[2] == 1);
      } else {
        this.gameObject.SetActive(Globals.vis[2] == 1 || Globals.vis[2] == 2);
      }
    } else {
      this.gameObject.SetActive(Globals.vis[2] == 0 || Globals.vis[2] == 2);
    }
  }

  //! Reset line for next gesture. Triggered by rewind.
  public void ResetLine() {
    // Set all icons in gesture to not active
    for (int i = 0; i < Globals.traces[Globals.move][lineNum].Positions.Count; i++) {
      if (icons[Globals.move][i] != null) {
        // Turn off icon
        icons[Globals.move][i].SetActive(false);
      }
    }
    prevFrame = 0;
  }
}
