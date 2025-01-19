using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//! The logic for the individual hands
/*!
  Every frame, checks for the distance and rotation difference. If it falls under the set allowance, move this hand to the next frame and update icon line accordingly.
*/
public class HandTrack : MonoBehaviour
{
  [Tooltip("Which hand is this (0 = left, 1 = right)")]
  [Range(0, 3)]
  //! Which hand the script is currently attatched to (0 = left, 1 = right)
  public int handIndex = 0;

  //! [Input] Respective icon line script
  [Tooltip("The respective icon line draw script")]
  public LineDraw iconLine;

  //! [Input] The UI selection line
  [Tooltip("The UI selection line")]
  public GameObject UILine;

  public ExportData exportData;

  //! Current frame
  [HideInInspector] public int currFrame = 0;

  private GhostHands ghostHands;
  private bool first = true;
  private float timer = 0f;

  void Start()
  {
    ghostHands = this.GetComponent<GhostHands>();
    
    if (iconLine == null) {
      Debug.LogError("Icon line is not set for hand " + handIndex);
    }
  }

  void Update()
  {
    if (Globals.start) {
      UILine.SetActive(false);
    }

    // Check if initial load is done and video is paused
    if (Globals.start && Globals.paused) {
      // Set current frame to start if not correct
      if (currFrame < Globals.traces[Globals.move][handIndex].Positions.Count) {
        currFrame = 0;
        if (Globals.vis[1]) {
          iconLine.UpdateLine(currFrame);
        }
      }
      else { return; }
      timer += Time.deltaTime;

      // +/- is to offset to where the line center is so the line point and hand are on the same coordinate system
      Vector3 original = Globals.traces[Globals.move][handIndex].Positions[currFrame];
      Vector3 check = new Vector3(
        original.x,
        original.y,
        original.z + Globals.ghostOffset
      );

      // Check distance between hand and correct position
      float dist;
      if (Globals.vis[0]) {
        dist = Vector3.Distance(check, ghostHands.sceneGhost.transform.localPosition);
      } else {
        dist = Vector3.Distance(check, this.transform.localPosition);
      }
      
      if (dist <= Globals.distAllow) {

        // Use euler angles to make comparison easier
        float angleDist;
        if (Globals.vis[0]) {
          angleDist = Vector3.Distance(ghostHands.sceneGhost.transform.eulerAngles, Globals.userHands[handIndex].Rotations[currFrame]);
        } else {
          angleDist = Vector3.Distance(this.transform.eulerAngles, Globals.userHands[handIndex].Rotations[currFrame]);
        }

        if (angleDist <= Globals.angleAllow) {
          // Save hand position & rotation for replay
          if (Globals.vis[0]) {
            Globals.userHands[handIndex].Positions.Add(ghostHands.sceneGhost.transform.localPosition);
            Globals.userHands[handIndex].Rotations.Add(ghostHands.sceneGhost.transform.eulerAngles);
          } else {
            Globals.userHands[handIndex].Positions.Add(this.transform.localPosition);
            Globals.userHands[handIndex].Rotations.Add(this.transform.eulerAngles);
          }

          // Next frame
          Globals.userHands[handIndex].Timestamps[currFrame] = timer;
          
          // Turn off current icon (if multiple traces)
          if (!Globals.vis[1]) {
            iconLine.UpdateLine(currFrame, false);
          }
          
          currFrame++;
          
          // Show icon for next frame
          if (Globals.vis[1]) {
            iconLine.UpdateLine(currFrame);
          }
        }
      }
    }
  }

  //! Move hand back to the start of the current gesture. Triggered by rewind.
  public void ResetHand() {
    currFrame = 0;
    Globals.currFrame = 0;
  }
}
