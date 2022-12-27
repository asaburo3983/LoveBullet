using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : SingletonMonoBehaviour<InputSystem>
{
    private PlayerInput input;

    [SerializeField]
    string currentScheme;
    public string CurrentScheme { get { return currentScheme; } }
    [SerializeField]
    string currentActionMap;

    public bool vibration = true;


    private InputAction _pressAnyKeyAction =
              new InputAction(type: InputActionType.PassThrough, binding: "*/<Button>", interactions: "Press");

    private void OnEnable() => _pressAnyKeyAction.Enable();
    private void OnDisable() => _pressAnyKeyAction.Disable();

    private void Awake()
    {
        if (SingletonCheck(this, false))
        {
            input = GetComponent<PlayerInput>();
            if (!input)
            {
                Debug.LogError("PlayerInput None");
            }
        }
    }

    private void Update()
    {
        currentScheme = input.currentControlScheme;
        currentActionMap = GetCurrentActionMap().name;
    }

    public bool AnyKey()
    {
        return _pressAnyKeyAction.triggered;
    }

    /// <summary>
    /// 指定したアクションを返す // mapを考慮しない
    /// </summary>
    /// <param name="_key"></param>
    /// <returns></returns>
    public InputAction GetInputAction(string _key)
    {
        return input.actions[_key];
    }

    /// <summary>
    /// 指定したアクションを返す // mapを考慮する
    /// </summary>
    /// <param name="_map"></param>
    /// <param name="_key"></param>
    /// <returns></returns>
    public InputAction GetInputAction(string _map,string _key)
    {
        return input.actions.FindActionMap(_map).FindAction(_key);
    }

    /// <summary>
    /// 引数をもとにアクションマップを返す
    /// </summary>
    /// <param name="_map"> マップの検索キー </param>
    /// <returns> 検索結果のアクションマップ </returns>
    public InputActionMap GetActionMap(string _map)
    {
        //欲しいアクションマップをゲットするためファインドする
        return input.actions.FindActionMap(_map);
    }

    /// <summary>
    /// カレントマップをかえす
    /// </summary>
    /// <returns></returns>
    public InputActionMap GetCurrentActionMap()
    {
        return input.currentActionMap;
    }

    /// <summary>
    /// アクションマップの変更
    /// </summary>
    /// <param name="_map"></param>
    public void ChangeActionMap(string _map)
    {
        input.SwitchCurrentActionMap(_map);
    }

    /// <summary>
    /// ボタンの押下を取得
    /// </summary>
    /// <param name="_map">マップ名</param>
    /// <param name="_action">アクション名</param>
    /// <returns></returns>
    public bool WasPressThisFlame(string _map , string _action)
    {
        return input.actions.FindActionMap(_map).FindAction(_action).WasPressedThisFrame();
    }

    /// <summary>
    /// ボタンの解放を取得
    /// </summary>
    /// <param name="_map">マップ名</param>
    /// <param name="_action">アクション名</param>
    /// <returns></returns>
    public bool WasReleasedThisFlame(string _map, string _action)
    {
        return input.actions.FindActionMap(_map).FindAction(_action).WasReleasedThisFrame();
    }

    /// <summary>
    /// ボタンが押されているかを取得
    /// </summary>
    /// <param name="_map">マップ名</param>
    /// <param name="_action">アクション名</param>
    /// <returns></returns>
    public bool WasPressed(string _map, string _action)
    {
        return input.actions.FindActionMap(_map).FindAction(_action).IsPressed();
    }

    /// <summary>
    /// アクションマップの入力を取得
    /// </summary>
    /// <param name="_map">マップ名</param>
    /// <param name="_action">アクション名</param>
    /// <returns></returns>
    public Vector2 GetValue(string _map, string _action)
    {
        var map = input.actions.FindActionMap(_map);
        Vector2 p = map.FindAction(_action).ReadValue<Vector2>();
        return p;

    }

    /// <summary>
    /// カレントマップ名を確認する
    /// </summary>
    /// <param name="_map">マップ名</param>
    /// <returns></returns>
    public bool IsCurrentMap(string _map)
    {
        return input.currentActionMap.name == _map;
    }

    /// <summary>
    /// パッドの振動を変更する
    /// </summary>
    /// <param name="_power">振動の大きさ</param>
    public void VibrationGamePad(float _power)
    {
        if (!vibration) {
            return;
        }
        Gamepad pad = Gamepad.current;
        if (pad != null) {
            pad.SetMotorSpeeds(_power, _power);
        }
    }

    /// <summary>
    /// パッドの振動を一定時間行う
    /// </summary>
    /// <param name="_power">振動の大きさ</param>
    /// <param name="_time">時間</param>
        public void VibrationGamePadSetTime(float _power,float _time)
    {
        if (!vibration) {
            return;
        }
        _power = Mathf.Clamp(_power, 0f, 1f);
        StartCoroutine(VibrationTime(_power,_time));
    }

    IEnumerator VibrationTime(float _power,float _time)
    {
        Gamepad pad = Gamepad.current;
        if(pad != null) {
            pad.SetMotorSpeeds(_power, _power);
            yield return new WaitForSeconds(_time);
            pad.SetMotorSpeeds(0, 0);
        }
        yield return new WaitForSeconds(0.01f);
    }

}
