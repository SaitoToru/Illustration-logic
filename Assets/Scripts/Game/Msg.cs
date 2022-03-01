using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Msg : MonoBehaviour
{
    public static Msg instance = null;

    public GameObject panel;
    public Text text;

    private float t_db = 0;
    private float tmax_db = 4.5f;


    void Start()
    {
        try
        {
            instance = this;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + " Stack: " + e.StackTrace);
        }

        Hide();
    }

    void Update()
    {
        if (panel.activeSelf)
        {
            t_db += Time.deltaTime;
            if (t_db >= tmax_db)
                Hide();
        }
    }

    public void StartShow(string msg)
    {
        panel.SetActive(true);
        text.text = msg;
        t_db = 0;
    }

    void Hide()
    {
        panel.SetActive(false);
    }


    public static Msg getInstance()
    {
        return instance;
    }
}
