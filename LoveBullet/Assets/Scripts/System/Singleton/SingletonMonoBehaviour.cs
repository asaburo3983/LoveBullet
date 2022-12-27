using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance { get; protected set; }

    ///---------------------------------------------------------------///
    /// <summary>
    /// �V���O���g���ɂȂ�悤�ɏ�������/
    /// </summary>
    /// <param name="_instance"> �C���X�^���X�i�K��this��n�����Ɓj </param>
    /// <param name="_dontDestroy"> DontDestroyOnLoad�Ƃ��ēo�^���邩 </param>
    /// <returns> �V���O���g���ł������� </returns>
    ///---------------------------------------------------------------///
    protected bool SingletonCheck(T _instance, bool _dontDestroy = true)
    {
        if (instance == null)
        {
            instance = _instance;
            if (_dontDestroy) DontDestroyOnLoad(gameObject);  //�V�[���؂�ւ����ɔj������Ȃ��悤�ɕύX
            return true;
        }

        Debug.LogWarning("�V���O���g���`�F�b�N\n���ɑ��݂���C���X�^���X����������܂����F" + _instance.name);
        Destroy(gameObject);//���ɑ��݂��Ă���̂ŃI�u�W�F�N�g��j��
        return false;
    }
}