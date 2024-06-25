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

    private CanvasGroupController controller; // panel megjelenitése és interaktivitása
    public string lastInput { get; private set; } = string.Empty; // utolsó bevitt szöveg
    public bool isWaitingOnUserInput { get; private set; } // várja a felh. be kell-e vigyen adatot

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

    public void Show(string title) // adott cimen input panel megjelenése
    {
        titleText.text = title;
        inputField.text = string.Empty;
        controller.Show();
        controller.SetInteractableState(active: true);
        isWaitingOnUserInput = true;
    }

    public void Hide() // elrejti az input panelt
    {
        controller.Hide();
        controller.SetInteractableState(active: false);
        isWaitingOnUserInput= false;
    }

    private void OnAcceptInput() // kezeli a szöveg bevitelének elfogadását
    {
        if (inputField.text == string.Empty)
            return;

        lastInput = inputField.text;
        Hide();
    }

    public void OnInputChanged(string value) // kezeli beviteli mező tartalmának változását
    {
        playButton.gameObject.SetActive(HasValidText());
    }
    private bool HasValidText() // ne legyen üres a beviteli mező
    {
        return inputField.text != string.Empty;
    }
}
