using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {
    //==========================================================================
    public Sprite[] Skins; //見た目
    public bool DestroyFlag; //破壊フラグ
    public int Mode; //操縦モード

    private bool onceDestroy;//一回だけ破壊処理する

    private int FrameNum; //フレーム数

    //==========================================================================
    //コンポーネント
    private SoundManager soundManagerSE;
    private InvaderManager invaderManager;
    private SpriteRenderer spriteRenderer;
    private TextController textController;
    private ScoreManager scoreManager;

    //==========================================================================
    //コンポーネント参照
    private void CRef () {
        soundManagerSE = GameObject.Find("Manager/SoundManagerSE").GetComponent<SoundManager>();
        invaderManager = GameObject.Find("Manager/InvaderManager").GetComponent<InvaderManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        textController = GameObject.Find("TextController").GetComponent<TextController>();
        scoreManager = GameObject.Find("Manager/ScoreManager").GetComponent<ScoreManager>();
    }

    //==========================================================================
    //初期化
    private void Init () {
        CRef();

        Mode = (int)VD.SHIP_MODE.PLAY;
    }

    //==========================================================================
    private void Awake () {
        Init();
    }

    private void Start () {

    }

    private void Update () {
        if(invaderManager.IsPreparationInterval == false) {
            if(DestroyFlag == false) {
                bool checkLeft = transform.position.x <= -VD.EDGE_POSITION_X;
                bool checkRight = transform.position.x >= VD.EDGE_POSITION_X;

                switch(Mode) {
                    case (int)VD.SHIP_MODE.PLAY:
                    if(Input.GetKey(KeyCode.LeftArrow)) {
                        MoveLeft();
                    }

                    if(Input.GetKey(KeyCode.RightArrow)) {
                        MoveRight();
                    }

                    if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
                        ShotBullet();
                    }

                    if(!(checkLeft && checkRight)) {
                        break;
                    }

                    if(checkRight) {
                        transform.position = new Vector3(VD.EDGE_POSITION_X,VD.SHIP_POSITION_Y,0);
                        break;
                    }

                    if(checkLeft) {
                        transform.position = new Vector3(VD.EDGE_POSITION_X,VD.SHIP_POSITION_Y,0);
                        break;
                    }

                    break;

                    case (int)VD.SHIP_MODE.LEARN:

                    break;

                    case (int)VD.SHIP_MODE.RANDOM:
                    int rand2 = Random.Range(0,4);
                    switch(rand2) {
                        case 0:
                        MoveLeft();
                        break;

                        case 1:
                        MoveRight();
                        break;

                        case 2:
                        ShotBullet();
                        break;

                        case 3:
                        break;
                    }

                    if(!(checkLeft && checkRight)) {
                        break;
                    }

                    if(checkRight) {
                        transform.position = new Vector3(VD.EDGE_POSITION_X,VD.SHIP_POSITION_Y,0);
                        break;
                    }

                    if(checkLeft) {
                        transform.position = new Vector3(VD.EDGE_POSITION_X,VD.SHIP_POSITION_Y,0);
                        break;
                    }

                    break;
                }

                FrameNum++;
            }

            if(DestroyFlag && onceDestroy == false) {
                Debug.Log("FrameNum:" + FrameNum);
                onceDestroy = true;
                StartCoroutine(DestroyRoutine());
            }
        } else {
            //弾を全除去
            GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
            for(int i = bullets.Length - 1;i >= 0;i--) {
                Destroy(bullets[i]);
            }

            FrameNum = 0;
            transform.position = new Vector3(0,VD.SHIP_POSITION_Y,0);
        }
    }

    //==========================================================================
    //右に移動
    private void MoveRight () {
        if(!(transform.position.x >= VD.EDGE_POSITION_X)) {
            transform.position += Vector3.right * VD.CHARACTERS_SPRITE_SIZE * (int)VD.MOVE_DIRECTION.RIGHT * VD.SHIP_SPEED;
        } else {
            transform.position = new Vector3(VD.EDGE_POSITION_X,VD.SHIP_POSITION_Y,0);
        }
    }

    //==========================================================================
    //左に移動
    private void MoveLeft () {
        if(!(transform.position.x <= -VD.EDGE_POSITION_X)) {
            transform.position += Vector3.right * VD.CHARACTERS_SPRITE_SIZE * (int)VD.MOVE_DIRECTION.LEFT * VD.SHIP_SPEED;
        } else {
            transform.position = new Vector3(-VD.EDGE_POSITION_X,VD.SHIP_POSITION_Y,0);
        }
    }

    //==========================================================================
    //弾を発射する
    private void ShotBullet () {
        if(GameObject.FindGameObjectsWithTag("Bullet").Length < VD.BULLET_NUM_LIMIT) {
            GameObject obj = Instantiate(Resources.Load("P/Bullet")) as GameObject;
            obj.transform.position = this.transform.position;
            soundManagerSE.TriggerSE(0);
        }
    }

    //==========================================================================
    //破壊時のルーチン
    private IEnumerator DestroyRoutine () {
        float time = 0;

        while(true) {
            time += Time.deltaTime * 10;
            spriteRenderer.sprite = Skins[(int)time % 2 + 1];

            if(time >= 20) {
                //インベーダーを全除去
                GameObject[] invaders = GameObject.FindGameObjectsWithTag("Invader");
                for(int i = invaders.Length - 1;i >= 0;i--) {
                    Destroy(invaders[i]);
                }
                invaderManager.InvaderNum = 0;
                onceDestroy = false;
                DestroyFlag = false;
                spriteRenderer.sprite = Skins[0];

                break;
            }

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}