using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class ScoreAttackManager : MonoBehaviour
{
    public List<GameObject> Fallens;

    void Start()
    {
        var fallenCount = Fallens.Count;
        var firstPos = new Vector3(0f, 8f, 0f);
        GameManager.Instance.NextTurnStream.Subscribe(_ =>
        {
            var fallen = Instantiate(Fallens[Random.Range(0, fallenCount)]);
            fallen.transform.position = firstPos;
            GameManager.Instance.CurrentActiveFallen = fallen;
            GameManager.Instance.isWaiting = false;
        });
    }

    void PopNextFallen()
    {

    }

}
