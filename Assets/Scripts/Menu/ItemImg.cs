using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemImg : MonoBehaviour
{

    [HideInInspector]
    public string defText;
    private Text text;
    private Button button;
    [HideInInspector]
    public int value;


    void Awake()
    {
        text = this.GetComponentsInChildren<Text>()[0];
        button = this.GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Menu.getInstance().LoadGameQuestion(value);
        });
    }

    void Start()
    {
        text.text = this.defText;
    }
}
