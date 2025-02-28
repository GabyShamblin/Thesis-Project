using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;

//! Controls the videos, lines, and audio
public class Controller : MonoBehaviour
{
  [SerializeField] private AudioSource audioFeedback;
  [SerializeField] private AudioClip successAudio;
  [SerializeField] private AudioClip failAudio;
  [SerializeField] private GameObject[] numbers;
  [SerializeField] private GameObject pauseText;

  private LineLogic lineLogic;
  private ExportData exportData;
  private HandTrack[] hands;
  private bool act = false;
  private float timer = 0f;
  private bool showText = false;



  void Awake() {
    // Create randomized list of trials left
    Globals.leftover = new List<int>();
    int i = 0;
    while (i < 12) {
      Globals.leftover.Add(i++);
    }
  }

  void Start() {
    hands = GetComponent<HandLogic>().hands;
  }

  //! Create tracing lines. Triggered by import data.
  public void StartTracing() {
    Globals.start = true;
    Globals.paused = false;
    lineLogic = GetComponent<LineLogic>();
    lineLogic.StartLines();
    exportData = GetComponent<ExportData>();
    exportData.StartData();

    UpdateVis();
    // StartCoroutine(PlayMovement());
    Globals.waitAnimation = true;
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
      yield return new WaitForSeconds(0.03f);
    }

    // Reset back to first frame for user to follow
    lineLogic.UpdateLines(0);
    Globals.waiting = true;
    // Globals.paused = true;
  }

  void UpdateVis() {
    if (Globals.leftover.Count <= 0) { return; }

    // Randomly get next trial number
    int index = Random.Range(0, Globals.leftover.Count);
    Globals.trial = Globals.leftover[index];
    Globals.leftover.RemoveAt(index);
    Debug.Log("Controller: Next visualization");

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
  }

  void Update() {
    if (showText) {
      timer += Time.deltaTime;
      if (timer >= 5) {
        showText = false;
        pauseText.SetActive(false);
        timer = 0f;
      }
    }

    else if (Globals.waiting || Globals.waitAnimation) {
      bool both = true;
      for (int i = 0; i < hands.Length; i++) {
        hands[i].real.gameObject.SetActive(true);
        // If the hand is out of reach of the restart position, don't continue
        if (Vector3.Distance(hands[i].real.position, hands[i].position) > Globals.distAllow) {
          both = false;
        }
      }
      if (both) {
        if (Globals.waitAnimation) {
          Globals.waitAnimation = false;
          Globals.paused = false;
          Forward();
        }
        else if (Globals.waiting) {
          Globals.waiting = false;
          Globals.paused = true;
        }
      }
    }

    else if (Globals.start && Globals.paused) {
      for (int i = 0; i < hands.Length; i++) {
        hands[i].real.gameObject.SetActive(false);
      }

      if (Input.GetKeyDown("up")) {
        StartCoroutine(PlayMovement());
      }

      if (Input.GetKeyDown("right")) {
        WaitForward();
      }
      else if (Input.GetKeyDown("left")) {
        Rewind();
      }

      // Click Y to trigger a rewind
      if (OVRInput.GetDown(OVRInput.Button.Four)) {
        Globals.userResets++;
        Rewind();
      }

      if (Input.GetKeyDown("l")) {
        act = !act;
        foreach (GameObject num in numbers) {
          num.SetActive(act);
        }
      }
    }
  }

  public void WaitForward() {
    Debug.Log("Wait Forward");
    Globals.waitAnimation = true;
    Globals.paused = false;

    audioFeedback.PlayOneShot(successAudio);
    lineLogic.ResetLines();
    exportData.SaveData();

    if (Globals.leftover.Count <= 0 && Globals.move >= 2) {
      Debug.Log("WE ARE DONE!!!!!!!!!!!");
      pauseText.SetActive(true);
      pauseText.GetComponent<TMP_Text>().text = "Done";
      return;
    }

    if (Globals.move < 2) {
      Debug.Log("Controller: Next movement");
      Globals.move++;
      // Forward();
    } else {
      UpdateVis();
      showText = true;
      pauseText.SetActive(true);
    }
  }

  //! Move video to next gesture. Triggered by moving hands to correct positions or clicking right arrow key.
  private void Forward() {
    Debug.Log("Forward");
    Globals.moveAttempt = 0;
    Globals.userResets = 0;
    StartCoroutine(PlayMovement());
  }

  //! Replay current gesture. Triggered by geting hand positions wrong or clicking left arrow key.
  public void Rewind() {
    Debug.Log("Rewind");
    audioFeedback.PlayOneShot(failAudio);
    Globals.paused = false;
    Globals.moveAttempt++;
    lineLogic.ResetLines();
    Globals.userHands[0] = new Hand();
    Globals.userHands[1] = new Hand();

    StartCoroutine(PlayMovement());
  }
}
