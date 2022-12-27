using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance { get; protected set; }

    ///---------------------------------------------------------------///
    /// <summary>
    /// シングルトンになるように処理する/
    /// </summary>
    /// <param name="_instance"> インスタンス（必ずthisを渡すこと） </param>
    /// <param name="_dontDestroy"> DontDestroyOnLoadとして登録するか </param>
    /// <returns> シングルトンであったか </returns>
    ///---------------------------------------------------------------///
    protected bool SingletonCheck(T _instance, bool _dontDestroy = true)
    {
        if (instance == null)
        {
            instance = _instance;
            if (_dontDestroy) DontDestroyOnLoad(gameObject);  //シーン切り替え時に破棄されないように変更
            return true;
        }

        Debug.LogWarning("シングルトンチェック\n既に存在するインスタンスが生成されました：" + _instance.name);
        Destroy(gameObject);//既に存在しているのでオブジェクトを破棄
        return false;
    }
}