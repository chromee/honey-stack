using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] PlaySceneManager playSceneManager;
    [SerializeField] GameObject parent;

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
}
