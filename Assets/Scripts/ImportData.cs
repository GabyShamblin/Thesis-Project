using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

//! Imports data from the jigsaw dataset
/*!
  Opens a file browser ui to select a folder with the needed files. Requires two video files (left and right eyes)(.avi) and two text files (machine data and gestures)(.txt). Save all processed data in global variables.
*/
public class ImportData : MonoBehaviour
{
  public TextAsset positionMain;
  public TextAsset mixed;
  public TextAsset rotationMain;
  [HideInInspector] public string title = "";

  void Awake() {
    Globals.traces = new List<List<Hand>>(3);
  }

  void Start()
  {
    if (positionMain != null || mixed != null || rotationMain != null) {
      Globals.traces.Add(new List<Hand>(2));
      Globals.traces.Add(new List<Hand>(2));
      Globals.traces.Add(new List<Hand>(2));
      ReadPositions(positionMain.ToString(), 0);
      ReadPositions(mixed.ToString(), 1);
      ReadPositions(rotationMain.ToString(), 2);

      this.GetComponent<Controller>().StartTracing();
    } else {
      Debug.LogError("Data file(s) not found");
    }
  }

  //! Read and store kinematics data
  void ReadPositions(string data, int movement) {
    string line;
    string[] lines = data.Split("\n");
    Globals.traces[movement].Add(new Hand());
    Globals.traces[movement].Add(new Hand());

    for (int i = 0; i < lines.Length; i++) {
      line = lines[i].Trim();
      if (line == "" || line == "\n") {
        break;
      }
      // Seperate line into array
      float[] nums = line.Split("  ").Select(str => float.Parse(str.Trim())).ToArray();

      // Grab each value
      Vector3 posL = new Vector3(nums[0], nums[1] , nums[2] );
      Vector3 rotL = new Vector3(nums[3], nums[4] , nums[5] );
      Vector3 posR = new Vector3(nums[6], nums[7] , nums[8] );
      Vector3 rotR = new Vector3(nums[9], nums[10], nums[11]);
      float time = nums[12];

      // Put data into arm info
      Globals.traces[movement][0].Positions.Add(posL);
      Globals.traces[movement][0].Rotations.Add(rotL);
      Globals.traces[movement][0].Timestamps.Add(time);
      Globals.traces[movement][1].Positions.Add(posR);
      Globals.traces[movement][1].Rotations.Add(rotR);
      Globals.traces[movement][1].Timestamps.Add(time);
    }
  }

  //! Turn 3x3 matrix into 4x4 matrix
  Quaternion ToMatrix(float[] nums) {
    if (nums.Length < 9) {
      return new Quaternion();
    }
    Matrix4x4 matrix = Matrix4x4.identity;
    matrix[0,0] = nums[0];
    matrix[0,1] = nums[1];
    matrix[0,2] = nums[2];
    matrix[1,0] = nums[3];
    matrix[1,1] = nums[4];
    matrix[1,2] = nums[5];
    matrix[2,0] = nums[6];
    matrix[2,1] = nums[7];
    matrix[2,2] = nums[8];

    return matrix.rotation;
  }
}