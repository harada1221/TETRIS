// ---------------------------------------------------------
// #SCRIPTNAME#.cs
//
// ì¬“ú10Œ17“ú:
// ì¬Ò:Œ´“c
// ---------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MinoScript : MonoBehaviour
{
    [SerializeField, Header("ƒ~ƒm‚Ì—‚¿‚éŠÔ")]
    private float _fallTime = default;
    private float _fallTimeSave = default;//ƒ~ƒm‚Ì—‚¿‚éŠÔ‚ğ•Û‘¶
    private float _timeCount = default;//ƒ~ƒm‚Ì—‚¿‚éŠÔ‚ğ”äŠr‚·‚é

    private Vector3 _fallDistance = default;//ƒ~ƒm‚ª‰º“®‚­‹——£
    private Vector3 _moveDistance = default;//ƒ~ƒm‚ª¶‰E“®‚­‹——£
    private Vector3 _rotationPoint = default;//ƒ~ƒm‚Ì’†S
    private Vector3 _rotaionZ = default;//‰ñ“]‚ÌŒü‚«

    private Transform _mytransform = default;

    [SerializeField,Header("ƒ~ƒm‚Ìí—Ş")]
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
    }
    private void Update()
    {
        MoveMino();
    }
    private void MoveMino()
    {
        _timeCount += 20 * Time.deltaTime;
        //ˆê’èŠÔ‚½‚Á‚½‚çƒ~ƒm‚ğ—‰º‚³‚¹‚é
        if (_timeCount > _fallTime)
        {
            transform.position += _fallDistance;
            _timeCount = 0;
        }
        //ƒ~ƒm‚ğ¶‚É“®‚©‚·
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position -= _moveDistance;
        }
        //ƒ~ƒm‚ğ‰E‚É“®‚©‚·
        else if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += _moveDistance;
        }
        //ƒ~ƒm‚Ì—‰º‘¬“x‚ğ‚ ‚°‚é
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += _fallDistance;
            _fallTimeSave = _fallTime;
            _fallTime = _fallTime * 0.1f;
        }
        //ƒ~ƒm‚Ì—‰º‘¬“x‚ğŒ³‚É–ß‚·
        if (Input.GetKeyUp(KeyCode.S))
        {
            _fallTime = _fallTimeSave;
        }
        //ƒ~ƒm‚ğ‰ñ“]‚³‚¹‚é
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
