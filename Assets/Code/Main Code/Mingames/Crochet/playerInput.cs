using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class playerInput : MonoBehaviour
{
    [Header("References")]
    public RectTransform playerRT;
    public Crochet crochet;

    [Header("Movement")]
    public float speed = 500f;
    public float minX = -400f;
    public float maxX = 400f;

    [Header("Input Animation")]
    public float pressDistance = 15f;
    public float pressDuration = 0.1f;

    private PlayerControls input;
    private float moveInput;

    private InputSlot currentSlot = InputSlot.None;

    private enum InputSlot
    {
        None = -1,
        Blue = 0,
        White = 1,
        Red = 2,
        Green = 3
    }

    private void Awake()
    {
        input = new PlayerControls();
    }

    private void OnEnable()
    {
        input.Enable();
        input.Crochet.Move.performed += OnMove;
        input.Crochet.Move.canceled += OnMove;
        input.Crochet.Space.performed += OnPress;
    }

    private void OnDisable()
    {
        input.Crochet.Move.performed -= OnMove;
        input.Crochet.Move.canceled -= OnMove;
        input.Crochet.Space.performed -= OnPress;
        input.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    private void Update()
    {
        if (!crochet.playerTurn) return;

        Vector2 pos = playerRT.anchoredPosition;
        pos.x += moveInput * speed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        playerRT.anchoredPosition = pos;
    }

    private void OnPress(InputAction.CallbackContext context)
    {
        if (!crochet.playerTurn) return;

        if (currentSlot == InputSlot.None) { Debug.Log(currentSlot == InputSlot.None); return; }

        int slotIndex = (int)currentSlot;

        if (slotIndex < 0 || slotIndex >= crochet.playerInputs.Count)
            return;

        int expected = crochet.index[crochet.currentIndex];
        int inputId = crochet.playerInputs[slotIndex].id;

        if (inputId == expected)
        {
            crochet.repeatedIndex.Add(inputId);
            StartCoroutine(AnimateInputPress(crochet.playerInputs[slotIndex].input));
            crochet.currentIndex++;
        }
        else
        {
            crochet.restartRound();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (TryGetSlotFromTag(col.tag, out InputSlot slot))
        {
            currentSlot = slot;
        }
    }


    void OnTriggerExit2D(Collider2D col)
    {
        if (TryGetSlotFromTag(col.tag, out InputSlot slot))
        {
            if (currentSlot == slot)
            {
                currentSlot = InputSlot.None;
            }
        }
    }

    private bool TryGetSlotFromTag(string tag, out InputSlot slot)
    {
        switch (tag)
        {
            case "Blue": slot = InputSlot.Blue; return true;
            case "Red": slot = InputSlot.Red; return true;
            case "White": slot = InputSlot.White; return true;
            case "Green": slot = InputSlot.Green; return true;
            default:
                slot = InputSlot.None;
                return false;
        }
    }


    IEnumerator AnimateInputPress(GameObject inputObj)
    {
        if (inputObj == null) yield break;

        RectTransform rt = inputObj.GetComponent<RectTransform>();
        if (rt == null) yield break;

        Vector2 originalPos = rt.anchoredPosition;
        Vector2 pressedPos = originalPos - new Vector2(0f, pressDistance);

        float t = 0f;

        // Press down
        while (t < pressDuration)
        {
            rt.anchoredPosition = Vector2.Lerp(originalPos, pressedPos, t / pressDuration);
            t += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = pressedPos;

        // Brief hold
        yield return new WaitForSeconds(pressDuration * 0.5f);

        t = 0f;

        // Return
        while (t < pressDuration)
        {
            rt.anchoredPosition = Vector2.Lerp(pressedPos, originalPos, t / pressDuration);
            t += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = originalPos;
    }
}
