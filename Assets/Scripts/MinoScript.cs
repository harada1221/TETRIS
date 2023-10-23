// ---------------------------------------------------------
// #SCRIPTNAME#.cs
//
// �쐬��10��17��:
// �쐬��:���c
// ---------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MinoScript : MonoBehaviour
{
    [SerializeField, Header("�~�m�̗����鎞��")]
    private float _fallTime = default;
    private float _fallTimeSave = default;//�~�m�̗����鎞�Ԃ�ۑ�
    private float _timeCount = default;//�~�m�̗����鎞�Ԃ��r����

    private Vector3 _fallDistance = default;//�~�m������������
    private Vector3 _moveDistance = default;//�~�m�����E��������
    private Vector3 _rotationPoint = default;//�~�m�̒��S
    private Vector3 _rotaionZ = default;//��]�̌���

    private Transform _mytransform = default;

    [SerializeField,Header("�~�m�̎��")]
    private MinoType _minoType = default;
    //�~�m�̉�]�̌���
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
    }
    private void Update()
    {
        MoveMino();
    }
    private void MoveMino()
    {
        _timeCount += 20 * Time.deltaTime;
        //��莞�Ԃ�������~�m�𗎉�������
        if (_timeCount > _fallTime)
        {
            transform.position += _fallDistance;
            _timeCount = 0;
        }
        //�~�m�����ɓ�����
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position -= _moveDistance;
        }
        //�~�m���E�ɓ�����
        else if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += _moveDistance;
        }
        //�~�m�̗������x��������
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += _fallDistance;
            _fallTimeSave = _fallTime;
            _fallTime = _fallTime * 0.1f;
        }
        //�~�m�̗������x�����ɖ߂�
        if (Input.GetKeyUp(KeyCode.S))
        {
            _fallTime = _fallTimeSave;
        }
        //�~�m����]������
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
