using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    //==========================================================================
    public int Score; //スコア
    public float _Time; //時間

    //==========================================================================
    //コンポーネント
    private GameObject score_Canv;
    private GameObject time_Canv;
    private InvaderManager invaderManager;

    //==========================================================================
    //コンポーネント参照
    private void CRef () {
        score_Canv = GameObject.Find("Canvas/Score");
        time_Canv = GameObject.Find("Canvas/Time");
        invaderManager = GameObject.Find("InvaderManager").GetComponent<InvaderManager>();
    }

    //==========================================================================
    //初期化
    private void Init () {
        CRef();

        Score = 0;
        _Time = 0;
    }

    //==========================================================================
    private void Awake () {
        Init();
    }

    private void Start () {

    }

    private void Update () {
        if(invaderManager.IsPreparationInterval==false) _Time += Time.deltaTime;

        score_Canv.GetComponent<Text>().text = "SCORE " + Score;
        time_Canv.GetComponent<Text>().text = "TIME " + (int)_Time;
    }
}