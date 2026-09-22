using System;
using System.Collections.Generic;

// Plain C# progression: no scene objects or UI are needed to navigate a chapter.
public class NarrativeSession
{
    private readonly Dictionary<string, DialogueNodeData> nodes =
        new Dictionary<string, DialogueNodeData>(StringComparer.Ordinal);

    public DialogueNodeData CurrentNode { get; private set; }
    public bool HasChoices => CurrentNode.choices != null && CurrentNode.choices.Length > 0;

    public NarrativeSession(ChapterData chapter)
    {
        if (chapter == null || chapter.nodes == null || chapter.nodes.Length == 0)
            throw new ArgumentException("Chapter has no nodes.");

        foreach (DialogueNodeData node in chapter.nodes)
        {
            if (node == null || string.IsNullOrWhiteSpace(node.id))
                throw new ArgumentException("Chapter contains a node with no ID.");
            if (nodes.ContainsKey(node.id))
                throw new ArgumentException($"Duplicate node ID '{node.id}'.");
            nodes.Add(node.id, node);
        }

        RequireTarget(chapter.startNode, "startNode");
        foreach (DialogueNodeData node in chapter.nodes)
        {
            bool hasChoices = node.choices != null && node.choices.Length > 0;
            if (hasChoices && !string.IsNullOrEmpty(node.next))
                throw new ArgumentException($"Node '{node.id}' has both next and choices.");
            if (!string.IsNullOrEmpty(node.next))
                RequireTarget(node.next, $"Node '{node.id}'");
            if (!hasChoices)
                continue;

            foreach (ChoiceData choice in node.choices)
            {
                if (choice == null || string.IsNullOrWhiteSpace(choice.text))
                    throw new ArgumentException($"Node '{node.id}' has an empty choice.");
                RequireTarget(choice.target, $"Choice in node '{node.id}'");
            }
        }
        CurrentNode = nodes[chapter.startNode];
    }

    private void RequireTarget(string target, string source)
    {
        if (string.IsNullOrWhiteSpace(target) || !nodes.ContainsKey(target))
            throw new ArgumentException($"{source} references missing node '{target}'.");
    }

    public bool Advance()
    {
        // No outgoing link means the chapter has ended; keep the final node visible.
        if (HasChoices || string.IsNullOrEmpty(CurrentNode.next))
            return false;
        CurrentNode = nodes[CurrentNode.next];
        return true;
    }

    public bool TryChoose(int index, out ChoiceData selectedChoice)
    {
        selectedChoice = null;
        if (!HasChoices || index < 0 || index >= CurrentNode.choices.Length)
            return false;
        selectedChoice = CurrentNode.choices[index];
        CurrentNode = nodes[selectedChoice.target];
        return true;
    }
}
