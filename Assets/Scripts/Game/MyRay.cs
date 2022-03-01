using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MyRay : MonoBehaviour
{
    public static MyRay instance = null;

    public GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    public EventSystem m_EventSystem;



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
    }
	
	public bool HasUI (Vector2 pos) {
        bool b = false;

        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = pos;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        if (results.Count > 0)
        {
            b = true;
        }

        return b;
	}


    public static MyRay getInstance()
    {
        return instance;
    }
}
