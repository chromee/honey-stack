using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class ScoreText : MonoBehaviour
{

    [SerializeField] ScoreAttackManager scoreAttackManager;
    [SerializeField] TextMeshProUGUI text;

    void Start()
    {
        scoreAttackManager.CurrentScore.Subscribe(v =>
        {
            text.text = v.ToString();
        });
    }
}
