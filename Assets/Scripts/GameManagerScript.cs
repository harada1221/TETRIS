using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField, Header("ゴーストブロックの色")]
    private Color _colorWhite = default;
    [SerializeField, Header("タイトルのシーン")]
    private string _titleScen = default;
    //ゴーストブロックの位置調整
    private Vector3 _ghostBlockPosition = new Vector3(0.5f, 0.5f, 0);
    //ホールドブロックの位置
    private Vector3 _holdBlockPosition = new Vector3(-6, 15, 0);

    //ブロックスポナー
    private SpawnerScript _spawner = default;
    //生成されたブロック格納
    private BlockScript _activeBlock = default;
    //生成されたゴーストブロック
    private BlockScript _ghostBlock = default;
    //ホールドしたブロックを格納
    private BlockScript _holdBlock = default;
    //ホールドしたブロックを入れ替える用
    private BlockScript _saveBlock = default;
    [SerializeField, Header("ブロックが落ちる時間")]
    private float _dropInterval = 0.25f;
    //次にブロックが落ちるまでの時間をカウント
    private float _nextDropTimer = default;

    //フィールドのスクリプト
    private FieldScripts _fildScript = default;
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
    [SerializeField, Header("ゲームオーバー時のパネル")]
    private GameObject _gameOverPanel = default;
    //ゲームオーバーの判定
    private bool _isGameOver = false;
    //ホールドをしたか
    private bool _isChangeBlock = false;
    //ブロックが下にぶつかったか
    private bool _isGround = false;

    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {
        //スポナーオブジェクトを格納
        _spawner = GameObject.FindObjectOfType<SpawnerScript>();
        //ボードのスクリプトを格納
        _fildScript = GameObject.FindObjectOfType<FieldScripts>();

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
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
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
    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //ゲームオーバーだったら動かさない
        if (_isGameOver)
        {
            return;
        }
        //ゴーストブロックを下まで落とす
        DownGhostBlock();
        //プレイヤーの操作
        PlayerInput();
    }
    /// <summary>
    /// プレイヤー操作
    /// 別クラスにする
    /// プレイヤーのインプットをゲームマネージャに書くのはよくない別クラスにすべき
    /// </summary>
    private void PlayerInput()
    {
        //Dを押したときと押している間右に移動
        if (Input.GetKey(KeyCode.D) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.D))
        {
            //右移動
            MoveRight();
        }
        //Aを押したときと押してる間左に移動
        else if (Input.GetKey(KeyCode.A) && (Time.time > _nextKeySideTime) || Input.GetKeyDown(KeyCode.A))
        {
            //左移動
            MoveLeft();
        }
        //Eを押したとき右回転
        else if (Input.GetKey(KeyCode.E) && (Time.time > _nextKeyRotateTime))
        {
            //右回転
            RotationRight();
        }
        //Qを押したら左回転
        else if (Input.GetKey(KeyCode.Q) && (Time.time > _nextKeyRotateTime))
        {
            //左回転
            RotationLeft();
        }
        //ソフトドロップSを押すと早く落ちる
        else if (Input.GetKey(KeyCode.S) && (Time.time > _nextKeyDownTime) || Time.time > _nextDropTimer)
        {
            //落下速度上昇させる
            SoftDrop();
        }
        //Wを押したらハードドロップ
        else if (Input.GetKeyDown(KeyCode.W))
        {
            //ハードドロップさせる
            HardDrop();
        }
        //ホールドの処理
        else if (Input.GetKeyDown(KeyCode.Z) && _isChangeBlock == false)
        {
            Hold();
        }
    }
    /// <summary>
    /// ホールドの処理
    /// </summary>
    private void Hold()
    {
        //ホールドを１度だけにする
        _isChangeBlock = true;
        //ホールドが１回目の時
        if (_holdBlock == null)
        {
            //現在のブロックを生成
            _holdBlock = Instantiate(_activeBlock, _holdBlockPosition, Quaternion.identity);
            //ブロック削除
            _activeBlock.gameObject.SetActive(false);
            _ghostBlock.gameObject.SetActive(false);
            //ブロック生成
            _activeBlock = _spawner.SpwnBlock();
            //ゴーストブロック生成
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
            //色を変える
            ColorChange();
            //タイムの初期化
            ResetTime();
        }
        //ホールドが2回目の時
        else
        {
            //ブロックの入れ替え
            _saveBlock = _activeBlock;
            _activeBlock = _holdBlock;
            _holdBlock = _saveBlock;

            //元のブロックを削除
            _activeBlock.gameObject.SetActive(false);
            _ghostBlock.gameObject.SetActive(false);
            _holdBlock.gameObject.SetActive(false);

            //新しくブロックを生成
            _activeBlock = Instantiate(_activeBlock, _spawner.transform.position, Quaternion.identity);
            //ブロックの表示
            _activeBlock.gameObject.SetActive(true);
            //Iミノだったら位置調整
            if (_activeBlock.GetISpin)
            {
                _activeBlock.transform.position += _ghostBlockPosition;
            }
            //ゴーストブロック生成
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
            //ゴーストブロック表示
            _ghostBlock.gameObject.SetActive(true);
            //色を変える
            ColorChange();
            //表示ようホールドブロック生成
            _holdBlock = Instantiate(_saveBlock, _holdBlockPosition, Quaternion.identity);
            _holdBlock.gameObject.SetActive(true);
            //タイムの初期化
            ResetTime();
        }
    }
    /// <summary>
    /// ハードドロップさせる
    /// </summary>
    private void HardDrop()
    {
        //ぶつかるまで繰り返す
        while (_fildScript.CheckPosition(_activeBlock))
        {
            //下に動かす
            _activeBlock.MoveDown();
        }
        //底についた処理
        BottomBoard();
    }
    /// <summary>
    /// 右に移動する
    /// </summary>
    private void MoveRight()
    {
        //固定まで判定
        BlockLook();
        //右に動かす
        _activeBlock.MoveRight();
        _ghostBlock.MoveRight();
        //タイマー更新
        _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
        //はみ出してたら戻す
        if (!_fildScript.CheckPosition(_activeBlock))
        {
            //左に戻す
            _activeBlock.MoveLeft();
            _ghostBlock.MoveLeft();
        }
    }
    /// <summary>
    /// 左移動をする
    /// </summary>
    private void MoveLeft()
    {
        //固定まで判定
        BlockLook();
        //左に動かす
        _activeBlock.MoveLeft();
        _ghostBlock.MoveLeft();
        //タイマー更新
        _nextKeySideTime = Time.time + _nextKeySideTimeInterval;
        //はみ出してたら戻す
        if (!_fildScript.CheckPosition(_activeBlock))
        {
            //右に戻す
            _activeBlock.MoveRight();
            _ghostBlock.MoveRight();
        }
    }
    /// <summary>
    /// 右回転をする
    /// </summary>
    private void RotationRight()
    {
        //固定まで判定
        BlockLook();
        //右回転
        _activeBlock.RotateRight();
        _ghostBlock.RotateRight();
        //タイマー更新
        _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
        //フィールドの外に出たか
        if (!_fildScript.CheckPosition(_activeBlock))
        {
            //左回転
            _activeBlock.RotateLeft();
            _ghostBlock.RotateLeft();
        }
    }
    private void RotationLeft()
    {
        //固定まで判定
        BlockLook();
        //左回転
        _activeBlock.RotateLeft();
        _ghostBlock.RotateLeft();
        //タイマー更新
        _nextKeyRotateTime = Time.time + _nextKeyRotateTimeInterval;
        //フィールドの外に出たか
        if (!_fildScript.CheckPosition(_activeBlock))
        {
            //右回転させる
            _activeBlock.RotateRight();
            _ghostBlock.RotateRight();
        }
    }
    /// <summary>
    /// 落下速度を早くする
    /// </summary>
    private void SoftDrop()
    {
        //下に動かす
        _activeBlock.MoveDown();
        //入力タイムカウントする
        _nextKeyDownTime = Time.time + _nextKeyDownTimeInterval;
        _nextDropTimer = Time.time + _dropInterval;

        //はみ出してたら戻す
        if (!_fildScript.CheckPosition(_activeBlock))
        {
            //範囲外だとゲームオーバー
            if (_fildScript.GameOverLimit(_activeBlock))
            {
                GameOver();
            }
            else
            {
                //着地したらカウント開始
                if (!_isGround)
                {
                    _isGround = true;
                    _lookTime = Time.time + _lookTimeInterval;
                }
                //底についた処理
                BottomBoard();
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
        //一定時間経過したまたは１５回ミノブロックを動かしたら固定
        if (Time.time > _lookTime || Input.GetKeyDown(KeyCode.W) || _moveCount >= 15)
        {
            //配列に格納
            _fildScript.SaveBlockInGrid(_activeBlock);
            //ホールドをできるように
            _isChangeBlock = false;
            //ブロックを消して次のブロックを生成
            _ghostBlock.gameObject.SetActive(false);
            _activeBlock = _spawner.SpwnBlock();
            _ghostBlock = Instantiate(_activeBlock, _activeBlock.transform.position, Quaternion.identity);
            _ghostBlock.gameObject.SetActive(true);
            ColorChange();
            //値の初期化
            ResetTime();
            _moveCount = 0;
            _isGround = false;
            //揃っていれば削除
            _fildScript.ClearAllRows();
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
        _isGameOver = true;
    }
    /// <summary>
    /// シーンを呼び出す
    /// </summary>
    public void Restart()
    {
        //ボタンで呼び出すシーン
        SceneManager.LoadScene(_titleScen);
    }
    /// <summary>
    /// ぶつかるまで下におとす
    /// </summary>
    private void DownGhostBlock()
    {
        //ゴーストブロックの位置を親の場所に移動
        _ghostBlock.transform.position = _activeBlock.transform.position;
        //ぶつかるまで繰り返す
        while (_fildScript.CheckPosition(_ghostBlock))
        {
            //下に落とす
            _ghostBlock.MoveDown();
        }
        //ぶつかったら１つ上に
        _ghostBlock.MoveUp();
    }
    /// <summary>
    /// ゴーストブロックの色を変える
    /// </summary>
    private void ColorChange()
    {
        //親オブジェクト
        Transform parent = _ghostBlock.transform;
        //子オブジェクトのSpriteRenderer
        SpriteRenderer chidren = default;

        // 子オブジェクトを全て取得する
        foreach (Transform child in parent)
        {
            //SpriteRendererの色を変える
            chidren = child.GetComponent<SpriteRenderer>();
            chidren.color = _colorWhite;
            child.gameObject.tag = "Untagged";
        }
    }
}
