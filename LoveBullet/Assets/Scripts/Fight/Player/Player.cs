using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UniRx;

[System.Serializable]
public struct ReductionRate
{
    [Header("0% ~ 100%"), Header("�f�o�t���̔{��")]
    [Range(0, 100)] public int AT;
    [Header("100% ~ 200%"), Range(100, 200)] public int DF;
}

public enum RecieveDamageReaction
{
    EnemyAttack,
    Guard,
    Poison,
    Self,
}

public class Player : SingletonMonoBehaviour<Player>
{
    [SerializeField] ReductionRate rate;
    public ReductionRate Rate => rate;

    [System.Serializable]
    public struct InGameState
    {
        public IntReactiveProperty Atk;
        public IntReactiveProperty Atk_Never;
        public IntReactiveProperty Def;
        public IntReactiveProperty Def_Never;
        public IntReactiveProperty ATWeaken;
        public IntReactiveProperty DFWeaken;

        public int reloadAP;
        public int cockingAP;

        public int freeCocking;
    }
    public InGameState gameState = new InGameState();


    // TweenAnimation
    Tween fireTw = null;
    [SerializeField] float attackMovePosX;
    [SerializeField] float attackMoveSpeed;
    [SerializeField] float receiveMovePosX;
    [SerializeField] float receiveMoveSpeed;

    BandManager bandMana;

    [Header("�v���C���[�A�j��")]
    [SerializeField] Animator anim;

    private void Awake()
    {
        SingletonCheck(this);
    }
    private void Start()
    {
        bandMana = BandManager.instance;
        bandMana.playerHP.Pairwise().Subscribe(x => {
            // 0�ȏォ��0�ȉ��ɂȂ����ꍇ���S����
            if (x.Current <= 0 && x.Previous > 0)
            {
                Debug.Log("���S�����@���쐬");
            }
        }).AddTo(this);
    }
    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// DF�Ǘ�
    /// </summary>
    public void ResetDF()
    {
        instance.gameState.Def.Value = 0;
    }
    public void PlusDF(int _value)
    {
        instance.gameState.Def.Value += _value;
    }
    public void MinusDF(int _value)
    {
        instance.gameState.Def.Value -= _value;
    }
    public void ReceiveDamage(int _damage)
    {
        var state = instance.gameState;
        var dmg = _damage - state.Def.Value;
        if (dmg > 0) {
            bandMana.playerHP.Value -= dmg;
        }
        state.Def.Value = Mathf.Clamp(state.Def.Value - _damage, 0, 9999);

        //�U�����󂯂��ꍇ�̂݃_���[�W���󂯂�
        if (dmg > 0)
        {
            ReceiveAnim();
        }
    }

    public void IdleAnim()
    {
        anim.SetInteger("AnimeNum", 0);
    }
    public void ShotAnim()
    {
        anim.SetInteger("AnimeNum", 1);

        // �v���C���[�A�j���[�V����
        if (fireTw != null) fireTw.Kill(true);
        var pi = Player.instance;
        fireTw = Player.instance.transform.DOMoveX(attackMovePosX, attackMoveSpeed).SetLoops(2, LoopType.Yoyo).OnComplete(() => fireTw = null);
    }
    public void ReloadAnim()
    {
        anim.SetInteger("AnimeNum", 2);
    }
    public void DeadAnim()
    {
        anim.SetInteger("AnimeNum", 3);
    }

    // ��_���A�j���[�V����
    Sequence seq;
    public void ReceiveAnim()
    {
        if (seq != null) { seq.Kill(true); }
        seq = DOTween.Sequence()
            .Append(transform.DOMoveX(receiveMovePosX, receiveMoveSpeed).SetLoops(2, LoopType.Yoyo))
            .Join(transform.GetChild(0).GetComponent<SpriteRenderer>().DOColor(new Color(1, 0, 0, 1), 0.1f).SetLoops(2, LoopType.Yoyo))
            .OnComplete(() => seq = null);

    }



}
