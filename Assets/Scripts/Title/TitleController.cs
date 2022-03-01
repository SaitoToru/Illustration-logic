using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;
using System;

public class TitleController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Observable.Timer(TimeSpan.FromSeconds(1.0f))
          .Subscribe(_ =>
          {
              SceneManager.LoadScene("Home");
          });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
