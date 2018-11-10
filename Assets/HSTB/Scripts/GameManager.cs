using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Subject<Unit> FallenStream;
    public Subject<Unit> NextTurnStream;

    public GameObject CurrentActiveFallen;
    public bool isWaiting = false;

    void Start()
    {
        Instance = this;

        FallenStream = new Subject<Unit>();
        FallenStream.Subscribe(_ =>
        {
            Debug.Log("Game End");
        });

        NextTurnStream = new Subject<Unit>();
        NextTurnStream.Subscribe(_ =>
        {
            Debug.Log("Next turn");
            GameManager.Instance.isWaiting = false;
        });

    }

}
