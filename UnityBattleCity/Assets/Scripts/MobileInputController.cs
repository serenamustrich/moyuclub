using UnityEngine;

public class MobileInputController : MonoBehaviour
{
    public static MobileInputController instance;
    private float moveInput = 0f;
    private float rotateInput = 0f;
    private bool fireInput = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 重置输入
        moveInput = 0f;
        rotateInput = 0f;
        fireInput = false;

        // 处理触摸输入
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                // 左半屏幕控制移动和旋转
                if (touch.position.x < Screen.width / 2)
                {
                    // 计算移动和旋转输入
                    Vector2 touchDelta = touch.deltaPosition;
                    moveInput = Mathf.Clamp(touchDelta.y, -1f, 1f);
                    rotateInput = Mathf.Clamp(touchDelta.x, -1f, 1f);
                }
                // 右半屏幕控制射击
                else
                {
                    fireInput = true;
                }
            }
        }
    }

    public float GetMoveInput()
    {
        return moveInput;
    }

    public float GetRotateInput()
    {
        return rotateInput;
    }

    public bool GetFireInput()
    {
        return fireInput;
    }
}
