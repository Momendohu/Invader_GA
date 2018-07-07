using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// インベーダーの管理
/// </summary>
public class InvaderManager : MonoBehaviour {
    //==========================================================================
    public bool IsInvederTouchedEdge; //インベーダが端にタッチしたかどうか
    public int InvaderNum; //インベーダーの数
    public int Wave;
    public int Generation;

    public bool IsPreparationInterval;

    private bool wasInitialilzedFirst;

    //==========================================================================
    //コンポーネント
    private ScoreManager scoreManager;
    private TextController textController;
    private Ship ship;

    //==========================================================================
    //コンポーネント参照
    private void CRef () {
        scoreManager = GameObject.Find("Manager/ScoreManager").GetComponent<ScoreManager>();
        textController = GameObject.Find("TextController").GetComponent<TextController>();
        ship = GameObject.Find("Ship").GetComponent<Ship>();
    }

    //==========================================================================
    //初期化
    private void Init () {
        CRef();
    }

    //==========================================================================
    private void Awake () {
        Init();
    }

    private void Start () {
        StartCoroutine(SendInvadersShiftAction());
        StartCoroutine(CheckInitialize());
    }

    private void Update () {

    }

    //==========================================================================
    private IEnumerator CheckInitialize () {
        while(true) {
            //Debug.Log("Wave " + Wave + ":InvaderNum " + InvaderNum + ":Annihilate " + IsIndaverAnnihilatedAll());
            if(InvaderNum <= 0) {
                if(IsIndaverAnnihilatedAll()) {
                    //バグ回避
                    if(InvaderNum < 0) {
                        InvaderNum = 0;
                    }

                    if(wasInitialilzedFirst) {
                        yield return GameObject.Find("Ship").GetComponent<Ship>().SaveGene();
                        Wave++;
                        if(Wave >= 10) {
                            Wave = 0;
                            Generation++;
                            if(ship.Mode == (int)VD.SHIP_MODE.LEARN) Learn();
                        }
                    } else {
                        wasInitialilzedFirst = true;
                        Wave = textController.GetDataSize() % 10;
                        Generation = (int)Mathf.Floor(textController.GetDataSize() / 10);

                        //世代交代のタイミングなら
                        if(Generation >= 1 && Wave == 0) {
                            if(ship.Mode == (int)VD.SHIP_MODE.LEARN) Learn();
                        }
                    }



                    scoreManager.Score = 0;
                    scoreManager._Time = 0;
                    IsPreparationInterval = true;
                    CreateWaveDisplayer();
                    CreateInvaders(Wave);
                }
            }
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    //==========================================================================
    //インベーダを全員殲滅したかどうか
    private bool IsIndaverAnnihilatedAll () {
        for(int i = 0;i < VD.INVADER_NUM_X * VD.INVADER_NUM_Y;i++) {
            if(transform.Find("" + i) != null) {
                return false;
            }
        }

        return true;
    }

    //==========================================================================
    //インベーダにシフト命令をだす
    private IEnumerator SendInvadersShiftAction () {
        while(true) {
            if(IsInvederTouchedEdge) {
                for(int i = 0;i < VD.INVADER_NUM_X * VD.INVADER_NUM_Y;i++) {
                    if(transform.Find("" + i) != null) {
                        transform.Find("" + i).GetComponent<Invader>().ShiftDownFlag = true;
                    }
                }
                IsInvederTouchedEdge = false;
            }
            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    //==========================================================================
    //インベーダを複数生成する
    //_wave -> ウェーブ
    private void CreateInvaders (int _wave) {
        for(int j = 0;j < VD.INVADER_NUM_Y;j++) {
            for(int i = 0;i < VD.INVADER_NUM_X;i++) {
                GameObject obj = Instantiate(Resources.Load("P/Invader")) as GameObject;
                obj.transform.position = new Vector3(VD.INVADERS_SPACE * (i - (int)(VD.INVADER_NUM_X / 2)),VD.INVADERS_SPACE * j,0);
                obj.transform.SetParent(this.transform,false);
                obj.name = "" + (i + j * VD.INVADER_NUM_X);

                obj.GetComponent<Invader>().Type = Mathf.FloorToInt(j / 2);

                InvaderNum++;
            }
        }
    }

    //==========================================================================
    private void CreateWaveDisplayer () {
        GameObject obj = Instantiate(Resources.Load("P/WaveDisplayer")) as GameObject;
        obj.transform.SetParent(GameObject.Find("Canvas").transform,false);
    }

    //==========================================================================
    int[] crossedOverGene1_1 = new int[VD.CROSS_OVER_POINT_1];
    int[] crossedOverGene1_2 = new int[VD.CROSS_OVER_POINT_2];
    int[] crossedOverGene1_3 = new int[VD.GENE_LENGTH - VD.CROSS_OVER_POINT_1 - VD.CROSS_OVER_POINT_2];

    int[] crossedOverGene2_1 = new int[VD.CROSS_OVER_POINT_1];
    int[] crossedOverGene2_2 = new int[VD.CROSS_OVER_POINT_2];
    int[] crossedOverGene2_3 = new int[VD.GENE_LENGTH - VD.CROSS_OVER_POINT_1 - VD.CROSS_OVER_POINT_2];

    //2点交叉
    private void Learn () {
        int[] order = { 0,1,2,3,4,5,6,7,8,9 };
        int tmp = -1;
        bool sorted = true;

        int loopBreak = 0;

        textController.ReadText(); //テキストを再ロード

        while(true) {
            sorted = true;
            for(int i = 0;i < 10 - 1;i++) {
                //Debug.Log(textController.GetData((Generation - 1) * 10 + i,1));
                //Debug.Log(textController.GetData((Generation - 1) * 10 + i + 1,1));
                //Debug.Log("GENERATION " + Generation);
                float a = float.Parse(textController.GetData((Generation - 1) * 10 + i,1));
                float b = float.Parse(textController.GetData((Generation - 1) * 10 + i + 1,1));

                //Debug.Log("a " + a);
                //Debug.Log("b " + b);
                //Debug.Log("GENERATION " + Generation);

                if(a > b) {
                    tmp = order[i];
                    order[i] = order[i + 1];
                    order[i + 1] = tmp;
                    sorted = false;
                }
            }

            if(sorted) {
                break;
            }

            loopBreak++;
            if(loopBreak >= 1000) {
                Debug.Log("LOOP RUN TOO MANY TIMES.");
                break;
            }
        }

        for(int i = 0;i < VD.CROSS_OVER_POINT_1;i++) {
            crossedOverGene1_1[i] = int.Parse(textController.GetData((Generation - 1) * 10 + order[0],i + 2));
            crossedOverGene2_1[i] = int.Parse(textController.GetData((Generation - 1) * 10 + order[1],i + 2));

            GameObject.Find("Ship").GetComponent<Ship>().Genes[0,i] = crossedOverGene1_1[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[1,i] = crossedOverGene1_1[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[2,i] = crossedOverGene1_1[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[3,i] = crossedOverGene1_1[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[4,i] = crossedOverGene2_1[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[5,i] = crossedOverGene2_1[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[6,i] = crossedOverGene2_1[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[7,i] = crossedOverGene2_1[i];
        }

        for(int i = VD.CROSS_OVER_POINT_1;i < VD.CROSS_OVER_POINT_1 + VD.CROSS_OVER_POINT_2;i++) {
            crossedOverGene1_2[i] = int.Parse(textController.GetData((Generation - 1) * 10 + order[0],i + 2));
            crossedOverGene2_2[i] = int.Parse(textController.GetData((Generation - 1) * 10 + order[1],i + 2));

            GameObject.Find("Ship").GetComponent<Ship>().Genes[0,i] = crossedOverGene1_2[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[1,i] = crossedOverGene1_2[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[2,i] = crossedOverGene2_2[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[3,i] = crossedOverGene2_2[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[4,i] = crossedOverGene1_2[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[5,i] = crossedOverGene1_2[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[6,i] = crossedOverGene2_2[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[7,i] = crossedOverGene2_2[i];
        }

        for(int i = VD.CROSS_OVER_POINT_1 + VD.CROSS_OVER_POINT_2;i < VD.GENE_LENGTH;i++) {
            crossedOverGene1_3[i] = int.Parse(textController.GetData((Generation - 1) * 10 + order[0],i + 2));
            crossedOverGene2_3[i] = int.Parse(textController.GetData((Generation - 1) * 10 + order[1],i + 2));

            GameObject.Find("Ship").GetComponent<Ship>().Genes[0,i] = crossedOverGene1_3[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[1,i] = crossedOverGene2_3[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[2,i] = crossedOverGene1_3[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[3,i] = crossedOverGene2_3[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[4,i] = crossedOverGene1_3[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[5,i] = crossedOverGene2_3[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[6,i] = crossedOverGene1_3[i];
            GameObject.Find("Ship").GetComponent<Ship>().Genes[7,i] = crossedOverGene2_3[i];
        }

        for(int i = 0;i < VD.GENE_LENGTH;i++) {
            if(Random.Range(0,1) <= 0.9f) {
                GameObject.Find("Ship").GetComponent<Ship>().Genes[8,i] = int.Parse(textController.GetData((Generation - 1) * 10 + order[0],i + 2));
            } else {
                GameObject.Find("Ship").GetComponent<Ship>().Genes[8,i] = Random.Range(0,3);
            }

            if(Random.Range(0,1) <= 0.9f) {
                GameObject.Find("Ship").GetComponent<Ship>().Genes[9,i] = int.Parse(textController.GetData((Generation - 1) * 10 + order[1],i + 2));
            } else {
                GameObject.Find("Ship").GetComponent<Ship>().Genes[9,i] = Random.Range(0,3);
            }
        }
    }
}