using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using System;

public class BulletUI : MonoBehaviour
{
    [SerializeField] List<Card.Card> cards = new List<Card.Card>();

    [SerializeField] List<Vector3> firstPos = new List<Vector3>();
    [SerializeField] float next, other;

    private void Start()
    {
        foreach (var c in cards) {
            firstPos.Add(c.transform.parent.position);
        }
        next = cards[0].transform.parent.localScale.x;
        other = cards[1].transform.parent.localScale.x;

        // �e���̃J�[�h�̏��Ԃ��ύX���ꂽ���ɏ���
        Card.Fight.instance.gunInCardsReactive.ObserveMove().Subscribe(x => {

            // ���݂̃J�[�h�����ɓ�����
            cards[0].transform.parent.DOMoveY(cards[0].transform.position.y - 500, 0.1f).OnComplete(() => {

                // ��ʊO�ɓ�������ʒu��␳���Č��ɖ߂�
                cards[0].transform.parent.localScale = new Vector3(other, other, other);
                cards[0].transform.parent.position = firstPos[5] - new Vector3(0, 500, 0);

                // �J�[�h���X�V
                cards[0].Initialize(Card.Fight.instance.GunInCards[5]);

                // �������z��̏��Ԃ�ύX����
                cards[0].transform.parent.DOMove(firstPos[5], 0.1f).OnComplete(() => {
                    (cards[0], cards[1], cards[2], cards[3], cards[4], cards[5]) =
                    (cards[1], cards[2], cards[3], cards[4], cards[5], cards[0]);
                });
            });

            // ���̑��̃J�[�h������炷
            for (int i = 1; i < 6; i++) {
                cards[i].transform.parent.DOMove(firstPos[i - 1], 0.1f);
            }
            cards[1].transform.parent.DOScale(next, 0.1f);

        }).AddTo(this);
        Card.Fight.instance.gunInCardsReactive.ObserveReset().Subscribe(x => {
            DOVirtual.DelayedCall(0.1f,()=> InitCard());
        }).AddTo(this);

        InitCard();
    }

    void InitCard()
    {
        for (int i = 0; i < 6; i++) {
            cards[i].Initialize(Card.Fight.instance.GunInCards[i]);
        }
    }

}