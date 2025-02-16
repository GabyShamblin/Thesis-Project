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
  [Range(0, 1)]
  //! Which hand the script is currently attatched to (0 = left, 1 = right)
  public int handIndex = 0;

  //! [Input] Respective icon line script
  [Tooltip("The respective icon line draw script")]
  public LineDraw iconLine;

  // public LineDraw mirrorIconLine;

  public ExportData exportData;

  //! Current frame
  [HideInInspector] public int currFrame = 0;

  private GhostHands ghostHands;
  private float timer = 0f;

  [HideInInspector] public Vector3 correct;
  [HideInInspector] public Vector3 correctR;
  [HideInInspector] public float dist;

  void Start()
  {
    ghostHands = this.GetComponent<GhostHands>();
    
    if (iconLine == null) {
      Debug.LogError("Icon line is not set for hand " + handIndex);
    }
  }

  void Update() {
    // Check if initial load is done and video is paused
    if (Globals.start && Globals.paused) {
      // Set current frame to start if not correct
      if (currFrame < Globals.traces[Globals.move][handIndex].Positions.Count) {
        // currFrame = 0;
        if (Globals.vis[1] == 1) {
          iconLine.UpdateLine(currFrame);
        }
      }
      else { return; }
      timer += Time.deltaTime;

      // +/- is to offset to where the line center is so the line point and hand are on the same coordinate system
      Vector3 original;
      Vector3 check;
      if (Globals.vis[2] == 1 && handIndex == 1) {
        // If mirror bimanuel, correct flip on y axis
        original = Globals.traces[Globals.move][0].Positions[currFrame];
        check = new Vector3(
          (original.x * -1) + iconLine.offset.x,
          original.y + iconLine.offset.y,
          original.z + iconLine.offset.z
        );
      } else {
        original = Globals.traces[Globals.move][handIndex].Positions[currFrame];
        check = original + iconLine.offset;
      }
      correct = check;

      // Check distance between hand and correct position (either this hand or ghost)
      if (Globals.vis[0] == 0) {
        dist = Vector3.Distance(check, this.transform.position);
      } else {
        dist = Vector3.Distance(check, ghostHands.sceneGhost.transform.position);
      }
      
      if (dist <= Globals.distAllow) {
        Quaternion angleCheck;
        if (Globals.vis[0] == 0) {
          angleCheck = Quaternion.Inverse(this.transform.rotation) * Globals.traces[Globals.move][handIndex].Rotations[currFrame];
        } else {
          angleCheck = Quaternion.Inverse(ghostHands.sceneGhost.transform.rotation) * Globals.traces[Globals.move][handIndex].Rotations[currFrame];
        }
        // Use euler angles to make comparison easier
        Vector3 angleDist = angleCheck.eulerAngles;
        correctR = angleDist;

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
          if (Globals.vis[0] == 0) {
            Globals.userHands[handIndex].Positions.Add(this.transform.localPosition);
            Globals.userHands[handIndex].Rotations.Add(this.transform.localRotation);
            Globals.userHands[handIndex].Timestamps.Add(timer);
          } else {
            Globals.userHands[handIndex].Positions.Add(ghostHands.sceneGhost.transform.position);
            Globals.userHands[handIndex].Rotations.Add(ghostHands.sceneGhost.transform.rotation);
            Globals.userHands[handIndex].Timestamps.Add(timer);
          }

          
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
