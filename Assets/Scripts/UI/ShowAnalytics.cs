using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowAnalytics : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI compltetionTime; 
    [SerializeField] TextMeshProUGUI leftActions; 
    [SerializeField] TextMeshProUGUI rightActions; 
    [SerializeField] TextMeshProUGUI calories; 
    [SerializeField] TextMeshProUGUI score;

    [SerializeField] TextMeshProUGUI DummyData1;
    [SerializeField] TextMeshProUGUI DummyData2;
    [SerializeField] TextMeshProUGUI DummyData3;
    [SerializeField] TextMeshProUGUI DummyData4;
    [SerializeField] TextMeshProUGUI DummyData5;



    public void OnEnable()
    {
        GameData gameData = SessionEndController.currentSession;

        if (gameData == null) return;

        compltetionTime.text = gameData.time;
        leftActions.text = gameData.leftActions.ToString();
        rightActions.text = gameData.rightActions.ToString();
        calories.text = gameData.calories.ToString();
        score.text = gameData.finalScore.ToString();

        if (DummyData1) DummyData1.text = "";
        if (DummyData2) DummyData2.text = "";
        if (DummyData3) DummyData3.text = "";
        if (DummyData4) DummyData4.text = "";
        if (DummyData5) DummyData5.text = "";
    }


}
