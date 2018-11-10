using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class FallenController : MonoBehaviour
{
    [SerializeField] float moveHeight;
    [SerializeField] float rotateCoefficient;

    void Start()
    {

        var mousePos = Vector3.zero;
        var moveVec = Vector3.zero;
        this.UpdateAsObservable()
            .Where(_ => !GameManager.Instance.isWaiting && Input.GetMouseButton(0))
            .Subscribe(_ =>
            {
                moveVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                moveVec.y = moveHeight;
                moveVec.z = 0;
                GameManager.Instance.CurrentActiveFallen.transform.position = moveVec;
            });

        var rotateVec = Vector3.zero;
        this.UpdateAsObservable()
            .Where(_ => !GameManager.Instance.isWaiting)
            .Subscribe(_ =>
            {
                rotateVec.z = Input.GetAxis("Mouse ScrollWheel") * rotateCoefficient;
                GameManager.Instance.CurrentActiveFallen.transform.Rotate(rotateVec);
            });

        this.UpdateAsObservable()
            .Where(_ => !GameManager.Instance.isWaiting && Input.GetMouseButtonDown(1))
            .Subscribe(_ =>
            {
                GameManager.Instance.CurrentActiveFallen.GetComponent<Rigidbody2D>().simulated = true;
                GameManager.Instance.CurrentActiveFallen.GetComponent<Fallen>().StopCheckStream.OnNext(Unit.Default);
                GameManager.Instance.isWaiting = true;
            });
    }
}
