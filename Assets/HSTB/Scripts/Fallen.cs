using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class Fallen : MonoBehaviour
{
    public Subject<Unit> StopCheckStream = new Subject<Unit>();

    Rigidbody2D rigid;

    public

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        this.UpdateAsObservable()
            .Where(_ => this.transform.position.y < -15)
            .Take(1)
            .Subscribe(_ => GameManager.Instance.FallenStream.OnNext(Unit.Default));

        StopCheckStream.Throttle(System.TimeSpan.FromMilliseconds(500))
            .Subscribe(_ =>
            {
                this.UpdateAsObservable()
                    .Where(__ => rigid.velocity.magnitude == 0)
                    .Take(1)
                    .Subscribe(__ =>
                    {
                        GameManager.Instance.NextTurnStream.OnNext(Unit.Default);
                        // Debug.Log($"STOP {gameObject.name}");
                    });
            });
    }


}
