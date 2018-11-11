using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
