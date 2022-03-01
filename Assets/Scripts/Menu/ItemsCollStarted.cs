using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCollStarted : MonoBehaviour
{
    public GameObject Pref_ItemImg;

    void Start()
    {
        GameSett G = GameSett.getInstance();

        for (int i = 0; i < G.StartedItemsImg.Count; i++)
        {
            int I = G.StartedItemsImg[i];

            GameObject o = GameObject.Instantiate(Pref_ItemImg);
            o.name += I.ToString();
            o.transform.SetParent(this.transform, false);

            ItemImg item = o.GetComponent<ItemImg>();
            item.value = I;
            item.defText = G.ItemsImg[I].text.name;
        }
    }
}
