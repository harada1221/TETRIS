using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    [SerializeField, Header("��]�����邩")]
    private bool _isRotate = true;

    [SerializeField, Header("�X�[�p�[���[�e�[�V��������")]
    private bool _isSuperRotatiion = true;

    [SerializeField, Header("T�X�s������")]
    private bool _isTSpin = default;

    [SerializeField, Header("I�~�m")]
    private bool _isISpin = default;
    //�X�p���[�e�[�V�������ł��邩
    public bool GetSuperspin { get => _isSuperRotatiion; }
    //T�~�m�u���b�N��
    public bool GetTspin { get => _isTSpin; }
    //I�~�m�u���b�N��
    public bool GetISpin { get => _isISpin; }
    /// <summary>
    /// �ړ�������
    /// </summary>
    /// <param name="moveDirection">�ړ��̌���</param>
    private void Move(Vector3 moveDirection)
    {
        transform.position += moveDirection;
    }
    /// <summary>
    /// ���ړ�
    /// </summary>
    public void MoveLeft()
    {
        Move(Vector3.left);
    }
    /// <summary>
    /// �E�ړ�
    /// </summary>
    public void MoveRight()
    {
        Move(Vector3.right);
    }
    /// <summary>
    /// ��ړ�
    /// </summary>
    public void MoveUp()
    {
        Move(Vector3.up);
    }
    /// <summary>
    /// ���ړ�
    /// </summary>
    public void MoveDown()
    {
        Move(Vector3.down);
    }
    /// <summary>
    /// �u���b�N���E��]������
    /// </summary>
    public void RotateRight()
    {
        if (_isRotate)
        {
            transform.Rotate(0, 0, -90);
        }
    }
    /// <summary>
    /// �u���b�N������]������
    /// </summary>
    public void RotateLeft()
    {
        if (_isRotate)
        {
            transform.Rotate(0, 0, 90);
        }
    }
}
