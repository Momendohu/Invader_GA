using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {
    //==========================================================================

    //==========================================================================
    //コンポーネント
    private SoundManager soundManager;

    //==========================================================================
    //コンポーネント参照
    private void CRef () {
        soundManager = transform.Find("SoundManager").GetComponent<SoundManager>();
    }

    //==========================================================================
    //初期化
    private void Init () {
        CRef();
    }

    //==========================================================================
    private void Awake () {
        Init();
        soundManager.Trigger(0,true);
    }

    private void Start () {
    }

    private void Update () {
    }
}