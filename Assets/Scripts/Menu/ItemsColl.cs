using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsColl : MonoBehaviour
{
    public GameObject Pref_ItemImg;

    void Start()
    {
        for (int i = 0; i < GameSett.getInstance().ItemsImg.Count; i++)
        {
            GameObject o = GameObject.Instantiate(Pref_ItemImg);
            o.name += i.ToString();
            o.transform.SetParent(this.transform, false);

            ItemImg item = o.GetComponent<ItemImg>();
            item.value = i;
            // item.defText = (GameSett.getInstance().ItemsImg[i].IsNew) ? GameSett.getInstance().ItemsImg[i].text.name : "GameSett.getInstance().ItemsImg[i].sp_mainMenu";
            item.defText = (GameSett.getInstance().ItemsImg[i].IsNew) ? GameSett.getInstance().ItemsImg[i].text.name : GameSett.getInstance().ItemsImg[i].text.name;
        }
    }
}
