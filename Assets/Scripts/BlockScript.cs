using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    [SerializeField]
    private bool isRotate = true;

    /// <summary>
    /// à⁄ìÆÇ≥ÇπÇÈ
    /// </summary>
    /// <param name="moveDirection"></param>
    private void Move(Vector3 moveDirection)
    {
        transform.position += moveDirection;
    }
    public void MoveLeft()
    {
        Move(new Vector3(-1, 0, 0));
    }
     public void MoveRight()
    {
        Move(new Vector3(1, 0, 0));
    }
    public void MoveUp()
    {
        Move(new Vector3(0, 1, 0));
    }
    public void MoveDown()
    {
        Move(new Vector3(0, -1, 0));
    }
    /// <summary>
    /// ÉuÉçÉbÉNÇâÒì]Ç≥ÇπÇÈ
    /// </summary>
    public void RotateRight()
    {
        if (isRotate)
        {
            transform.Rotate(0, 0, -90);
        }
    }
    public void RotateLeft()
    {
        if (isRotate)
        {
            transform.Rotate(0, 0, 90);
        }
    }
}
