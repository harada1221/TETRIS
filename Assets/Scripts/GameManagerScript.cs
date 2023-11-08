using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField, Header("ゴーストブロックの色")]
    private Color _colorWhite = default;
    //ゴーストブロックの位置調整
    private Vector3 _ghostBlockPosition = new Vector3(0.5f, 0.5f, 0);
    //ホールドブロックの位置
    private Vector3 _holdBlockPosition = new Vector3(-6, 15, 0);

    private SpawnerScript _spawner = default;//ブロックスポナー
    private BlockScript _activeBlock = default;//生成されたブロック格納
    private BlockScript _ghostBlock = default;//生成されたゴーストブロック
    private BlockScript _holdBlock = default;//ホールドしたブロックを格納
    private BlockScript _saveBlock = default;//ホールドしたブロックを入れ替える用
    [SerializeField, Header("ブロックが落ちる時間")]
    private float _dropInterval = 0.25f;
    private float _nextDropTimer = default;//次にブロックが落ちるまでの時間

    private BordScripts _bord = default;//フィールドのスクリプト
    //入力のタイム
    private float _nextKeyDownTime = default;
    private float _nextKeySideTime = default;
    private float _nextKeyRotateTime = default;
    //ブロックが着地してから止まる時間
    private float _lookTime = default;

    [SerializeField, Header("ブロックを動かした回数")]
    private int _moveCount = 15;

    [SerializeField, Header("下入力のインターバル")]
    private float _nextKeyDownTimeInterval = default;
    [SerializeField, Header("左右入力のインターバル")]
    private float _nextKeySideTimeInterval = default;
    [SerializeField, Header("回転入力のインターバル")]
    private float _nextKeyRotateTimeInterval = default;
    [SerializeField, Header("ブロックの固定の時間")]
    private float _lookTimeInterval = default;
    [SerializeField]
    private GameObject _gameOverPanel = default;//ゲームオーバー時のパネル
    private bool isGameOver = false;//ゲームオーバーの判定
    private bool isChangeBlock = false;//ホールドをしたか
    private bool isGround = false;//ブロックが下にぶつかったか

    private void Start()
    {
        //スポナーオブジェクトを格納
        _spawner = GameObject.FindObjectOfType<SpawnerScript>();
        //ボードのスクリプトを格納
        _bord = GameObject.FindObjectOfType<BordScripts>();

        //タイマーの初期設定
        _nextKeyDownTime = Time.time + _nextKeyDownTimeInterval;
        _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
        _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
        _lookTime = Time.time + _lookTimeInterval;

        //ブロックを生成して格納
        if (!_activeBlock)
        {
            //生成したブロックを格納する
            _activeBlock = _spawner.SpwnBlock();
            //生成された同じブロックをゴーストブロックとして生成
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position , Quaternion.identity);
            //ゴーストブロックの色変え
            ColorChange();
            //ゴーストを下まで落とす
            DownGhostBlock();
        }
        //アクティブ状態だとゲームオーバーパネル消す
        if (_gameOverPanel.activeInHierarchy)
        {
            _gameOverPanel.SetActive(false);
        }
    }
    private void Update()
    {
        //ゲームオーバーだったら動かさない
        if (isGameOver)
        {
            return;
        }
        //ゴーストブロックを下まで落とす
        DownGhostBlock();
        //プレイヤーの操作
        PlayerInput();
    }
    private void PlayerInput()
    {
        //Dを押している間右に移動
        if (Input.GetKey(KeyCode.D) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.D))
        {
            //固定まで判定
            BlockLook();
            //右に動かす
            _activeBlock.MoveRight();
            _ghostBlock.MoveRight();
            //タイマー更新
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //はみ出してたら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveLeft();
                _ghostBlock.MoveLeft();
            }
        }
        //Aを押してる間左に移動
        else if (Input.GetKey(KeyCode.A) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.A))
        {
            //固定まで判定
            BlockLook();
            //左に動かす
            _activeBlock.MoveLeft();
            _ghostBlock.MoveLeft();
            //タイマー更新
            _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
            //はみ出してたら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                _activeBlock.MoveRight();
                _ghostBlock.MoveRight();
            }
        }
        //Eを押したら右回転
        else if (Input.GetKey(KeyCode.E) && (Time.time > _nextKeyRotateTime))
        {
            //固定まで判定
            BlockLook();
            //右回転
            _activeBlock.RotateRight();
            _ghostBlock.RotateRight();
            //タイマー更新
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //はみ出たら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                //回転補正OとI以外
                if (_activeBlock.GetSuperspin)
                {
                    TRotationRight();
                }
                //回転補正Iミノブロック
                if (_activeBlock.GetISpin)
                {
                    IRotationRight();
                }
            }
        }
        //Qを押したら左回転
        else if (Input.GetKey(KeyCode.Q) && (Time.time > _nextKeyRotateTime))
        {
            //固定まで判定
            BlockLook();
            //左回転
            _activeBlock.RotateLeft();
            _ghostBlock.RotateLeft();
            //タイマー更新
            _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
            //はみ出たら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                //回転補正OとI以外
                if (_activeBlock.GetSuperspin)
                {
                    TRotationLeft();
                }
                //回転補正Iミノブロック
                if (_activeBlock.GetISpin)
                {
                    IRotationLeft();
                }
            }
        }
        //ソフトドロップSを押すと早く落ちる
        else if (Input.GetKey(KeyCode.S) && (Time.time > _nextKeyDownTime) || Time.time > _nextDropTimer)
        {
            //下に動かす
            _activeBlock.MoveDown();
            //入力タイム
            _nextKeyDownTime = Time.time + _nextKeyDownTimeInterval;
            _nextDropTimer = Time.time + _dropInterval;

            //はみ出してたら戻す
            if (!_bord.CheckPosition(_activeBlock))
            {
                //範囲外だとゲームオーバー
                if (_bord.OverLimit(_activeBlock))
                {
                    GameOver();
                }
                else
                {
                    //着地したらカウント開始
                    if (!isGround)
                    {
                        isGround = true;
                        _lookTime = Time.time + _lookTimeInterval;
                    }
                    //底についた処理
                    BottomBoard();
                }
            }
        }
        //Wを押したらハードドロップ
        else if (Input.GetKeyDown(KeyCode.W))
        {
            //ぶつかるまで繰り返す
            while (_bord.CheckPosition(_activeBlock))
            {
                //下に動かす
                _activeBlock.MoveDown();
            }
            //底についた処理
            BottomBoard();

        }
        //ホールドの処理
        else if (Input.GetKeyDown(KeyCode.Z) && isChangeBlock == false)
        {
            //ホールドを１度だけにする
            isChangeBlock = true;
            if (_holdBlock == default)
            {
                //現在のブロックを生成
                _holdBlock = Instantiate(_activeBlock, _holdBlockPosition, Quaternion.identity);
                //ブロック削除
                Destroy(_activeBlock.gameObject);
                Destroy(_ghostBlock.gameObject);
                //ブロック生成
                _activeBlock = _spawner.SpwnBlock();
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
                //色を変える
                ColorChange();
                //タイムの初期化
                ResetTime();
            }
            else
            {
                //入れ替え
                _saveBlock = _activeBlock;
                _activeBlock = _holdBlock;
                _holdBlock = _saveBlock;

                //元のブロックを削除
                Destroy(_saveBlock.gameObject);
                Destroy(_activeBlock.gameObject);
                Destroy(_ghostBlock.gameObject);
                //新しくブロックを生成
                _activeBlock = Instantiate(_activeBlock, _spawner.transform.position, Quaternion.identity);
                //Iミノだったら位置調整
                if (_activeBlock.GetISpin)
                {
                    _activeBlock.transform.position += _ghostBlockPosition;
                }
                _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position , Quaternion.identity);
                //色を変える
                ColorChange();
                //ホールドブロックを削除
                Destroy(_holdBlock.gameObject);
                //表示ようホールドブロック生成
                _holdBlock = Instantiate(_saveBlock, _holdBlockPosition, Quaternion.identity);
                //タイムの初期化
                ResetTime();
            }
        }
    }
    /// <summary>
    /// 着地してから固定までの時間と回数
    /// </summary>
    private void BlockLook()
    {
        //着地してから動ける回数をカウントアップ
        _moveCount++;
        //着地から固定までの時間
        _lookTime = Time.time + _lookTimeInterval;
    }
    /// <summary>
    /// タイムの初期化
    /// </summary>
    private void ResetTime()
    {
        //タイムの初期化
        _nextDropTimer = Time.time;
        _nextKeySideTime = Time.time;
        _nextKeyRotateTime = Time.time;
        _lookTime = Time.time;
    }
    /// <summary>
    /// 底に着いた時の処理
    /// </summary>
    private void BottomBoard()
    {
        //ミノブロックを1つ上に
        _activeBlock.MoveUp();
        //一定時間経過したまたは１５回ミノブロックを動かしたか
        if (Time.time > _lookTime || Input.GetKeyDown(KeyCode.W) || _moveCount >= 15)
        {
            //配列に格納
            _bord.SaveBlockInGrid(_activeBlock);
            //ホールドをできるように
            isChangeBlock = false;
            //ブロックを消して次のブロックを生成
            Destroy(_ghostBlock.gameObject);
            _activeBlock = _spawner.SpwnBlock();
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
            ColorChange();

            //値の初期化
            ResetTime();
            _moveCount = 0;
            isGround = false;
            //揃っていれば削除
            _bord.ClearAllRows();
        }
    }
    /// <summary>
    /// ゲームオーバーの時呼び出す
    /// </summary>
    private void GameOver()
    {
        //ブロックを上にあげる
        _activeBlock.MoveUp();
        //表示する
        if (!_gameOverPanel.activeInHierarchy)
        {
            _gameOverPanel.SetActive(true);
        }
        //ゲームオーバーにする
        isGameOver = true;
    }
    /// <summary>
    /// シーンを呼び出す
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    /// <summary>
    /// ぶつかるまで下におとす
    /// </summary>
    private void DownGhostBlock()
    {
        //ゴーストブロックの位置を親の場所に移動
        _ghostBlock.transform.position = _activeBlock.transform.position;
        //ぶつかるまで下に動かす
        while (_bord.CheckPosition(_ghostBlock))
        {
            _ghostBlock.MoveDown();
        }
        //ぶつかったら１つ上に
        _ghostBlock.MoveUp();
    }
    /// <summary>
    /// オブジェクトの色を変える
    /// </summary>
    private void ColorChange()
    {
        Transform parent = _ghostBlock.transform;
        SpriteRenderer chidren;

        // 子オブジェクトを全て取得する
        foreach (Transform child in parent)
        {
            //色を変える
            chidren = child.GetComponent<SpriteRenderer>();
            chidren.color = _colorWhite;
        }
    }
    /// <summary>
    /// 左回転の回転補正
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
    /// 角度が0度の左回転
    /// </summary>
    private void LeftAngleZero()
    {
        //ぶつからなくなるまで繰り返す
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
                    //ぶつからなかったら元に戻す
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
    /// 角度が90度の左回転
    /// </summary>
    private void LeftAngleNinety()
    {  //ぶつからなくなるまで繰り返す
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
                    //ぶつからなかったら元に戻す
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
    ///  角度が180度の左回転
    /// </summary>
    private void LeftAngleHundred()
    {
        //ぶつからなくなるまで繰り返す
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
                    //ぶつからなかったら元に戻す
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
    ///  角度が270度の左回転
    /// </summary>
    private void LeftAngleTwoHundred()
    {
        //ぶつからなくなるまで繰り返す
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
                    //ぶつからなかったら元に戻す
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
    /// 右回転の回転補正
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
    /// Iミノブロックの左回転補正
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
    /// Iミノブロックの右回転補正
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
