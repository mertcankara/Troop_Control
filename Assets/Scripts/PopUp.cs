using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : Observer
{
    private GameController _gameController;
    
    protected override void Start()
    {
        base.Start();
        _gameController = (GameController) subject;
        StartCoroutine(ScaleUp());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("space"))
        {
            StartCoroutine(ScaleDown());
            _gameController.SetState(GameState.Preparing);
            subject.Unsubscribe(this);
        }
    }

    private IEnumerator ScaleDown()
    {
        float total = 0.5f;
        float elapsed = 0f;

        Vector3 targetScale = new Vector3(0f, 0f, 0f);

        while (elapsed < total)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, elapsed / total);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.gameObject.SetActive(false);
    }

    private IEnumerator ScaleUp()
    {
        float total = 0.5f;
        float elapsed = 0f;

        Vector3 targetScale = new Vector3(1.2f, 1.2f, 1.2f);

        while (elapsed < total)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsed / total);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Vector3 startScale = transform.localScale;
        targetScale = new Vector3(1f, 1f, 1f);

        total = 0.1f;
        elapsed = 0f;

        while (elapsed < total)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / total);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}