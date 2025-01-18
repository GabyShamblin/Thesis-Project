using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//! The logic for the individual icon lines
/*!
  Gameobjects with interchangable icons that show the user where to put their hands.
*/
public class LineDraw : MonoBehaviour
{
  [Tooltip("Prefab containing the 3D icon for hand placement")]
  //! [Input] The prefab containing the 3D icon for hand placement
  public GameObject iconPrefab;

  [Tooltip("The point showing the current frame for hand tracing")]
  //! [Input] The point showing the current frame for hand tracing
  public GameObject point;

  [Tooltip("Whether the icon used is the quest controller")]
  //! [Input] Whether the icon used is the quest controller
  public bool controller = true;

  [Tooltip("Which hand is this (0 = left, 1 = right)")]
  [Range(0, 1)]
  //! [Input] Which line the script is currently attatched to (0 = left, 1 = right)
  public int lineNum = 0;

  [Tooltip("How much to offset line from the origin")]
  //! [Input] How much to offset this line from the origin
  public Vector3 offset = new Vector3(-0.45f, 1.5f, 0.5f);

  //! [Input] The debug text for what frame the line is on
  public TMP_Text frameText;

  //! How much to change the color
  private float blend = 0;
  //! The previous frame activated
  private int prevFrame = 0;
  //! The current hand position
  private Vector3 currPos;
  //! The current hand rotation
  private Vector3 currRot;
  //! The icons to use for the hands
  private GameObject[] icons;

  //! The original hand position
  private Vector3 ogHandPos;

  void Start()
  {
    if (iconPrefab == null) {
      Debug.LogError("Hand " + lineNum + " icon not set.");
      Application.Quit();
    }
    if (point == null) {
      point = this.transform.Find("Point").gameObject;
      if (point == null) {
        Debug.LogError("Point " + lineNum + " has not been set and cannot be found.");
      }
    }
    offset = this.transform.position;
  }

  void Update()
  {
    if (Globals.start && Globals.paused) {
      if (point != null) {
        // Set frame point
        Vector3 original = Globals.traces[Globals.move][lineNum].Positions[Globals.currFrame];
        // Debug.Log(lineNum + ": " + Globals.userHands[lineNum] + "/" + Globals.gestures[Globals.currGest].End);
        point.transform.position = new Vector3(
          original.x + offset.x, 
          original.y + offset.y, 
          original.z + offset.z + Globals.ghostOffset
        );
      }
    }
  }

  //! Set up icon objects. Triggered by import data.
  /*!
    Create and position all icons for the line tracing
  */
  public void CreateLine() {
    icons = new GameObject[Globals.traces[Globals.move][lineNum].Positions.Count];

    // Create all icons and set them not active
    // Create every other icon and gesture beginning and end for downsampling
    for (int i = 0; i < Globals.traces[Globals.move][lineNum].Positions.Count; i++) {
      if (i % 2 == 0 || i == Globals.traces[Globals.move][lineNum].Positions.Count) {
        currPos = Globals.traces[Globals.move][lineNum].Positions[i];
        currRot = Globals.traces[Globals.move][lineNum].Rotations[i];

        // Create point and set to correct position and rotation
        icons[i] = Instantiate(iconPrefab, transform, false);

        icons[i].name += i;
        icons[i].transform.localPosition = currPos;
        icons[i].transform.localEulerAngles = currRot;
        icons[i].SetActive(false);
      }
    }

    // Icons are set based on global (0,0,0), so move the parent to proper hand position afterwards
    transform.localPosition = new Vector3(
      offset.x, 
      offset.y, 
      offset.z + Globals.ghostOffset
    );
  }

  //! Set icons active are line is drawn. Triggered by hand track.
  public void UpdateLine(int frame, bool toggle = true) {
    // Percent of the way through the gesture for color changing
    blend = (float)frame / (float)Globals.traces[Globals.move][lineNum].Positions.Count;

    if (frame < icons.Length && icons[frame] != null) {
      // vis[1] = animation type
      if (Globals.vis[1] && icons[prevFrame] != null) {
        icons[prevFrame].SetActive(false); 
      }
      if (frameText != null) {
        frameText.text = "" + frame;
      }

      if (!Globals.vis[1]) {
        icons[frame].SetActive(toggle);
      } else {
        icons[frame].SetActive(true);
        prevFrame = frame;
      }
      
      // Controller has a weird structure, so make sure other icons have a renderer attatched to the parent
      if (controller) {
        Renderer render = icons[frame].transform.GetChild(2).GetComponent<Renderer>();
        render.material.SetFloat("_Blend", blend);
      } else {
        Renderer render = icons[frame].transform.GetComponent<Renderer>();
        render.material.SetFloat("_Blend", blend);
      }
    }
  }

  //! Set icons active are line is drawn. Triggered by hand logic.
  public void UpdateLine() {
    int startFrame = 0;
    int endFrame = Globals.traces[Globals.move][lineNum].Positions.Count + 1;

    // Make sure traces are in the gesture
    for (int frame = startFrame; frame < endFrame; frame++) {
      // Percent of the way through the gesture for color changing
      blend = (float)frame / (float)Globals.traces[Globals.move][lineNum].Positions.Count;

      if (frame < icons.Length && icons[frame] != null) {
        if (icons[prevFrame] != null) {
          icons[prevFrame].SetActive(false); 
        }
        if (frameText != null) {
          frameText.text = "" + frame;
        }
        icons[frame].SetActive(true);
        prevFrame = frame;
        
        // Controller has a weird structure, so make sure other icons have a renderer attatched to the parent
        if (controller) {
          Renderer render = icons[frame].transform.GetChild(2).GetComponent<Renderer>();
          render.material.SetFloat("_Blend", blend);
        } else {
          Renderer render = icons[frame].transform.GetComponent<Renderer>();
          render.material.SetFloat("_Blend", blend);
        }
      }
    }
  }

  //! Reset line for next gesture. Triggered by rewind.
  public void ResetLine() {
    point.transform.position = Vector3.zero;

    // Set all icons in gesture to not active
    for (int i = 0; i < Globals.traces[Globals.move][lineNum].Positions.Count; i++) {
      if (icons[i] != null) {
        // Turn off icon
        icons[i].SetActive(false);
      }
    }
  }

  // ----- For controller stuff -----

  //! Hold trigger to move line
  public void GrabHandPos(Transform hand) {
    ogHandPos = hand.position;
    Debug.Log("Original position: " + hand.position);
  }

  //! Hold trigger to move line. Triggered by the respective hand trigger.
  public void MoveLine(Transform hand) {
    if (Globals.start && Globals.paused) {
      Debug.Log("Offset: " + offset);
      Debug.Log("Hand pos: " + hand.position);
      Debug.Log("Original: " + ogHandPos);
      Debug.Log("New position: " + (offset + (hand.position - ogHandPos)));
      this.transform.localPosition = offset + (hand.position - ogHandPos);
      // Globals.userHands[lineNum].Offset = offset + (hand.position - ogHandPos);
    }
	}

  //! Reset the line position to the original offset
  public void ResetLinePos() {
    Debug.Log("Reset lines");
    this.transform.localPosition = offset;
	}

  //! Get this lines position
  public Vector3 GetCurrHandPos() {
    return this.transform.localPosition;
  }
  
  //! Set the line position to current position
  public void SetHandPos(Transform hand) {
    ogHandPos = hand.position;
  }
}
