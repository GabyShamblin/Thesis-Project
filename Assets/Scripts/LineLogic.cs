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

	//! Controller
  private Controller control;
	//! Hand logic
	private HandLogic handLogic;

  void Start()
  {
    if (lines == null || lines.Length == 0) {
      Debug.LogError("Lines are not set");
    }
		control = this.GetComponent<Controller>();
		handLogic = this.GetComponent<HandLogic>();
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

	//! Call reset line functions for all lines. Triggered by forward and rewind.
	public void ResetLines() {
		foreach (LineDraw line in lines) {
			line.ResetLine();
		}
		handLogic.ResetHands();
	}
}
