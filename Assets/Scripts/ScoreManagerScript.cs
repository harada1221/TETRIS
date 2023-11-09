using UnityEngine;
using UnityEngine.UI;

public class ScoreManagerScript : MonoBehaviour
{
    //スコアの数値
    private int _scorePoint = default;
    [SerializeField, Header("スコア加算の倍率")]
    private int _addScore = 1000;

    [SerializeField, Header("スコアのテキスト")]
    private Text _scoreText = default;
    [SerializeReference, Header("特殊消しを表示するテキスト")]
    private Text _deleteType = default;

    //Tスピンのテキスト
    private const string _tspinText = "T-Spin";
    //テトリス消しのテキスト
    private const string _tetrisText = "TETRIS";
    //スコアのテキスト
    private const string _score = "Score:";

    /// <summary>
    /// TSpin後の消えた列数に応じてテキストを変える
    /// </summary>
    /// <param name="DeleteCount">消えたライン数</param>
    public void TspinDisplay(int DeleteCount)
    {
        //消えた列に応じて特殊消しとスコア加算をする
        switch (DeleteCount)
        {
            case 1:
                _deleteType.text = _tspinText + " Single";
                _scorePoint += _addScore * DeleteCount;
                break;
            case 2:
                _deleteType.text = _tspinText + "Double ";
                _scorePoint += _addScore * DeleteCount;
                break;
            case 3:
                _deleteType.text = _tspinText + " Triple";
                _scorePoint += _addScore * DeleteCount;
                break;
        }
    }
    /// <summary>
    /// TETRIS消しをしたとき
    /// </summary>
    public void TetrisDisply()
    {
        //テキストを表示
        _deleteType.text = _tetrisText;
    }
    /// <summary>
    /// テキストを非表示にする
    /// </summary>
    public void TextClear()
    {
        //テキストを消す
        _deleteType.text = "";
    }
    public void AddScorePoint()
    {
        //スコア加算
        _scorePoint += _addScore;
        //スコア表示
        _scoreText.text = _score + _scorePoint.ToString();
    }
}
