
using UnityEngine;
using TMPro;

public class DialogueController : MonoBehaviour
{
    public TMP_Text dialogueText;
    public TextAsset storyFile;

    private string[] lines;
    private int currentIndex = 0;

    void Start()
    {
        if (storyFile == null)
        {
            Debug.LogError("Story json file not found.");
            return;
        }

        DialogueData data = JsonUtility.FromJson<DialogueData>(storyFile.text);

        if (data == null || data.lines == null || data.lines.Length == 0)
        {
            Debug.LogError("Story file has no valid dialogue lines.");
            return;
        }

        lines = data.lines;
        currentIndex = 0;
        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (dialogueText != null && lines != null)
        {
            dialogueText.text = lines[currentIndex];
        }
    }

    public void NextLine()
    {
        if (lines != null && currentIndex < lines.Length - 1)
        {
            currentIndex++;
            ShowCurrentLine();
        }
    }
}