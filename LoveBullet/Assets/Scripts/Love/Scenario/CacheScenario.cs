using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheScenario : SingletonMonoBehaviour<CacheScenario>
{
    Database.Load loadDB;

    public List<Scenario> chapter1;
    public List<Tatie> tatie;
    // Start is called before the first frame update
    void Start()
    {
        if (SingletonCheck(this))
        {
            loadDB = Database.Load.instance;
            CacheTatie();
            CacheScenario1();
        }
    }
    void CacheTatie()
    {
        var db = loadDB.GetDatabase(Database.Value.Card);
        var cmd = "SELECT * FROM Chapter1";
        var table = db.ExecuteQuery(cmd);

        foreach (var row in table.Rows)
        {
            var ta = new Tatie();
            ta.textureName = (string)row["TextureName"];
            ta.number = (int)row["Number"];

            tatie.Add(ta);
        }
    }
    void CacheScenario1()
    {
        var db = loadDB.GetDatabase(Database.Value.Card);
        var cmd = "SELECT * FROM Chapter1";
        var table = db.ExecuteQuery(cmd);

        int page = 0;
        foreach (var row in table.Rows)
        {
            Scenario sc = new Scenario();
            sc.position = (string)row["Position"];
            sc.page = page;
            sc.text = (string)row["Text"];
            sc.newLine = (int)row["NewLine"];
            sc.charaImageR = (int)row["CharaImageR"];
            sc.effectR = (int)row["EffectR"];
            sc.charaImageL = (int)row["CharaImageL"];
            sc.effectL = (int)row["EffectL"];

            page++;

            chapter1.Add(sc);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
