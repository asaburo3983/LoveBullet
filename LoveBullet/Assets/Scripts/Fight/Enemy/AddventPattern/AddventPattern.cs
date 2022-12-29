using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class AddventPattern
    {
        public List<int> enemysId = new List<int>();

        public static int GetGroupMax(int _floor)
        {
            var pattern = CacheData.instance.enemyAddventPattern;
            return pattern[_floor].enemysId.Count / 5;
        }
        /// <summary>
        /// 敵グループを階層とグループ番号から得る
        /// </summary>
        /// <param name="_floor"></param>
        /// <param name="_groupNum"></param>
        /// <returns></returns>
        public static List<int> GetGroup(int _floor, int _groupNum)
        {
            var pattern = CacheData.instance.enemyAddventPattern;
            if (GetGroupMax(_floor) <= _groupNum || _groupNum < 0) {
                Debug.LogError("敵グループをデータ外の値で参照しようとしています");
                return null; 
            }
            List<int> group = new List<int>();
            var firstNum = _groupNum * 5;
            for (int i = firstNum; i < firstNum + 5; i++)
            {
                group.Add(pattern[_floor].enemysId[i]);
            }
            return group;
        }
    }
}