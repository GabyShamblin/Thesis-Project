using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class zzzDebug : MonoBehaviour
{
    public Transform hand;
    private HandTrack handTrack;
    private TMP_Text text;

    void Start() {
      text = GetComponent<TMP_Text>();
      handTrack = hand.GetComponent<HandTrack>();
    }

    void Update() {
      text.text = Visual() + "\n" +
        handTrack.correct.ToString() + " = " + hand.position.ToString() + "(" + Math.Round(handTrack.dist, 3) + ")" + "\n" +
        handTrack.correctR.ToString() + " = " + hand.rotation.eulerAngles.ToString() + "(" + handTrack.angleDist + ")";
    }

    string Visual() {
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
      debug += "" + Globals.move;

      return debug;
    }
}
