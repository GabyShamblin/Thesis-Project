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
                  handTrack.correct.ToString() + " = " + hand.position.ToString() + "\n" +
                  handTrack.correctR.ToString() + " = " + hand.rotation.eulerAngles.ToString();
    }

    string Visual() {
      string debug = Globals.trial + ": ";
      if (Globals.vis[0] == 1) {
        debug += "Offset, ";
      } else {
        debug += "In-place, ";
      }
      if (Globals.vis[1] == 1) {
        debug += "Keyframe, ";
      } else {
        debug += "Continuous, ";
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
