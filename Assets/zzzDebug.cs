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
      text.text = handTrack.correct.ToString() + " = " + hand.position.ToString() + "\n" +
                  handTrack.correctR.ToString() + " = " + hand.rotation.eulerAngles.ToString();
    }
}
