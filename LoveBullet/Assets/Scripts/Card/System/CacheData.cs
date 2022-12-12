using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CacheData : SingletonMonoBehaviour<CacheData>
{

    Database.Load loadDB;
    public List<Card.State> cardStates;

    // Start is called before the first frame update
    void Start()
    {
        if (SingletonCheck(this, false))
        {
            loadDB = Database.Load.instance;

            CacheCard();
        }
    }
    void CacheCard()
    {
        var db = loadDB.GetDatabase(Database.Value.Card);
        var cmd = "SELECT * FROM Cards";
        //Genre,Type,Explanation,AP,AT,DF,ATWeaken,DFWeaken
        var table = db.ExecuteQuery(cmd);

        foreach (var row in table.Rows)
        {
            Card.State state=new Card.State();
            state.genre = (Card.GENRE)row["Genre"];
            state.type = (Card.TYPE)row["Type"];
            state.name = (string)row["Name"];
            state.AP = (int)row["AP"];
            state.AT = (int)row["AT"];
            state.DF = (int)row["DF"];
            state.ATWeaken = (int)row["ATWeaken"];
            state.DFWeaken = (int)row["DFWeaken"];
            state.value.Add((int)row["Value0"]);
            state.value.Add((int)row["Value1"]);
            state.value.Add((int)row["Value2"]);
            state.value.Add((int)row["Value3"]);
            state.value.Add((int)row["Value4"]);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

}