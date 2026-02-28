using UnityEngine;
using UnityEngine.UI;

public class MobileInputController : MonoBehaviour
{
    private TetrisPiece tetrisPiece;

    private void Start()
    {
        // 找到俄罗斯方块控制器
        tetrisPiece = FindObjectOfType<TetrisPiece>();

        // 绑定按钮事件
        BindButtonEvents();
    }

    private void BindButtonEvents()
    {
        // 查找并绑定方向按钮
        Button leftButton = GameObject.Find("LeftButton").GetComponent<Button>();
        Button rightButton = GameObject.Find("RightButton").GetComponent<Button>();
        Button downButton = GameObject.Find("DownButton").GetComponent<Button>();
        Button rotateButton = GameObject.Find("RotateButton").GetComponent<Button>();
        Button hardDropButton = GameObject.Find("HardDropButton").GetComponent<Button>();

        if (leftButton != null)
            leftButton.onClick.AddListener(() => MoveLeft());

        if (rightButton != null)
            rightButton.onClick.AddListener(() => MoveRight());

        if (downButton != null)
            downButton.onClick.AddListener(() => MoveDown());

        if (rotateButton != null)
            rotateButton.onClick.AddListener(() => Rotate());

        if (hardDropButton != null)
            hardDropButton.onClick.AddListener(() => HardDrop());
    }

    private void MoveLeft()
    {
        if (tetrisPiece != null)
        {
            tetrisPiece.MoveLeft();
        }
    }

    private void MoveRight()
    {
        if (tetrisPiece != null)
        {
            tetrisPiece.MoveRight();
        }
    }

    private void MoveDown()
    {
        if (tetrisPiece != null)
        {
            tetrisPiece.MoveDown();
        }
    }

    private void Rotate()
    {
        if (tetrisPiece != null)
        {
            tetrisPiece.Rotate();
        }
    }

    private void HardDrop()
    {
        if (tetrisPiece != null)
        {
            tetrisPiece.HardDrop();
        }
    }
}