using System;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TextAsset storyFile;
    [SerializeField] private TextAsset chapterFile;
    [SerializeField] private ChoicePanel choicePanel;

    private string[] lines;
    private int currentIndex;
    private NarrativeSession session;
    private GameState gameState;
    private bool chapterStarted;
    private bool ready;
    private int lastInteractionFrame = -1;

    private void Start()
    {
        if (choicePanel != null)
            choicePanel.Hide();
        if (dialogueText == null || dialogueText.font == null || storyFile == null ||
            chapterFile == null || choicePanel == null || !choicePanel.IsConfigured)
        {
            Debug.LogError("Dialogue setup requires text with a font, intro and chapter files, and a configured ChoicePanel.", this);
            return;
        }

        try
        {
            DialogueData intro = JsonUtility.FromJson<DialogueData>(storyFile.text);
            if (intro == null || intro.lines == null || intro.lines.Length == 0)
                throw new ArgumentException($"Intro '{storyFile.name}' has no dialogue lines.");
            lines = intro.lines;
            // Validate the chapter up front so broken links cannot interrupt a playthrough.
            session = new NarrativeSession(JsonUtility.FromJson<ChapterData>(chapterFile.text));
        }
        catch (ArgumentException error)
        {
            Debug.LogError($"Cannot load '{storyFile.name}' / '{chapterFile.name}': {error.Message}", this);
            return;
        }

        gameState = new GameState();
        currentIndex = 0;
        chapterStarted = false;
        ready = true;
        dialogueText.text = lines[currentIndex];
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) ||
            Input.GetKeyDown(KeyCode.RightArrow))
            NextLine();
    }

    public void NextLine()
    {
        if (!ready || Time.frameCount == lastInteractionFrame ||
            (chapterStarted && session.HasChoices))
            return;
        lastInteractionFrame = Time.frameCount;

        if (!chapterStarted)
        {
            if (currentIndex < lines.Length - 1)
            {
                dialogueText.text = lines[++currentIndex];
                return;
            }
            chapterStarted = true;
            ShowCurrentNode();
        }
        else if (session.Advance())
        {
            ShowCurrentNode();
        }
    }

    private void ShowCurrentNode()
    {
        choicePanel.Hide();
        DialogueNodeData node = session.CurrentNode;
        dialogueText.text = string.IsNullOrEmpty(node.speaker)
            ? node.text : $"{node.speaker}\n{node.text}";
        if (session.HasChoices)
            choicePanel.Show(node.choices, dialogueText.font, SelectChoice);
    }

    private void SelectChoice(int index)
    {
        if (!ready || !chapterStarted || !session.TryChoose(index, out ChoiceData choice))
            return;

        // UI callbacks and Update can run in either order; block this frame's advance.
        lastInteractionFrame = Time.frameCount;
        // In this trial's schema, every affectionChange explicitly belongs to Hotaru.
        gameState.ChangeHotaruAffection(choice.affectionChange);
        Debug.Log($"Hotaru affection: {gameState.HotaruAffection}", this);
        ShowCurrentNode();
    }
}
