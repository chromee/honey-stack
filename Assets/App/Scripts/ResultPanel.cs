using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using NCMB;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] PlaySceneManager playSceneManager;
    [SerializeField] ScoreAttackManager scoreAttackManager;
    [SerializeField] GameObject parent;
    [SerializeField] InputField playerName;

    void Start()
    {

        playSceneManager.GameStartStream.Subscribe(_ =>
        {
            Init();
        });

        playSceneManager.GameEndStream.Subscribe(_ =>
        {
            parent.SetActive(true);
        });

        playSceneManager.GameRestartStream.Subscribe(_ =>
        {
            Init();
        });
    }

    void Init()
    {
        parent.SetActive(false);
    }

    public void OnRestart()
    {
        playSceneManager.GameRestartStream.OnNext(Unit.Default);
    }

    public void OnRanking()
    {
        var scores = new NCMBObject("Scores");
        var name = playerName.text == string.Empty ? "ななし" : playerName.text;
        Debug.Log(name);
        scores["name"] = name;
        scores["score"] = scoreAttackManager.CurrentScore.Value.ToString();
        scores.SaveAsync();
    }
}
