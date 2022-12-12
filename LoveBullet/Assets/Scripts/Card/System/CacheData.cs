using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CacheData : SingletonMonoBehaviour<CacheData>
{

    Database.Load loadDB;
    public List<Card.State> cardStates;
    public List<Enemy.Enemy.State> enemyStates;
    public List<Enemy.ActivePattern> enemyActivePattern;
    public List<Enemy.AddventPattern> enemyAddventPattern;


    // Start is called before the first frame update
    void Start()
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

        foreach (var row in table.Rows)
        {
            Card.State state=new Card.State();
            state.genre = (Card.GENRE)row["Genre"];
            state.type = (Card.TYPE)row["Type"];
            state.name = (string)row["Name"];
            state.explanation = (string)row["Explanation"];

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
            state.type = (int)row["Type"];
            state.type = (int)row["Strengeth"];

            state.name = (string)row["Name"];
            state.explanation = (string)row["Explanation"];
            state.hpMax = (int)row["HP"];
            state.hpFluctuationPlus = (int)row["HPFluctuationPlus"];
            state.hpFluctuationMinus = (int)row["HPFluctuationMinus"];
            state.ATFluctuationPlus = (int)row["ATFluctuationPlus"];
            state.ATFluctuationMinus = (int)row["DFFluctuationMinus"];

            state.pattern.Add((int)row["Pattern1"]);
            state.pattern.Add((int)row["Pattern2"]);
            state.pattern.Add((int)row["Pattern3"]);
            state.pattern.Add((int)row["Pattern4"]);

            state.value.Add((int)row["Value0"]);
            state.value.Add((int)row["Value1"]);
            state.value.Add((int)row["Value2"]);
            state.value.Add((int)row["Value3"]);
            state.value.Add((int)row["Value4"]);
        }
    }
    /// <summary>
    /// 敵行動パターンのキャッシュ
    /// </summary>   
    void CacheEnemyActivePattern()
    {
        var db = loadDB.GetDatabase(Database.Value.EnemyActionPattern);
    var cmd = "SELECT * FROM EnemyActivePattern";
    var table = db.ExecuteQuery(cmd);

        foreach (var row in table.Rows)
        {
            Enemy.ActivePattern state = new Enemy.ActivePattern();


    state.explanation = (string) row["Explanation"];
    state.AT = (int) row["AT"];
    state.DF = (int) row["DF"];
    state.ATWeaken = (int) row["ATWeaken"];
    state.DFWeaken = (int) row["DFWeaken"];
    state.value.Add((int) row["Value0"]);
            state.value.Add((int) row["Value1"]);
            state.value.Add((int) row["Value2"]);
            state.value.Add((int) row["Value3"]);
            state.value.Add((int) row["Value4"]);
        }
    }
    /// <summary>
    /// 敵の出現パターン
    /// </summary>
    void CacheEnemyAddventPattern()
    {
        var db = loadDB.GetDatabase(Database.Value.EnemyAddventPattern);
        var cmd = "SELECT * FROM EnemyActivePattern";
        var table = db.ExecuteQuery(cmd);

        foreach (var row in table.Rows)
        {
            Enemy.AddventPattern state = new Enemy.AddventPattern();

            for(int i=0;i< (table.Columns.Count - 2); i++)
            {
                string enemyTag = "Enemy" + i.ToString();
                state.enemysId.Add((int)row[enemyTag]);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

}