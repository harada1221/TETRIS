using UnityEngine;
using UnityEngine.UI;

public class ScoreManagerScript : MonoBehaviour
{
    //�X�R�A�̐��l
    private int _scorePoint = default;
    [SerializeField, Header("�X�R�A���Z�̔{��")]
    private int _addScore = 1000;

    [SerializeField, Header("�X�R�A�̃e�L�X�g")]
    private Text _scoreText = default;
    [SerializeReference, Header("���������\������e�L�X�g")]
    private Text _deleteType = default;

    //T�X�s���̃e�L�X�g
    private const string _tspinText = "T-Spin";
    //�e�g���X�����̃e�L�X�g
    private const string _tetrisText = "TETRIS";
    //�X�R�A�̃e�L�X�g
    private const string _score = "Score:";

    /// <summary>
    /// TSpin��̏������񐔂ɉ����ăe�L�X�g��ς���
    /// </summary>
    /// <param name="DeleteCount">���������C����</param>
    public void TspinDisplay(int DeleteCount)
    {
        //��������ɉ����ē�������ƃX�R�A���Z������
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
    /// TETRIS�����������Ƃ�
    /// </summary>
    public void TetrisDisply()
    {
        //�e�L�X�g��\��
        _deleteType.text = _tetrisText;
    }
    /// <summary>
    /// �e�L�X�g���\���ɂ���
    /// </summary>
    public void TextClear()
    {
        //�e�L�X�g������
        _deleteType.text = "";
    }
    public void AddScorePoint()
    {
        //�X�R�A���Z
        _scorePoint += _addScore;
        //�X�R�A�\��
        _scoreText.text = _score + _scorePoint.ToString();
    }
}
