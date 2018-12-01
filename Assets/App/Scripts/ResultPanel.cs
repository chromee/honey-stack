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
    [SerializeField] GameObject scoreForm;
    [SerializeField] JapaneseInputField playerName;
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

            StartCoroutine(ScreenCapAndShowParent());
        });

        playSceneManager.GameRestartStream.Subscribe(_ =>
        {
            Init();
        });
    }

    IEnumerator ScreenCapAndShowParent()
    {
        yield return new WaitForEndOfFrame();
        var tex = ScreenCapture.CaptureScreenshotAsTexture();
        naichilab.Scripts.Internal.GyazoUploader.jpgBytes = tex.EncodeToJPG();

        scoreForm.SetActive(true);
        parent.SetActive(true);
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
        scoreForm.SetActive(false);
        ReloadRanking();
    }

    string[] tags = new string[] { "ハニスト", "HoneyStack" };
    public void OnTweet()
    {
        naichilab.UnityRoomTweet.TweetWithImage("YOUR-GAMEID", $"{scoreAttackManager.CurrentScore.Value}ハニストつみました", tags);
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
