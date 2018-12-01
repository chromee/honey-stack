using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Fallen : MonoBehaviour
{
    public PlaySceneManager playSceneManager;
    public Subject<Unit> StopCheckStream = new Subject<Unit>();

    Rigidbody2D rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        this.UpdateAsObservable()
            .Where(_ => this.transform.position.y < -5)
            .Take(1)
            .Subscribe(_ => playSceneManager.GameEndStream.OnNext(Unit.Default));

        StopCheckStream.Throttle(System.TimeSpan.FromMilliseconds(500))
            .Subscribe(_ =>
            {
                this.UpdateAsObservable()
                    .Where(__ => !playSceneManager.isGameOver)
                    .Where(__ => rigid.velocity.magnitude < 0.001f)
                    .Take(1)
                    .Subscribe(__ =>
                    {
                        playSceneManager.NextTurnStream.OnNext(Unit.Default);
                        // Debug.Log($"STOP {gameObject.name}");
                    });
            });
    }
}
