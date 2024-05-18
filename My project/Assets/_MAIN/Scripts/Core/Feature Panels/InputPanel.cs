using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_InputField inputField;

    public static InputPanel instance { get; private set; } = null;

    private CanvasGroupController controller;
    public string lastInput { get; private set; }
    public bool isWaitingOnUserInput { get; private set; }

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        controller = new CanvasGroupController(this, canvas);

        canvas.alpha = 0;
        SetCanvasState(active: false);
        playButton.gameObject.SetActive(false);
        inputField.onValueChanged.AddListener(OnInputChanged);
        playButton.onClick.AddListener(OnAcceptInput);
    }

    public void Show(string title)
    {
        titleText.text = title;
        inputField.text = string.Empty;
        controller.Show();
        SetCanvasState(active: true);
        isWaitingOnUserInput = true;
    }

    public void Hide()
    {
        controller.Hide();
        SetCanvasState(active: false);
        isWaitingOnUserInput= false;
    }

    private void OnAcceptInput()
    {
        if (inputField.text == string.Empty)
            return;

        lastInput = inputField.text;
        Hide();
    }

    private void SetCanvasState(bool active)
    {
        canvas.interactable = active;
        canvas.blocksRaycasts = active;
    }

    public void OnInputChanged(string value)
    {
        playButton.gameObject.SetActive(HasValidText());
    }
    private bool HasValidText()
    {
        return inputField.text != string.Empty;
    }
}
