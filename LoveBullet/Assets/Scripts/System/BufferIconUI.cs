using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

[System.Serializable]
enum UnitType
{
    Player,
    Enemy
}

[System.Serializable]
enum BufferType
{
    Atk_Weak,
    Def_Weak,
    Atk_Buff,
    Armor,
    Stan,
}

public class BufferIconUI : MonoBehaviour
{
    [SerializeField] UnitType unit;
    [SerializeField] BufferType buffer;
    [SerializeField] Enemy.Enemy enemy;
    [SerializeField] Text text;

    void Start()
    {
        switch (unit) {
            case UnitType.Player:
                PlayerIconInit();
                break;
            case UnitType.Enemy:
                EnemyIconInit();
                break;
            default:
                break;
        }
    }

    void PlayerIconInit()
    {
        Player player = Player.instance;

        switch (buffer) {
            case BufferType.Atk_Weak:
                player.gameState.ATWeaken.Subscribe(x => {
                    transform.GetChild(0).gameObject.SetActive(x > 0);
                    text.text = x.ToString();
                }).AddTo(this);
                break;
            case BufferType.Def_Weak:
                player.gameState.DFWeaken.Subscribe(x => {
                    transform.GetChild(0).gameObject.SetActive(x > 0);
                    text.text = x.ToString();
                }).AddTo(this);
                break;
            case BufferType.Atk_Buff:
                player.gameState.Atk.Subscribe(x => {
                    transform.GetChild(0).gameObject.SetActive(x > 0);
                    text.text = x.ToString();
                }).AddTo(this);
                break;
            case BufferType.Armor:
                player.gameState.Def.Subscribe(x => {
                    transform.GetChild(0).gameObject.SetActive(x > 0);
                    text.text = x.ToString();
                }).AddTo(this);
                break;
            default:
                break;
        }
    }

    void EnemyIconInit()
    {
        switch (buffer) {
            case BufferType.Atk_Weak:
                enemy.gameState.ATWeaken.Subscribe(x => {
                    transform.GetChild(0).gameObject.SetActive(x > 0);
                    text.text = x.ToString();
                }).AddTo(this);
                break;
            case BufferType.Def_Weak:
                enemy.gameState.DFWeaken.Subscribe(x => {
                    transform.GetChild(0).gameObject.SetActive(x > 0);
                    text.text = x.ToString();
                }).AddTo(this);
                break;
            case BufferType.Armor:
                enemy.gameState.DFBuff.Subscribe(x => {
                    transform.GetChild(0).gameObject.SetActive(x > 0);
                    text.text = x.ToString();
                }).AddTo(this);
                break;
            case BufferType.Stan:
                enemy.gameState.stan.Subscribe(x => {
                    transform.GetChild(0).gameObject.SetActive(x > 0);
                    text.text = x.ToString();
                }).AddTo(this);
                break;
            default:
                break;
        }
    }

}
