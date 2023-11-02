using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField, Header("�S�[�X�g�u���b�N�̐F")]
    private Color _colorWhite = default;
    private Vector3 _ghostBlockPosition = new Vector3(0, 0, 0.5f);

    private SpawnerScript _spawner = default;//�u���b�N�X�|�i�[
    private BlockScript _activeBlock = default;//�������ꂽ�u���b�N�i�[
    private BlockScript _ghostBlock = default;//�������ꂽ�S�[�X�g�u���b�N
    private BlockScript _holdBlock = default;//�z�[���h�����u���b�N���i�[
    private BlockScript _saveBlock = default;//�z�[���h�����u���b�N�����ւ���p
    [SerializeField, Header("�u���b�N�������鎞��")]
    private float _dropInterval = 0.25f;
    private float _nextDropTimer = default;//���Ƀu���b�N��������܂ł̎���

    private BordScripts _bord = default;//�t�B�[���h�̃X�N���v�g
    private float _nextKeyDownTime = default;
    private float _nextKeySideTime = default;
    private float _nextKeyRotateTime = default;
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
    private GameObject _gameOverPanel = default;
    private bool isGameOver = false;//�Q�[���I�[�o�[�̔���
    private bool isChangeBlock = false;//�z�[���h����������
    private bool isGround = false;

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
            _activeBlock = _spawner.SpwnBlock();
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position + _ghostBlockPosition, Quaternion.identity);
            ColorChange();
            while (_bord.CheckPosition(_ghostBlock))
            {
                //���ɓ�����
                _ghostBlock.MoveDown();
            }
            _ghostBlock.MoveUp();
        }
        //�A�N�e�B�u��Ԃ��Ə���
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
        //�E�Ɉړ�
        if (Input.GetKey(KeyCode.D) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.D))
        {
            //��������񐔂��J�E���g�A�b�v
            _moveCount++;
            //���n����Œ�܂ł̎���
            _lookTime = Time.time + _lookTimeInterval;
            //�E�ɓ�����
            _activeBlock.MoveRight();
            _ghostBlock.MoveRight();
            //�^�C�}�[���Z�b�g
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //�͂ݏo���Ă���߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveLeft();
                _ghostBlock.MoveLeft();
            }
        }
        //���Ɉړ�
        else if (Input.GetKey(KeyCode.A) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.A))
        {
            //��������񐔂��J�E���g�A�b�v
            _moveCount++;
            //���n����Œ�܂ł̎���
            _lookTime = Time.time + _lookTimeInterval;
            //���ɓ�����
            _activeBlock.MoveLeft();
            _ghostBlock.MoveLeft();
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //�͂ݏo���Ă���߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveRight();
                _ghostBlock.MoveRight();
            }
        }
        //�E��]
        else if (Input.GetKey(KeyCode.E) && (Time.time > _nextKeyRotateTime))
        {
            //��������񐔂��J�E���g�A�b�v
            _moveCount++;
            //���n����Œ�܂ł̎���
            _lookTime = Time.time + _lookTimeInterval;
            //�E��]
            _activeBlock.RotateRight();
            _ghostBlock.RotateRight();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //�͂ݏo����߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                //��]�␳O��I�ȊO
                if (_activeBlock.GetSuperspin)
                {
                    TRotationRight();
                }
                if (_activeBlock.GetISpin)
                {
                    IRotationRight();
                }
            }
        }
        //����]
        else if (Input.GetKey(KeyCode.Q) && (Time.time > _nextKeyRotateTime))
        {
            //���n���Ă��瓮�����
            _moveCount++;
            //���n����Œ�܂ł̎���
            _lookTime = Time.time + _lookTimeInterval;
            //����]
            _activeBlock.RotateLeft();
            _ghostBlock.RotateLeft();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //�͂ݏo����߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                if (_activeBlock.GetSuperspin)
                {
                    TRotationLeft();
                }
                if (_activeBlock.GetISpin)
                {
                    IRotationLeft();
                }
            }
        }
        //�\�t�g�h���b�v
        else if (Input.GetKey(KeyCode.S) && (Time.time > _nextKeyDownTime) || Time.time > _nextDropTimer)
        {
            //���ɓ�����
            _activeBlock.MoveDown();

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
        //�n�[�h�h���b�v
        else if (Input.GetKeyDown(KeyCode.W))
        {
            //�Ԃ���܂ŌJ��Ԃ�
            while (_bord.CheckPosition(_activeBlock))
            {
                //���ɓ�����
                _activeBlock.MoveDown();
            }

            BottomBoard();

        }
        //�z�[���h�̏���
        else if (Input.GetKeyDown(KeyCode.Z) && isChangeBlock == false)
        {
            //�z�[���h���P�x�����ɂ���
            isChangeBlock = true;
            if (_holdBlock == default)
            {
                //1��ڂ̏���
                _holdBlock = Instantiate(_activeBlock, new Vector3(-6, 15, 0), Quaternion.identity);
                //�u���b�N�폜
                Destroy(_activeBlock.gameObject);
                Destroy(_ghostBlock.gameObject);
                //�u���b�N����
                _activeBlock = _spawner.SpwnBlock();
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position + _ghostBlockPosition, Quaternion.identity);
                //�F��ς���
                ColorChange();
                //�^�C���̏�����
                _nextDropTimer = Time.time;
                _nextKeySideTime = Time.time;
                _nextKeyRotateTime = Time.time;
                _lookTime = Time.time;
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
                    _activeBlock.transform.position += new Vector3(0.5f, 0.5f, 0);
                }
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position + _ghostBlockPosition, Quaternion.identity);
                //�F��ς���
                ColorChange();
                //�z�[���h�u���b�N���폜
                Destroy(_holdBlock.gameObject);
                //�\���悤�z�[���h�u���b�N����
                _holdBlock = Instantiate(_saveBlock, new Vector3(-6, 15, 0), Quaternion.identity);

                //�^�C���̏�����
                _nextDropTimer = Time.time;
                _nextKeySideTime = Time.time;
                _nextKeyRotateTime = Time.time;
                _lookTime = Time.time;
            }
        }
    }
    /// <summary>
    /// ��ɒ��������̏���4
    /// </summary>
    private void BottomBoard()
    {
        _activeBlock.MoveUp();
        if (Time.time > _lookTime || Input.GetKeyDown(KeyCode.W) || _moveCount >= 15)
        {
            //�z��Ɋi�[
            _bord.SaveBlockInGrid(_activeBlock);
            //�z�[���h���ł���悤��
            isChangeBlock = false;
            //�u���b�N�������Ď��̃u���b�N�𐶐�
            Destroy(_ghostBlock.gameObject);
            _activeBlock = _spawner.SpwnBlock();
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position + _ghostBlockPosition, Quaternion.identity);
            ColorChange();
            //�l�̏�����
            _nextDropTimer = Time.time;
            _nextKeySideTime = Time.time;
            _nextKeyRotateTime = Time.time;
            _lookTime = Time.time;
            _moveCount = 0;
            isGround = false;
            _bord.ClearAllRows();//�����Ă���΍폜


        }
    }
    /// <summary>
    /// �Q�[���I�[�o�[�̎��Ăяo��
    /// </summary>
    private void GameOver()
    {
        _activeBlock.MoveUp();
        //�\������
        if (!_gameOverPanel.activeInHierarchy)
        {
            _gameOverPanel.SetActive(true);
        }

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
        _ghostBlock.transform.position = _activeBlock.transform.position + _ghostBlockPosition;
        while (_bord.CheckPosition(_ghostBlock))
        {
            //���ɓ�����
            _ghostBlock.MoveDown();
        }
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
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
            case 90:
                _activeBlock.transform.position += new Vector3(1, 0, 0);

                if ((!_bord.CheckPosition(_activeBlock)))
                {
                    _activeBlock.transform.position += new Vector3(0, 1, 0);
                }
                if ((!_bord.CheckPosition(_activeBlock)))
                {
                    _activeBlock.transform.position += new Vector3(-1, -3, 0);
                }
                if ((!_bord.CheckPosition(_activeBlock)))
                {
                    _activeBlock.transform.position += new Vector3(1, 0, 0);
                }
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.transform.position += new Vector3(-1, 2, 0);
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
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
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
            case 180:
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
                    _activeBlock.RotateRight();
                    _ghostBlock.RotateRight();
                }
                break;
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
