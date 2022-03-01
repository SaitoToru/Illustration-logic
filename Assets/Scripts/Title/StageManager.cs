using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IngameDebugConsole;
using System;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{


    [SerializeField]
    private GameObject stage = null;

    [SerializeField]
    private GameObject stageBlock = null;

    [SerializeField]
    private GameObject rowHints = null;

    [SerializeField]
    private GameObject columnHints = null;

    [SerializeField]
    private GameObject rowHintBlock = null;

    [SerializeField]
    private GameObject columnHintBlock = null;

    [SerializeField]
    private GameObject hint = null;

    private const int BlockMassCnt = 25; // 5 * 5の基本形マス数

    private const int BlockMassRowCnt = 5; // １ブロックの横マス数

    private const int BlockMassColumnCnt = 5; // １ブロックの縦マス数

    // Start is called before the first frame update
    void Start()
    {

        LoadText loadText = new LoadText();
        string[] loadTextData = loadText.LoadLines("coffee");

        int stageColumnCnt = Convert.ToInt32(loadTextData[3].Split(' ')[0]);
        int stageRowCnt = Convert.ToInt32(loadTextData[4].Split(' ')[0]);

        Debug.Log("縦マス数:" + stageColumnCnt);
        Debug.Log("横マス数:" + stageRowCnt);

        int BlockCreateCnt = (stageRowCnt * stageColumnCnt) / BlockMassCnt;
        int BlockSqrtCnt = Mathf.FloorToInt(Mathf.Sqrt(BlockCreateCnt));
        for (int i = 0; i < BlockCreateCnt; i++)
        {

            GameObject block = Instantiate(stageBlock, stage.transform);
            GridLayoutGroup grid = stage.GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(stage.GetComponent<RectTransform>().sizeDelta.x / BlockSqrtCnt, stage.GetComponent<RectTransform>().sizeDelta.y / BlockSqrtCnt);
            GridLayoutGroup blockGrid = block.GetComponent<GridLayoutGroup>();
            Debug.Log(grid.cellSize.x);
            blockGrid.cellSize = new Vector2(grid.cellSize.x / BlockMassRowCnt, grid.cellSize.y / BlockMassColumnCnt);
        }


        // -----------------   行ヒント生成処理   Start ----------------------

        int startRowHintIndex = 5;
        int[][] aryRowHints = new int[stageRowCnt][];

        for (int i = 0; i < aryRowHints.Length; i++)
        {

            List<string> hintList = new List<string>(loadTextData[startRowHintIndex + i].Split(' '));
            hintList.RemoveAll(item => item.Equals(""));
            string[] hints = hintList.ToArray();

            aryRowHints[i] = new int[hints.Length];


            GameObject block = Instantiate(rowHintBlock, rowHints.transform);

            for (int hintsCnt = 0; hintsCnt < hints.Length; hintsCnt++)
            {


                Debug.Log("横マスヒント:" + hints[hintsCnt]);
                aryRowHints[i][hintsCnt] = Convert.ToInt32(hints[hintsCnt]);
                GameObject hintObject = Instantiate(hint, block.transform);
                hintObject.GetComponentInChildren<Text>().text = aryRowHints[i][hintsCnt].ToString();
            }
        }

        // -----------------   行ヒント生成処理   End ----------------------

        // -----------------   列ヒント生成処理   Start ----------------------

        int startColumnHintIndex = 10;
        int[][] aryColumnHints = new int[stageColumnCnt][];

        for (int i = 0; i < aryColumnHints.Length; i++)
        {

            List<string> hintList = new List<string>(loadTextData[startColumnHintIndex + i].Split(' '));
            hintList.RemoveAll(item => item.Equals(""));
            string[] hints = hintList.ToArray();

            aryColumnHints[i] = new int[hints.Length];


            GameObject block = Instantiate(columnHintBlock, columnHints.transform);

            for (int hintsCnt = 0; hintsCnt < hints.Length; hintsCnt++)
            {

                Debug.Log("縦マスヒント:" + hints[hintsCnt]);
                aryColumnHints[i][hintsCnt] = Convert.ToInt32(hints[hintsCnt]);
                GameObject hintObject = Instantiate(hint, block.transform);
                hintObject.GetComponentInChildren<Text>().text = aryColumnHints[i][hintsCnt].ToString();
            }
        }

        // -----------------   列ヒント生成処理   End ----------------------
    }

    // Update is called once per frame
    void Update()
    {

    }
}
