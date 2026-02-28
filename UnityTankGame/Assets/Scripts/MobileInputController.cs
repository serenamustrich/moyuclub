using UnityEngine;
using UnityEngine.UI;

public class MobileInputController : MonoBehaviour
{
    public static MobileInputController instance;
    public Button leftButton;
    public Button rightButton;
    public Button forwardButton;
    public Button fireButton;

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

    private void Start()
    {
        if (leftButton != null)
        {
            leftButton.onDown.AddListener(() => rotateInput = -1f);
            leftButton.onUp.AddListener(() => rotateInput = 0f);
        }

        if (rightButton != null)
        {
            rightButton.onDown.AddListener(() => rotateInput = 1f);
            rightButton.onUp.AddListener(() => rotateInput = 0f);
        }

        if (forwardButton != null)
        {
            forwardButton.onDown.AddListener(() => moveInput = 1f);
            forwardButton.onUp.AddListener(() => moveInput = 0f);
        }

        if (fireButton != null)
        {
            fireButton.onClick.AddListener(() => fireInput = true);
        }
    }

    private void Update()
    {
        // 重置射击输入
        fireInput = false;
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
