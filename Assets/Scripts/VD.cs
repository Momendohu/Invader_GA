using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VD : MonoBehaviour {
    public static readonly float CHARACTERS_SPRITE_SIZE = 0.12f; //キャラクターのサイズ
    public static readonly float EDGE_POSITION_X = 3.2f; //ステージ端の位置
    public static readonly float EDGE_POSITION_UPSIDE = 6; //ステージ端の位置(上)
    public static readonly float SHIP_SPEED = 1.2f; //プレイヤーシップのスピード
    public static readonly float SHIP_POSITION_Y = -4.1f; //プレイヤーシップの縦のポジション
    public static readonly float INVADERS_SPACE = 0.6f; //インベーダー間のすきまの広さ
    public static readonly int INVADER_NUM_X = 9; //インベーダの横の数
    public static readonly int INVADER_NUM_Y = 6; //インベーダの横の数
    public static readonly float INVADER_SPEED = 10000; //インベーダーのスピード
    public static readonly float SHIP_BULLET_SPEED = 0.1f; //プレイヤーシップの弾のスピード
    public static readonly float PREPARATION_INTERVAL_LENGTH = 1f; //ウェーブ間の時間の長さ
    public static readonly float DEFENSE_LINE_Y = -3.7f; //防衛ライン
    public static readonly int BULLET_NUM_LIMIT = 50; //弾数制限

    public enum MOVE_DIRECTION { LEFT = -1, RIGHT = 1 }; //移動方向
    public enum SHIP_MODE { PLAY = 0, LEARN = 1, RANDOM = 2 }; //プレイヤーシップのモード

    public enum INVADER_SCORE { INV1 = 10, INV2 = 20, INV3 = 30, UFO1 = 50, UFO2 = 100, UFO3 = 150, UFO4 = 300 }; //スコア

    public static int GENE_LENGTH=3000; //遺伝子の長さ
    public static readonly int CROSS_OVER_POINT_1 = 200; //交叉点
    public static readonly int CROSS_OVER_POINT_2 = 300; //交叉点
}