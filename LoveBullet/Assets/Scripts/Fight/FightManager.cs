using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : SingletonMonoBehaviour<FightManager> {


    int floor;
    List<Enemy.Enemy.State> enemys; 
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartTurn()
    {
        AdventEnemys();
    }
    /// <summary>
    /// �K�w�ɉ������G���o��������
    /// </summary>
    void AdventEnemys()
    {
        //�G�̏o�������ɂ���
        //�Ƃ肠�����w��p�^�[���ŏo��������i���DB�����Ďw��p�^�[���̒��ł������_���ŏo��������Ȃǂ��s���j
    }
    void EndTurn()
    {

    }
}
