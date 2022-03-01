using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameSett : MonoBehaviour
{
    [Serializable]
    public class ItemsStarted
    {
        public int lvl;
        public TextAsset text;
        public Sprite sp_mainMenu;
        public bool IsNew;
        public bool IsEnd;
    }

    public enum TypeMarket
    {
        NONE = -1,
        GOOGLE = 0,
        AMAZON = 1,
        IOS = 2
    }


    public static GameSett instance = null;

    public TypeMarket RateUsOnMarket = TypeMarket.AMAZON;   // Type market for rate
    [Space(20)]
    public Sprite DefSprite;                                // default sprite for levels that did not start

    [HideInInspector]
    public TextAsset[] Stages;                                // list of pictures with levels
    [HideInInspector]
    [SerializeField]
    public List<ItemsStarted> ItemsImg;                     // level class list

    [HideInInspector]
    public List<int> StartedItemsImg;                       // list of classes of levels that have already begun

    public GameObject Item;                       // list of classes of levels that have already begun

    public Transform ContentsCanvas;                       // list of classes of levels that have already begun


    void Awake()
    {
        try
        {
            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
            //else
            //{
            //    Destroy(gameObject);
            //    return;
            //}
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + " Stack: " + e.StackTrace);
        }


        Stages = Resources.LoadAll<TextAsset>("Stage"); // load all levels from resources

        ItemsImg = new List<ItemsStarted>();
        StartedItemsImg = new List<int>();

        for (int i = 0; i < Stages.Length; i++) // cycle through the levels to prepare them for the game
        {

            TextAsset text = Stages[i];

            GameObject newItem = Instantiate(Item, Vector3.zero, Quaternion.identity, ContentsCanvas);

            newItem.GetComponent<ItemImg>().defText = text.name;
            newItem.GetComponent<ItemImg>().value = i;

            LoadText loadText = new LoadText();
            string[] loadTextData = loadText.LoadLines(text);

            int width = Convert.ToInt32(loadTextData[3].Split(' ')[0]);
            int height = Convert.ToInt32(loadTextData[4].Split(' ')[0]);

            bool _new = true;
            bool _end = true;

            #region prepare an empty picture, the same size as the level

            Texture2D texture_mainMenu = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture_mainMenu.filterMode = FilterMode.Point;
            Color[] pixels_sp = new Color[width * height];
            for (int ij = 0; ij < width * height; ij++)
            {
                pixels_sp[ij] = new Color(1, 1, 1, 1);
            }
            texture_mainMenu.SetPixels(pixels_sp);
            texture_mainMenu.Apply();

            #endregion

            SaveGameXML.LoadLvl(texture_mainMenu, i); // load in an empty picture what was done earlier


            #region check empty picture after loading, "true" - empty, otherwise it was saved before
            Color[] pixels_mainMenu = texture_mainMenu.GetPixels();
            for (int ij = 0; ij < pixels_mainMenu.Length; ij++)
            {
                if (pixels_mainMenu[ij] != pixels_sp[ij])
                {
                    _new = false;
                    break;
                }
            }
            #endregion


            #region check with the original picture, "true" - then the picture is completed
            // pixels_sp = sp.texture.GetPixels();
            // for (int ij = 0; ij < pixels_mainMenu.Length; ij++)
            // {
            //     if (pixels_mainMenu[ij] != pixels_sp[ij])
            //     {
            //         _end = false;
            //         break;
            //     }
            // }
            #endregion


            Rect r_mainMenu = new Rect(0, 0, texture_mainMenu.width, texture_mainMenu.height);
            Sprite sprite_mainMenu = Sprite.Create(texture_mainMenu, r_mainMenu, new Vector2(0.5f, 0.5f)); // create a sprite from the loaded image




            ItemsImg.Add(new ItemsStarted() // level class list
            {
                lvl = i,
                text = text,
                sp_mainMenu = sprite_mainMenu,
                IsNew = _new,
                IsEnd = _end
            });

            if (!_new && !_end) // if the level was started but not completed
            {
                StartedItemsImg.Add(i);
            }
        }
    }


    // update after leaving the scene Game
    public void UpdMainSp(int lvl)
    {
        bool _new = true;
        bool _end = true;

        LoadText loadText = new LoadText();
        string[] loadTextData = loadText.LoadLines(ItemsImg[lvl].text);
        int width = Convert.ToInt32(loadTextData[3].Split(' ')[0]);
        int height = Convert.ToInt32(loadTextData[4].Split(' ')[0]);
        #region prepare an empty picture, the same size as the level
        Color[] pixels_sp = new Color[width * height];
        for (int ij = 0; ij < width * height; ij++)
        {
            pixels_sp[ij] = new Color(1, 1, 1, 1);
        }

        if (!SaveGameXML.LoadLvl(ItemsImg[lvl].sp_mainMenu.texture, lvl)) // load in an empty picture what was done earlier
        {
            ItemsImg[lvl].sp_mainMenu.texture.SetPixels(pixels_sp);
            ItemsImg[lvl].sp_mainMenu.texture.Apply();
        }
        #endregion


        #region check empty picture after loading, "true" - empty, otherwise it was saved before

        Color[] c_mainMenu = ItemsImg[lvl].sp_mainMenu.texture.GetPixels();

        for (int ij = 0; ij < c_mainMenu.Length; ij++)
        {
            if (c_mainMenu[ij] != pixels_sp[ij])
            {
                _new = false;
                break;
            }
        }
        #endregion


        #region check with the original picture, "true" - then the picture is completed

        // pixels_sp = ItemsImg[lvl].sp.texture.GetPixels();
        // for (int ij = 0; ij < c_mainMenu.Length; ij++)
        // {
        //     if (c_mainMenu[ij] != pixels_sp[ij])
        //     {
        //         _end = false;
        //         break;
        //     }
        // }
        #endregion

        ItemsImg[lvl].IsNew = _new;
        ItemsImg[lvl].IsEnd = _end;


        if (!_new && !_end) // if the level was started but not completed
        {
            bool b = false;

            foreach (int ll in StartedItemsImg)
            {
                if (ll == lvl)
                {
                    b = true;
                    break;
                }
            }

            if (!b)
            {
                StartedItemsImg.Add(lvl);
            }
        }
    }



    public int GetLevel()
    {
        int l = PlayerPrefs.GetInt("Level", -1);

        return l;
    }

    public void SetLevel(int l)
    {
        PlayerPrefs.SetInt("Level", l);
    }

    public bool HasFirstStart()
    {
        bool b = false;

        if (PlayerPrefs.HasKey("FirstStart"))
            b = System.Convert.ToBoolean(PlayerPrefs.GetInt("FirstStart"));

        return b;
    }

    public void SetFirstStart()
    {
        PlayerPrefs.SetInt("FirstStart", 1);
    }


    #region count of the number of levels that passed

    public void EndLvlRestart()
    {
        PlayerPrefs.SetInt("EndLvl", 0);
    }

    public void EndLvl()
    {
        PlayerPrefs.SetInt("EndLvl", GetEndLvl() + 1);
    }

    public int GetEndLvl()
    {
        int l = PlayerPrefs.GetInt("EndLvl", 0);

        return l;
    }

    #endregion



    #region asynchronous scene loading

    public void LoadingLevel(string nameLevel, Action<float> call)
    {
        StartCoroutine(ProgressState(nameLevel, call));
    }

    IEnumerator ProgressState(string nameLevel, Action<float> call)
    {
        yield return new WaitForSeconds(1);

        AsyncOperation asyncLvl = SceneManager.LoadSceneAsync(nameLevel);
        while (!asyncLvl.isDone)
        {
            float ProgressLoadLevel = asyncLvl.progress;

            call(ProgressLoadLevel * 100.0f);

            yield return null;
        }
    }

    #endregion



    public bool HasOpenOnMarket()
    {
        return (RateUsOnMarket != TypeMarket.NONE) ? true : false;
    }

    public void openOnMarket()
    {
        string uri = "";

        if (HasOpenOnMarket())
        {
            switch (RateUsOnMarket)
            {
                case TypeMarket.AMAZON:
                    uri = "amzn://apps/android?p=" + Application.identifier;
                    break;
                case TypeMarket.GOOGLE:
                    uri = "market://details?id=" + Application.identifier;
                    break;
                case TypeMarket.IOS:
                    uri = "itms-apps://itunes.apple.com/app/id" + Application.identifier;
                    break;
            }

            Application.OpenURL(uri);
        }
    }




    public void RestartLevel(int i)
    {
        SaveGameXML.DeleteAll(i);

        UpdMainSp(i);


        int id = -1;
        int ind = 0;

        foreach (int l in StartedItemsImg)
        {
            if (i == l)
            {
                id = ind;
                break;
            }
            ind++;
        }

        if (id >= 0)
        {
            StartedItemsImg.RemoveAt(id);
        }
    }

    public ItemsStarted GetSelItem()
    {
        return ItemsImg[GetLevel()];
    }


    public static GameSett getInstance()
    {
        return instance;
    }
}
