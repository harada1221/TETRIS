// ---------------------------------------------------------
// #SCRIPTNAME#.cs
//
// 作成日10月17日:
// 更新日10月18日:
// 作成者:原田
// ---------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MinoScript : MonoBehaviour
{
    [SerializeField, Header("ミノの落ちる時間")]
    private float _fallTime = default;
    private float _timeCount = default;
    [SerializeField, Header("ミノが下動く距離")]
    private Vector3 _fallDistance = default;
    [SerializeField, Header("ミノが左右動く距離")]
    private Vector3 _moveDistance = default;
    private Vector3 _rotationPoint = default;
    private Vector3 _rotaionZ = default;
    private MinoType _minoTpe = default;
    private RotationType _rotationType = RotationType.TOP;
    public enum MinoType
    {
        IMINO,
        TMINO,
        ZMINO,
        SMINO,
        LMINO,
        JMINO,
        OMINO
    }
    public enum RotationType
    {
        TOP,
        RIGHT,
        LEFT,
        BOTTOM
    }
    public MinoType GetminoType { get => _minoTpe; }
    // Update is called once per frame
    private void Start()
    {
        _fallDistance = new Vector3(0, -1, 0);
        _moveDistance = new Vector3(1, 0, 0);
        _rotaionZ = new Vector3(0, 0, 1);
    }
    private void Update()
    {
        MoveMino();
    }
    private void MoveMino()
    {
        _timeCount += 20 * Time.deltaTime;
        if (_timeCount > _fallTime)
        {
            transform.position += _fallDistance;
            _timeCount = 0;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position -= _moveDistance;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += _moveDistance;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += _fallDistance;
            _fallTime = _fallTime * 0.1f;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            _fallTime = 10f;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            transform.RotateAround(transform.TransformPoint(_rotationPoint), _rotaionZ, -90);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.RotateAround(transform.TransformPoint(_rotationPoint), _rotaionZ, 90);
        }
    }
}
