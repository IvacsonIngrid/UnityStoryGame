using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputPanel : MonoBehaviour
{
    public static InputPanel instance { get; private set; } = null;
    
    [SerializeField] private CanvasGroup canvas;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_InputField inputField;

    private CanvasGroupController controller;
    public string lastInput { get; private set; } = string.Empty;
    public bool isWaitingOnUserInput { get; private set; }

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        controller = new CanvasGroupController(this, canvas);

        controller.alpha = 0;
        controller.SetInteractableState(active: false);
        playButton.gameObject.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputChanged);
        playButton.onClick.AddListener(OnAcceptInput);
    }

    public void Show(string title)
    {
        titleText.text = title;
        inputField.text = string.Empty;
        controller.Show();
        controller.SetInteractableState(active: true);
        isWaitingOnUserInput = true;
    }

    public void Hide()
    {
        controller.Hide();
        controller.SetInteractableState(active: false);
        isWaitingOnUserInput= false;
    }

    private void OnAcceptInput()
    {
        if (inputField.text == string.Empty)
            return;

        lastInput = inputField.text;
        Hide();
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
