using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlaySceneManager : MonoBehaviour
{
    public Subject<Unit> GameStartStream = new Subject<Unit>();
    public Subject<Unit> NextTurnStream = new Subject<Unit>();
    public Subject<Unit> GameEndStream = new Subject<Unit>();
    public Subject<Unit> GameRestartStream = new Subject<Unit>();

    [HideInInspector] public GameObject CurrentActiveFallen;
    public bool isWaiting;
    public bool isGameOver;

    [SerializeField] List<GameObject> Fallens;
    [SerializeField] Vector3 initPostion = new Vector3(0f, 8f, 0f);

    List<Fallen> stackedFallens;
    Vector3 firstPostion;
    RaycastHit2D hit;
    int fallenCount;

    [SerializeField] GameObject devPoint;

    void Start()
    {
        GameStartStream.Subscribe(_ =>
        {
            Init();
            NextTurnStream.OnNext(Unit.Default);
            // Debug.Log("Game Start");
        });

        NextTurnStream.Where(_ => !isGameOver)
        .Subscribe(_ =>
        {
            GenerateNextFallen();
            MoveCameraToJustPos();
            isWaiting = false;
            // Debug.Log("Next turn");
        });

        GameEndStream.Subscribe(_ =>
        {
            isGameOver = true;
            // Debug.Log("Game End");
        });

        GameRestartStream.Subscribe(_ =>
        {
            Init();
            Observable.Timer(System.TimeSpan.FromMilliseconds(1000))
                .Subscribe(__ => NextTurnStream.OnNext(Unit.Default));
            // Debug.Log("Game ReStart");
        });

        GameStartStream.OnNext(Unit.Default);
    }

    void Init()
    {
        if (stackedFallens != null && stackedFallens.Count > 0)
        {
            foreach (var fallen in stackedFallens)
            {
                Destroy(fallen.gameObject);
            }
        }
        isWaiting = true;
        isGameOver = false;
        fallenCount = Fallens.Count;
        stackedFallens = new List<Fallen>();
        firstPostion = initPostion;
    }

    public void GenerateNextFallen()
    {
        var fallenObj = Instantiate(Fallens[Random.Range(0, fallenCount)]);
        var fallen = fallenObj.GetComponent<Fallen>();
        fallen.playSceneManager = this;
        stackedFallens.Add(fallen);

        hit = Physics2D.BoxCast(firstPostion + Vector3.up * 5, Vector3.one * 3, 0, Vector3.down, 100);
        var distanceFromTop = Vector3.Distance(firstPostion, hit.point);
        var fallenHeight = fallenObj.GetComponent<SpriteRenderer>().bounds.size.y;

        // Instantiate(devPoint, hit.point, Quaternion.identity);
        // Debug.Log(hit.point);

        firstPostion.y = hit.point.y + fallenHeight / 2;
        if (firstPostion.y < 8)
            firstPostion.y = 8;

        fallenObj.transform.position = firstPostion;
        CurrentActiveFallen = fallenObj;
        isWaiting = false;
    }

    public void MoveCameraToJustPos()
    {
        var y = hit.point.y > 4 ? hit.point.y : 4;
        iTween.MoveTo(Camera.main.gameObject, iTween.Hash("y", y, "time", 1f));
    }
}
