using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

//! Controls the videos, lines, and audio
public class Controller : MonoBehaviour
{
  [SerializeField] private AudioSource audioFeedback;
  [SerializeField] private AudioClip successAudio;
  [SerializeField] private AudioClip failAudio;
  [SerializeField] private GameObject[] numbers;

  //! Line logic
  private LineLogic lineLogic;
  //! Export data
  private ExportData exportData;
  private bool act = false;



  void Awake() {
    // Create randomized list of trials left
    Globals.leftover = new List<int>();
    int i = 0;
    while (i < 12) {
      Globals.leftover.Add(i++);
    }
  }

  //! Start video player. Triggered by import data.
  public void StartTracing() {
    lineLogic = this.GetComponent<LineLogic>();
    lineLogic.StartLines();
    Globals.start = true;
    Globals.paused = false;
    UpdateVis();
    
    exportData = this.GetComponent<ExportData>();
    exportData.StartData();
    StartCoroutine(PlayMovement());
  }

  private IEnumerator PlayMovement() {
    if (true) {
      string debug = Globals.trial + ": ";
      if (Globals.vis[0] == 0) {
        debug += "In-place, ";
      } else {
        debug += "Offset, ";
      }
      if (Globals.vis[1] == 0) {
        debug += "Continuous, ";
      } else {
        debug += "Keyframe, ";
      }
      if (Globals.vis[2] == 0) {
        debug += "Unimanuel, ";
      } else if (Globals.vis[2] == 1) {
        debug += "Mirror Bimanuel, ";
      } else {
        debug += "Async Bimanuel, ";
      }
      debug += "Move " + Globals.move;
      Debug.Log(debug);
    }

    // Animate movement
    for (int frame = 0; frame < Globals.traces[Globals.move][0].Positions.Count; frame++) {
      lineLogic.UpdateLines(frame);
      yield return new WaitForSeconds(0.025f);
    }
    // Reset back to first frame for user to follow
    lineLogic.UpdateLines(0);
    Globals.paused = true;
  }

  void UpdateVis() {
    if (Globals.leftover.Count <= 0) { return; }

    // Randomly get next trial number
    int index = Random.Range(0, Globals.leftover.Count);
    Globals.trial = Globals.leftover[index];
    Globals.leftover.RemoveAt(index);
    Debug.Log("Controller: Next visualization " + Globals.trial);

    // Trials 0-5 are not offset
    // Trials 6-11 are offset
    if (Globals.trial < 6) {
      Globals.vis[0] = 0;
    } else {
      Globals.vis[0] = 1;
    }

    // Trials 0-2 and 6-8 are continuous animated
    // Trials 3-5 and 9-11 are keyframe animated
    if (Globals.trial < 3 || (Globals.trial >= 6 && Globals.trial < 9)) {
      Globals.vis[1] = 0;
    } else {
      Globals.vis[1] = 1;
    }

    // Trials 0,3,6,9 are unimanuel
    // Trials 1,4,7,10 are sync bimanuel
    // Trials 2,5,8,11 are async bimanuel
    if (Globals.trial % 3 == 0) {
      Globals.vis[2] = 0;
    }
    else if (Globals.trial % 3 == 1) {
      Globals.vis[2] = 1;
    } 
    else {
      Globals.vis[2] = 2;
    }

    lineLogic.UpdateVis();

    Globals.move = 0;
    Globals.moveAttempt = 0;
  }

  void Update() {
    if (Globals.paused) {
      if (Input.GetKeyDown("up")) {
        StartCoroutine(PlayMovement());
      }

      if (Input.GetKeyDown("right")) {
        Forward();
      }
      else if (Input.GetKeyDown("left")) {
        StartCoroutine(Rewind());
      }

      // Click Y to trigger a rewind
      if (OVRInput.GetDown(OVRInput.Button.Four)) {
        StartCoroutine(Rewind());
      }

      if (Input.GetKeyDown("l")) {
        act = !act;
        foreach (GameObject num in numbers) {
          num.SetActive(act);
        }
      }
    }
  }

  //! Move video to next gesture. Triggered by moving hands to correct positions or clicking right arrow key.
  public void Forward() {
    Debug.Log("Forward");
    audioFeedback.PlayOneShot(successAudio);
    lineLogic.ResetLines();
    exportData.SaveData();
    Globals.paused = false;

    if (Globals.leftover.Count <= 0) {
      // exportData.WriteData();
      Debug.Log("WE ARE DONE!!!!!!!!!!!");
      return;
    }

    if (Globals.move < 2) {
      Debug.Log("Controller: Next movement");
      Globals.move++;
    } else {
      UpdateVis();
    }

    StartCoroutine(PlayMovement());
  }

  //! Replay current gesture. Triggered by geting hand positions wrong or clicking left arrow key.
  public IEnumerator Rewind() {
    Debug.Log("Rewind");
    audioFeedback.PlayOneShot(failAudio);
    Globals.paused = false;
    Globals.moveAttempt++;
    lineLogic.ResetLines();
    Globals.userHands[0] = new Hand();
    Globals.userHands[1] = new Hand();
    yield return StartCoroutine(lineLogic.ReplayHands());

    StartCoroutine(PlayMovement());
  }
}
