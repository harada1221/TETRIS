// ---------------------------------------------------------
// #SCRIPTNAME#.cs
//
// ì¬“ú10ŒŽ17“ú:
// ì¬ŽÒ:Œ´“c
// ---------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MinoScript : MonoBehaviour
{
    [SerializeField, Header("ƒ~ƒm‚Ì—Ž‚¿‚éŽžŠÔ")]
    private float _fallTime = default;
    private float _fallTimeSave = default;//ƒ~ƒm‚Ì—Ž‚¿‚éŽžŠÔ‚ð•Û‘¶
    private float _timeCount = default;//ƒ~ƒm‚Ì—Ž‚¿‚éŽžŠÔ‚ð”äŠr‚·‚é

    private Vector3 _fallDistance = default;//ƒ~ƒm‚ª‰º“®‚­‹——£
    private Vector3 _moveDistance = default;//ƒ~ƒm‚ª¶‰E“®‚­‹——£
    private Vector3 _rotationPoint = default;//ƒ~ƒm‚Ì’†S
    private Vector3 _rotaionZ = default;//‰ñ“]‚ÌŒü‚«
    private Vector3 _worldAngle = default;//ƒ~ƒm‚ÌŠp“x

    private Transform _myTransform = default;

    private GameControllerScript _gameControllerScript = default;

    [SerializeField, Header("ƒ~ƒm‚ÌŽí—Þ")]
    private MinoType _minoType = default;
    //ƒ~ƒm‚Ì‰ñ“]‚ÌŒü‚«
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
        //ˆê’èŽžŠÔ‚½‚Á‚½‚çƒ~ƒm‚ð—Ž‰º‚³‚¹‚é
        if (_timeCount > _fallTime)
        {
            _gameControllerScript.DownYPosition();
            transform.position += _fallDistance;
            _timeCount = 0;
        }
        //ƒ~ƒm‚ð¶‚É“®‚©‚·
        if (Input.GetKeyDown(KeyCode.A) && _gameControllerScript.GetLeftWall == false)
        {
            transform.position -= _moveDistance;
            _gameControllerScript.LeftXPosition();
        }
        //ƒ~ƒm‚ð‰E‚É“®‚©‚·
        else if (Input.GetKeyDown(KeyCode.D) && _gameControllerScript.GetRightWall == false)
        {
            transform.position += _moveDistance;
            _gameControllerScript.RightXPosition();
        }
        //ƒ~ƒm‚Ì—Ž‰º‘¬“x‚ð‚ ‚°‚é
        if (Input.GetKeyDown(KeyCode.S))
        {
            _fallTimeSave = _fallTime;
            _fallTime = _fallTime * 0.1f;
        }
        //ƒ~ƒm‚Ì—Ž‰º‘¬“x‚ðŒ³‚É–ß‚·
        if (Input.GetKeyUp(KeyCode.S))
        {
            _fallTime = _fallTimeSave;
        }
        //ƒ~ƒm‚ð‰ñ“]‚³‚¹‚é
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
