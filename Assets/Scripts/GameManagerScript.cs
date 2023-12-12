using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField, Header("�S�[�X�g�u���b�N�̐F")]
    private Color _colorWhite = default;
    [SerializeField, Header("�^�C�g���̃V�[��")]
    private string _titleScen = default;
    //�S�[�X�g�u���b�N�̈ʒu����
    private Vector3 _ghostBlockPosition = new Vector3(0.5f, 0.5f, 0);
    //�z�[���h�u���b�N�̈ʒu
    private Vector3 _holdBlockPosition = new Vector3(-6, 15, 0);

    //�u���b�N�X�|�i�[
    private SpawnerScript _spawner = default;
    //�������ꂽ�u���b�N�i�[
    private BlockScript _activeBlock = default;
    //�������ꂽ�S�[�X�g�u���b�N
    private BlockScript _ghostBlock = default;
    //�z�[���h�����u���b�N���i�[
    private BlockScript _holdBlock = default;
    //�z�[���h�����u���b�N�����ւ���p
    private BlockScript _saveBlock = default;
    [SerializeField, Header("�u���b�N�������鎞��")]
    private float _dropInterval = 0.25f;
    //���Ƀu���b�N��������܂ł̎��Ԃ��J�E���g
    private float _nextDropTimer = default;

    //�t�B�[���h�̃X�N���v�g
    private FieldScripts _fildScript = default;
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
    [SerializeField, Header("�Q�[���I�[�o�[���̃p�l��")]
    private GameObject _gameOverPanel = default;
    //�Q�[���I�[�o�[�̔���
    private bool _isGameOver = false;
    //�z�[���h��������
    private bool _isChangeBlock = false;
    //�u���b�N�����ɂԂ�������
    private bool _isGround = false;

    /// <summary>
    /// �X�V�O����
    /// </summary>
    private void Start()
    {
        //�X�|�i�[�I�u�W�F�N�g���i�[
        _spawner = GameObject.FindObjectOfType<SpawnerScript>();
        //�{�[�h�̃X�N���v�g���i�[
        _fildScript = GameObject.FindObjectOfType<FieldScripts>();

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
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
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
    /// <summary>
    /// �X�V����
    /// </summary>
    private void Update()
    {
        //�Q�[���I�[�o�[�������瓮�����Ȃ�
        if (_isGameOver)
        {
            return;
        }
        //�S�[�X�g�u���b�N�����܂ŗ��Ƃ�
        DownGhostBlock();
        //�v���C���[�̑���
        PlayerInput();
    }
    /// <summary>
    /// �v���C���[����
    /// �ʃN���X�ɂ���
    /// �v���C���[�̃C���v�b�g���Q�[���}�l�[�W���ɏ����̂͂悭�Ȃ��ʃN���X�ɂ��ׂ�
    /// </summary>
    private void PlayerInput()
    {
        //D���������Ƃ��Ɖ����Ă���ԉE�Ɉړ�
        if (Input.GetKey(KeyCode.D) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.D))
        {
            //�E�ړ�
            MoveRight();
        }
        //A���������Ƃ��Ɖ����Ă�ԍ��Ɉړ�
        else if (Input.GetKey(KeyCode.A) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.A))
        {
            //���ړ�
            MoveLeft();
        }
        //E���������Ƃ��E��]
        else if (Input.GetKey(KeyCode.E) && (Time.time > _nextKeyRotateTime))
        {
            //�E��]
            RotationRight();
        }
        //Q���������獶��]
        else if (Input.GetKey(KeyCode.Q) && (Time.time > _nextKeyRotateTime))
        {
            //����]
            RotationLeft();
        }
        //�\�t�g�h���b�vS�������Ƒ���������
        else if (Input.GetKey(KeyCode.S) && (Time.time > _nextKeyDownTime) || Time.time > _nextDropTimer)
        {
            //�������x�㏸������
            SoftDrop();
        }
        //W����������n�[�h�h���b�v
        else if (Input.GetKeyDown(KeyCode.W))
        {
            //�n�[�h�h���b�v������
            HardDrop();
        }
        //�z�[���h�̏���
        else if (Input.GetKeyDown(KeyCode.Z) && _isChangeBlock == false)
        {
            Hold();
        }
    }
    /// <summary>
    /// �z�[���h�̏���
    /// </summary>
    private void Hold()
    {
        //�z�[���h���P�x�����ɂ���
        _isChangeBlock = true;
        //�z�[���h���P��ڂ̎�
        if (_holdBlock == null)
        {
            //���݂̃u���b�N�𐶐�
            _holdBlock = Instantiate(_activeBlock, _holdBlockPosition, Quaternion.identity);
            //�u���b�N�폜
            _activeBlock.gameObject.SetActive(false);
            _ghostBlock.gameObject.SetActive(false);
            //�u���b�N����
            _activeBlock = _spawner.SpwnBlock();
            //�S�[�X�g�u���b�N����
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
            //�F��ς���
            ColorChange();
            //�^�C���̏�����
            ResetTime();
        }
        //�z�[���h��2��ڂ̎�
        else
        {
            //�u���b�N�̓���ւ�
            _saveBlock = _activeBlock;
            _activeBlock = _holdBlock;
            _holdBlock = _saveBlock;

            //���̃u���b�N���폜
            _activeBlock.gameObject.SetActive(false);
            _ghostBlock.gameObject.SetActive(false);
            _holdBlock.gameObject.SetActive(false);

            //�V�����u���b�N�𐶐�
            _activeBlock = Instantiate(_activeBlock, _spawner.transform.position, Quaternion.identity);
            //�u���b�N�̕\��
            _activeBlock.gameObject.SetActive(true);
            //I�~�m��������ʒu����
            if (_activeBlock.GetISpin)
            {
                _activeBlock.transform.position += _ghostBlockPosition;
            }
            //�S�[�X�g�u���b�N����
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
            //�S�[�X�g�u���b�N�\��
            _ghostBlock.gameObject.SetActive(true);
            //�F��ς���
            ColorChange();
            //�\���悤�z�[���h�u���b�N����
            _holdBlock = Instantiate(_saveBlock, _holdBlockPosition, Quaternion.identity);
            _holdBlock.gameObject.SetActive(true);
            //�^�C���̏�����
            ResetTime();
        }
    }
    /// <summary>
    /// �n�[�h�h���b�v������
    /// </summary>
    private void HardDrop()
    {
        //�Ԃ���܂ŌJ��Ԃ�
        while (_fildScript.CheckPosition(_activeBlock))
        {
            //���ɓ�����
            _activeBlock.MoveDown();
        }
        //��ɂ�������
        BottomBoard();
    }
    /// <summary>
    /// �E�Ɉړ�����
    /// </summary>
    private void MoveRight()
    {
        //�Œ�܂Ŕ���
        BlockLook();
        //�E�ɓ�����
        _activeBlock.MoveRight();
        _ghostBlock.MoveRight();
        //�^�C�}�[�X�V
        _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
        //�͂ݏo���Ă���߂�
        if (!_fildScript.CheckPosition(_activeBlock))
        {
            //���ɖ߂�
            _activeBlock.MoveLeft();
            _ghostBlock.MoveLeft();
        }
    }
    /// <summary>
    /// ���ړ�������
    /// </summary>
    private void MoveLeft()
    {
        //�Œ�܂Ŕ���
        BlockLook();
        //���ɓ�����
        _activeBlock.MoveLeft();
        _ghostBlock.MoveLeft();
        //�^�C�}�[�X�V
        _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
        //�͂ݏo���Ă���߂�
        if (!_fildScript.CheckPosition(_activeBlock))
        {
            //�E�ɖ߂�
            _activeBlock.MoveRight();
            _ghostBlock.MoveRight();
        }
    }
    /// <summary>
    /// �E��]������
    /// </summary>
    private void RotationRight()
    {
        //�Œ�܂Ŕ���
        BlockLook();
        //�E��]
        _activeBlock.RotateRight();
        _ghostBlock.RotateRight();
        //�^�C�}�[�X�V
        _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
        //�t�B�[���h�̊O�ɏo����
        if (!_fildScript.CheckPosition(_activeBlock))
        {
            //����]
            _activeBlock.RotateLeft();
            _ghostBlock.RotateLeft();
        }
    }
    private void RotationLeft()
    {
        //�Œ�܂Ŕ���
        BlockLook();
        //����]
        _activeBlock.RotateLeft();
        _ghostBlock.RotateLeft();
        //�^�C�}�[�X�V
        _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
        //�t�B�[���h�̊O�ɏo����
        if (!_fildScript.CheckPosition(_activeBlock))
        {
            //�E��]������
            _activeBlock.RotateRight();
            _ghostBlock.RotateRight();
        }
    }
    /// <summary>
    /// �������x�𑁂�����
    /// </summary>
    private void SoftDrop()
    {
        //���ɓ�����
        _activeBlock.MoveDown();
        //���̓^�C���J�E���g����
        _nextKeyDownTime = Time.time + _nextKeyDownTimeInterval;
        _nextDropTimer = Time.time + _dropInterval;

        //�͂ݏo���Ă���߂�
        if (!_fildScript.CheckPosition(_activeBlock))
        {
            //�͈͊O���ƃQ�[���I�[�o�[
            if (_fildScript.GameOverLimit(_activeBlock))
            {
                GameOver();
            }
            else
            {
                //���n������J�E���g�J�n
                if (!_isGround)
                {
                    _isGround = true;
                    _lookTime = Time.time + _lookTimeInterval;
                }
                //��ɂ�������
                BottomBoard();
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
        //��莞�Ԍo�߂����܂��͂P�T��~�m�u���b�N�𓮂�������Œ�
        if (Time.time > _lookTime || Input.GetKeyDown(KeyCode.W) || _moveCount >= 15)
        {
            //�z��Ɋi�[
            _fildScript.SaveBlockInGrid(_activeBlock);
            //�z�[���h���ł���悤��
            _isChangeBlock = false;
            //�u���b�N�������Ď��̃u���b�N�𐶐�
            _ghostBlock.gameObject.SetActive(false);
            _activeBlock = _spawner.SpwnBlock();
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
            _ghostBlock.gameObject.SetActive(true);
            ColorChange();
            //�l�̏�����
            ResetTime();
            _moveCount = 0;
            _isGround = false;
            //�����Ă���΍폜
            _fildScript.ClearAllRows();
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
        _isGameOver = true;
    }
    /// <summary>
    /// �V�[�����Ăяo��
    /// </summary>
    public void Restart()
    {
        //�{�^���ŌĂяo���V�[��
        SceneManager.LoadScene(_titleScen);
    }
    /// <summary>
    /// �Ԃ���܂ŉ��ɂ��Ƃ�
    /// </summary>
    private void DownGhostBlock()
    {
        //�S�[�X�g�u���b�N�̈ʒu��e�̏ꏊ�Ɉړ�
        _ghostBlock.transform.position = _activeBlock.transform.position;
        //�Ԃ���܂ŌJ��Ԃ�
        while (_fildScript.CheckPosition(_ghostBlock))
        {
            //���ɗ��Ƃ�
            _ghostBlock.MoveDown();
        }
        //�Ԃ�������P���
        _ghostBlock.MoveUp();
    }
    /// <summary>
    /// �S�[�X�g�u���b�N�̐F��ς���
    /// </summary>
    private void ColorChange()
    {
        //�e�I�u�W�F�N�g
        Transform parent = _ghostBlock.transform;
        //�q�I�u�W�F�N�g��SpriteRenderer
        SpriteRenderer chidren = default;

        // �q�I�u�W�F�N�g��S�Ď擾����
        foreach (Transform child in parent)
        {
            //SpriteRenderer�̐F��ς���
            chidren = child.GetComponent<SpriteRenderer>();
            chidren.color = _colorWhite;
            child.gameObject.tag = "Untagged";
        }
    }
}
