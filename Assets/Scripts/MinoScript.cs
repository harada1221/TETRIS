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
    private float _fallTimeSave = default;//ミノの落ちる時間を保存
    private float _timeCount = default;//ミノの落ちる時間を比較する

    private Vector3 _fallDistance = default;//ミノが下動く距離
    private Vector3 _moveDistance = default;//ミノが左右動く距離
    private Vector3 _rotationPoint = default;//ミノの中心
    private Vector3 _rotaionZ = default;//回転の向き
    private Vector3 _worldAngle = default;//ミノの角度

    private Transform _myTransform = default;

    [SerializeField, Header("ミノの種類")]
    private MinoType _minoType = default;
    //ミノの回転の向き
    private RotationMinoType _rotationType = RotationMinoType.TOP;
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
    public enum RotationMinoType
    {
        TOP,
        RIGHT,
        LEFT,
        BOTTOM
    }
    public MinoType GetminoType { get => _minoType; }
    public RotationMinoType GetRotationType { get => _rotationType; }
    // Update is called once per frame
    private void Start()
    {
        _fallDistance = new Vector3(0, -1, 0);
        _moveDistance = new Vector3(1, 0, 0);
        _rotaionZ = new Vector3(0, 0, 1);
        _fallTimeSave = _fallTime;
        _myTransform = this.transform;
    }
    private void Update()
    {
        MoveMino();
    }
    private void MoveMino()
    {
        _timeCount += 20 * Time.deltaTime;
        //一定時間たったらミノを落下させる
        if (_timeCount > _fallTime)
        {
            transform.position += _fallDistance;
            _timeCount = 0;
        }
        //ミノを左に動かす
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position -= _moveDistance;
        }
        //ミノを右に動かす
        else if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += _moveDistance;
        }
        //ミノの落下速度をあげる
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += _fallDistance;
            _fallTimeSave = _fallTime;
            _fallTime = _fallTime * 0.1f;
        }
        //ミノの落下速度を元に戻す
        if (Input.GetKeyUp(KeyCode.S))
        {
            _fallTime = _fallTimeSave;
        }
        //ミノを回転させる
        if (Input.GetKeyDown(KeyCode.E))
        {
            transform.RotateAround(transform.TransformPoint(_rotationPoint), _rotaionZ, 270);
            AngleCheck();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.RotateAround(transform.TransformPoint(_rotationPoint), _rotaionZ, 90);
            AngleCheck();
        }
    }
    private void AngleCheck()
    {
        _worldAngle = _myTransform.eulerAngles;
        float angle = _worldAngle.z;
        for (float i = _worldAngle.z; i >= 4;)
        {
            i = angle / 90;
            angle = i;
        }
        if (angle == 0)
        {
            _rotationType = RotationMinoType.TOP;
        }
        if (angle == 1)
        {
            _rotationType = RotationMinoType.LEFT;
        }
        if (angle == 2)
        {
            _rotationType = RotationMinoType.BOTTOM;
        }
        if (angle == 3)
        {
            _rotationType = RotationMinoType.RIGHT;
        }
    }
}
