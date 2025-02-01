using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class RecordMovement : MonoBehaviour
{
	[SerializeField] private Transform LeftHand;
	[SerializeField] private Transform RightHand;
	string line = "";
	float time = 0f;
	int fileCount = 0;

  void Update()
  {
		// While holding A, record movements of both hands
    if (OVRInput.Get(OVRInput.Button.One)) {
			Recording();
		} else {
			time = 0.0f;
		}

		// On clicking B, save movements to desktop
		if (OVRInput.GetDown(OVRInput.Button.Two)) {
			Saving();
		} 

		// On clicking X, reset recording
		if (OVRInput.GetDown(OVRInput.Button.Three)) {
			line = "";
		} 
  }
	
	public void Recording() {
		Debug.Log("Recording");
		Vector3 leftPos = LeftHand.position;
		Quaternion leftAngle = LeftHand.rotation;
		Vector3 rightPos = RightHand.position;
		Quaternion rightAngle = RightHand.rotation;
		
		time += Time.deltaTime;
		line += 
			leftPos.x		 +"  "+ leftPos.y    +"  "+ leftPos.z    +"  "+ 
			leftAngle.x  +"  "+ leftAngle.y  +"  "+ leftAngle.z  +"  "+ leftAngle.w  +"  "+ 
			rightPos.x 	 +"  "+ rightPos.y   +"  "+ rightPos.z 	 +"  "+ 
			rightAngle.x +"  "+ rightAngle.y +"  "+ rightAngle.z +"  "+ rightAngle.w +"  "+ 
			time.ToString() + "\n";
	}

	public void Saving() {
		Debug.Log("Saving");
		string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

		using (StreamWriter writer = new StreamWriter(Path.Combine(path, "recording" + fileCount + ".txt"))) {
			writer.WriteLine(line);
		}
		fileCount++;
		line = "";

		Debug.Log("Recording saved");
	}
}
