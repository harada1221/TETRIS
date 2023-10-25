using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BordScripts : MonoBehaviour
{
    [SerializeField]
    private Transform _emptySprite = default;
    [SerializeField, Header("マップの広さ")]
    private int _hight = 30, _width = 10, _header = 8;
}
