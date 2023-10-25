using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private SpawnerScript _spawner;//スポナー
    private BlockScript _activeBlock;//生成されたブロック格納
    [SerializeField, Header("ブロックが落ちる時間")]
    private float _dropInterval = 0.25f;
    private float _nextDropTimer = default;//次にブロックが落ちるまでの時間

    private BordScripts _bord = default;
    private void Start()
    {
        //スポナーオブジェクトを格納
        _spawner = GameObject.FindObjectOfType<SpawnerScript>();
        _bord = GameObject.FindObjectOfType<BordScripts>();

        //ブロックを生成して格納
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

                //枠からはみ出していないか
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
