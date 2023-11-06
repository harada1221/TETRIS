using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    [SerializeField, Header("��]�����邩")]
    private bool isRotate = true;

    [SerializeField, Header("�X�[�p�[���[�e�[�V��������")]
    private bool isSuperRotatiion = true;

    [SerializeField, Header("T�X�s������")]
    private bool isTSpin = default;

    [SerializeField, Header("I�~�m")]
    private bool isISpin = default;
    public bool GetSuperspin { get => isSuperRotatiion; }
    public bool GetTspin { get => isTSpin; }
    public bool GetISpin { get => isISpin; }
    /// <summary>
    /// �ړ�������
    /// </summary>
    /// <param name="moveDirection"></param>
    private void Move(Vector3 moveDirection)
    {
        transform.position += moveDirection;
    }
    /// <summary>
    /// ���ړ�
    /// </summary>
    public void MoveLeft()
    {
        Move(new Vector3(-1, 0, 0));
    }
    /// <summary>
    /// �E�ړ�
    /// </summary>
    public void MoveRight()
    {
        Move(new Vector3(1, 0, 0));
    }
    /// <summary>
    /// ��ړ�
    /// </summary>
    public void MoveUp()
    {
        Move(new Vector3(0, 1, 0));
    }
    /// <summary>
    /// ���ړ�
    /// </summary>
    public void MoveDown()
    {
        Move(new Vector3(0, -1, 0));
    }
    /// <summary>
    /// �u���b�N���E��]������
    /// </summary>
    public void RotateRight()
    {
        if (isRotate)
        {
            transform.Rotate(0, 0, -90);
        }
    }
    /// <summary>
    /// �u���b�N������]������
    /// </summary>
    public void RotateLeft()
    {
        if (isRotate)
        {
            transform.Rotate(0, 0, 90);
        }
    }
}
