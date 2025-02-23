using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

//! Exports data from the user
/*!
  Opens a file browser ui to select a folder with the needed files. Requires two video files (left and right eyes)(.avi) and two text files (machine data and gestures)(.txt). Save all processed data in global variables.
*/
public class ExportData : MonoBehaviour
{
	private Stats posStats = new Stats();
	private Stats rotStats = new Stats();
	private string path = "";
	private string folderName = "User Data";
	private HandTrack[] handTracks;

	private string docName;
	private string title;
	private int saveMove = 0;
	private int visOffset = 0;
	private int visAnim = 0;
	private int visHands = 0;

	void Start() {
		handTracks = GetComponent<HandLogic>().hands;
	}

	public void StartData() {
		string key = "userNum";
		if (PlayerPrefs.HasKey(key)) {
			PlayerPrefs.SetString(key, (Int32.Parse(PlayerPrefs.GetString(key)) + 1).ToString());
		} else {
			PlayerPrefs.SetString(key, "0");
		}
		PlayerPrefs.Save();

		docName = "user" + PlayerPrefs.GetString("userNum") + ".csv";
		path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + folderName;
	}

	public void SaveData() {
		Debug.Log("Saving data...");
		// WriteLine: thing, new block, another block ->
		// https://discussions.unity.com/t/write-data-from-list-to-csv-file/735424/3
		// https://discussions.unity.com/t/writing-position-data-to-a-csv-file/923012

		saveMove = Globals.move;
		visOffset = Globals.vis[0];
		visAnim = Globals.vis[1];
		visHands = Globals.vis[2];
		title = GenerateTitle();

		bool exists = true;
		// If the needed folder doesn't exist, create it
		if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
		if (!File.Exists(Path.Combine(path, docName))) { exists = false; }

		using (StreamWriter writer = new StreamWriter(Path.Combine(path, docName), true)) {
			if (!exists) {
				// If this is the first time seeing the file, create the column names
				writer.WriteLine(
					"trial, l_pos_x, l_pos_y, l_pos_z, l_pos_score, " +
					"l_rot_x, l_rot_y, l_rot_z, l_rot_w, l_rot_score, " +
					"r_pos_x, r_pos_y, r_pos_z, r_pos_score, " +
					"r_rot_x, r_rot_y, r_rot_z, r_rot_w, r_rot_score, " +
					"timestamp, score");
				Debug.Log("Title written");
			}

			int mostHand = 0;
			int mostFrames = 0;
			for (int i = 0; i < Globals.userHands.Count; i++) {
				if (Globals.userHands[i].Positions.Count > mostFrames) {
					mostFrames = Globals.userHands[i].Positions.Count;
					mostHand = i;
				}
			}
			float moveAccuracy = 0.0f;
			float posAccuracy = 0.0f;
			float rotAccuracy = 0.0f;
			bool single = false;

			string line;
			int j = 0;
			// Note: User position/rotation needs to be offset by the start of the first gesture because nothing before that is shown to the user or captured
			for (int i = 0; i < mostFrames; i++) {
				try {
					// Add the title to every row to make it easy to sort and analyze later
					line = title + ", ";
					for (j = 0; j < 2; j++) {
						if (i >= Globals.userHands[j].Positions.Count) {
							line += "0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, ";
							single = true;
						} else {
							Vector3 pos = Globals.userHands[j].Positions[i];
							Quaternion rot = Globals.userHands[j].Rotations[i];
							posAccuracy = GetPosAccuracy(j, i, Globals.distAllow);
							rotAccuracy = GetRotAccuracy(j, i, Globals.angleAllow);
							moveAccuracy += posAccuracy + rotAccuracy;

							line += pos.x + ", " + pos.y + ", " + pos.z + ", " + posAccuracy + ", ";
							line += rot.x + ", " + rot.y + ", " + rot.z + ", " + rot.w + ", " + rotAccuracy + ", ";
						}
					}

					// If only one hand has been calculated, the just take the average of the single hand pos and rot
					if (single) {
						line += Globals.userHands[mostHand].Timestamps[i] + ", " + (moveAccuracy/2);
					} else {
						line += Globals.userHands[mostHand].Timestamps[i] + ", " + (moveAccuracy/4);
					}
					moveAccuracy = 0.0f;
					single = false;

					writer.WriteLine(line);
				} catch (Exception e) {
					writer.WriteLine("Error line " + i + "/" + j + ": " + e.ToString());
					writer.WriteLine("Hand position count: " + Globals.userHands[j].Positions.Count);
				}
			}

			posStats.total += Globals.userHands[0].Positions.Count + Globals.userHands[1].Positions.Count;
			rotStats.total += Globals.userHands[0].Rotations.Count + Globals.userHands[1].Rotations.Count;
			line = posStats.ToString() + ", " + rotStats.ToString();
			Debug.Log("The end?");
			writer.WriteLine(line);
		}

		// Reset user data so it doesnt cause problems
		Globals.userHands[0] = new Hand();
		Globals.userHands[1] = new Hand();
	}

	// Get accuracy between correct and user position
	private float GetPosAccuracy(int hand, int frame, float allowance) {
		// Note: The position/rotation needs to be offset by the start of the first gesture because nothing before that is shown to the user or captured
		Vector3 original; Vector3 offset; Vector3 check;
		if (visHands == 1 && hand == 1) {
			// If mirror bimanuel, correct flip on y axis
			original = Globals.traces[saveMove][0].Positions[frame];
			offset = handTracks[hand].iconLines[1].offset;
			check = new Vector3(-original.x, original.y, original.z) + offset;
		} else {
			original = Globals.traces[saveMove][hand].Positions[frame];
			offset = handTracks[hand].iconLines[0].offset;
			check = original + offset;
		}

		// Calculate percent correct
		float perc = 1 - (Vector3.Distance(check, Globals.userHands[hand].Positions[frame]) / allowance);
		perc = (float)Math.Round(perc, 2);

		if (perc < 0) {
			Debug.Log("Frame: " + frame);
			Debug.Log("Original: " + original);
			Debug.Log("Check: " + check);
			Debug.Log("User: " + Globals.userHands[hand].Positions[frame]);
			Debug.Log("Distance: " + Vector3.Distance(check, Globals.userHands[hand].Positions[frame]));
			Debug.Log("Allowance: " + allowance);
			Debug.Log("Percent: " + perc);
			Debug.Log("--------------------------------------------------------");
		}

		// Save percent and check min and max
		posStats.perc += perc;
		if (perc > posStats.max) {
			posStats.max = perc;
		}
		if (perc < posStats.min) {
			posStats.min = perc;
		}

		return perc;
	}

	// Get accuracy between correct and user rotation
	private float GetRotAccuracy(int hand, int frame, float allowance) {
		// Note: The position/rotation needs to be offset by the start of the first gesture because nothing before that is shown to the user or captured
		Quaternion original = Globals.traces[saveMove][hand].Rotations[frame];
		Quaternion user = Globals.userHands[hand].Rotations[frame];

		Quaternion dist = Quaternion.Inverse(user) * original;
		Vector3 angleDist = dist.eulerAngles;

		if (angleDist.x > 180) { angleDist.x -= 360; }
		if (angleDist.y > 180) { angleDist.y -= 360; }
		if (angleDist.z > 180) { angleDist.z -= 360; }

		float perc = 1 - ((
			(Math.Abs(angleDist.x) / allowance) + 
			(Math.Abs(angleDist.y) / allowance) + 
			(Math.Abs(angleDist.z) / allowance)) / 3);
		perc = (float)Math.Round(perc, 2);

		rotStats.perc += perc;
		if (perc > rotStats.max) {
			rotStats.max = perc;
		}
		if (perc < rotStats.min) {
			rotStats.min = perc;
		}

		return perc;
	}

	private string GenerateTitle() {
		// string debug = Globals.trial + ": ";
		string debug = "";
    if (visOffset == 0) {
      debug += "place_";
    } else {
      debug += "offset_";
    }
    if (visAnim == 0) {
      debug += "cont_";
    } else {
      debug += "key_";
    }
    if (visHands == 0) {
      debug += "uni_";
    } else if (visHands == 1) {
      debug += "mir_";
    } else {
      debug += "asyc_";
    }
    debug += saveMove + "_" + Globals.moveAttempt;

		return debug;
	}
}

public class Stats
{
	public float perc { get; set; }
	public float total { get; set; }
	public float min { get; set; }
	public float max { get; set; }

	public Stats() {
		perc = 0.0f;
		total = 0.0f;
		min = 1.0f;
		max = 0.0f;
	}

	public float Average() {
		return perc/total;
	}

	public override string ToString() {
		return (perc/total) + ", " + min + ", " + max;
	}
}