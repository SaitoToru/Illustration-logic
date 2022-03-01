using System.IO;
using System.Xml;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;


public class SaveGameXML
{
    static string GetFileName(int lvl)
    {
        return Application.persistentDataPath + "/GAME_" + lvl.ToString() + ".png";
    }


    public static void SaveLvl(Texture2D tex_paint, int lvl)
    {
        try
        {
            string file_name = GetFileName(lvl);

            if (File.Exists(file_name))
            {
                File.Delete(file_name);
            }

            System.IO.File.WriteAllBytes(file_name, tex_paint.EncodeToPNG());
        }
        catch (System.Exception e)
        {
            Debug.LogError("SaveLvl - " + e.ToString());
        }
    }

    public static bool LoadLvl(Texture2D tex_paint, int lvl)
    {
        bool b = false;
        try
        {
            string file_name = GetFileName(lvl);

            // Debug.Log(file_name);

            if (File.Exists(file_name))
            {
                byte[] bytes;
                bytes = System.IO.File.ReadAllBytes(file_name);
                tex_paint.LoadImage(bytes);

                b = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("LoadLvl - " + e.ToString());
        }

        return b;
    }



    public static void DeleteAll(int lvl)
    {
        try
        {
            string file_name = GetFileName(lvl);

            if (File.Exists(file_name))
            {
                File.Delete(file_name);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("DeleteAll - " + e.ToString());
        }
    }

}
