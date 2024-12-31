using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//! Shows information about the progress, current gesture, and loading
public class UserInterface : MonoBehaviour
{
  [Tooltip("The canvas containing the help UI")]
  //! [Input] The canvas containing the help UI
  [SerializeField] private GameObject helpCanvas;

  //! The console canvas
  [HideInInspector] public GameObject canvas;
  //! Progress slider
  private Slider progress;
  //! Gesture code text
  private TMP_Text gesture;
  //! Debug message text
  private TMP_Text message;
  //! Loading animation object
  private GameObject loader;

  void Start()
  {
    if (helpCanvas == null) {
      Debug.LogWarning("Help canvas is not set");
    } else {
      helpCanvas.SetActive(false);
    }
  }

  //! Populate info canvas with ui components
  public void SetCanvas(GameObject canvas) {
    this.canvas = canvas;

    // Make sure all elements are there
    if (canvas == null) {
      Debug.LogError("UI canvas is not set and no UI elements will appear.");
    } else {
      progress = canvas.transform.Find("Progress").GetComponent<Slider>();
      gesture = canvas.transform.Find("Gesture").GetComponent<TMP_Text>();
      message = canvas.transform.Find("Update").GetComponent<TMP_Text>();
      loader = canvas.transform.Find("Loader").gameObject;

      if (progress == null) {
        progress = canvas.transform.Find("Slider").GetComponent<Slider>();
        if (progress == null) {
          Debug.LogError("Progress bar UI not set.");
        }
      }
      if (gesture == null) {
        Debug.LogError("Gesture UI not set.");
      }
      if (message == null) {
        Debug.LogError("Text UI not set.");
      }
      if (loader == null) {
        Debug.LogError("Loading UI not set.");
      } else {
        loader.SetActive(false);
      }
    }
  }

  //! Update progress bar ui
  public void Progress(int value) {
    if (progress != null) {
      progress.value = (float)(value - Globals.gestures[0].Start) / (float)Globals.gestures[Globals.gestures.Count-1].End;
    }
  }

  //! Update gesture id
  public void Gesture(string value) {
    if (gesture != null && Globals.module != 0) {
      gesture.text = value;
    }
  }

  //! Update text message ui
  public void Message(string value) {
    if (message != null) {
      message.text = value;
    }
  }

  //! Start the loading animation
  public void StartLoading() {
    loader.SetActive(true);
    loader.GetComponent<Animator>().Play("Loading");
  }

  //! Stop the loading animation
  public void StopLoading() {
    loader.SetActive(false);
    loader.GetComponent<Animator>().StopPlayback();
  }

  //! Toggle the help screen
  public void HelpScreen() {
    helpCanvas.SetActive(!helpCanvas.activeSelf);
  }
}
