using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using System.Linq;

public class Game : MonoBehaviour
{
    [Serializable]
    public enum TypeCheck
    {
        None,
        Question,
        Yes
    }

    public struct Point
    {
        public int x;
        public int y;

        public Point(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    [Serializable]
    public class PosPointXY
    {
        public int x;
        public int y;
        public bool IsPaint;
        public TypeCheck selectTypeCheck;
        public bool HasOK;
    }

    [Serializable]
    public class RowCol
    {
        public int num;
        public bool IsOn;
        public TextMesh text_num;
        public List<Point> points;
    }


    private class PaintHistory
    {
        public Point point;
        public TypeCheck beforeTypeCheck;
        public TypeCheck afterTypeCheck;
        public bool isLock;
    }

    public static Game instance = null;

    // public Button B_Back;                   // Button exit in the level selection menu
    // public Button B_Clean;                  // Button clearing the playing field
    public GameObject PanelOK;              // UI if game WIN
    public GameObject PanelLoading;         // UI loading
    // public GameObject PanelQuestClear;      // UI question about clear game field
    public GameObject PanelRate;            // UI rate this game (Google, iOS or Amazon)
    // public GameObject PanelHelp;            // UI menu help
    [Space(20)]
    public GameObject CanvasShareObj;       // UI expectations shared image
    public Image ImageView;                 // On "PanelOK". Shows unraveled picture
    public Button B_Home;
    // public Button B_Back_Menu;              // On "PanelOK". Exit in the level selection menu
    // public Button B_Next_LVL;               // On "PanelOK". Go next level
    // public Button B_Quest_No;               // On "PanelQuestClear". Question answer button "No"
    // public Button B_Quest_Yes;              // On "PanelQuestClear". Question answer button "Yes"
    public Button B_Undo_Tile;               // 元に戻すボタン
    public Button B_Redo_Tile;              // 元に戻す操作をキャンセルするボタン
    public Button B_Rate_No;                // On "PanelRate". Do not rate the application.
    public Button B_Rate_Yes;               // On "PanelRate". Rate app
    // public Button B_Show_Help;              // Button show UI help
    // public Button B_Back_Help;              // On "PanelHelp". Button close UI help
    [Space(20)]
    public Tilemap tilemap;                 // Tilemap main playing field
    public Tile tile_none;                  // In Tilemap "tilemap". Tile nothing is selected (void)
    public Tile tile_question;              // In Tilemap "tilemap". Tile we assume that there is something here (question mark)
    public Tile tile_yes;                   // In Tilemap "tilemap". Tile note that it should be drawn here (black square)
    [Space(20)]
    public Transform ColsObj;               // This adds objects with numbers that are in columns.
    public Transform RowsObj;               // This adds objects with numbers that are in rows.
    public GameObject fpref_back_num;       // Prefab "TileText" in which numbers are put
    public RectTransform FonTransform;      // Canvas whith background
    [Space(10)]
    public Tilemap TilemapBack;             // Tilemap draws a background grid in the main playing field
    public Tilemap TilemapBack_Rows;        // Tilemap draws a background grid in a field with numbers for rows
    public Tilemap TilemapBack_Cols;        // Tilemap draws a background grid in a field with numbers for columns
    [Space(10)]
    public Tile tile_back_TL;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_TC;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_TR;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_CL;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_CC;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_CR;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_BL;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_BC;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_BR;               // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    [Space(10)]
    public Tile tile_back_ONE_LR_T;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_LR_C;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_LR_B;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_TB_L;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_TB_C;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_TB_R;         // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid
    public Tile tile_back_ONE_CC;           // In Tilemap "TilemapBack", "TilemapBack_Rows" and "TilemapBack_Cols". grid

    [HideInInspector]
    public bool HasPause = false;           // If "true" pause, or "false" play

    private GameSett.ItemsStarted itSt;     // information about the selected level

    private int width;

    private int height;

    private bool IsMove = false;                                // "true" if you made a motion with the taped
    private Vector3 startPos;                                   // position at the beginning of the tapa
    private bool isOn = false;                                  // "true" if you quickly tapped without moving
    private Vector3Int id_click = new Vector3Int(-1, -1, -1);   // cell field for which tap

    private bool IsMoveClick = false;                           // "true" if you squeezed for a certain time and began to move (horizontally or vertically) to sketches
    private float d_Time_move = 0;                              // time counter
    private float d_Time_move_max = 1.5f;                      // time you need to hold tap
    private TypeCheck newTileMove = TypeCheck.None;             // type of sketches when moving
    private Vector2 move_napr = new Vector2(0, 0);              // sketching direction

    List<List<PosPointXY>> field = new List<List<PosPointXY>>();// main field
    List<List<RowCol>> rows = new List<List<RowCol>>();         // numbers in rows
    List<List<RowCol>> cols = new List<List<RowCol>>();         // numbers in columns

    private List<List<PaintHistory>> undoHistory = new List<List<PaintHistory>>();

    private List<List<PaintHistory>> redoHistory = new List<List<PaintHistory>>();

    private List<PaintHistory> tempHistory = new List<PaintHistory>();

    int max_length_row = 0;                                     // max width game field in this level
    int max_length_col = 0;                                     // max height game field in this level

    int undoIndex = 0;

    void Awake()
    {
        IsMove = true;

        try
        {
            instance = this;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + " Stack: " + e.StackTrace);
        }

        itSt = GameSett.getInstance().GetSelItem();


        Debug.Log(itSt.text.name);
        field = GetField("Stage/" + itSt.text.name);
        ResetField();

        B_Home.onClick.RemoveAllListeners();
        B_Home.onClick.AddListener(() =>
                {

                    GameSett.getInstance().LoadingLevel("Menu", (float a) => { });
                });

        // B_Clean.onClick.RemoveAllListeners();
        // B_Clean.onClick.AddListener(() =>
        // {
        //     PanelQuestClear.SetActive(true);
        //     HasPause = true;
        // });

        // B_Back.onClick.RemoveAllListeners();
        // B_Back.onClick.AddListener(() =>
        // {
        //     PanelLoading.SetActive(true);

        //     GameSett.getInstance().UpdMainSp(itSt.lvl);

        //     GameSett.getInstance().LoadingLevel("Menu", (float a) => { });
        // });

        // B_Clean.onClick.RemoveAllListeners();
        // B_Clean.onClick.AddListener(() =>
        // {
        //     PanelQuestClear.SetActive(true);
        //     HasPause = true;
        // });



        // B_Quest_No.onClick.RemoveAllListeners();
        // B_Quest_No.onClick.AddListener(() =>
        // {
        //     PanelQuestClear.SetActive(false);
        //     HasPause = false;
        // });

        // B_Quest_Yes.onClick.RemoveAllListeners();
        // B_Quest_Yes.onClick.AddListener(() =>
        // {
        //     GameSett.getInstance().RestartLevel(itSt.lvl);

        //     PanelQuestClear.SetActive(false);
        //     PanelLoading.SetActive(true);

        //     GameSett.getInstance().SetLevel(itSt.lvl);
        //     GameSett.getInstance().LoadingLevel("Game", (float a) => { });
        // });

        B_Undo_Tile.interactable = false;
        B_Undo_Tile.onClick.RemoveAllListeners();
        B_Undo_Tile.onClick.AddListener(() =>
        {
            undoTile();
        });

        B_Redo_Tile.interactable = false;
        B_Redo_Tile.onClick.RemoveAllListeners();
        B_Redo_Tile.onClick.AddListener(() =>
        {
            redoTile();
        });

        B_Rate_No.onClick.RemoveAllListeners();
        B_Rate_No.onClick.AddListener(() =>
        {
            PanelRate.SetActive(false);
        });

        B_Rate_Yes.onClick.RemoveAllListeners();
        B_Rate_Yes.onClick.AddListener(() =>
        {
            PanelRate.SetActive(false);

            GameSett.getInstance().EndLvlRestart();

            GameSett.getInstance().openOnMarket();
        });


        // B_Show_Help.onClick.RemoveAllListeners();
        // B_Show_Help.onClick.AddListener(() =>
        // {
        //     ShowHelp();
        // });

        // B_Back_Help.onClick.RemoveAllListeners();
        // B_Back_Help.onClick.AddListener(() =>
        // {
        //     PanelHelp.SetActive(false);
        //     HasPause = false;
        // });




        // B_Back_Menu.onClick.RemoveAllListeners();
        // B_Back_Menu.onClick.AddListener(() =>
        // {
        //     if (ImageView.sprite)
        //         Destroy(ImageView.sprite);

        //     PanelLoading.SetActive(true);
        //     GameSett.getInstance().UpdMainSp(itSt.lvl);
        //     GameSett.getInstance().LoadingLevel("Menu", (float a) => { });
        // });

        // B_Next_LVL.onClick.RemoveAllListeners();
        // B_Next_LVL.onClick.AddListener(() =>
        // {
        //     if (ImageView.sprite)
        //         Destroy(ImageView.sprite);

        //     int I = itSt.lvl + 1;

        //     PanelLoading.SetActive(true);
        //     GameSett.getInstance().UpdMainSp(itSt.lvl);

        //     if (I >= GameSett.getInstance().ItemsSp.Length)
        //     {
        //         GameSett.getInstance().LoadingLevel("Menu", (float a) => { });
        //     }
        //     else
        //     {
        //         GameSett.getInstance().SetLevel(I);
        //         GameSett.getInstance().LoadingLevel("Game", (float a) => { });
        //     }
        // });

        CanvasShareObj.SetActive(false);


        PanelOK.SetActive(false);
        PanelLoading.SetActive(false);
        HasPause = false;


        if (!GameSett.getInstance().HasFirstStart())
        {
            GameSett.getInstance().SetFirstStart();
            // ShowHelp();
        }
    }

    void Start()
    {
        // determine the boundaries of the camera movement
        PinchZoom.Instance().SetGranici(-width, -(height + max_length_col), max_length_row, 0);
        PinchZoom.Instance().ResetPosition();

        // stretch canvas background
        FonTransform.localPosition = new Vector3((width - max_length_row) / 2.0f, (height + max_length_col) / 2.0f, 0);
        FonTransform.sizeDelta = new Vector2(width + max_length_row + 10, height + max_length_col + 10);
    }

    //create a playing field
    List<List<PosPointXY>> GetField(String texName)
    {

        LoadText loadText = new LoadText();
        string[] loadTextData = loadText.LoadLines(texName);

        width = Convert.ToInt32(loadTextData[3].Split(' ')[0]);
        height = Convert.ToInt32(loadTextData[4].Split(' ')[0]);

        List<List<PosPointXY>> co = new List<List<PosPointXY>>(); // tmp game field

        Color black = Color.black;

        #region  create a playing field and its grid in the background, and determine what type each cell
        for (int x = 0; x < width; x++)
        {
            List<PosPointXY> row_point = new List<PosPointXY>();

            for (int y = 0; y < height; y++)
            {
                // int id = y * width + x;
                string paintData = loadTextData[width + height + (height - y - 1) + 5];
                PosPointXY point = new PosPointXY()
                {
                    HasOK = false,
                    IsPaint = false,
                    // selectTypeCheck = (cc_paint[id] == black) ? TypeCheck.Yes : TypeCheck.None,
                    selectTypeCheck = TypeCheck.None,
                    x = x,
                    y = y
                };

                if (Convert.ToInt32(paintData.Substring(x, 1)) == 1)
                {
                    point.IsPaint = true;
                }

                row_point.Add(point);

                #region Background
                Tile tile = tile_back_CC;
                bool b_x = ((x + 1) % 5 == 0 || (x + 1) == width);
                bool b_x0 = (x % 5 == 0 || x == 0);
                bool b_y = ((height - (y + 1)) % 5 == 0 || (y + 1) == height);
                bool b_y0 = ((height - y) % 5 == 0 || y == 0);

                if (b_x && !b_x0 && b_y && !b_y0)
                    tile = tile_back_TR;
                else if (b_x && !b_x0 && !b_y && !b_y0)
                    tile = tile_back_CR;
                else if (b_x && !b_x0 && !b_y && b_y0)
                    tile = tile_back_BR;

                else if (!b_x && b_x0 && b_y && !b_y0)
                    tile = tile_back_TL;
                else if (!b_x && b_x0 && !b_y && !b_y0)
                    tile = tile_back_CL;
                else if (!b_x && b_x0 && !b_y && b_y0)
                    tile = tile_back_BL;

                else if (!b_x && !b_x0 && b_y && !b_y0)
                    tile = tile_back_TC;
                else if (!b_x && !b_x0 && !b_y && !b_y0)
                    tile = tile_back_CC;
                else if (!b_x && !b_x0 && !b_y && b_y0)
                    tile = tile_back_BC;


                else if (b_x && b_x0 && b_y && !b_y0)
                    tile = tile_back_ONE_LR_T;
                else if (b_x && b_x0 && !b_y && !b_y0)
                    tile = tile_back_ONE_LR_C;
                else if (b_x && b_x0 && !b_y && b_y0)
                    tile = tile_back_ONE_LR_B;

                else if (b_x && !b_x0 && b_y && b_y0)
                    tile = tile_back_ONE_TB_R;
                else if (!b_x && !b_x0 && b_y && b_y0)
                    tile = tile_back_ONE_TB_C;
                else if (!b_x && b_x0 && b_y && b_y0)
                    tile = tile_back_ONE_TB_L;

                else if (b_x && b_x0 && b_y && b_y0)
                    tile = tile_back_ONE_CC;

                TilemapBack.SetTile(new Vector3Int(x, y, 0), tile);
                // Debug.Log("SetTile:" + tile);
                #endregion
            }
            co.Add(row_point);
        }
        #endregion


        max_length_row = 0;
        max_length_col = 0;

        #region numbers in columns
        List<Point> points = new List<Point>();

        for (int x = 0; x < width; x++)
        {
            List<RowCol> col_tmp = new List<RowCol>();
            int count = 0;
            for (int y = 0; y < height; y++)
            {
                if (co[x][y].IsPaint)
                {
                    count++;
                    points.Add(new Point(x, y));
                }

                if ((!co[x][y].IsPaint || (y + 1) == height) && count != 0)
                {
                    col_tmp.Add(new RowCol()
                    {
                        IsOn = false,
                        num = count,
                        points = points
                    });
                    count = 0;
                    points = new List<Point>();
                }
            }
            cols.Add(col_tmp);

            if (col_tmp.Count > max_length_col)
                max_length_col = col_tmp.Count;
        }
        #endregion


        #region numbers in rows
        points = new List<Point>();

        for (int y = 0; y < height; y++)
        {
            List<RowCol> row_tmp = new List<RowCol>();
            int count = 0;
            for (int x = 0; x < width; x++)
            {
                if (co[x][y].IsPaint)
                {
                    count++;
                    points.Add(new Point(x, y));
                }

                if ((!co[x][y].IsPaint || (x + 1) == width) && count != 0)
                {
                    row_tmp.Add(new RowCol()
                    {
                        IsOn = false,
                        num = count,
                        points = points
                    });
                    count = 0;
                    points = new List<Point>();
                }
            }
            rows.Add(row_tmp);

            if (row_tmp.Count > max_length_row)
                max_length_row = row_tmp.Count;
        }
        #endregion


        #region Background and numbs for cols and rows
        TilemapBack_Cols.transform.localPosition = new Vector3(0, height, 0);
        TilemapBack_Rows.transform.localPosition = new Vector3(-max_length_row, 0, 0);

        ColsObj.transform.localPosition = new Vector3(0, height, 0);
        RowsObj.transform.localPosition = new Vector3(-max_length_row, 0, 0);


        for (int x = 0; x < cols.Count; x++)
        {
            for (int y = 0; y < max_length_col; y++)
            {
                Tile tile_cr = tile_back_CC;
                bool b_x_cr = ((x + 1) % 5 == 0 || (x + 1) == cols.Count);
                bool b_x0_cr = (x % 5 == 0 || x == 0);
                bool b_y_cr = ((y + 1) % 5 == 0 || (y + 1) == max_length_col);
                bool b_y0_cr = (y % 5 == 0 || y == 0);

                if (b_x_cr && !b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TR;
                else if (b_x_cr && !b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CR;
                else if (b_x_cr && !b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BR;

                else if (!b_x_cr && b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TL;
                else if (!b_x_cr && b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CL;
                else if (!b_x_cr && b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BL;

                else if (!b_x_cr && !b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TC;
                else if (!b_x_cr && !b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CC;
                else if (!b_x_cr && !b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BC;


                else if (b_x_cr && b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_ONE_LR_T;
                else if (b_x_cr && b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_ONE_LR_C;
                else if (b_x_cr && b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_LR_B;

                else if (b_x_cr && !b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_R;
                else if (!b_x_cr && !b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_C;
                else if (!b_x_cr && b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_L;

                else if (b_x_cr && b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_CC;

                TilemapBack_Cols.SetTile(new Vector3Int(x, y, 0), tile_cr);

                if (y < cols[x].Count)
                {
                    cols[x][y].text_num = AddNumTile(ColsObj, TilemapBack_Cols, x, y, cols[x][y].num);
                }
            }
        }

        for (int y = 0; y < rows.Count; y++)
        {
            for (int x = 0; x < max_length_row; x++)
            {
                Tile tile_cr = tile_back_CC;
                bool b_x_cr = ((max_length_row - (x + 1)) % 5 == 0 || (x + 1) == max_length_row);
                bool b_x0_cr = ((max_length_row - x) % 5 == 0 || x == 0);
                bool b_y_cr = ((rows.Count - (y + 1)) % 5 == 0 || (y + 1) == rows.Count);
                bool b_y0_cr = ((rows.Count - y) % 5 == 0 || y == 0);

                if (b_x_cr && !b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TR;
                else if (b_x_cr && !b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CR;
                else if (b_x_cr && !b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BR;

                else if (!b_x_cr && b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TL;
                else if (!b_x_cr && b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CL;
                else if (!b_x_cr && b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BL;

                else if (!b_x_cr && !b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_TC;
                else if (!b_x_cr && !b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_CC;
                else if (!b_x_cr && !b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_BC;


                else if (b_x_cr && b_x0_cr && b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_ONE_LR_T;
                else if (b_x_cr && b_x0_cr && !b_y_cr && !b_y0_cr)
                    tile_cr = tile_back_ONE_LR_C;
                else if (b_x_cr && b_x0_cr && !b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_LR_B;

                else if (b_x_cr && !b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_R;
                else if (!b_x_cr && !b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_C;
                else if (!b_x_cr && b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_TB_L;

                else if (b_x_cr && b_x0_cr && b_y_cr && b_y0_cr)
                    tile_cr = tile_back_ONE_CC;

                TilemapBack_Rows.SetTile(new Vector3Int(x, y, 0), tile_cr);

                if (x < rows[y].Count)
                {
                    rows[y][x].text_num = AddNumTile(RowsObj, TilemapBack_Rows, (max_length_row - rows[y].Count) + x, y, rows[y][x].num);
                }
            }
        }
        #endregion

        return co;
    }

    // create TextMesh for numbers
    TextMesh AddNumTile(Transform tr, Tilemap tm, int x, int y, int num)
    {
        GameObject o = GameObject.Instantiate(fpref_back_num);
        o.name += "_" + x.ToString() + "_" + y.ToString();
        o.transform.SetParent(tr, false);
        o.transform.localPosition = tm.CellToLocal(new Vector3Int(x, y, 0));

        TextMesh tt = o.GetComponentInChildren<TextMesh>();

        tt.text = num.ToString();

        return tt;
    }

    // sets tiles at the beginning of the game
    void ResetField()
    {
        for (int x = 0; x < field.Count; x++)
        {
            for (int y = 0; y < field[x].Count; y++)
            {
                SetTile(new Vector3Int(x, y, 0), field[x][y].selectTypeCheck);
            }
        }
    }

    // void ShowHelp()
    // {
    //     PanelHelp.SetActive(true);
    //     HasPause = true;
    // }


    // returns cell numbers ('x' and 'y') at specified position
    Vector3Int GetIdCell(Vector3 pos)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(pos);

        // int width = itSt.sp.texture.width;
        // int height = itSt.sp.texture.height;

        if (cellPosition.x >= width || cellPosition.y >= height || cellPosition.x < 0 || cellPosition.y < 0)
        {
            cellPosition = new Vector3Int(-1, -1, -1);
        }

        return cellPosition;
    }

    // change and set the type of choice in the cell
    void ClickTile(Vector3Int pos)
    {
        if (pos.x != -1)
        {
            TypeCheck tc_new = GetNewTile(pos);

            SetTempHistory(pos, tc_new);
            SetHistory(tempHistory);
            SetTile(pos, tc_new);
        }
    }

    // changes the type of the selected cell and returns a new (by cycle)
    TypeCheck GetNewTile(Vector3Int pos)
    {
        TypeCheck tc_new = TypeCheck.None;

        if (pos.x != -1)
        {
            int i = pos.x;
            int j = pos.y;

            TypeCheck tc = field[i][j].selectTypeCheck;

            if (tc == TypeCheck.None)
                tc_new = TypeCheck.Yes;
            if (tc == TypeCheck.Yes)
                tc_new = TypeCheck.Question;
        }

        return tc_new;
    }

    // sets a new cell type and changes to a tile
    void SetTile(Vector3Int pos, TypeCheck tc_new)
    {
        if (pos.x != -1)
        {
            int i = pos.x;
            int j = pos.y;

            Tile tile = tile_none;

            if (tc_new == TypeCheck.Yes)
                tile = tile_yes;
            if (tc_new == TypeCheck.Question)
                tile = tile_question;

            tilemap.SetTile(pos, tile);

            field[i][j].selectTypeCheck = tc_new;
            field[i][j].HasOK = ((tc_new == TypeCheck.Yes && field[i][j].IsPaint) || (tc_new != TypeCheck.Yes && !field[i][j].IsPaint)) ? true : false;

            // itSt.sp_mainMenu.texture.SetPixel(pos.x, pos.y, (field[i][j].selectTypeCheck == TypeCheck.Yes) ? new Color(0, 0, 0, 1) : new Color(1, 1, 1, 1));
            // itSt.sp_mainMenu.texture.Apply();

            UpdNum(i, j);

            Save();
        }
    }

    void UpdateTile(Vector3Int pos, TypeCheck tc_new)
    {

        if (pos.x != -1)
        {
            int i = pos.x;
            int j = pos.y;

            Tile tile = tile_none;

            if (tc_new == TypeCheck.Yes)
                tile = tile_yes;
            if (tc_new == TypeCheck.Question)
                tile = tile_question;

            tilemap.SetTile(pos, tile);

            field[i][j].selectTypeCheck = tc_new;
            field[i][j].HasOK = ((tc_new == TypeCheck.Yes && field[i][j].IsPaint) || (tc_new != TypeCheck.Yes && !field[i][j].IsPaint)) ? true : false;

            // itSt.sp_mainMenu.texture.SetPixel(pos.x, pos.y, (field[i][j].selectTypeCheck == TypeCheck.Yes) ? new Color(0, 0, 0, 1) : new Color(1, 1, 1, 1));
            // itSt.sp_mainMenu.texture.Apply();

            Save();
        }
    }

    void SetTempHistory(Vector3Int pos, TypeCheck tc_new)
    {
        if (pos.x != -1)
        {
            int i = pos.x;
            int j = pos.y;

            Tile tile = tile_none;

            bool isDistinct = false;
            foreach (var item in tempHistory)
            {

                isDistinct = (item.point.x == pos.x && item.point.y == pos.y);

                if (isDistinct) break;
            }

            if (!isDistinct)
            {

                PaintHistory history = new PaintHistory();
                history.beforeTypeCheck = field[i][j].selectTypeCheck;
                history.afterTypeCheck = tc_new;
                Debug.Log("type;" + field[i][j].selectTypeCheck + ":" + tc_new);
                history.isLock = false;
                history.point = new Point(pos.x, pos.y);
                tempHistory.Add(history);
            }
        }
    }

    void SetHistory(List<PaintHistory> temp)
    {
        if (temp.Count() > 0)
        {

            undoHistory.Add(new List<PaintHistory>(temp));
            B_Undo_Tile.interactable = true;
            Debug.Log("完了:" + undoHistory.Count());

            redoHistory.Clear();
            B_Redo_Tile.interactable = (redoHistory.Count() > 0);
        }

        tempHistory.Clear();
    }

    void undoTile()
    {

        Debug.Log("タイル:" + undoHistory.Count());

        if (undoHistory.Count() > 0)
        {

            List<PaintHistory> undoList = undoHistory[undoHistory.Count() - 1];

            foreach (var item in undoList)
            {

                Debug.Log(item.point.x + ":" + item.point.y + "before:" + item.beforeTypeCheck + "after:" + item.afterTypeCheck);
                SetTile(new Vector3Int(item.point.x, item.point.y, 0), item.beforeTypeCheck);
            }

            redoHistory.Add(new List<PaintHistory>(undoHistory[undoHistory.Count() - 1]));
            undoHistory.RemoveAt(undoHistory.Count() - 1);
            B_Undo_Tile.interactable = (undoHistory.Count() > 0);
            B_Redo_Tile.interactable = (redoHistory.Count() > 0);
        }
    }

    void redoTile()
    {

        Debug.Log("タイル:" + redoHistory.Count());

        if (redoHistory.Count() > 0)
        {

            List<PaintHistory> undoList = redoHistory[redoHistory.Count() - 1];

            foreach (var item in undoList)
            {

                Debug.Log(item.point.x + ":" + item.point.y + ":" + item.afterTypeCheck);
                SetTile(new Vector3Int(item.point.x, item.point.y, 0), item.afterTypeCheck);
            }

            undoHistory.Add(new List<PaintHistory>(redoHistory[redoHistory.Count() - 1]));
            redoHistory.RemoveAt(redoHistory.Count() - 1);
            B_Undo_Tile.interactable = (undoHistory.Count() > 0);
            B_Redo_Tile.interactable = (redoHistory.Count() > 0);

        }
    }

    // determine which numbers in the columns and rows are already correctly selected
    void UpdNum(int x, int y)
    {
        int width = field.Count;
        int height = field[x].Count;

        for (int i = 0; i < rows[y].Count; i++)
        {
            rows[y][i].IsOn = false;
        }

        for (int j = 0; j < cols[x].Count; j++)
        {
            cols[x][j].IsOn = false;
        }

        if (rows[y].Count > 0)
        {

            List<int> selectList = new List<int>();
            int selectCnt = 0;

            for (int i = 0; i < width; i++)
            {

                if (field[i][y].selectTypeCheck == TypeCheck.Yes)
                {

                    selectCnt++;

                    if (i == width - 1)
                    {

                        if (selectCnt > 0)
                        {

                            selectList.Add(selectCnt);
                            selectCnt = 0;
                        }
                    }
                }
                else
                {
                    if (selectCnt > 0)
                    {

                        selectList.Add(selectCnt);
                        selectCnt = 0;
                    }
                }
            }

            bool completeFlag = true;
            for (int _i = 0; _i < rows[y].Count; _i++)
            {

                if (selectList.Count > _i)
                {

                    if (rows[y][_i].num == selectList[_i])
                    {

                        rows[y][_i].IsOn = true;
                    }
                    else
                    {

                        completeFlag = false;
                    }
                }
                else
                {

                    completeFlag = false;
                }
            }

            if (completeFlag)
            {

                Debug.Log("コンプリート");
                for (int i = 0; i < width; i++)
                {
                    if (field[i][y].selectTypeCheck != TypeCheck.Yes)
                    {

                        Debug.Log("SetTile:" + y + ":" + i);
                        UpdateTile(new Vector3Int(i, y, 0), TypeCheck.Question);
                    }
                }
            }
        }

        if (cols[x].Count > 0)
        {

            List<int> selectList = new List<int>();
            int selectCnt = 0;

            for (int j = 0; j < height; j++)
            {

                if (field[x][j].selectTypeCheck == TypeCheck.Yes)
                {

                    selectCnt++;

                    if (j == height - 1)
                    {

                        if (selectCnt > 0)
                        {

                            selectList.Add(selectCnt);
                            selectCnt = 0;
                        }
                    }
                }
                else
                {

                    if (selectCnt > 0)
                    {

                        selectList.Add(selectCnt);
                        selectCnt = 0;
                    }
                }
            }

            bool completeFlag = true;
            for (int _j = 0; _j < cols[x].Count; _j++)
            {

                if (selectList.Count > _j)
                {

                    if (cols[x][_j].num == selectList[_j])
                    {

                        cols[x][_j].IsOn = true;
                    }
                    else
                    {

                        completeFlag = false;
                    }
                }
                else
                {

                    completeFlag = false;
                }
            }


            if (completeFlag)
            {

                for (int i = 0; i < height; i++)
                {
                    if (field[x][i].selectTypeCheck != TypeCheck.Yes)
                    {

                        UpdateTile(new Vector3Int(x, i, 0), TypeCheck.Question);
                    }
                }
            }
        }

        for (int i = 0; i < rows[y].Count; i++)
        {
            if (rows[y][i].IsOn)
                rows[y][i].text_num.color = Color.gray;
            else
                rows[y][i].text_num.color = Color.black;
        }

        for (int j = 0; j < cols[x].Count; j++)
        {
            if (cols[x][j].IsOn)
                cols[x][j].text_num.color = Color.gray;
            else
                cols[x][j].text_num.color = Color.black;
        }
    }

    // tap action
    void UpdMous()
    {
        if (!HasPause)
        {
            d_Time_move += Time.deltaTime;

            if (Input.touchCount == 0) // for mouse (in editor or for Windows)
            {
                if (!IsMove && !IsMoveClick && Input.GetMouseButton(0) && d_Time_move >= d_Time_move_max) // if it was clamped a certain time, then you can make a sketch of the movement
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(startPos);
                    id_click = GetIdCell(pos);
                    newTileMove = GetNewTile(id_click);


                    Debug.Log("長押し");
                    if (id_click.x != -1) // fall on the playing field
                    {
                        IsMove = true;
                        IsMoveClick = true;
                        move_napr = new Vector2(0, 0);
                    }
                }
                if (!IsMove && Input.GetMouseButton(0) && Vector3.Distance(Input.mousePosition, startPos) > 1f) // started moving
                {
                    IsMove = true;
                    IsMoveClick = false;
                }
                else if (Input.GetMouseButtonDown(0)) // just clicked
                {
                    d_Time_move = 0;
                    startPos = Input.mousePosition;
                    IsMove = false;
                    IsMoveClick = false;
                }

                if (!IsMove && Input.GetMouseButtonUp(0)) // if you quickly tapped
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(startPos);

                    id_click = GetIdCell(pos);

                    Debug.Log(id_click);
                    if (!MyRay.getInstance().HasUI(startPos)) // nothing overlaps from UI
                    {
                        if (!isOn)
                        {
                            isOn = true;
                        }
                    }

                    IsMove = true;
                }
                if (Input.GetMouseButtonUp(0))// スクリーンから指を離した
                {
                    IsMoveClick = false;

                    if (!isOn)
                    {

                        SetHistory(tempHistory);
                    }
                }

            }
            else if (Input.touchCount == 1) // for phones or tablets
            {
                if (!IsMove && !IsMoveClick && d_Time_move >= d_Time_move_max) // if it was clamped a certain time, then you can make a sketch of the movement
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));
                    id_click = GetIdCell(pos);
                    newTileMove = GetNewTile(id_click);

                    if (id_click.x != -1) // fall on the playing field
                    {
                        IsMove = true;
                        IsMoveClick = true;
                        move_napr = new Vector2(0, 0);
                    }
                }
                if (!IsMove && Input.GetTouch(0).phase == TouchPhase.Moved) // started moving
                {
                    IsMove = true;
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Began) // just clicked
                {
                    d_Time_move = 0;
                    startPos = Input.GetTouch(0).position;
                    IsMove = false;
                    IsMoveClick = false;
                }

                if (!IsMove && Input.GetTouch(0).phase == TouchPhase.Ended) // if you quickly tapped
                {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y));

                    id_click = GetIdCell(pos);

                    if (!MyRay.getInstance().HasUI(Input.GetTouch(0).position)) // UIから重複するものはありません
                    {
                        if (!isOn)
                        {
                            isOn = true;
                        }
                    }

                    IsMove = true;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Ended) // スクリーンから指を離した
                    IsMoveClick = false;
            }


            if (isOn) // single tap without movement
            {

                ClickTile(id_click);
                id_click = new Vector3Int(-1, -1, -1);
                isOn = false;
            }


            if (IsMoveClick) // sketching with taped
            {
                if (Input.touchCount == 0) // for mouse (in editor or for Windows)
                {
                    if (move_napr.x == 0 && move_napr.y == 0)
                    {
                        Vector2 v2_del = startPos - Input.mousePosition;
                        if (Math.Abs(v2_del.x) > Math.Abs(v2_del.y))
                            move_napr = new Vector2(1, 0);
                        else if (Math.Abs(v2_del.x) < Math.Abs(v2_del.y))
                            move_napr = new Vector2(0, 1);
                    }
                    Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3((move_napr.x == 1) ? Input.mousePosition.x : startPos.x
                        , (move_napr.y == 1) ? Input.mousePosition.y : startPos.y));
                    id_click = GetIdCell(pos);
                }
                else if (Input.touchCount == 1) // for phones or tablets
                {
                    if (move_napr.x == 0 && move_napr.y == 0)
                    {
                        Vector2 v2_del = new Vector2(startPos.x, startPos.y) - Input.GetTouch(0).position;
                        if (Math.Abs(v2_del.x) > Math.Abs(v2_del.y))
                            move_napr = new Vector2(1, 0);
                        else if (Math.Abs(v2_del.x) < Math.Abs(v2_del.y))
                            move_napr = new Vector2(0, 1);
                    }
                    Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3((move_napr.x == 1) ? Input.GetTouch(0).position.x : startPos.x
                        , (move_napr.y == 1) ? Input.GetTouch(0).position.y : startPos.y));
                    id_click = GetIdCell(pos);
                }

                SetTempHistory(id_click, newTileMove);
                SetTile(id_click, newTileMove);
                Debug.Log(id_click + ":SetTempHistory" + newTileMove);
            }
        }
    }

    // check whether the picture is finished and if the "true" then show the end UI
    void TheEnd()
    {
        bool b_end = true;

        for (int i = 0; i < field.Count; i++)
        {
            for (int j = 0; j < field[i].Count; j++)
            {
                if (!field[i][j].HasOK)
                {
                    b_end = false;
                    break;
                }
            }
            if (!b_end)
                break;
        }

        if (b_end)
        {
            PinchZoom.Instance().ResetPosition();
            HasPause = true;
            PanelOK.SetActive(true);

            GameSett.getInstance().EndLvl();
            if (GameSett.getInstance().GetEndLvl() >= 5)
            {
                PanelRate.SetActive(true);
            }

            // Texture2D texture_sp_mainMenu = new Texture2D(itSt.sp_mainMenu.texture.width, itSt.sp_mainMenu.texture.height, TextureFormat.RGBA32, false);
            // texture_sp_mainMenu.filterMode = FilterMode.Point;
            // texture_sp_mainMenu.SetPixels(itSt.sp_mainMenu.texture.GetPixels());
            // texture_sp_mainMenu.Apply();

            // Rect r_sp_mainMenu = new Rect(0, 0, texture_sp_mainMenu.width, texture_sp_mainMenu.height);
            // Sprite sprite_sp_mainMenu = Sprite.Create(texture_sp_mainMenu, r_sp_mainMenu, new Vector2(0.5f, 0.5f), itSt.sp_mainMenu.pixelsPerUnit);

            // ImageView.sprite = sprite_sp_mainMenu;
        }
    }

    void Update()
    {
        PinchZoom.Instance().SetPause(HasPause || IsMoveClick);

        if (!HasPause)
        {
            UpdMous();
            TheEnd();
        }
    }


    // save changes to the game
    void Save()
    {
        try
        {
            // SaveGameXML.SaveLvl(itSt.sp_mainMenu.texture, itSt.lvl);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Save - " + e.ToString());
        }
    }





    public static Game getInstance()
    {
        return instance;
    }
}
