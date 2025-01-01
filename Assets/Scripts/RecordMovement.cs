using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class RecordMovement : MonoBehaviour
{
	[SerializeField] private GameObject LeftHand;
	[SerializeField] private GameObject RightHand;
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
		time += Time.deltaTime;
		line += 
			LeftHand.transform.position.x +"  "+ LeftHand.transform.position.y +"  "+ LeftHand.transform.position.z +"  "+ 
			LeftHand.transform.rotation.x +"  "+ LeftHand.transform.rotation.y +"  "+ LeftHand.transform.rotation.z +"  "+ 
			RightHand.transform.position.x +"  "+ RightHand.transform.position.y +"  "+ RightHand.transform.position.z +"  "+ 
			RightHand.transform.rotation.x +"  "+ RightHand.transform.rotation.y +"  "+ RightHand.transform.rotation.z +"  "+ 
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
