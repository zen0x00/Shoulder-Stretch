using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphController : MonoBehaviour
{

    public Sprite dotSprite;
    public Sprite lineSprite;
    private RectTransform graphCotainer;
    public GameObject xLabelTemplate;
    public GameObject yLabelTemplate;
    public GameObject yDashTemplate;

    public float numberOfYValues = 5f;


    private void Start()
    {
        graphCotainer = GetComponent<RectTransform>();
        //List<int> valueList = new List<int>() { 5, 38, 56, 45, 78, 32, 12, 38, 56, 23, 98, 54, 5, 38, 56, 45 };
        string jsonStrng = PlayerPrefs.GetString("ZleaderboardEntries");
        LeaderBoard.LeaderboardEntries entries = JsonUtility.FromJson<LeaderBoard.LeaderboardEntries>(jsonStrng);


        if(entries != null && entries.leaderboardEntryDataList != null && entries.leaderboardEntryDataList.Count != 0)
        {
            ShowGraph(entries.leaderboardEntryDataList);
        }
    }

    GameObject CreateDot(Vector2 anchoredPosition)
    {
        GameObject dot = new GameObject("dot", typeof(Image));
        dot.transform.SetParent(graphCotainer, false);
        dot.GetComponent<Image>().sprite = dotSprite;
        RectTransform dotRectTf = dot.GetComponent<RectTransform>();
        dotRectTf.anchoredPosition = anchoredPosition;
        dotRectTf.sizeDelta = new Vector2(30, 30);
        dotRectTf.anchorMin = new Vector2(0, 0);
        dotRectTf.anchorMax = new Vector2(0, 0);

        return dot;
    }


    void ShowGraph(List<LeaderBoard.LeaderboardEntryData> valueList)
    {
        float graphHeight = graphCotainer.sizeDelta.y;
        float yMaximum = 100f;
        foreach (var entry in valueList) if (entry.score > yMaximum) yMaximum = entry.score;
        yMaximum *= 1.1f;
        float xSize = graphCotainer.sizeDelta.x / (valueList.Count + 1);
        GameObject lastDotGameObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize * i;
            float yPosition = (valueList[i].score / yMaximum) * graphHeight;
            GameObject currentDotGameObject = CreateDot(new Vector2(xPosition, yPosition));

            if (lastDotGameObject != null)
            {
                DrawDotConnection(lastDotGameObject.GetComponent<RectTransform>().anchoredPosition, currentDotGameObject.GetComponent<RectTransform>().anchoredPosition);
            }

            lastDotGameObject = currentDotGameObject;


            GameObject xLabel = Instantiate(xLabelTemplate);
            xLabel.transform.SetParent(graphCotainer, false);

            RectTransform xLabelRectTf = xLabel.GetComponent<RectTransform>();
            xLabelRectTf.anchorMin = new Vector2(0, 0);
            xLabelRectTf.anchorMax = new Vector2(0, 0);

            xLabelRectTf.anchoredPosition = new Vector2(xPosition + 12f, -7f);
            xLabel.gameObject.GetComponent<TextMeshProUGUI>().text = i.ToString();
            xLabel.gameObject.SetActive(true);

        }
            
        for (int y = 0; y <= numberOfYValues; y++)
        {
            GameObject yLabel = Instantiate(yLabelTemplate);
            yLabel.transform.SetParent(graphCotainer, false);

            RectTransform yLabelRectTf = yLabel.GetComponent<RectTransform>();
            yLabelRectTf.anchorMin = new Vector2(0, 0);
            yLabelRectTf.anchorMax = new Vector2(0, 0);

            float normalizedValue = y / numberOfYValues;

            yLabelRectTf.anchoredPosition = new Vector2(-7, normalizedValue * graphHeight);

            //this condition is to stop the creation of 0 label in y axis
            if (y != 0)
            {
                yLabel.gameObject.GetComponent<TextMeshProUGUI>().text = Mathf.Round(normalizedValue * yMaximum).ToString();
                yLabel.gameObject.SetActive(true);
            }


        //this condition is to stop the creation of last yDash and first yDash

        if (!(y == 0 || y == numberOfYValues))
            {
                GameObject yDash = Instantiate(yDashTemplate);
                yDash.transform.SetParent(graphCotainer, false);

                RectTransform yDashRectTf = yDash.GetComponent<RectTransform>();
                yDash.SetActive(true);
                yDashRectTf.anchoredPosition = new Vector2(graphCotainer.sizeDelta.x / 2f, normalizedValue * graphHeight);
                yDash.GetComponent<Image>().color = new Color(255, 127, 127, 0.7f);
                yDashRectTf.sizeDelta = new Vector2(graphCotainer.sizeDelta.x, 3f);
                yDashRectTf.anchorMin = new Vector2(0, 0);
                yDashRectTf.anchorMax = new Vector2(0, 0);
        }
                


        }
    }


    void DrawDotConnection(Vector2 dotAPos, Vector2 dotBPos)
    {
        GameObject connectionLine = new GameObject("dotConnection", typeof(Image));
        connectionLine.transform.SetParent(graphCotainer, false);
        connectionLine.GetComponent<Image>().sprite = lineSprite;
        connectionLine.GetComponent<Image>().color = Color.red;

        Vector2 dir = (dotBPos - dotAPos).normalized;
        float distance = Vector2.Distance(dotAPos, dotBPos);

        RectTransform rectTf = connectionLine.GetComponent<RectTransform>();
        rectTf.anchorMin = new Vector2(0, 0);
        rectTf.anchorMax = new Vector2(0, 0);

        rectTf.sizeDelta = new Vector2(distance, 3.5f);
        rectTf.anchoredPosition = dotAPos + dir * distance * .5f;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rectTf.localEulerAngles = new Vector3(0, 0, angle);
    }

}
