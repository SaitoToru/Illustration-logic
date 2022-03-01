using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

public class PinchZoom : MonoBehaviour
{

    public GameObject scrollPanel;

    public float maxScale = 1.0f;
    public float minScale = 0.05f;

    bool is_move_now = true;
    bool is_zoom = false;
    bool is_pause = false;

    Vector3 DeltaPos = Vector3.zero;

    Vector2 minPos = Vector2.zero;
    Vector2 maxPos = Vector2.zero;


    private static PinchZoom pinchZoom;
    public static PinchZoom Instance()
    {
        if (!pinchZoom)
        {
            pinchZoom = FindObjectOfType(typeof(PinchZoom)) as PinchZoom;

            if (!pinchZoom)
            {
                Debug.LogError("Error class PinchZoom in " + Application.loadedLevelName + "!");
            }
        }
        return pinchZoom;
    }

    public void SetGranici(float _min_x, float _min_y, float _max_x, float _max_y)
    {
        if (scrollPanel)
        {
            minPos = new Vector2(_min_x, _min_y);
            maxPos = new Vector2(_max_x, _max_y);

            scrollPanel.transform.localPosition = new Vector3((_min_x + _max_x) / 2,
                                                              (_min_y + _max_y) / 2,
                                                              scrollPanel.transform.localPosition.z);
            DeltaPos = Vector3.zero;
            Move(new Vector3((_min_x + _max_x) / 2, (_min_y + _max_y) / 2));
        }
    }


    public void ResetPosition()
    {
        if (scrollPanel)
        {
            scrollPanel.transform.localPosition = new Vector3((minPos.x + maxPos.x) / 2,
                                                              (minPos.y + maxPos.y) / 2,
                                                              scrollPanel.transform.localPosition.z);
            DeltaPos = Vector3.zero;
            Move(new Vector3((minPos.x + maxPos.x) / 2, (minPos.y + maxPos.y) / 2));

            float z_1 = Math.Abs(maxPos.y - minPos.y) / 1.9f;
            float z_2 = (Math.Abs(maxPos.x - minPos.x)) / 1.9f * Screen.height / Screen.width;

            float zoom = Mathf.Max(z_1, z_2);

            SetZoom(zoom);

            maxScale = Camera.main.orthographicSize;
        }
    }



    void Update()
    {
        if (!is_pause)
        {
            if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved
                && !MyRay.getInstance().HasUI(Input.GetTouch(0).position) && !MyRay.getInstance().HasUI(Input.GetTouch(1).position))
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = Vector2.Distance(touchZero.position, touchOne.position);
                float touchDeltaMag = Vector2.Distance(touchZeroPrevPos, touchOnePrevPos);

                float deltaMagnitudeDiff = Mathf.Clamp((touchDeltaMag - prevTouchDeltaMag) / (100.0f / Camera.main.orthographicSize), -5.0f, 5.0f);


                Zoom(deltaMagnitudeDiff, true);

                is_zoom = true;
            }
            else if (Input.touchCount == 0)
            {
                is_zoom = false;
            }

            if (!is_zoom)
            {
                if (Input.touchCount == 1 && !MyRay.getInstance().HasUI(Input.GetTouch(0).position))
                {
                    Vector3 v3 = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);

                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        DeltaPos = scrollPanel.transform.localPosition - Camera.main.ScreenToWorldPoint(v3);
                        is_move_now = false;
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Moved && !is_move_now)
                    {
                        Move(Camera.main.ScreenToWorldPoint(v3));
                    }
                    else if (Input.GetTouch(0).phase == TouchPhase.Ended && !is_move_now)
                    {
                        is_move_now = true;
                    }
                }

                if (Input.touchCount == 0)
                {
                    Zoom(-(Input.mouseScrollDelta.y / (Camera.main.orthographicSize / maxScale)), true);

                    if (!MyRay.getInstance().HasUI(Input.mousePosition))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            DeltaPos = scrollPanel.transform.localPosition - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            is_move_now = false;
                        }
                        else if (!is_move_now && Input.GetMouseButton(0))
                            Move(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        is_move_now = true;
                    }
                }
            }
        }
    }

    private void Move(Vector3 v2)
    {
        if (scrollPanel)
        {
            Vector3 pos_new = new Vector3(v2.x, v2.y, 0) + new Vector3(DeltaPos.x, DeltaPos.y);

            scrollPanel.transform.localPosition = new Vector3(Mathf.Clamp(pos_new.x, minPos.x, maxPos.x)
                                                              , Mathf.Clamp(pos_new.y, minPos.y, maxPos.y)
                                                              , scrollPanel.transform.localPosition.z);
        }
    }

    private void Zoom(float direction, bool pinching)
    {
        float zoom = Camera.main.orthographicSize;
        float new_zoom = zoom;

        if (direction > 0)
        {
            new_zoom = zoom + (pinching ? direction : 1);
        }
        else if (direction < 0)
        {
            new_zoom = zoom + (pinching ? direction : -1);
        }

        if (zoom != new_zoom)
            SetZoom(Mathf.MoveTowards(zoom, new_zoom, (pinching ? 100 : 20) * Time.deltaTime));
    }

    private void SetZoom(float zoom)
    {
        Camera.main.orthographicSize = Mathf.Clamp(zoom, minScale, maxScale);
    }

    public void SetPause(bool b)
    {
        is_pause = b;
    }
}
