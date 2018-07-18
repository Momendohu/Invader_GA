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
    //private TextController textController;
    private Ship ship;

    //==========================================================================
    //コンポーネント参照
    private void CRef () {
        scoreManager = GameObject.Find("Manager/ScoreManager").GetComponent<ScoreManager>();
        //textController = GameObject.Find("TextController").GetComponent<TextController>();
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
                        Wave++;
                        if(Wave >= 10) {
                            Wave = 0;
                            Generation++;
                        }
                    } else {
                        wasInitialilzedFirst = true;
                        Wave = 0;
                        Generation = 0;
                    }

                    scoreManager.Score = 0;
                    scoreManager._Time = 0;
                    IsPreparationInterval = true;
                    CreateWaveDisplayer();
                    CreateInvaders();
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
    private void CreateInvaders () {
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
}