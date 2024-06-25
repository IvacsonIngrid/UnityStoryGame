using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour
{
    public static ChoicePanel instance {  get; private set; }

    // gomb méreteinek beállitása
    private const float BUTTON_MIN_WIDTH = 50;
    private const float BUTTON_MAX_SIZE = 1000;
    private const float BUTTON_WIDTH_PADDING = 25;

    private const float BUTTON_HEIGHT_PER_LINE = 50f;
    private const float BUTTON_HEIGHT_PADDING = 20f;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI titleText; // kérdés szövege
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private VerticalLayoutGroup buttonLayoutGroup; // gombok vertikális elrendezése

    private CanvasGroupController controller = null; // panel megjelenitéséhez, interaktivitáshoz
    private List<ChoiceButton> buttons = new List<ChoiceButton>(); // választási gombok

    public ChoicePanelDecision lastDecision { get; private set; } = null; // utolsó választás részletei
    public bool isWaitingOnUserChoice { get; private set; } = false; // rendszer vár-e felhasználó válaszára

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        controller = new CanvasGroupController(this, canvasGroup);
        controller.alpha = 0;
        controller.SetInteractableState(false);
    }

    public void Show(string question, string[] choices) // választási panel: kérdések - válaszok megjelenitése
    {
        lastDecision = new ChoicePanelDecision(question, choices);
        isWaitingOnUserChoice = true;

        controller.Show();
        controller.SetInteractableState(active: true);
        titleText.text = question;
        StartCoroutine(GeneratorChoices(choices));
    }

    private IEnumerator GeneratorChoices(string[] choices) // gombok - választási lehetőségekkel való megadása
    {
        float maxWidth = 0;

        for (int i = 0; i < choices.Length; i++)
        {
            ChoiceButton choiceButton;

            if (i < buttons.Count)
            {
                choiceButton = buttons[i];
            }
            else
            {
                GameObject newButtonObject = Instantiate(choiceButtonPrefab, buttonLayoutGroup.transform);
                newButtonObject.SetActive(true);

                Button newButton = newButtonObject.GetComponent<Button>();
                TextMeshProUGUI newTitle = newButton.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayout = newButton.GetComponent<LayoutElement>();

                choiceButton = new ChoiceButton { button = newButton, layout = newLayout, title = newTitle };
                buttons.Add(choiceButton);
            }

            choiceButton.button.onClick.RemoveAllListeners();

            int buttonIndex = i;
            choiceButton.button.onClick.AddListener(() => AcceptAnswer(buttonIndex));
            choiceButton.title.text = choices[i];

            float buttonWidth = Mathf.Clamp(BUTTON_WIDTH_PADDING + choiceButton.title.preferredWidth, BUTTON_MIN_WIDTH, BUTTON_MAX_SIZE);
            maxWidth = Mathf.Max(maxWidth, buttonWidth);
        }

        foreach (var button in buttons)
        {
            button.layout.preferredWidth = maxWidth;
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            bool show = i < choices.Length;
            buttons[i].button.gameObject.SetActive(show);
        }

        yield return new WaitForEndOfFrame();

        foreach (var button in buttons)
        {
            int lines = button.title.textInfo.lineCount;
            Debug.Log(lines);
            button.layout.preferredHeight = BUTTON_HEIGHT_PADDING + (BUTTON_HEIGHT_PER_LINE * lines);
        }
    }

    public void Hide() // választási panel elrejtése
    {
        controller.Hide();
        controller.SetInteractableState(false);
    }

    private void AcceptAnswer(int index) // felhasználó válaszának kezelése
    {
        if (index < 0 || index > lastDecision.choices.Length - 1)
            return;

        lastDecision.answerIndex = index;
        isWaitingOnUserChoice = false;
        Hide();
    }

    public class ChoicePanelDecision // választás részletei
    {
        public string question = string.Empty;
        public int answerIndex = -1;
        public string[] choices = new string[0];

        public ChoicePanelDecision(string question, string[] choices)
        {
            this.question = question;
            this.choices = choices;
            answerIndex = -1;
        }
    }

    private struct ChoiceButton // választási gomb struktúrája
    {
        public Button button;
        public TextMeshProUGUI title;
        public LayoutElement layout;
    }
}
