using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    public bool clear = false;
    public Transform entriesContainer;
    public Transform entryTemplate;
    private List<LeaderboardEntryData> leaderboardEntryDataList = new List<LeaderboardEntryData>();
    private List<Transform> leaderboardEntryTransformList = new List<Transform>();
    public float templateHeight = 30f;


    private void OnEnable()
    {
        if(SessionEndController.currentSession == null)
        {
            Debug.Log("Session Data is not Recevied Yet");
            return;
        }



        entryTemplate.gameObject.SetActive(false);
        

        AddNewEntry("Badri", SessionEndController.currentSession.time.ToString(), SessionEndController.currentSession.accuracy, SessionEndController.currentSession.calories, SessionEndController.currentSession.finalScore);



        string jsonString = PlayerPrefs.GetString("ZleaderboardEntries", "");

        LeaderboardEntries leaderboard = (jsonString != "") ? JsonUtility.FromJson<LeaderboardEntries>(jsonString) : null;
        if (leaderboard == null)
        {
            leaderboard = new LeaderboardEntries();
            leaderboard.leaderboardEntryDataList = new List<LeaderboardEntryData>();
        }

        leaderboardEntryDataList = leaderboard.leaderboardEntryDataList;

        //sorting based on score
        leaderboardEntryDataList.Sort((a, b) => b.score.CompareTo(a.score));

        foreach (LeaderboardEntryData entry in leaderboardEntryDataList)
        {
            CreateLeaderboardEntryTransform(entry, entriesContainer, leaderboardEntryTransformList);
        }

    }

    private void Update()
    {
        if (clear)
        {
            ClearLeaderboardData();
        }
    }

    private void AddNewEntry(string name, string time, float accuracy,float calories, float score)
    {
        //create entry
        LeaderboardEntryData entry = new LeaderboardEntryData { playerName = name, time = time, accuracy = accuracy, calories = calories, score = score };

        //load current entries data if not present fill empty string
        string jsonString = PlayerPrefs.GetString("ZleaderboardEntries", "");

        LeaderboardEntries entries;
        if (jsonString != null && jsonString != "")
        {
            entries = JsonUtility.FromJson<LeaderboardEntries>(jsonString);
        }
        else
        {
            entries = new LeaderboardEntries();
            entries.leaderboardEntryDataList = new List<LeaderboardEntryData>();
        }

        //update and save
        entries.leaderboardEntryDataList.Add(entry);
        string json = JsonUtility.ToJson(entries);
        PlayerPrefs.SetString("ZleaderboardEntries", json);
        PlayerPrefs.Save();

    }

    private void CreateLeaderboardEntryTransform(LeaderboardEntryData leaderboardEntryData, Transform container, List<Transform> transformList)
    {

        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -transformList.Count * templateHeight);

        entryRectTransform.gameObject.SetActive(true);


        int rank = transformList.Count + 1;
        string rankString;

        switch (rank)
        {
            case 1: rankString = "1st"; break;
            case 2: rankString = "2nd"; break;
            case 3: rankString = "3rd"; break;
            default: rankString = rank + "th"; break;
        }
        entryTransform.Find("Rank").GetComponent<TextMeshProUGUI>().text = rankString;

        string name = leaderboardEntryData.playerName;
        entryTransform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;

        string time = leaderboardEntryData.time;
        entryTransform.Find("Time").GetComponent<TextMeshProUGUI>().text = time;

        float accuracy = leaderboardEntryData.accuracy;
        entryTransform.Find("Accuracy").GetComponent<TextMeshProUGUI>().text = accuracy.ToString() + "%";

        float calories = leaderboardEntryData.calories;
        entryTransform.Find("Calories").GetComponent<TextMeshProUGUI>().text = calories.ToString();

        float score = leaderboardEntryData.score;
        entryTransform.Find("Score").GetComponent<TextMeshProUGUI>().text = score.ToString();





        transformList.Add(entryTransform);
    }

    public void ClearLeaderboardData()
    {
        PlayerPrefs.DeleteKey("ZleaderboardEntries");
        PlayerPrefs.Save();
        Debug.Log("Leaderboard data cleared");
    }

    //created this separate class to make our list of entries to an obj, so we convert them into string using jsonUtility and save in playerPrefs
    public class LeaderboardEntries
    {
        public List<LeaderboardEntryData> leaderboardEntryDataList;
    }


    [System.Serializable] //added this to make it able to pass in functions, in my case setString
    public class LeaderboardEntryData
    {
        public string playerName;
        public string time;
        public float accuracy;
        public float calories;
        public float score;
    }
}
