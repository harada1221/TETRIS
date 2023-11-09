using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    [SerializeField, Header("回転させるか")]
    private bool isRotate = true;

    [SerializeField, Header("スーパーローテーション判定")]
    private bool isSuperRotatiion = true;

    [SerializeField, Header("Tスピン判定")]
    private bool isTSpin = default;

    [SerializeField, Header("Iミノ")]
    private bool isISpin = default;
    //スパローテーションができるか
    public bool GetSuperspin { get => isSuperRotatiion; }
    //Tミノブロックか
    public bool GetTspin { get => isTSpin; }
    //Iミノブロックか
    public bool GetISpin { get => isISpin; }
    /// <summary>
    /// 移動させる
    /// </summary>
    /// <param name="moveDirection">移動の向き</param>
    private void Move(Vector3 moveDirection)
    {
        transform.position += moveDirection;
    }
    /// <summary>
    /// 左移動
    /// </summary>
    public void MoveLeft()
    {
        Move(Vector3.left);
    }
    /// <summary>
    /// 右移動
    /// </summary>
    public void MoveRight()
    {
        Move(Vector3.right);
    }
    /// <summary>
    /// 上移動
    /// </summary>
    public void MoveUp()
    {
        Move(Vector3.up);
    }
    /// <summary>
    /// 下移動
    /// </summary>
    public void MoveDown()
    {
        Move(Vector3.down);
    }
    /// <summary>
    /// ブロックを右回転させる
    /// </summary>
    public void RotateRight()
    {
        if (isRotate)
        {
            transform.Rotate(0, 0, -90);
        }
    }
    /// <summary>
    /// ブロックを左回転させる
    /// </summary>
    public void RotateLeft()
    {
        if (isRotate)
        {
            transform.Rotate(0, 0, 90);
        }
    }
}
