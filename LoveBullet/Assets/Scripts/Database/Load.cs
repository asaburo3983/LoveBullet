using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

//using SQLite;
using SQLiteUnity;

namespace Database
{
    public enum Value
    {
        Card = 0,
        Enemy = 1,
        EnemyActionPattern = 2,
        EnemyAddventPattern=3,
        Party = 4

    }
    public class Load : SingletonMonoBehaviour<Load>
    {

        [SerializeField]

        private SQLite[] database;
        [SerializeField]
        private List<string> databaseName = new List<string>();

        // Start is called before the first frame update
        void Awake()
        {
            if(SingletonCheck(this, false))
            {
                LoadDatabase();
            }
        }
        /// <summary>
        /// データベースの読み込み
        /// </summary>
        void LoadDatabase()
        {
            var databasePath = Application.streamingAssetsPath + "/Database/";

            database = new SQLite[databaseName.Count];
            for (int i = 0; i < databaseName.Count; i++)
            {
                database[i] = new SQLite(databaseName[i], null, databasePath);
            }
        }
        /// <summary>
        /// 外部からDBの取得
        /// </summary>
        /// <param name="_dbNum"></param>
        /// <returns></returns>
        public SQLite GetDatabase(Value _dbNum)
        {
            return database[(int)_dbNum];
        }
        ////キャッシュ処理サンプル


    }
}