using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private Button buttonTemplate;

    private readonly List<Button> buttons = new List<Button>();
    private bool acceptingChoice;

    public bool IsConfigured => panel != null && buttonTemplate != null &&
        buttonTemplate.transform.IsChildOf(panel) &&
        buttonTemplate.GetComponentInChildren<TMP_Text>(true) != null;

    public void Hide()
    {
        acceptingChoice = false;
        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
            Destroy(button.gameObject);
        }
        buttons.Clear();
        if (buttonTemplate != null)
            buttonTemplate.gameObject.SetActive(false);
        if (panel != null)
            panel.gameObject.SetActive(false);
    }

    public void Show(ChoiceData[] choices, TMP_FontAsset font, Action<int> onSelected)
    {
        Hide();
        panel.gameObject.SetActive(true);
        panel.SetAsLastSibling();
        acceptingChoice = true;

        for (int i = 0; i < choices.Length; i++)
        {
            int choiceIndex = i;
            Button button = Instantiate(buttonTemplate, panel);
            buttons.Add(button);
            button.name = $"ChoiceButton_{i + 1}";
            // Disable keyboard submission/navigation: only clicks select trial choices.
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(() =>
            {
                if (!acceptingChoice)
                    return;
                acceptingChoice = false;
                onSelected(choiceIndex);
            });

            RectTransform rect = (RectTransform)button.transform;
            rect.anchorMin = new Vector2(0.15f, 0.5f);
            rect.anchorMax = new Vector2(0.85f, 0.5f);
            rect.sizeDelta = new Vector2(0f, 64f);
            rect.anchoredPosition = new Vector2(0f, -130f - i * 76f);

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            label.font = font;
            label.fontSharedMaterial = font.material;
            label.text = choices[i].text;
            label.enableAutoSizing = true;
            label.fontSizeMin = 16f;
            label.fontSizeMax = 28f;
            button.gameObject.SetActive(true);
        }
    }
}
