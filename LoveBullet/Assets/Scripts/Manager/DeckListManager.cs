using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class DeckListManager : MonoBehaviour {

    [SerializeField] GameObject canvas;
    [SerializeField] GameObject cardBase;
    [SerializeField] Vector2 cardLeftUpPos;
    [SerializeField] Vector2 cardPosDist;

    enum Sort
    {
        NONE=0
    }
    // Start is called before the first frame update

    Card.Fight fight;

    ReactiveProperty<int> deckCount = new ReactiveProperty<int>();
    List<GameObject> cardListObject = new List<GameObject>();
    private void Awake()
    {
        fight = Card.Fight.instance;

        deckCount.Subscribe(x =>
        {
                //�����f�[�^���N���A����
                //�f�b�L���X�g���ɂ���J�[�h�𐶐�
                for (int i = 0; i < cardListObject.Count; i++)
            {
                Destroy(cardListObject[i]);
            }
            cardListObject.Clear();

            foreach (var card in fight.deckList)
            {
                var obj = Instantiate(cardBase, canvas.transform);
                var sc = obj.GetComponent<Card.CanvasCard>();
                sc.Initialize(card);
                cardListObject.Add(obj);
            }
                //�I�u�W�F�N�g�̈ʒu���\�[�g����
                SortPos(Sort.NONE);
        }).AddTo(this);
    }
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        deckCount.Value = fight.deckList.Count;
    }
    //�f�b�L���̃J�[�h��\������
    void SortPos(Sort sort)
    {
        //�J�[�h���f�b�L���X�g�ɓ����Ă��鏇�ԂŔz�u����
        if (sort == Sort.NONE)
        {
            for(int i = 0; i < cardListObject.Count; i++)
            {
                var pos = new Vector2(
                    cardLeftUpPos.x + (i % 5) * cardPosDist.x,
                    cardLeftUpPos.y + (i / 5) * cardPosDist.y
                    );

                cardListObject[i].transform.position = pos;
            }
        }
    }

    public void OnActive()
    {
        canvas.SetActive(true);
        SortPos(Sort.NONE);
    }
    public void OffActive()
    {
        canvas.SetActive(false);
    }
}
