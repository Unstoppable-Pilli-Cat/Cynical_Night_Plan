using System;

[Serializable]
public class DialogueData
{
    public string[] lines;
}

[Serializable]
public class ChapterData
{
    public string startNode;
    public DialogueNodeData[] nodes;
}

[Serializable]
public class DialogueNodeData
{
    public string id;
    public string speaker;
    public string text;
    public string next;
    public ChoiceData[] choices;
}

[Serializable]
public class ChoiceData
{
    public string text;
    public string target;
    public int affectionChange;
}
