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

            int buff = (int)row["Buff"];
            int debuff = (int)row["Debuff"];
            int special = (int)row["Special"];

            // 4ビットずつ格納
            state.buff.AT = (buff >> 0) & 0x0f;          // 0 ~ 4   bit
            state.buff.AT_Never = (buff >> 4) & 0x0f;    // 5 ~ 8   bit
            state.buff.DF = (buff >> 8) & 0x0f;          // 9 ~ 12  bit
            state.buff.DF_Never = (buff >> 12) & 0x0f;   // 13 ~ 16 bit

            // 4ビットずつ格納
            state.buff.AT_Weak = (debuff >> 0) & 0x0f;  // 0 ~ 4 bit
            state.buff.DF_Weak = (debuff >> 4) & 0x0f;  // 5 ~ 8 bit
            state.buff.Stan = (debuff >> 8) & 0x0f;      // 9 ~ 12 bit

            // 4ビットずつ格納
            // boolは1ビットずつ格納
            state.Whole = ((special >> 0) & 0x01) == 1;     // 0 bit
            state.Random = ((special >> 1) & 0x01) == 1;    // 1 bit
            state.MultiAttack = (special >> 2) & 0x0f;      // 2 ~ 6 bit
            state.SelfDamage= (special >> 6) & 0x0f;        // 7 ~ 10 bit
            state.Reload = (special >> 10) & 0x0f;          // 11 ~ 14 bit
            state.Cocking = (special >> 14) & 0x0f;         // 15 ~ 18 bit
            state.Scrap = ((special >> 18) & 0x01) == 1;    // 19 bit

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

            state.Damage = (int)row["AT"];

            int buff = (int)row["Buff"];
            int buff2 = (int)row["Buff2"];

            // buff1
            state.buff.AT = (buff >> 0) & 0x0f;         // 0 ~ 4   bit
            state.buff.AT_Never = (buff >> 4) & 0x0f;   // 5 ~ 8   bit
            state.buff.DF = (buff >> 8) & 0x0f;         // 9 ~ 12  bit
            state.buff.DF_Never = (buff >> 12) & 0x0f;  // 13 ~ 16 bit

            state.buff.AT_Weak = (buff >> 16) & 0x0f;   // 17 ~ 20 bit
            state.buff.DF_Weak = (buff >> 20) & 0x0f;   // 21 ~ 24 bit

            // buff2
            state.buff.Stan = (buff2 >> 0) & 0x0f;  // 0 ~ 4   bit
            state.buff.Heal = (buff2 >> 4) & 0x0f;  // 5 ~ 8   bit

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