using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLiteUnity;
using UnityEngine.UI;
using System.IO;


namespace Database
{
    public class Load : SingletonMonoBehaviour<Load>
    {
        //読み込みは外部で行う
        static Load instance = null;
        public static SC_LoadDB Instance
        {
            get { return instance; }
        }

        [SerializeField]
        private SQLite[] database;

        [SerializeField]
        private List<string> databaseName = new List<string>();

        // Start is called before the first frame update
        void Awake()
        {
            SingletonCheck(this, false);
            instance = this;

            LoadDatabase();
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
        public SQLite GetDatabase(int _dbNum)
        {
            return database[_dbNum];
        }
        //キャッシュ処理サンプル
        public void CacheLevelUpExp()
        {
            var db = SC_LoadDB.Instance.GetDatabase((int)DatabaseNumbar.LEVEL_UP);
            var cmd = "SELECT level,levelUpExp FROM LevelUp";
            var table = db.ExecuteQuery(cmd);

            int i = 0;
            levelUpExp = new int[table.Rows.Count];
            foreach (var row in table.Rows)
            {

                levelUpExp[i] = (int)row["levelUpExp"];
                i++;
            }
        }
    }
}