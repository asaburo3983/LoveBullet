using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    // 生成後のエフェクトオブジェクトのリスト
    private GameObject effect;
    private List<GameObject> effectList = new List<GameObject>();

    public enum EffectSide
    {
        effL,
        effR
    }
    private GameObject targetObject;
    private EffectSide effectSide;

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
            effectSide = EffectSide.effL;

            // 演出エフェクトを再生
            PlayEffect(_effectL);
        }
        // 右画像に対応
        else if(_effectL == -1 && _effectR != -1)
        {
            // 対象オブジェクト指定
            targetObject = _tatieR.gameObject;
            effectSide = EffectSide.effR;

            // 演出エフェクトを再生
            PlayEffect(_effectR);
        }
    }

    // エフェクトオブジェクトを取得
    public GameObject GetEffectObj()
    {
        if (effect != null) return effect;
        else return null;
    }
    // ターゲットされるオブジェクト取得
    public GameObject GetEffectTargetObj()
    {
        return targetObject;
    }
    // ターゲットされる側を取得
    public EffectSide GetEffectSide()
    {
        return effectSide;
    }
    // 存在してるエフェクトがあれば
    public void RemoveEffect()
    {
        if (effectList.Count != 0)
        {
            foreach (var eff in effectList)
            {
                // 画面上に残るタイプのエフェクトの場合のみ実行する処理
                if (eff.GetComponent<CE_Base>().GetEffectType() == CE_Base.Type.Stay)
                {
                    eff.GetComponent<CE_Base>().FadeOut();
                }
            }
            effectList.Clear();
        }
    }

    #endregion

    // プライベート関数
    #region private functions

    private void PlayEffect(int _num)
    {
        effect = Instantiate(cinematicEffectObjects[_num - 1], Vector3.zero, Quaternion.identity);
        // 残るタイプであればリストに追加
        if (effect.GetComponent<CE_Base>().GetEffectType() == CE_Base.Type.Stay)
        {
            effectList.Add(effect);
        }
    }

    #endregion
}
