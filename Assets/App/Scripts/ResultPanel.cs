using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using NCMB;
using TMPro;
using System.Runtime.InteropServices;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] PlaySceneManager playSceneManager;
    [SerializeField] ScoreAttackManager scoreAttackManager;

    [SerializeField] GameObject parent;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] InputField playerName;
    [SerializeField] Transform scoresParent;
    [SerializeField] RectTransform scorePanel;
    [SerializeField] Image youtubeButtonImage;
    [SerializeField] List<YoutubeURLandImage> youtubeList;

    [System.Serializable]
    struct YoutubeURLandImage
    {
        public Sprite sprite;
        public string url;
    }

    string ncmbClass = "Scores";
    string youtubeURL = "";

    void Start()
    {
        playSceneManager.GameStartStream.Subscribe(_ =>
        {
            Init();
        });

        playSceneManager.GameEndStream.Subscribe(_ =>
        {
            scoreText.text = scoreAttackManager.CurrentScore.Value.ToString();
            ReloadRanking();

            var youtubeCh = youtubeList[Random.Range(0, youtubeList.Count)];
            youtubeButtonImage.sprite = youtubeCh.sprite;
            youtubeURL = youtubeCh.url;

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

    void ReloadRanking()
    {
        foreach (Transform child in scoresParent)
        {
            Destroy(child.gameObject);
        }
        var query = new NCMBQuery<NCMBObject>(ncmbClass);
        query.Limit = 50;
        query.OrderByDescending("score");
        query.FindAsync((List<NCMBObject> scores, NCMBException e) =>
        {
            if (e != null)
            {
                //検索失敗時の処理
                return;
            }

            foreach (var score in scores)
            {
                var scoreObj = Instantiate(scorePanel) as RectTransform;
                scoreObj.SetParent(scoresParent, false);
                var scoreText = scoreObj.GetComponentInChildren<Text>();
                scoreText.text = score["name"].ToString() + ": " + score["score"].ToString();
            }
        });
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
        scores["score"] = scoreAttackManager.CurrentScore.Value;
        scores.SaveAsync();
        StartCoroutine(ReloadRankingAfter1minute());
    }

    IEnumerator ReloadRankingAfter1minute()
    {
        yield return new WaitForSeconds(1);
        ReloadRanking();
    }

    public void OnTweet()
    {
        parent.SetActive(false);
        naichilab.UnityRoomTweet.TweetWithImage("YOUR-GAMEID", "ツイートサンプルです。", "unityroom");
        parent.SetActive(true);
    }

    [DllImport("__Internal")] private static extern void OpenNewTab(string URL);
    public void OnYoutube()
    {
#if UNITY_EDITOR
        Application.OpenURL(youtubeURL);
#elif UNITY_WEBGL
        OpenNewTab(youtubeURL);
#else
        Application.OpenURL(youtubeURL);
#endif
    }
}
