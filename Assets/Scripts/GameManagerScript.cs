using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField, Header("�S�[�X�g�u���b�N�̐F")]
    private Color _colorWhite = default;
    //�S�[�X�g�u���b�N�̈ʒu����
    private Vector3 _ghostBlockPosition = new Vector3(0.5f, 0.5f, 0);
    //�z�[���h�u���b�N�̈ʒu
    private Vector3 _holdBlockPosition = new Vector3(-6, 15, 0);

    private SpawnerScript _spawner = default;//�u���b�N�X�|�i�[
    private BlockScript _activeBlock = default;//�������ꂽ�u���b�N�i�[
    private BlockScript _ghostBlock = default;//�������ꂽ�S�[�X�g�u���b�N
    private BlockScript _holdBlock = default;//�z�[���h�����u���b�N���i�[
    private BlockScript _saveBlock = default;//�z�[���h�����u���b�N�����ւ���p
    [SerializeField, Header("�u���b�N�������鎞��")]
    private float _dropInterval = 0.25f;
    private float _nextDropTimer = default;//���Ƀu���b�N��������܂ł̎���

    private BordScripts _bord = default;//�t�B�[���h�̃X�N���v�g
    //���͂̃^�C��
    private float _nextKeyDownTime = default;
    private float _nextKeySideTime = default;
    private float _nextKeyRotateTime = default;
    //�u���b�N�����n���Ă���~�܂鎞��
    private float _lookTime = default;

    [SerializeField, Header("�u���b�N�𓮂�������")]
    private int _moveCount = 15;

    [SerializeField, Header("�����͂̃C���^�[�o��")]
    private float _nextKeyDownTimeInterval = default;
    [SerializeField, Header("���E���͂̃C���^�[�o��")]
    private float _nextKeySideTimeInterval = default;
    [SerializeField, Header("��]���͂̃C���^�[�o��")]
    private float _nextKeyRotateTimeInterval = default;
    [SerializeField, Header("�u���b�N�̌Œ�̎���")]
    private float _lookTimeInterval = default;
    [SerializeField]
    private GameObject _gameOverPanel = default;//�Q�[���I�[�o�[���̃p�l��
    private bool isGameOver = false;//�Q�[���I�[�o�[�̔���
    private bool isChangeBlock = false;//�z�[���h��������
    private bool isGround = false;//�u���b�N�����ɂԂ�������

    private void Start()
    {
        //�X�|�i�[�I�u�W�F�N�g���i�[
        _spawner = GameObject.FindObjectOfType<SpawnerScript>();
        //�{�[�h�̃X�N���v�g���i�[
        _bord = GameObject.FindObjectOfType<BordScripts>();

        //�^�C�}�[�̏����ݒ�
        _nextKeyDownTime = Time.time + _nextKeyDownTimeInterval;
        _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
        _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
        _lookTime = Time.time + _lookTimeInterval;

        //�u���b�N�𐶐����Ċi�[
        if (!_activeBlock)
        {
            //���������u���b�N���i�[����
            _activeBlock = _spawner.SpwnBlock();
            //�������ꂽ�����u���b�N���S�[�X�g�u���b�N�Ƃ��Đ���
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position , Quaternion.identity);
            //�S�[�X�g�u���b�N�̐F�ς�
            ColorChange();
            //�S�[�X�g�����܂ŗ��Ƃ�
            DownGhostBlock();
        }
        //�A�N�e�B�u��Ԃ��ƃQ�[���I�[�o�[�p�l������
        if (_gameOverPanel.activeInHierarchy)
        {
            _gameOverPanel.SetActive(false);
        }
    }
    private void Update()
    {
        //�Q�[���I�[�o�[�������瓮�����Ȃ�
        if (isGameOver)
        {
            return;
        }
        //�S�[�X�g�u���b�N�����܂ŗ��Ƃ�
        DownGhostBlock();
        //�v���C���[�̑���
        PlayerInput();
    }
    private void PlayerInput()
    {
        //D�������Ă���ԉE�Ɉړ�
        if (Input.GetKey(KeyCode.D) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.D))
        {
            //�Œ�܂Ŕ���
            BlockLook();
            //�E�ɓ�����
            _activeBlock.MoveRight();
            _ghostBlock.MoveRight();
            //�^�C�}�[�X�V
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //�͂ݏo���Ă���߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveLeft();
                _ghostBlock.MoveLeft();
            }
        }
        //A�������Ă�ԍ��Ɉړ�
        else if (Input.GetKey(KeyCode.A) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.A))
        {
            //�Œ�܂Ŕ���
            BlockLook();
            //���ɓ�����
            _activeBlock.MoveLeft();
            _ghostBlock.MoveLeft();
            //�^�C�}�[�X�V
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //�͂ݏo���Ă���߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveRight();
                _ghostBlock.MoveRight();
            }
        }
        //E����������E��]
        else if (Input.GetKey(KeyCode.E) && (Time.time > _nextKeyRotateTime))
        {
            //�Œ�܂Ŕ���
            BlockLook();
            //�E��]
            _activeBlock.RotateRight();
            _ghostBlock.RotateRight();
            //�^�C�}�[�X�V
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //�͂ݏo����߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                //��]�␳O��I�ȊO
                if (_activeBlock.GetSuperspin)
                {
                    TRotationRight();
                }
                //��]�␳I�~�m�u���b�N
                if (_activeBlock.GetISpin)
                {
                    IRotationRight();
                }
            }
        }
        //Q���������獶��]
        else if (Input.GetKey(KeyCode.Q) && (Time.time > _nextKeyRotateTime))
        {
            //�Œ�܂Ŕ���
            BlockLook();
            //����]
            _activeBlock.RotateLeft();
            _ghostBlock.RotateLeft();
            //�^�C�}�[�X�V
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //�͂ݏo����߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                //��]�␳O��I�ȊO
                if (_activeBlock.GetSuperspin)
                {
                    TRotationLeft();
                }
                //��]�␳I�~�m�u���b�N
                if (_activeBlock.GetISpin)
                {
                    IRotationLeft();
                }
            }
        }
        //�\�t�g�h���b�vS�������Ƒ���������
        else if (Input.GetKey(KeyCode.S) && (Time.time > _nextKeyDownTime) || Time.time > _nextDropTimer)
        {
            //���ɓ�����
            _activeBlock.MoveDown();
            //���̓^�C��
            _nextKeyDownTime = Time.time + _nextKeyDownTimeInterval;
            _nextDropTimer = Time.time + _dropInterval;

            //�͂ݏo���Ă���߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                //�͈͊O���ƃQ�[���I�[�o�[
                if (_bord.OverLimit(_activeBlock))
                {
                    GameOver();
                }
                else
                {
                    //���n������J�E���g�J�n
                    if (!isGround)
                    {
                        isGround = true;
                        _lookTime = Time.time + _lookTimeInterval;
                    }
                    //��ɂ�������
                    BottomBoard();
                }
            }
        }
        //W����������n�[�h�h���b�v
        else if (Input.GetKeyDown(KeyCode.W))
        {
            //�Ԃ���܂ŌJ��Ԃ�
            while (_bord.CheckPosition(_activeBlock))
            {
                //���ɓ�����
                _activeBlock.MoveDown();
            }
            //��ɂ�������
            BottomBoard();

        }
        //�z�[���h�̏���
        else if (Input.GetKeyDown(KeyCode.Z) && isChangeBlock == false)
        {
            //�z�[���h���P�x�����ɂ���
            isChangeBlock = true;
            if (_holdBlock == default)
            {
                //���݂̃u���b�N�𐶐�
                _holdBlock = Instantiate(_activeBlock, _holdBlockPosition, Quaternion.identity);
                //�u���b�N�폜
                Destroy(_activeBlock.gameObject);
                Destroy(_ghostBlock.gameObject);
                //�u���b�N����
                _activeBlock = _spawner.SpwnBlock();
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
                //�F��ς���
                ColorChange();
                //�^�C���̏�����
                ResetTime();
            }
            else
            {
                //����ւ�
                _saveBlock = _activeBlock;
                _activeBlock = _holdBlock;
                _holdBlock = _saveBlock;

                //���̃u���b�N���폜
                Destroy(_saveBlock.gameObject);
                Destroy(_activeBlock.gameObject);
                Destroy(_ghostBlock.gameObject);
                //�V�����u���b�N�𐶐�
                _activeBlock = Instantiate(_activeBlock, _spawner.transform.position, Quaternion.identity);
                //I�~�m��������ʒu����
                if (_activeBlock.GetISpin)
                {
                    _activeBlock.transform.position += _ghostBlockPosition;
                }
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position , Quaternion.identity);
                //�F��ς���
                ColorChange();
                //�z�[���h�u���b�N���폜
                Destroy(_holdBlock.gameObject);
                //�\���悤�z�[���h�u���b�N����
                _holdBlock = Instantiate(_saveBlock, _holdBlockPosition, Quaternion.identity);
                //�^�C���̏�����
                ResetTime();
            }
        }
    }
    /// <summary>
    /// ���n���Ă���Œ�܂ł̎��ԂƉ�
    /// </summary>
    private void BlockLook()
    {
        //���n���Ă��瓮����񐔂��J�E���g�A�b�v
        _moveCount++;
        //���n����Œ�܂ł̎���
        _lookTime = Time.time + _lookTimeInterval;
    }
    /// <summary>
    /// �^�C���̏�����
    /// </summary>
    private void ResetTime()
    {
        //�^�C���̏�����
        _nextDropTimer = Time.time;
        _nextKeySideTime = Time.time;
        _nextKeyRotateTime = Time.time;
        _lookTime = Time.time;
    }
    /// <summary>
    /// ��ɒ��������̏���
    /// </summary>
    private void BottomBoard()
    {
        //�~�m�u���b�N��1���
        _activeBlock.MoveUp();
        //��莞�Ԍo�߂����܂��͂P�T��~�m�u���b�N�𓮂�������
        if (Time.time > _lookTime || Input.GetKeyDown(KeyCode.W) || _moveCount >= 15)
        {
            //�z��Ɋi�[
            _bord.SaveBlockInGrid(_activeBlock);
            //�z�[���h���ł���悤��
            isChangeBlock = false;
            //�u���b�N�������Ď��̃u���b�N�𐶐�
            Destroy(_ghostBlock.gameObject);
            _activeBlock = _spawner.SpwnBlock();
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
            ColorChange();

            //�l�̏�����
            ResetTime();
            _moveCount = 0;
            isGround = false;
            //�����Ă���΍폜
            _bord.ClearAllRows();
        }
    }
    /// <summary>
    /// �Q�[���I�[�o�[�̎��Ăяo��
    /// </summary>
    private void GameOver()
    {
        //�u���b�N����ɂ�����
        _activeBlock.MoveUp();
        //�\������
        if (!_gameOverPanel.activeInHierarchy)
        {
            _gameOverPanel.SetActive(true);
        }
        //�Q�[���I�[�o�[�ɂ���
        isGameOver = true;
    }
    /// <summary>
    /// �V�[�����Ăяo��
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    /// <summary>
    /// �Ԃ���܂ŉ��ɂ��Ƃ�
    /// </summary>
    private void DownGhostBlock()
    {
        //�S�[�X�g�u���b�N�̈ʒu��e�̏ꏊ�Ɉړ�
        _ghostBlock.transform.position = _activeBlock.transform.position;
        //�Ԃ���܂ŉ��ɓ�����
        while (_bord.CheckPosition(_ghostBlock))
        {
            _ghostBlock.MoveDown();
        }
        //�Ԃ�������P���
        _ghostBlock.MoveUp();
    }
    /// <summary>
    /// �I�u�W�F�N�g�̐F��ς���
    /// </summary>
    private void ColorChange()
    {
        Transform parent = _ghostBlock.transform;
        SpriteRenderer chidren;

        // �q�I�u�W�F�N�g��S�Ď擾����
        foreach (Transform child in parent)
        {
            //�F��ς���
            chidren = child.GetComponent<SpriteRenderer>();
            chidren.color = _colorWhite;
        }
    }
    /// <summary>
    /// ����]�̉�]�␳
    /// </summary>
    private void TRotationLeft()
    {
        switch (_activeBlock.transform.rotation.eulerAngles.z)
        {
            case 0:
                LeftAngleZero();
                break;
            case 90:
                LeftAngleNinety();
                break;
            case 180:
                LeftAngleHundred();
                break;
            case 270:
                LeftAngleTwoHundred();
                break;

        }
    }
    /// <summary>
    /// �p�x��0�x�̍���]
    /// </summary>
    private void LeftAngleZero()
    {
        //�Ԃ���Ȃ��Ȃ�܂ŌJ��Ԃ�
        for (int i = 0; i <= 4; i++)
        {
            switch (i)
            {
                case 0:
                    _activeBlock.transform.position += new Vector3(1, 0, 0);
                    break;
                case 1:
                    _activeBlock.transform.position += new Vector3(0, -1, 0);
                    break;
                case 2:
                    _activeBlock.transform.position += new Vector3(-1, 3, 0);
                    break;
                case 3:
                    _activeBlock.transform.position += new Vector3(1, 0, 0);
                    break;
                case 4:
                    //�Ԃ���Ȃ������猳�ɖ߂�
                    _activeBlock.transform.position += new Vector3(-1, -2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                    break;
            }
            if (_bord.CheckPosition(_activeBlock))
            {
                break;
            }
        }
    }
    /// <summary>
    /// �p�x��90�x�̍���]
    /// </summary>
    private void LeftAngleNinety()
    {  //�Ԃ���Ȃ��Ȃ�܂ŌJ��Ԃ�
        for (int i = 0; i <= 4; i++)
        {
            switch (i)
            {
                case 0:
                    _activeBlock.transform.position += new Vector3(1, 0, 0);
                    break;
                case 1:
                    _activeBlock.transform.position += new Vector3(0, 1, 0);
                    break;
                case 2:
                    _activeBlock.transform.position += new Vector3(-1, -3, 0);
                    break;
                case 3:
                    _activeBlock.transform.position += new Vector3(1, 0, 0);
                    break;
                case 4:
                    //�Ԃ���Ȃ������猳�ɖ߂�
                    _activeBlock.transform.position += new Vector3(-1, 2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                    break;
            }
            if (_bord.CheckPosition(_activeBlock))
            {
                break;
            }
        }

    }
    /// <summary>
    ///  �p�x��180�x�̍���]
    /// </summary>
    private void LeftAngleHundred()
    {
        //�Ԃ���Ȃ��Ȃ�܂ŌJ��Ԃ�
        for (int i = 0; i <= 4; i++)
        {
            switch (i)
            {
                case 0:
                    _activeBlock.transform.position += new Vector3(-1, 0, 0);
                    break;
                case 1:
                    _activeBlock.transform.position += new Vector3(0, -1, 0);
                    break;
                case 2:
                    _activeBlock.transform.position += new Vector3(1, 3, 0);
                    break;
                case 3:
                    _activeBlock.transform.position += new Vector3(-1, 0, 0);
                    break;
                case 4:
                    //�Ԃ���Ȃ������猳�ɖ߂�
                    _activeBlock.transform.position += new Vector3(1, -2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                    break;
            }
            if (_bord.CheckPosition(_activeBlock))
            {
                break;
            }
        }
    }
    /// <summary>
    ///  �p�x��270�x�̍���]
    /// </summary>
    private void LeftAngleTwoHundred()
    {
        //�Ԃ���Ȃ��Ȃ�܂ŌJ��Ԃ�
        for (int i = 0; i <= 4; i++)
        {
            switch (i)
            {
                case 0:
                    _activeBlock.transform.position += new Vector3(-1, 0, 0);
                    break;
                case 1:
                    _activeBlock.transform.position += new Vector3(0, 1, 0);
                    break;
                case 2:
                    _activeBlock.transform.position += new Vector3(1, -3, 0);
                    break;
                case 3:
                    _activeBlock.transform.position += new Vector3(-1, 0, 0);
                    break;
                case 4:
                    //�Ԃ���Ȃ������猳�ɖ߂�
                    _activeBlock.transform.position += new Vector3(1, 2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                    break;
            }
            if (_bord.CheckPosition(_activeBlock))
            {
                break;
            }
        }
    }
   
    /// <summary>
    /// �E��]�̉�]�␳
    /// </summary>
    private void TRotationRight()
    {
        switch (_activeBlock.transform.rotation.eulerAngles.z)
        {
            case 0:
                _activeBlock.transform.position += new Vector3(-1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, -2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
            case 90:
                _activeBlock.transform.position += new Vector3(1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, 1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, -3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
            case 270:
                _activeBlock.transform.position += new Vector3(-1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, 1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, -3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
            case 180:
                _activeBlock.transform.position += new Vector3(1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, -2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
        }

    }
    /// <summary>
    /// I�~�m�u���b�N�̍���]�␳
    /// </summary>
    private void IRotationLeft()
    {
        switch (_activeBlock.transform.rotation.eulerAngles.z)
        {
            case 0:
                _activeBlock.transform.position += new Vector3(2, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, -3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(1, 2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
            case 90:
                _activeBlock.transform.position += new Vector3(-1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 2, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, -3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-2, 1, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
            case 180:
                _activeBlock.transform.position += new Vector3(1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, -2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
            case 270:
                _activeBlock.transform.position += new Vector3(1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(2, -1, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
        }
    }
    /// <summary>
    /// I�~�m�u���b�N�̉E��]�␳
    /// </summary>
    private void IRotationRight()
    {
        switch (_activeBlock.transform.rotation.eulerAngles.z)
        {
            case 0:
                _activeBlock.transform.position += new Vector3(-2, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, -2, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(2, -2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;

            case 90:
                _activeBlock.transform.position += new Vector3(2, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(0, 1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(2, 2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
            case 180:
                _activeBlock.transform.position += new Vector3(-1, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 2, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, -3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(2, 2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
            case 270:
                _activeBlock.transform.position += new Vector3(-2, 0, 0);
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-3, -1, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(3, 3, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-2, -2, 0);
                    _activeBlock.RotateLeft();
                    _ghostBlock.RotateLeft();
                }
                break;
        }

    }
}
