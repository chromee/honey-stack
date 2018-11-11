using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class ScoreAttackManager : MonoBehaviour
{
    public ReactiveProperty<int> CurrentScore = new ReactiveProperty<int>();
    [SerializeField] PlaySceneManager playSceneManager;

    void Start()
    {
        playSceneManager.GameRestartStream.Subscribe(_ =>
        {
            Init();
        });

        playSceneManager.NextTurnStream.Subscribe(_ =>
        {
            CurrentScore.Value += 1;
        });

        playSceneManager.GameRestartStream.Subscribe(_ =>
        {
            Init();
        });
    }

    void Init()
    {
        CurrentScore.Value = 0;
    }
}
