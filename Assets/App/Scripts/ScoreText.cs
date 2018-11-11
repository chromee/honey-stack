using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ScoreText : MonoBehaviour
{

    [SerializeField] ScoreAttackManager scoreAttackManager;
    [SerializeField] Text text;

    void Start()
    {
        scoreAttackManager.CurrentScore.Subscribe(v =>
        {
            text.text = v.ToString();
        });
    }
}
