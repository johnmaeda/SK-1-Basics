using System.Text.RegularExpressions;
public class ColumnDisplay
{
    private int bufferWidth;
    private int numColumns;

    public ColumnDisplay(int width, int columns)
    {
        bufferWidth = width;
        numColumns = columns;
    }

    public string FormatTextInColumns(Dictionary<int, string> columnTexts)
    {
        // Adjust the column width to leave space between columns
        int columnWidth = (bufferWidth - (numColumns - 1)) / numColumns;
        StringBuilder formattedText = new StringBuilder();
        List<string>[] columnLines = new List<string>[numColumns];

        // Initialize lists for each column
        for (int i = 0; i < numColumns; i++)
        {
            columnLines[i] = new List<string>();
        }

        // Split text for each column and store in respective list
        foreach (var entry in columnTexts)
        {
            int columnIndex = entry.Key;
            if (columnIndex < 0 || columnIndex >= numColumns)
            {
                throw new ArgumentException($"Invalid column index: {columnIndex}");
            }

            string[] words = entry.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder currentLine = new StringBuilder();

            foreach (var word in words)
            {
                if (currentLine.Length + word.Length + 2 > columnWidth)
                {
                    columnLines[columnIndex].Add(currentLine.ToString());
                    currentLine.Clear();
                }
                if (currentLine.Length > 0) currentLine.Append(" ");
                currentLine.Append(word);
            }

            if (currentLine.Length > 0)
            {
                columnLines[columnIndex].Add(currentLine.ToString());
            }
        }

        // Find the maximum number of lines in any column
        int maxLines = columnLines.Max(column => column.Count);

        // Construct the formatted text with space between columns
        for (int line = 0; line < maxLines; line++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                if (line < columnLines[col].Count)
                {
                    formattedText.Append(columnLines[col][line].PadRight(columnWidth));
                }
                else
                {
                    formattedText.Append(new string(' ', columnWidth));
                }

                // Add a space between columns, except after the last column
                if (col < numColumns - 1)
                {
                    formattedText.Append(" ");
                }
            }
            formattedText.AppendLine();
        }

        return formattedText.ToString();
    }
}

public class DisplayManager
{
    private ColumnDisplay columnDisplay;
    private Dictionary<string, string> columnTexts;
    private Dictionary<string, int> agentColumnMap;
    private string mainSpeaker;
    private int nextColumnIndex;
    const String speakerMark = "🟨💬>>";


    public DisplayManager(int width, int columns)
    {
        columnDisplay = new ColumnDisplay(width, columns);
        columnTexts = new Dictionary<string, string>();
        agentColumnMap = new Dictionary<string, int>();
        mainSpeaker = null;
        nextColumnIndex = 0;
    }

    public void SetColumnText(string agentName, string text)
    {
        // Replace runs of whitespace with a single space
        string sanitizedText = Regex.Replace(text, @"\s+", " ");

        if (!agentColumnMap.ContainsKey(agentName))
        {
            agentColumnMap[agentName] = nextColumnIndex++;
        }
        columnTexts[agentName] = sanitizedText;
    }

    public void SetMainSpeaker(string agentName)
    {
        // Clear '**' from previous main speaker, if any
        if (mainSpeaker != null && columnTexts.ContainsKey(mainSpeaker))
        {
            columnTexts[mainSpeaker] = columnTexts[mainSpeaker].Replace(speakerMark, "");
        }

        // Set new main speaker
        mainSpeaker = agentName;
    }

    public string GetFormattedDisplay()
    {
        Dictionary<int, string> combinedTexts = new Dictionary<int, string>();
        foreach (var agentName in columnTexts.Keys)
        {
            int column = agentColumnMap[agentName];
            string text = agentName + ": " + columnTexts[agentName];
            
            if (mainSpeaker != null && agentName == mainSpeaker)
            {
                text = speakerMark + text.ToUpper(); // Add newline for non-main speaker
            }
            
            combinedTexts[column] = text;
        }
        return columnDisplay.FormatTextInColumns(combinedTexts);
    }

    public void UpdateColumnText(string agentName, string newText)
    {
        SetColumnText(agentName, newText);
    }
}