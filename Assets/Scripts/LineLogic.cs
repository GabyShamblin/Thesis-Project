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

	//! Video controller
  private VideoController video;
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
	public void UpdateLines(int[] frames, bool toggle = true) {
		int len = lines.Length < frames.Length ? lines.Length : frames.Length;
		for (int i = 0; i < len; i++) {
			lines[i].UpdateLine(frames[i], toggle);
		}
	}

	//! Play ghost hand animation to show what the user did wrong. Triggered by rewind.
	public IEnumerator ReplayHands() {
		if (replayHands.Length == 0) { 
			Debug.LogError("Replay icons are not set");
			yield break;
		}

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
						replayHands[j].transform.eulerAngles = Globals.traces[Globals.move][Globals.currFrame].Rotations[i];
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
  }

	//! Save the user's movements. Triggered by forward.
	public void SaveHands(int currGest) {
		// Go through all hand movements
		string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		string docName = "output.txt";

		using (StreamWriter writer = File.AppendText(Path.Combine(path, docName))) {
			int pos = 0;
			string line = "";
			while (pos < Globals.userHands[0].Positions.Count) {
				for (int i = 0; i < 2; i++) {
					line += Globals.userHands[i].Positions[pos] + " " + Globals.userHands[i].Rotations[pos] + " ";
					pos++;
				}
				line += Globals.userHands[0].Timestamps[pos];
				writer.WriteLine(line);
			}
		}
	}
	// public void SaveHands(int currGest) {
	// 	// Go through all hand movements
	// 	for (int i = 0; i < Globals.hands.Count; i++) {
	// 		int pos = 0;
	// 		for (int j = 0; j < Globals.traces[i][currGest].Count; j++) {
	// 			try {
	// 				if (pos >= Globals.hands[i][currGest].Count) { break; }
	// 				// Globals.hands[i].UserInfo[j].Position = Globals.hands[i].Positions[pos];
	// 				// Globals.hands[i].UserInfo[j].Rotation = Globals.hands[i].Rotations[pos];
	// 				Globals.userHands[i].Add(new Hand(Globals.traces[i][currGest][pos].Position, Globals.traces[i][currGest][pos].Rotation));
	// 				pos++;
	// 			} catch (Exception e) {
	// 				Debug.Log(e.ToString());
	// 				Debug.Log("i: " + i + " < " + Globals.traces.Count);
	// 				Debug.Log("j: " + j + " < " + Globals.userHands[i].Count);
	// 				break;
	// 			}
	// 		}
	// 	}
	// }

	//! Call reset line functions for all lines. Triggered by forward and rewind.
	public void ResetLines() {
		for (int i = 0; i < lines.Length; i++) {
			lines[i].ResetLine();
			// Globals.hands[i].Positions = new List<Vector3>();
			// Globals.hands[i].Rotations = new List<Quaternion>();
		}
		handLogic.ResetHands();
	}
}
