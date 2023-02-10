using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheScenario : SingletonMonoBehaviour<CacheScenario>
{
    Database.Load loadDB;

    public List<Scenario> chapter1 = new List<Scenario>();
    public List<Tatie> tatie=new List<Tatie>();
    public List<DropObject> dropObject = new List<DropObject>();

    private void Awake()
    {
        if (SingletonCheck(this))
        {
            loadDB = Database.Load.instance;
            CacheTatie();
            CacheScenario1();
            CacheDropObject();
        }
    }


    void CacheTatie()
    {
        var db = loadDB.GetDatabase((Database.Value)0);
        var cmd = "SELECT * FROM Tatie";
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
        var db = loadDB.GetDatabase((Database.Value)1);
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
    void CacheDropObject()
    {
        var db = loadDB.GetDatabase((Database.Value)2);
        var cmd = "SELECT * FROM DropObject";
        var table = db.ExecuteQuery(cmd);

        foreach (var row in table.Rows)
        {
            var tmp = new DropObject();
            tmp.number = (int)row["Number"];
            tmp.name   = (string)row["Name"];
            tmp.value = (int)row["Value"];
            tmp.percent = (int)row["Percent"];

            dropObject.Add(tmp);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
