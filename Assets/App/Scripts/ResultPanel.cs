using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using NCMB;
using TMPro;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] PlaySceneManager playSceneManager;
    [SerializeField] ScoreAttackManager scoreAttackManager;
    [SerializeField] GameObject parent;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] InputField playerName;

    [SerializeField] Transform scoresParent;
    [SerializeField] RectTransform scorePanel;

    string ncmbClass = "Scores";

    void Start()
    {
        playSceneManager.GameStartStream.Subscribe(_ =>
        {
            Init();
        });

        playSceneManager.GameEndStream.Subscribe(_ =>
        {
            scoreText.text = scoreAttackManager.CurrentScore.Value.ToString();

            var query = new NCMBQuery<NCMBObject>(ncmbClass);
            query.OrderByDescending("score");
            query.Limit = 50;
            query.FindAsync((List<NCMBObject> scores, NCMBException e) =>
            {
                if (e != null)
                {
                    //検索失敗時の処理
                    return;
                }

                // とりあえずEVEはランキング非対応で
                return;

                foreach (var score in scores.OrderBy(v => v["score"]).Reverse())
                {
                    var scoreObj = Instantiate(scorePanel) as RectTransform;
                    scoreObj.SetParent(scoresParent, false);
                    var scoreText = scoreObj.GetComponentInChildren<Text>();
                    scoreText.text = score["score"].ToString();
                }
            });

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
        var scores = new NCMBObject(ncmbClass);
        var name = playerName.text == string.Empty ? "ななし" : playerName.text;
        scores["name"] = name;
        scores["score"] = scoreAttackManager.CurrentScore.Value.ToString();
        scores.SaveAsync();
    }

    public void OnTweet()
    {
        parent.SetActive(false);
        naichilab.UnityRoomTweet.TweetWithImage("YOUR-GAMEID", "ツイートサンプルです。", "unityroom");
        parent.SetActive(true);
    }
}
