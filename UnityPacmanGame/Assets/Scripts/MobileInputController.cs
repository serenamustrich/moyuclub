using UnityEngine;
using UnityEngine.UI;

public class MobileInputController : MonoBehaviour
{
    private PacmanController pacmanController;

    private void Start()
    {
        // 找到吃豆豆控制器
        pacmanController = FindObjectOfType<PacmanController>();

        // 绑定按钮事件
        BindButtonEvents();
    }

    private void BindButtonEvents()
    {
        // 查找并绑定方向按钮
        Button upButton = GameObject.Find("UpButton").GetComponent<Button>();
        Button downButton = GameObject.Find("DownButton").GetComponent<Button>();
        Button leftButton = GameObject.Find("LeftButton").GetComponent<Button>();
        Button rightButton = GameObject.Find("RightButton").GetComponent<Button>();

        if (upButton != null)
            upButton.onClick.AddListener(() => OnDirectionButtonClick(Vector2.up));

        if (downButton != null)
            downButton.onClick.AddListener(() => OnDirectionButtonClick(Vector2.down));

        if (leftButton != null)
            leftButton.onClick.AddListener(() => OnDirectionButtonClick(Vector2.left));

        if (rightButton != null)
            rightButton.onClick.AddListener(() => OnDirectionButtonClick(Vector2.right));
    }

    private void OnDirectionButtonClick(Vector2 direction)
    {
        if (pacmanController != null)
        {
            // 直接调用PacmanController的HandleKeyDown方法
            pacmanController.HandleKeyDown(direction);
            // 立即调用HandleKeyUp，实现按一下动一下
            pacmanController.HandleKeyUp();
        }
    }

    // 长按开始事件
    public void OnDirectionButtonDown(Vector2 direction)
    {
        if (pacmanController != null)
        {
            pacmanController.HandleKeyDown(direction);
        }
    }

    // 长按结束事件
    public void OnDirectionButtonUp()
    {
        if (pacmanController != null)
        {
            pacmanController.HandleKeyUp();
        }
    }
}
