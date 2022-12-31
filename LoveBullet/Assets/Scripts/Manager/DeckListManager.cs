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
                //内部データをクリアして
                //デッキリスト内にあるカードを生成
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
                //オブジェクトの位置をソートする
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
    //デッキ内のカードを表示する
    void SortPos(Sort sort)
    {
        //カードをデッキリストに入っている順番で配置する
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
