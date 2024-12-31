using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XCharts.Runtime;

public class ScoreChart : MonoBehaviour
{
  [SerializeField] private ImportData importData;
  [SerializeField] private Transform panelParent;
  [SerializeField] private GameObject panelPrefab;
  [SerializeField] private LineChart lineChart;
  private GameObject[] scoreInfoPanels;

  //! Display and fill the score chart
  public void DisplayScore() {
    gameObject.SetActive(true);

    // Grab the info from saved data
    string[] titles = PlayerPrefs.GetString("scoreTitles").Split(", ");
    string[] dates = PlayerPrefs.GetString("scoreDates").Split(", ");
    string[] scores = PlayerPrefs.GetString("scoreList").Split(", ");

    // Set up
    int n = titles.Length;
    scoreInfoPanels = new GameObject[n];
    lineChart.EnsureChartComponent<Tooltip>().show = false;
    lineChart.EnsureChartComponent<Legend>().show = false;

    // Set up y axis
    var yAxis = lineChart.EnsureChartComponent<YAxis>();
    yAxis.type = Axis.AxisType.Value;

    // Set up x axis
    var xAxis = lineChart.EnsureChartComponent<XAxis>();
    xAxis.type = Axis.AxisType.Category;
    xAxis.splitNumber = n;
    xAxis.boundaryGap = true;

    // Reset chart and add line data
    lineChart.RemoveData();
    lineChart.AddSerie<Line>();
    for (int i = 0; i < n; i++) {
      lineChart.AddXAxisData(dates[i].Substring(0, 5));
      lineChart.AddData(0, float.Parse(scores[i]));
    }
    // Display the newest scores first
    for (int i = n-1; i >= 0; i--) {
      scoreInfoPanels[i] = Instantiate(panelPrefab, panelParent);
      scoreInfoPanels[i].GetComponent<ScorePanel>().CreatePanel(titles[i], dates[i], scores[i]);
    }
  }
}
