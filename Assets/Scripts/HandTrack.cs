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
  public LineDraw[] iconLines;

  public Transform real;

  [SerializeField] private GameObject controller;
  private bool meshVisible = true;

  // public Transform debugCont;

  public ExportData exportData;

  //! Current frame
  [HideInInspector] public int currFrame = 0;
  [HideInInspector] public Vector3 position;

  private GhostHands ghostHands;
  private float timer = 0f;

  [HideInInspector] public Vector3 correct;
  [HideInInspector] public Vector3 correctR;
  [HideInInspector] public float dist;
  [HideInInspector] public float angleDist;

  void Start()
  {
    ghostHands = this.GetComponent<GhostHands>();
    
    if (iconLines.Length == 0) {
      Debug.LogError("Icon line is not set for hand " + handIndex);
    }
  }

  void Update() {
    // Check if initial load is done and video is paused
    if (Globals.start && Globals.paused && !Globals.waiting) {
      // Set current frame to start if not correct
      if (currFrame > Globals.traces[Globals.move][handIndex].Positions.Count) {
        currFrame = 0;
        if (Globals.vis[1] == 1) {
          iconLines[0].UpdateLine(0);
        }
      }
      else if (currFrame == Globals.traces[Globals.move][handIndex].Positions.Count) {
        if (Globals.vis[2] == 1 && handIndex == 1) {
          iconLines[1].UpdateLine(currFrame);
        } else {
          iconLines[0].UpdateLine(currFrame);
        }
        Debug.Log("Finished hand " + handIndex);
        return;
      }
      timer += Time.deltaTime;

      // Calculate the correct and hand positions
      Vector3 check = new Vector3();
      if (Globals.vis[2] == 1 && handIndex == 1) {
        // If mirror bimanuel, correct flip on y axis
        Vector3 original = Globals.traces[Globals.move][0].Positions[currFrame];
        check = new Vector3(-original.x, original.y, original.z) + iconLines[1].offset;
      } else {
        check = Globals.traces[Globals.move][handIndex].Positions[currFrame] + iconLines[0].offset;
      }
      correct = check;
      position = this.transform.position;

      // Check distance between hand and correct position (either this hand or ghost)
      if (Globals.vis[0] == 0) {
        if (!meshVisible) {
          controller.SetActive(true);
          meshVisible = true;
        }
        dist = Vector3.Distance(check, this.transform.position);
      } else {
        if (meshVisible) {
          controller.SetActive(false);
          meshVisible = false;
        }
        dist = Vector3.Distance(check, ghostHands.sceneGhost.transform.position);
      }
      
      if (dist <= Globals.distAllow) {
        Quaternion correctAngle;
        if (Globals.vis[2] == 1 && handIndex == 1) {
          correctAngle = Globals.traces[Globals.move][0].Rotations[currFrame];
          Vector3 correctEuler = correctAngle.eulerAngles;
          correctAngle = Quaternion.Euler(correctEuler.x, -correctEuler.y, -correctEuler.z);
          // debugCont.position = correct;
          // debugCont.rotation = correctAngle;
        } else {
          correctAngle = Globals.traces[Globals.move][handIndex].Rotations[currFrame];
        }
        // Use euler angles to make comparison easier
        correctR = correctAngle.eulerAngles;

        if (Globals.vis[0] == 0) {
          angleDist = Quaternion.Angle(this.transform.rotation, correctAngle);
        } else {
          angleDist = Quaternion.Angle(ghostHands.sceneGhost.transform.rotation, correctAngle);
        }

        if (angleDist <= Globals.angleAllow) 
        {
          // Save hand position & rotation for replay
          if (Globals.vis[0] == 0) {
            Globals.userHands[handIndex].Positions.Add(this.transform.position);
            Globals.userHands[handIndex].Rotations.Add(this.transform.rotation);
            Globals.userHands[handIndex].Timestamps.Add(timer);
          } else {
            Globals.userHands[handIndex].Positions.Add(ghostHands.sceneGhost.transform.position);
            Globals.userHands[handIndex].Rotations.Add(ghostHands.sceneGhost.transform.rotation);
            Globals.userHands[handIndex].Timestamps.Add(timer);
          }
                   
          // Turn off current icon (if continuous mode)
          if (Globals.vis[1] == 0) {
            if (Globals.vis[2] == 1 && handIndex == 1) {
              iconLines[1].UpdateLine(currFrame, false);
            } else {
              iconLines[0].UpdateLine(currFrame, false);
            }
          }
          
          currFrame++;
          
          // Show icon for next frame (if keyframe mode)
          if (Globals.vis[1] == 1) {
            if (Globals.vis[2] == 1 && handIndex == 1) {
              iconLines[1].UpdateLine(currFrame);
            } else {
              iconLines[0].UpdateLine(currFrame);
            }
          }
        }
      }
    } else {
      // Reset timer at start of new movement
      timer = 0f;
      controller.SetActive(true);
      meshVisible = true;
    }
    position = transform.position;
  }

  //! Move hand back to the start of the current gesture. Triggered by rewind.
  public void ResetHand() {
    Globals.currFrame = 0;
    currFrame = 0;
    timer = 0;
  }
}
