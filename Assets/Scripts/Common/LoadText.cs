using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadText
{

    public string[] LoadLines(string fileName)
    {
        //テキストファイルのデータを取得するインスタンスを作成//Resourcesフォルダから対象テキストを取得
        TextAsset textasset = Resources.Load(fileName, typeof(TextAsset)) as TextAsset;

        return LoadLines(textasset);
    }

    public string[] LoadLines(TextAsset file)
    {

        // 全体を取得
        string TextLines = new StringReader(file.text).ReadToEnd();

        // Debug.Log(TextLines.Length);
        //Splitで一行づつを代入した1次配列を作成
        char[] SPLIT_TEXT = { '\r', '\n' };
        string[] textMessage = TextLines.Split(SPLIT_TEXT);

        Debug.Log(textMessage.Length);
        return textMessage;
    }
}
