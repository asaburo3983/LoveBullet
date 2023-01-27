using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheScenario : SingletonMonoBehaviour<CacheScenario>
{
    Database.Load loadDB;

    public List<Scenario> scenarios;
    // Start is called before the first frame update
    void Start()
    {
        if (SingletonCheck(this))
        {
            loadDB = Database.Load.instance;
            CacheScenario1();
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
            sc.text= (string)row["Text"];
            sc.newLine = (int)row["NewLine"];

            page++;

            scenarios.Add(sc);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
