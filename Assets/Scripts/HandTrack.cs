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
        if (Globals.vis[1] == 1) {
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
      if (Globals.vis[0] == 1) {
        dist = Vector3.Distance(check, ghostHands.sceneGhost.transform.localPosition);
      } else {
        dist = Vector3.Distance(check, this.transform.localPosition);
      }
      
      if (dist <= Globals.distAllow) {

        Quaternion angleCheck;
        if (Globals.vis[0] == 1) {
          angleCheck = Quaternion.Inverse(ghostHands.sceneGhost.transform.rotation) * Globals.userHands[handIndex].Rotations[currFrame];
        } else {
          angleCheck = Quaternion.Inverse(this.transform.rotation) * Globals.userHands[handIndex].Rotations[currFrame];
        }
        // Use euler angles to make comparison easier
        Vector3 angleDist = angleCheck.eulerAngles;

        // Correct for angles that end up over 360
        if (angleDist.x > 180) { angleDist.x -= 360; }
        if (angleDist.y > 180) { angleDist.y -= 360; }
        if (angleDist.z > 180) { angleDist.z -= 360; }

        if (Math.Abs(angleDist.x) <= Globals.angleAllow && 
            Math.Abs(angleDist.y) <= Globals.angleAllow && 
            Math.Abs(angleDist.z) <= Globals.angleAllow) 
        {
          // Save hand position & rotation for replay
          // TODO: Make sure local rotation is correct and not just rotation
          if (Globals.vis[0] == 1) {
            Globals.userHands[handIndex].Positions.Add(ghostHands.sceneGhost.transform.localPosition);
            Globals.userHands[handIndex].Rotations.Add(ghostHands.sceneGhost.transform.localRotation);
          } else {
            Globals.userHands[handIndex].Positions.Add(this.transform.localPosition);
            Globals.userHands[handIndex].Rotations.Add(this.transform.localRotation);
          }

          // Next frame
          Globals.userHands[handIndex].Timestamps[currFrame] = timer;
          
          // Turn off current icon (if multiple traces)
          if (Globals.vis[1] == 0) {
            iconLine.UpdateLine(currFrame, false);
          }
          
          currFrame++;
          
          // Show icon for next frame
          if (Globals.vis[1] == 1) {
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
