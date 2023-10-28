using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    private SpawnerScript _spawner = default;//�u���b�N�X�|�i�[
    private BlockScript _activeBlock = default;//�������ꂽ�u���b�N�i�[
    private BlockScript _ghostBlock = default;//�������ꂽ�S�[�X�g�u���b�N
    private BlockScript _holdBlock = default;//�z�[���h�����u���b�N���i�[
    private BlockScript _saveBlock = default;//�z�[���h�����u���b�N�����ւ���p
    [SerializeField, Header("�u���b�N�������鎞��")]
    private float _dropInterval = 0.25f;
    private float _nextDropTimer = default;//���Ƀu���b�N��������܂ł̎���

    private BordScripts _bord = default;
    private float _nextKeyDownTime = default;
    private float _nextKeySideTime = default;
    private float _nextKeyRotateTime = default;
    private float _lookTime = default;

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
    private bool isGameOver = false;
    private bool isChangeBlock = false;

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
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
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

        if (isGameOver)
        {
            return;
        }
        DownGhostBlock();
        PlayerInput();
    }
    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.D) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.D))
        {
            _lookTime = Time.time + _lookTimeInterval;
            //�E�ɓ�����
            _activeBlock.MoveRight();
            _ghostBlock.MoveRight();
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //�͂ݏo���Ă���߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveLeft();
                _ghostBlock.MoveLeft();
            }
        }
        else if (Input.GetKey(KeyCode.A) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.A))
        {
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
        else if (Input.GetKey(KeyCode.E) && (Time.time > _nextKeyRotateTime))
        {
            _lookTime = Time.time + _lookTimeInterval;
            //�E��]
            _activeBlock.RotateRight();
            _ghostBlock.RotateRight();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //�͂ݏo����߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.RotateLeft();
                _ghostBlock.RotateLeft();
            }
        }
        else if (Input.GetKey(KeyCode.Q) && (Time.time > _nextKeyRotateTime))
        {
            _lookTime = Time.time + _lookTimeInterval;
            //�E��]
            _activeBlock.RotateLeft();
            _ghostBlock.RotateLeft();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //�͂ݏo����߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.RotateRight();
                _ghostBlock.RotateRight();
            }
        }
        else if (Input.GetKey(KeyCode.S) && (Time.time > _nextKeyDownTime) || Time.time > _nextDropTimer)
        {
            //���ɓ�����
            _activeBlock.MoveDown();

            _nextKeyDownTime = Time.time + _nextKeyDownTimeInterval;
            _nextDropTimer = Time.time + _dropInterval;

            //�͂ݏo���Ă���߂�
            if (!_bord.CheckPosition(_activeBlock))
            {

                if (_bord.OverLimit(_activeBlock))
                {
                    GameOver();
                }
                else
                {
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
            isChangeBlock = true;
            if (_holdBlock == default)
            {
                //1��ڂ̏���
                _holdBlock = Instantiate(_activeBlock, new Vector3(-6, 15, 0), Quaternion.identity);
                Destroy(_activeBlock.gameObject);
                Destroy(_ghostBlock.gameObject);
                _activeBlock = _spawner.SpwnBlock();
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);

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

                Destroy(_saveBlock.gameObject);
                Destroy(_activeBlock.gameObject);
                Destroy(_ghostBlock.gameObject);
                _activeBlock = Instantiate(_activeBlock, _spawner.transform.position, Quaternion.identity);
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);

                Destroy(_holdBlock.gameObject);

                _holdBlock = Instantiate(_saveBlock, new Vector3(-6, 15, 0), Quaternion.identity);

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
        if (Time.time > _lookTime || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.W))
        {
            _bord.SaveBlockInGrid(_activeBlock);

            isChangeBlock = false;
            Destroy(_ghostBlock.gameObject);
            _activeBlock = _spawner.SpwnBlock();
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);

            //�^�C���̏�����
            _nextDropTimer = Time.time;
            _nextKeySideTime = Time.time;
            _nextKeyRotateTime = Time.time;
            _lookTime = Time.time;
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
        SceneManager.LoadScene(1);
    }
    private void DownGhostBlock()
    {
        _ghostBlock.transform.position = _activeBlock.transform.position;
        while (_bord.CheckPosition(_ghostBlock))
        {
            //���ɓ�����
            _ghostBlock.MoveDown();
        }
        _ghostBlock.MoveUp();
    }
    private void ColorChange()
    {
        Transform parentTransform = _ghostBlock.transform;

        // �q�I�u�W�F�N�g��S�Ď擾����
        foreach (Transform child in parentTransform)
        {
         
        }
    }
}
