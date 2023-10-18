// ---------------------------------------------------------
// #SCRIPTNAME#.cs
//
// 作成日10月17日:
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
    // Update is called once per frame
    private void Start()
    {
        _fallDistance = new Vector3(0, -1, 0);
        _moveDistance = new Vector3(1, 0, 0);
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
    }
}
