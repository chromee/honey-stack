using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class FallenController : MonoBehaviour
{
    [SerializeField] PlaySceneManager playSceneManager;
    [SerializeField] float rotateCoefficient;

    void Start()
    {
        var mousePos = Vector3.zero;
        var moveVec = Vector3.zero;
        this.UpdateAsObservable()
            .Where(_ => !playSceneManager.isWaiting && Input.GetMouseButton(0))
            .Subscribe(_ =>
            {
                moveVec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                moveVec.y = playSceneManager.CurrentActiveFallen.transform.position.y;
                moveVec.z = 0;
                playSceneManager.CurrentActiveFallen.transform.position = moveVec;
            });

        var rotateVec = Vector3.zero;
        this.UpdateAsObservable()
            .Where(_ => !playSceneManager.isWaiting)
            .Subscribe(_ =>
            {
                rotateVec.z = Input.GetAxis("Mouse ScrollWheel") * rotateCoefficient;
                playSceneManager.CurrentActiveFallen.transform.Rotate(rotateVec);
            });

        this.UpdateAsObservable()
            .Where(_ => !playSceneManager.isWaiting && Input.GetMouseButtonDown(1))
            .Subscribe(_ =>
            {
                playSceneManager.CurrentActiveFallen.GetComponent<Rigidbody2D>().simulated = true;
                playSceneManager.CurrentActiveFallen.GetComponent<Fallen>().StopCheckStream.OnNext(Unit.Default);
                playSceneManager.isWaiting = true;
            });
    }
}
