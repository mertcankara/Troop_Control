using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRotation : Observer
{
    private Coroutine _coroutine;
    private Vector3 _velocity = Vector3.zero;

    protected override void Start()
    {
        base.Start();
        _coroutine = StartCoroutine(Rotate());
    }

    public override void Notify(GameState state)
    {
        if (state != GameState.Preparing) return;
        
        subject.Unsubscribe(this);
        
        StopCoroutine(_coroutine);
        StartCoroutine(ResetRotate());
    }

    private IEnumerator Rotate()
    {
        while (true)
        {
            transform.Rotate(0f, 30f * Time.deltaTime, 0f);
            yield return null;
        }
    }

    private IEnumerator ResetRotate()
    {
        float elapsed = 0f;
        float total = 2f;

        Vector3 startRot = transform.rotation.eulerAngles;
        
        while (elapsed < total)
        {
            transform.rotation = Quaternion.Euler(Vector3.Lerp(startRot, Quaternion.identity.eulerAngles, elapsed / total));
            //transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(transform.rotation.eulerAngles, Quaternion.identity.eulerAngles, ref _velocity, total));
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.rotation = Quaternion.identity;
        
        ((GameController) subject).SetState(GameState.Running);
    }
}
