using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//! The logic controlling the icon lines
public class LineLogic : MonoBehaviour
{
	[Tooltip("The icon lines for hand tracing")]
	//! [Input List] The icon lines to use
  public LineDraw[] lines;
	//! [Input List] The icons to use for replay
	public GameObject[] replayIcons;

	//! Controller
  private Controller control;
	//! Hand logic
	private HandLogic handLogic;
	//! List of the replay hands
	private GameObject[] replayHands;

  void Start()
  {
    if (lines == null || lines.Length == 0) {
      Debug.LogError("Lines are not set");
    }
		if (replayIcons == null || replayIcons.Length == 0) {
			Debug.LogError("Replay icons not set");
		}
		control = this.GetComponent<Controller>();
		handLogic = this.GetComponent<HandLogic>();

		replayHands = new GameObject[replayIcons.Length];
		for (int i = 0; i < replayIcons.Length; i++) {
			replayHands[i] = Instantiate(replayIcons[i]);
			replayHands[i].SetActive(false);
		}
  }

	//! Call create line functions for all lines. Triggered by import data start.
	public void StartLines() {
		foreach (LineDraw line in lines) {
			line.CreateLine();
		}
	}

	//! Call update lines functions for both hands. Triggered by hand logic.
	public void UpdateLines(int frame, bool toggle = true) {
		foreach (LineDraw line in lines) {
			line.UpdateLine(frame, toggle);
		}
	}

	//! Update the visualization of all lines
	public void UpdateVis() {
		foreach (LineDraw line in lines) {
			line.UpdateVis();
		}
	}

	//! Play ghost hand animation to show what the user did wrong. Triggered by rewind.
	public IEnumerator ReplayHands() {
		if (replayHands.Length > 0) {
			// If the hands have different counts, make sure to use the lowest
			int lessHands = 
				Globals.traces[Globals.move][0].Positions.Count < Globals.traces[Globals.move][1].Positions.Count ? 
				Globals.traces[Globals.move][0].Positions.Count : Globals.traces[Globals.move][1].Positions.Count;
			// Set all hands active
			foreach (GameObject hand in replayHands) { hand.SetActive(true); }

			// Go through all hand movements
			for (int i = 0; i < lessHands; i++) {
				for (int j = 0; j < 2; j++) {
					try {
						replayHands[j].transform.position = Globals.traces[Globals.move][Globals.currFrame].Positions[i];
						replayHands[j].transform.rotation = Globals.traces[Globals.move][Globals.currFrame].Rotations[i];
					} catch (Exception e) {
						Debug.LogError(e.ToString());
						if (j >= replayHands.Length) {
							Debug.Log("Replay hands problem");
						}
						else if (j >= Globals.traces.Count) {
							Debug.Log("Arms problem");
						}
						else if (i >= Globals.traces[Globals.move][j].Positions.Count) {
							Debug.Log("Hands position problem");
						}
						else if (i >= Globals.traces[Globals.move][j].Rotations.Count) {
							Debug.Log("Hands rotation problem");
						}
						continue;
					}
				}

				// Debug.Log("Ghost hand " + i + "/" + lessHands + ": " + replayHands[0].transform.position);
				yield return new WaitForSeconds(0.1f);
			}

			foreach (GameObject hand in replayHands) { hand.SetActive(false); }
		} else {
			Debug.LogWarning("Replay hands not set");
		}
  }

	//! Call reset line functions for all lines. Triggered by forward and rewind.
	public void ResetLines() {
		foreach (LineDraw line in lines) {
			line.ResetLine();
		}
		handLogic.ResetHands();
	}
}
