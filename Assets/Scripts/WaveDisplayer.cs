using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveDisplayer : MonoBehaviour {
    //==========================================================================
    public AnimationCurve anim_Image;

    //==========================================================================
    //コンポーネント
    private Image image;
    private Text text;
    private InvaderManager invaderManager;

    //==========================================================================
    //コンポーネント参照
    private void CRef () {
        image = GetComponent<Image>();
        text = transform.Find("Text").GetComponent<Text>();
        invaderManager = GameObject.Find("Manager/InvaderManager").GetComponent<InvaderManager>();
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
        StartCoroutine(MoveImage());
    }

    private void Update () {
        text.text = "Wave " + invaderManager.Generation + "." + invaderManager.Wave;
    }

    //==========================================================================
    private IEnumerator MoveImage () {
        float time = 0;

        while(true) {
            time += Time.deltaTime * VD.PREPARATION_INTERVAL_LENGTH;
            transform.localScale = new Vector3(1,anim_Image.Evaluate(time),1);


            if(time >= 2) {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(this.gameObject);
        yield break;
    }
}