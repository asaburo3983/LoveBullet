using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CacheData : SingletonMonoBehaviour<CacheData>
{

    Database.Load loadDB;
    public List<Card.Card.State> cardStates=new List<Card.Card.State>();
    public List<Enemy.Enemy.State> enemyStates=new List<Enemy.Enemy.State>();
    public List<Enemy.ActivePattern> enemyActivePattern=new List<Enemy.ActivePattern>();
    public List<Enemy.AddventPattern> enemyAddventPattern=new List<Enemy.AddventPattern>();


    // Start is called before the first frame update
    void Awake()
    {
        if (SingletonCheck(this))
        {
            loadDB = Database.Load.instance;

            CacheCard();
            CacheEnemy();
            CacheEnemyActivePattern();
            CacheEnemyAddventPattern();
        }
    }
    /// <summary>
    /// カードのキャッシュ化
    /// </summary>
    void CacheCard()
    {
        var db = loadDB.GetDatabase(Database.Value.Card);
        var cmd = "SELECT * FROM Cards";
        var table = db.ExecuteQuery(cmd);

        int idCount = 0;
        foreach (var row in table.Rows) {
            Card.Card.State state = new Card.Card.State();
            state.id = idCount;
            state.genre = (Card.Card.GENRE)row["Genre"];
            state.type = (Card.Card.TYPE)row["Type"];
            state.name = (string)row["Name"];
            state.explanation = (string)row["Explanation"];

            state.AP = (int)row["AP"];
            state.Damage = (int)row["Damage"];
            state.rank = (int)row["Rank"];
            state.number = (int)row["Number"];

            state.SE = (int)row["SE"];
            state.Effect = (int)row["Effect"];

            foreach (var value in System.Enum.GetValues(typeof(BuffEnum))) {
                string _key = System.Enum.GetName(typeof(BuffEnum), value);
                if (row.ContainsKey(_key)) state.buff[(int)value] = (int)row[_key];
            }

            cardStates.Add(state);
            idCount++;
        }
    }
    /// <summary>
    /// 道中敵データのキャッシュ
    /// </summary>
    void CacheEnemy()
    {
        var db = loadDB.GetDatabase(Database.Value.Enemy);
        var cmd = "SELECT * FROM Enemy";
        var table = db.ExecuteQuery(cmd);

        foreach (var row in table.Rows)
        {
            Enemy.Enemy.State state = new Enemy.Enemy.State();

            state.number = (int)row["Number"];
            state.type = (int)row["Type"];
            state.strengeth = (int)row["Strengeth"];

            state.name = (string)row["Name"];
            state.explanation = (string)row["Explanation"];
            state.hpMax = (int)row["HP"];
            state.hpFluctuationPlus = (int)row["HPFluctuationPlus"];

            state.pattern.Add((int)row["Pattern1"]);
            state.pattern.Add((int)row["Pattern2"]);
            state.pattern.Add((int)row["Pattern3"]);
            state.pattern.Add((int)row["Pattern4"]);

            //state.value.Add((int)row["Value1"]);
            //state.value.Add((int)row["Value2"]);
            //state.value.Add((int)row["Value3"]);
            //state.value.Add((int)row["Value4"]);
            enemyStates.Add(state);
        }
    }
    /// <summary>
    /// 敵行動パターンのキャッシュ
    /// </summary>   
    void CacheEnemyActivePattern()
    {
        var db = loadDB.GetDatabase(Database.Value.EnemyActionPattern);
        var cmd = "SELECT * FROM ActivePattern";
        var table = db.ExecuteQuery(cmd);

        foreach (var row in table.Rows)
        {
            Enemy.ActivePattern state = new Enemy.ActivePattern();

            state.name = (string)row["Name"];
            state.explanation = (string)row["Explanation"];

            state.Damage = (int)row["Damage"];

            foreach (var value in System.Enum.GetValues(typeof(BuffEnum))) {
                string _key = System.Enum.GetName(typeof(BuffEnum), value);
                if (row.ContainsKey(_key)) state.buff[(int)value] = (int)row[_key];
            }

            state.Turn = (int)row["Turn"];
            state.Type = (int)row["Type"];
            state.Fluctuation = (int)row["FluctuationPlus"];

            state.SE = (int)row["SE"];
            state.Effect = (int)row["Effect"];

            enemyActivePattern.Add(state);
        }
    }
    /// <summary>
    /// 敵の出現パターン
    /// </summary>
    void CacheEnemyAddventPattern()
    {
        var db = loadDB.GetDatabase(Database.Value.Party);
        var cmd = "SELECT * FROM Party";
        var table = db.ExecuteQuery(cmd);

        foreach (var row in table.Rows)
        {
            Enemy.AddventPattern state = new Enemy.AddventPattern();

            for(int i=0;i< (table.Columns.Count - 2); i++)
            {
                string enemyTag = "Enemy" + (i + 1).ToString();
                state.enemysId.Add((int)row[enemyTag]);
            }
            enemyAddventPattern.Add(state);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

}