using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScorePanel : MonoBehaviour
{
  [SerializeField] private TMP_Text title;
  [SerializeField] private TMP_Text date;
  [SerializeField] private TMP_Text score;

  //! Create and populate a score panel
  public void CreatePanel(string setTitle, string setDate, string setScore) {
    title.text = setTitle;
    date.text = setDate;
    score.text = setScore + "%";
  }
}
