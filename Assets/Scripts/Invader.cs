using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invader : MonoBehaviour {
    //==========================================================================
    public Sprite[] Skins; //見た目
    public int Type; //タイプ
    public bool ShiftDownFlag; //下にシフトするフラグ

    private int move_direction; //進む方向
    private bool checkDirectionFlag;
    private bool sleepChangeDirection; //方向転換を1ターン待つために使用
    private bool removeFlag; //消滅判定

    private bool WasMovedFirst; //最初の動きを行ったかどうか

    //==========================================================================
    //コンポーネント
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private InvaderManager invaderManager;
    private ScoreManager scoreManager;
    private SoundManager soundManagerSE;
    private Ship ship;

    //==========================================================================
    //コンポーネント参照
    private void CRef () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        invaderManager = GameObject.Find("InvaderManager").GetComponent<InvaderManager>();
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        soundManagerSE = GameObject.Find("Manager/SoundManagerSE").GetComponent<SoundManager>();
        ship = GameObject.Find("Ship").GetComponent<Ship>();
    }

    //==========================================================================
    //初期化
    private void Init () {
        CRef();

        move_direction = (int)VD.MOVE_DIRECTION.LEFT;
    }

    //==========================================================================
    private void Awake () {
        Init();
    }

    private void Start () {
        StartCoroutine(Move());
    }

    private void LateUpdate () {
        if(checkDirectionFlag) {
            if(ShiftDownFlag) {
                transform.position += Vector3.down * VD.CHARACTERS_SPRITE_SIZE * 2;
                move_direction *= -1;
                ShiftDownFlag = false;
            } else {
                transform.position += Vector3.right * VD.CHARACTERS_SPRITE_SIZE * move_direction;
            }

            checkDirectionFlag = false;
        }
    }

    //==========================================================================
    //動作処理
    private IEnumerator Move () {
        Vector3 prePos = transform.position;

        float time = 0; //状態遷移管理時間
        int skinNum = 0; //見た目制御用

        ChangeSkin(skinNum);

        while(true) {
            if(WasMovedFirst) {
                time += Time.deltaTime * VD.INVADER_SPEED;

                //消滅判定が出たら
                if(removeFlag) {
                    checkDirectionFlag = false;
                    ChangeSkin(2);
                    boxCollider2D.enabled = false;
                    yield return new WaitForSeconds(0.1f);
                    Destroy(this.gameObject);
                }

                //防衛ラインを割ったら
                if(transform.position.y <= VD.DEFENSE_LINE_Y) {
                    ship.DestroyFlag = true;
                }

                if(time >= 1) {

                    //見た目制御用変数を0と1で切り替える
                    skinNum++;
                    if(skinNum >= 2) {
                        skinNum = 0;
                    }

                    ChangeSkin(skinNum);

                    switch(CheckDirection()) {
                        case 1:
                        invaderManager.IsInvederTouchedEdge = true;
                        break;

                        case 2:
                        invaderManager.IsInvederTouchedEdge = true;
                        break;

                        default:
                        sleepChangeDirection = false;
                        break;
                    }

                    checkDirectionFlag = true;

                    time = 0;
                }
            } else {
                time += Time.deltaTime * 0.5f * VD.PREPARATION_INTERVAL_LENGTH;
                transform.position = Vector3.Lerp(prePos + new Vector3(0,10,0),prePos,time);

                if(time >= 1) {
                    time = 0;
                    invaderManager.IsPreparationInterval = false;
                    WasMovedFirst = true;
                }
            }

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    //==========================================================================
    //見た目を変える
    //state 進行時、消滅時に見た目を変えるため(0,1進行時 2消滅時)
    private void ChangeSkin (int state) {
        switch(state) {
            case 0:
            spriteRenderer.sprite = Skins[Type * 2];
            break;

            case 1:
            spriteRenderer.sprite = Skins[Type * 2 + 1];
            break;

            case 2:
            spriteRenderer.sprite = Skins[7];
            break;
        }
    }

    //==========================================================================
    //方向を変える
    private int CheckDirection () {
        if(!sleepChangeDirection) {
            if(transform.position.x + VD.CHARACTERS_SPRITE_SIZE * move_direction < -VD.EDGE_POSITION_X) {
                sleepChangeDirection = true;
                return 1;
            }

            if(transform.position.x + VD.CHARACTERS_SPRITE_SIZE * move_direction > VD.EDGE_POSITION_X) {
                sleepChangeDirection = true;
                return 2;
            }
        }

        return 0;
    }

    //==========================================================================
    private void OnTriggerEnter2D (Collider2D collision) {
        if(collision.tag == "Bullet") {
            switch(Type) {
                case 0:
                scoreManager.Score += (int)VD.INVADER_SCORE.INV1;
                break;

                case 1:
                scoreManager.Score += (int)VD.INVADER_SCORE.INV2;
                break;

                case 2:
                scoreManager.Score += (int)VD.INVADER_SCORE.INV3;
                break;

                default:
                break;
            }

            removeFlag = true;

            soundManagerSE.TriggerSE(1);
            invaderManager.InvaderNum--;

            Destroy(collision.gameObject);
        }
    }
}