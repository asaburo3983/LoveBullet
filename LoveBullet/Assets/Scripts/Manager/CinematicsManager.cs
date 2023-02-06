using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 演出エフェクトマネージャー
/// </summary>
public class CinematicsManager : MonoBehaviour
{
    // インスタンス化
    #region instancing

    static CinematicsManager instance = null;
    public static CinematicsManager Instance
    {
        get { return instance; }
    }

    #endregion

    // インスペクター公開変数
    #region serialized variables

    [SerializeField, Header("演出エフェクトオブジェクト一覧")]
    private GameObject[] cinematicEffectObjects;

    #endregion

    // 変数定義
    #region variables

    private GameObject targetObject;

    #endregion

    // Unityビルトイン関数
    #region unity functions

    private void Awake()
    {
        // インスタンスとシングルトン起動
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    #endregion

    // 公開関数(public)
    #region public functions

    /// <summary>
    /// 演出エフェクトを発生する
    /// </summary>
    /// <param name="_effectL">左の画像に対する演出エフェクト(対応したい画像ではないなら-1を入力)</param>
    /// <param name="_tatieL">左の画像オブジェクト</param>
    /// <param name="_effectR">右の画像に対する演出エフェクト(対応したい画像ではないなら-1を入力)</param>
    /// <param name="_tatieR">右の画像オブジェクト</param>
    public void PlayCinematicEffect(
        int _effectL, SpriteRenderer _tatieL,
        int _effectR, SpriteRenderer _tatieR)
    {
        // 左画像に対応
        if(_effectL != -1 && _effectR == -1)
        {
            // 対象オブジェクト指定
            targetObject = _tatieL.gameObject;

            // 演出エフェクトを再生
            PlayEffect(_effectL);
        }
        // 右画像に対応
        else if(_effectL == -1 && _effectR != -1)
        {
            // 対象オブジェクト指定
            targetObject = _tatieR.gameObject;

            // 演出エフェクトを再生
            PlayEffect(_effectR);
        }
    }

    public GameObject GetEffectTargetObj()
    {
        return targetObject;
    }

    #endregion

    // プライベート関数
    #region private functions

    private void PlayEffect(int _num)
    {
        Instantiate(cinematicEffectObjects[_num - 1], Vector3.zero, Quaternion.identity);
    }

    #endregion
}
