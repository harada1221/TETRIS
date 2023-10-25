using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private SpawnerScript _spawner;//�X�|�i�[
    private BlockScript _activeBlock;//�������ꂽ�u���b�N�i�[
    [SerializeField, Header("�u���b�N�������鎞��")]
    private float _dropInterval = 0.25f;
    private float _nextDropTimer = default;//���Ƀu���b�N��������܂ł̎���

    private BordScripts _bord = default;
    private void Start()
    {
        //�X�|�i�[�I�u�W�F�N�g���i�[
        _spawner = GameObject.FindObjectOfType<SpawnerScript>();
        _bord = GameObject.FindObjectOfType<BordScripts>();

        //�u���b�N�𐶐����Ċi�[
        if (!_activeBlock)
        {
            _activeBlock = _spawner.SpwnBlock();
        }
    }
    private void Update()
    {
        if (Time.time > _nextDropTimer)
        {
            _nextDropTimer = Time.time + _dropInterval;
            if (_activeBlock)
            {
                _activeBlock.MoveDown();

                //�g����͂ݏo���Ă��Ȃ���
                if (!_bord.CheckPosition(_activeBlock))
                {
                    _activeBlock.MoveUp();

                    _bord.SaveBlockInGrid(_activeBlock);

                    _activeBlock = _spawner.SpwnBlock();
                }
            }
        }
    }
}
