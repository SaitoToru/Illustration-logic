using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Menu instance = null;

    public GameObject PanelQuest;
    public GameObject PanelLoading;

    [Space(20)]

    public Button Tab01;

    public RectTransform RectTab01;

    public Transform Bottom01;

    [Space(20)]

    private int SelI = -1;
    public Button BContinue;
    public Button BRestart;
    public Button BBack;

    void Awake()
    {
        try
        {
            instance = this;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + " Stack: " + e.StackTrace);
        }


        // Tab01.onClick.RemoveAllListeners();
        // Tab01.onClick.AddListener(() =>
        // {
        //     if (Bottom01.localPosition.x >= Bottom02.localPosition.x)
        //     {
        //         Vector3 v3 = Bottom01.localPosition;
        //         Bottom01.localPosition = Bottom02.localPosition;

        //         Vector2 v2 = RectTab01.offsetMax;
        //         RectTab01.offsetMax = RectTab02.offsetMax;
        //     }
        // });

        BContinue.onClick.RemoveAllListeners();
        BContinue.onClick.AddListener(() =>
        {
            LoadGame(SelI);
        });

        BRestart.onClick.RemoveAllListeners();
        BRestart.onClick.AddListener(() =>
        {
            GameSett.getInstance().RestartLevel(SelI);
            LoadGame(SelI);
        });

        BBack.onClick.RemoveAllListeners();
        BBack.onClick.AddListener(() =>
        {
            SelI = -1;
            PanelQuest.SetActive(false);
        });


        PanelQuest.SetActive(false);
        PanelLoading.SetActive(false);
    }

    public void LoadGameQuestion(int I)
    {
        if (!GameSett.getInstance().ItemsImg[I].IsNew)
        {
            SelI = I;
            PanelQuest.SetActive(true);

            if (GameSett.getInstance().ItemsImg[I].IsEnd)
                BContinue.gameObject.SetActive(false);
            else
                BContinue.gameObject.SetActive(true);
        }
        else
        {
            LoadGame(I);
        }
    }


    void LoadGame(int I)
    {
        PanelLoading.SetActive(true);

        GameSett.getInstance().SetLevel(I);
        GameSett.getInstance().LoadingLevel("Game", (float a) => { });
    }


    public static Menu getInstance()
    {
        return instance;
    }
}
