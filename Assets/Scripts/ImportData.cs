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
  //! The user interface script
  private UserInterface ui;
  //! The selected folder
  private string folder = "";
  [HideInInspector] public string title = "";

  void Start()
  {
    ui = GetComponent<UserInterface>();

    TextAsset txtData = (TextAsset)Resources.Load("input");
    string data = txtData.text;
    if (data != "") {
      ReadPositions(data);
    } else {
      Debug.Log("Data file not found");
    }
  }

  //! When file browser finishes, read through files in the folder
  // void OnSuccess( string[] filePaths ) {
  //   Debug.Log("Importing...");

  //   // Get all selected files
  //   for( int i = 0; i < filePaths.Length; i++ )
  //     Debug.Log( filePaths[i] );
  //   folder = filePaths[0];

  //   // Get files in selected folder
  //   string[] files = Directory.GetFiles(folder);

  //   // Init file name strings
  //   string text = "";
  //   string videoLeft = "";
  //   string videoRight = "";
  //   string textG = "";

  //   for( int i = 0; i < files.Length; i++ ) {
  //     // Match the txt files for kinematics and gestures
  //     if (Regex.IsMatch(files[i], @"(\.txt)$")) {
  //       if (i < files.Length/2) {
  //         text = files[i];
  //         title = Regex.Replace(files[i], "_|-", " ");
  //       } else {
  //         textG = files[i];
  //       }
  //     }
  //     // Match video files
  //     if (Regex.IsMatch(files[i], @"(\.avi)$")) {
  //       if (i < files.Length/2) {
  //         videoLeft = files[i];
  //       } else {
  //         videoRight = files[i];
  //       }
  //     }
  //   }

  //   // Testing();

  //   if (text == "") {
  //     Debug.LogWarning("Files not found");
  //     return;
  //   }
  //   ReadPositions(text);

  //   // ReadGestures(textG);

  //   // for (int i = 0; i < Globals.traces.Length; i++) {
  //   //   Globals.traces[currSet][i].Positions = new List<Vector3>(Globals.traces[currSet][i].Info.Count);
  //   //   Globals.traces[currSet][i].Rotations = new List<Quaternion>(Globals.traces[currSet][i].Info.Count);
  //   // }

  //   // Moved to start video 
  //   // GetComponent<LineLogic>().StartLines();
  // }

  //! Read and store kinematics data
  void ReadPositions(string filePath) {
    Debug.Log("Position file: " + filePath);
    StreamReader reader = new StreamReader(filePath);

    bool first = true;
    string line;
    while ((line = reader.ReadLine()) != null) {
      line = line.Trim();
      // Seperate line into array
      float[] nums = line.Split("  ").Select(str => float.Parse(str.Trim())).ToArray();

      int move = 0;
      int cont = 0;

      // For each arm
      int val = 0;
      for (int i = 0; i < Globals.traces.Count; i++) {
        // Grab each value
        Vector3 posL = new Vector3(nums[0], nums[1] , nums[2] );
        Vector3 rotL = new Vector3(nums[3], nums[4] , nums[5] );
        Vector3 posR = new Vector3(nums[6], nums[7] , nums[8] );
        Vector3 rotR = new Vector3(nums[9], nums[10], nums[11]);
        float time = nums[12];

        // if (first) {
        //   Debug.Log("Rotation: " + ToMatrix(matrixNums));
        //   Debug.Log("Rotation*Offset: " + (ToMatrix(matrixNums) * (i == 0 ? LROffset : RROffset)));
        //   Debug.Log("");
        //   first = false;
        // }

        // Put data into arm info
        Globals.traces[Globals.currSet][0].Positions.Add(posL);
        Globals.traces[Globals.currSet][0].Rotations.Add(rotL);
        Globals.traces[Globals.currSet][0].Timestamps.Add(time);
        Globals.traces[Globals.currSet][1].Positions.Add(posR);
        Globals.traces[Globals.currSet][1].Rotations.Add(rotR);
        Globals.traces[Globals.currSet][1].Timestamps.Add(time);
      }
    }

    reader.Close();
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