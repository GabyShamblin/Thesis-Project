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
	public HandTrack[] handTracks;
	private Stats posStats = new Stats();
	private Stats rotStats = new Stats();

	//! Write user data to a text file
	public void WriteData(string docName = "output.txt") {
		Debug.Log("Writing data");
		string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

		using (StreamWriter writer = new StreamWriter(Path.Combine(path, docName))) {
			int mostFrames = 
				Globals.userHands[0].Positions.Count > Globals.userHands[1].Positions.Count ? 
				Globals.userHands[0].Positions.Count : Globals.userHands[1].Positions.Count;

			try {
				string line;
				// Note: User position/rotation needs to be offset by the start of the first gesture because nothing before that is shown to the user or captured
				for (int i = 0; i < mostFrames; i++) {
					line = "";
					for (int j = 0; j < 2; j++) {
						if (i >= Globals.userHands[j].Positions.Count) {
							line += "0.00 0.00 ";
						} else {
							// line += GetPosAccuracy(j, i, handTracks[j].distAllowance) + " " + GetRotAccuracy(j, i, handTracks[j].angleAllowance) + " ";
							line += Globals.userHands[j].Positions[i] + " " + Globals.userHands[j].Rotations[i] + " ";
						}
					}
					line += Globals.userHands[0].Timestamps[i];
					writer.WriteLine(line);
				}

				posStats.total += Globals.userHands[0].Positions.Count + Globals.userHands[1].Positions.Count;
				rotStats.total += Globals.userHands[0].Positions.Count + Globals.userHands[1].Positions.Count;
				line = posStats.ToString() + " " + rotStats.ToString();
				writer.WriteLine(line);
			} catch (Exception e) {
				writer.WriteLine("Error: " + e.ToString());
			}
			// string line = "";
			// for (int j = 0; j < Globals.traces[Globals.moveSet][Globals.contSet].Count; j++) {
			// 	line += Globals.traces[Globals.moveSet][Globals.contSet][j].ToString() + "    ";
			// }

			// writer.WriteLine(line);
		}

		float score = (posStats.Average() + rotStats.Average()) / 2;
		score = (float)Math.Round(score, 2);

		PlayerPrefs.DeleteAll();

		string key = "scoreList";
		if (PlayerPrefs.HasKey(key)) {
			PlayerPrefs.SetString(key, PlayerPrefs.GetString(key) + ", " + score.ToString());
		} else {
			// PlayerPrefs.SetString(key, score.ToString());
			PlayerPrefs.SetString(key, "67, 79, 81, 55, 91, 98");
		}
		PlayerPrefs.Save();

		key = "scoreDate";
		if (PlayerPrefs.HasKey(key)) {
			PlayerPrefs.SetString(key, PlayerPrefs.GetString(key) + ", " + DateTime.Now.ToString("MM/dd/yy"));
		} else {
			// PlayerPrefs.SetString(key, score.ToString());
			PlayerPrefs.SetString(key, "10/24/24, 11/01/24, 11/12/24, 11/23/24, 12/03/24, 01/15/25");
		}
		PlayerPrefs.Save();

		key = "scoreTitle";
		if (PlayerPrefs.HasKey(key)) {
			PlayerPrefs.SetString(key, PlayerPrefs.GetString(key) + ", " + gameObject.GetComponent<ImportData>().title);
		} else {
			// PlayerPrefs.SetString(key, score.ToString());
			PlayerPrefs.SetString(key, "Knot Tying E003, Knot Tying D005, Knot Tying E003, Needle Passing F001, Suturing E002, Knot Tying E003");
		}
		PlayerPrefs.Save();

		Debug.Log("Score: " + score);
		Debug.Log("Done writing");
		Debug.Log("Score list: " + PlayerPrefs.GetString(key));
		// chart.DisplayScore();
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
		return (perc/total) + " " + min + " " + max;
	}
	// float CalcDiff(Vector3 v1, Vector3 v2) {
	// 	Quaternion angleCheck = Quaternion.Inverse(transform.rotation) * Globals.hands[handIndex].Info[currFrame].Rotation
	// 	return (Math.Abs(v1 - v2)) / ((v1 + v2) / 2);
	// }
}