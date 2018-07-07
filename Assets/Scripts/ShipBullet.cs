using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBullet : MonoBehaviour {
    //==========================================================================

    //==========================================================================
    //コンポーネント

    //==========================================================================
    //コンポーネント参照
    private void CRef () {
    }

    //==========================================================================
    //初期化
    private void Init () {
    }

    //==========================================================================
    private void Awake () {

    }

    private void Start () {

    }

    private void Update () {
        transform.position += Vector3.up * VD.SHIP_BULLET_SPEED;

        if(transform.position.y > VD.EDGE_POSITION_UPSIDE) {
            Destroy(this.gameObject);
        }
    }
}