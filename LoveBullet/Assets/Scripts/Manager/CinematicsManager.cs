using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���o�G�t�F�N�g�}�l�[�W���[
/// </summary>
public class CinematicsManager : MonoBehaviour
{
    // �C���X�^���X��
    #region instancing

    static CinematicsManager instance = null;
    public static CinematicsManager Instance
    {
        get { return instance; }
    }

    #endregion

    // �C���X�y�N�^�[���J�ϐ�
    #region serialized variables

    [SerializeField, Header("���o�G�t�F�N�g�I�u�W�F�N�g�ꗗ")]
    private GameObject[] cinematicEffectObjects;

    #endregion

    // �ϐ���`
    #region variables

    private GameObject targetObject;

    #endregion

    // Unity�r���g�C���֐�
    #region unity functions

    private void Awake()
    {
        // �C���X�^���X�ƃV���O���g���N��
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    #endregion

    // ���J�֐�(public)
    #region public functions

    /// <summary>
    /// ���o�G�t�F�N�g�𔭐�����
    /// </summary>
    /// <param name="_effectL">���̉摜�ɑ΂��鉉�o�G�t�F�N�g(�Ή��������摜�ł͂Ȃ��Ȃ�-1�����)</param>
    /// <param name="_tatieL">���̉摜�I�u�W�F�N�g</param>
    /// <param name="_effectR">�E�̉摜�ɑ΂��鉉�o�G�t�F�N�g(�Ή��������摜�ł͂Ȃ��Ȃ�-1�����)</param>
    /// <param name="_tatieR">�E�̉摜�I�u�W�F�N�g</param>
    public void PlayCinematicEffect(
        int _effectL, SpriteRenderer _tatieL,
        int _effectR, SpriteRenderer _tatieR)
    {
        // ���摜�ɑΉ�
        if(_effectL != -1 && _effectR == -1)
        {
            // �ΏۃI�u�W�F�N�g�w��
            targetObject = _tatieL.gameObject;

            // ���o�G�t�F�N�g���Đ�
            PlayEffect(_effectL);
        }
        // �E�摜�ɑΉ�
        else if(_effectL == -1 && _effectR != -1)
        {
            // �ΏۃI�u�W�F�N�g�w��
            targetObject = _tatieR.gameObject;

            // ���o�G�t�F�N�g���Đ�
            PlayEffect(_effectR);
        }
    }

    public GameObject GetEffectTargetObj()
    {
        return targetObject;
    }

    #endregion

    // �v���C�x�[�g�֐�
    #region private functions

    private void PlayEffect(int _num)
    {
        Instantiate(cinematicEffectObjects[_num - 1], Vector3.zero, Quaternion.identity);
    }

    #endregion
}
