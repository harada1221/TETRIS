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
    private Vector3 _worldAngle = default;//�~�m�̊p�x

    private Transform _myTransform = default;

    private GameControllerScript _gameControllerScript = default;

    [SerializeField, Header("�~�m�̎��")]
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
        _myTransform = this.transform;
        _gameControllerScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }
    private void Update()
    {
        if (_gameControllerScript.GetisDown == false)
        {
            MoveMino();
        }
    }
    private void MoveMino()
    {
        _timeCount += 20 * Time.deltaTime;
        //��莞�Ԃ�������~�m�𗎉�������
        if (_timeCount > _fallTime)
        {
            _gameControllerScript.DownYPosition();
            transform.position += _fallDistance;
            _timeCount = 0;
        }
        //�~�m�����ɓ�����
        if (Input.GetKeyDown(KeyCode.A) && _gameControllerScript.GetLeftWall == false)
        {
            transform.position -= _moveDistance;
            _gameControllerScript.LeftXPosition();
        }
        //�~�m���E�ɓ�����
        else if (Input.GetKeyDown(KeyCode.D) && _gameControllerScript.GetRightWall == false)
        {
            transform.position += _moveDistance;
            _gameControllerScript.RightXPosition();
        }
        //�~�m�̗������x��������
        if (Input.GetKeyDown(KeyCode.S))
        {
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
