using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    private SpawnerScript _spawner;//�u���b�N�X�|�i�[
    private BlockScript _activeBlock;//�������ꂽ�u���b�N�i�[
    [SerializeField, Header("�u���b�N�������鎞��")]
    private float _dropInterval = 0.25f;
    private float _nextDropTimer = default;//���Ƀu���b�N��������܂ł̎���

    private BordScripts _bord = default;
    private float _nextKeyDownTime = default;
    private float _nextKeySideTime = default;
    private float _nextKeyRotateTime = default;

    [SerializeField, Header("�����͂̃C���^�[�o��")]
    private float _nextKeyDownTimeInterval = default;
    [SerializeField, Header("���E���͂̃C���^�[�o��")]
    private float _nextKeySideTimeInterval = default;
    [SerializeField, Header("��]���͂̃C���^�[�o��")]
    private float _nextKeyRotateTimeInterval = default;
    [SerializeField]
    private GameObject _gameOverPanel = default;
    private bool isGameOver = false;
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

        //�u���b�N�𐶐����Ċi�[
        if (!_activeBlock)
        {
            _activeBlock = _spawner.SpwnBlock();
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
        PlayerInput();
    }
    private void PlayerInput()
    {
        if (Input.GetKey(KeyCode.D) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.D))
        {
            //�E�ɓ�����
            _activeBlock.MoveRight();
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //�͂ݏo���Ă���߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveLeft();
            }
        }
        else if (Input.GetKey(KeyCode.A) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.A))
        {
            //���ɓ�����
            _activeBlock.MoveLeft();
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //�͂ݏo���Ă���߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveRight();
            }
        }
        else if (Input.GetKey(KeyCode.E) && (Time.time > _nextKeyRotateTime))
        {
            //�E��]
            _activeBlock.RotateRight();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //�͂ݏo����߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.RotateLeft();
            }
        }
        else if (Input.GetKey(KeyCode.Q) && (Time.time > _nextKeyRotateTime))
        {
            //�E��]
            _activeBlock.RotateLeft();
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //�͂ݏo����߂�
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.RotateRight();
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
    }
    /// <summary>
    /// ��ɒ��������̏���4
    /// </summary>
    private void BottomBoard()
    {
        _activeBlock.MoveUp();
        _bord.SaveBlockInGrid(_activeBlock);

        _activeBlock = _spawner.SpwnBlock();

        //�^�C���̏�����
        _nextDropTimer = Time.time;
        _nextKeySideTime = Time.time;
        _nextKeyRotateTime = Time.time;

        _bord.ClearAllRows();//�����Ă���΍폜
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
}
