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

  void Update()
  {
		// While holding A
    if (OVRInput.Get(OVRInput.Button.One)) {
			Recording();
		}
		else if (OVRInput.Get(OVRInput.Button.Two)) {
			Saving();
		} else {
			time = 0.0f;
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
		string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

		using (StreamWriter writer = new StreamWriter(Path.Combine(path, "recording.txt"))) {
			writer.WriteLine(line);
		}
		Debug.Log("Recording saved");
	}
}
