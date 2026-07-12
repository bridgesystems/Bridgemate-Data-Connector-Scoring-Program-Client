
using System.IO;
using System.Text;

namespace BridgeSystems.Bridgemate.DataConnectorClientEmulator.Support;

public class ExternalPlayerDataMultiCsvFormatsCommunicator : ExternalPlayerDataTextFileCommunicator
{
    public const string FirstNameField = "FirstName";
    public const string LastNameField = "LastName";
    public const string PlayersIDField = "PlayersID";

    private int _firstNameIndex = 0;
    private int _lastNameIndex = 1;
    private int _playerNumberIdIndex = 2;

    public ExternalPlayerDataMultiCsvFormatsCommunicator(Func<string> getFilePath) :
        base(getFilePath)
    {
    }

    /// <summary>
    /// Reads players from a CSV file.
    /// </summary>
    /// <param name="filePath">Path to CSV file</param>
    /// <param name="headerIsFirstRow">Whether first row is header and should be skipped</param>
    /// <param name="errors">List of parsing errors</param>
    /// <returns>List of PlayerModel objects</returns>
    protected override (List<string> errors, List<ImportedPlayer> players) ReadData()
    {
        try
        {
            var errors = new List<string>();
            var importedPlayers= new List<ImportedPlayer>();

            var filePath = GetFilePath();
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                errors.Add($"File not found: {filePath}");
                return (errors, new List<ImportedPlayer>());
            }

            using var reader = new StreamReader(filePath, Encoding.UTF8);

            string firstLine = null;
            var lineNum = 0;

            // Find first non-empty line for delimiter detection
            while (!reader.EndOfStream)
            {
                firstLine = reader.ReadLine();
                lineNum++;
                if (!string.IsNullOrWhiteSpace(firstLine)) break;
            }

            if (string.IsNullOrWhiteSpace(firstLine))
            {
                errors.Add("File is empty.");
                return (errors, new List<ImportedPlayer>());
            }

            var delimiter = DetectDelimiter(firstLine!);
            if (delimiter == null)
            {
                errors.Add("Not a valid CSV: could not detect delimiter.");
                return (errors, new List<ImportedPlayer>());
            }

            var headerIsFirstRow = DetermineHeaderIsFirstRow(firstLine, delimiter.Value);

            if (!headerIsFirstRow)
            {
                _firstNameIndex = 0;
                _lastNameIndex = 1;
                _playerNumberIdIndex = 2;
            }

            List<string> fields;
            // If first row is not header, process it
            if (!headerIsFirstRow)
            {
                var result = TryParseCsvLine(firstLine!, delimiter.Value);
                if (result.success)
                {
                    fields = result.fields;
                    if (fields.Count == 3)
                        importedPlayers.Add(MakePlayerModel(fields));
                    else
                        errors.Add($"Line {lineNum}: expected 3 columns, found {fields.Count}.");
                }
                else
                {
                    errors.Add($"Line {lineNum}: {result.error}");
                }
            }

            // Remaining lines
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                lineNum++;

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var result = TryParseCsvLine(line!, delimiter.Value);
                if (result.success)
                {
                    fields = result.fields;
                    if (fields.Count == 3)
                        importedPlayers.Add(MakePlayerModel(fields));
                    else
                        errors.Add($"Line {lineNum}: expected 3 columns, found {fields.Count}.");
                }
                else
                {
                    errors.Add($"Line {lineNum}: {result.error}");
                }
            }

            return (errors, importedPlayers);
        }
        catch (Exception ex)
        {
            return (new List<string>([ex.Message]), new List<ImportedPlayer>());
        }
    }

    private ImportedPlayer MakePlayerModel(IReadOnlyList<string> fields) =>
        new()
        {
            FirstName = fields[_firstNameIndex],
            LastName = fields[_lastNameIndex],
            PlayerNumberId = fields[_playerNumberIdIndex]
        };

    private char? DetectDelimiter(string sample)
    {
        int comma = 0, semi = 0;
        var inQuotes = false;

        for (var i = 0; i < sample.Length; i++)
        {
            var c = sample[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < sample.Length && sample[i + 1] == '"')
                {
                    i++; // escaped quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (!inQuotes)
            {
                if (c == ',') comma++;
                else if (c == ';') semi++;
            }
        }

        if (comma == 0 && semi == 0) return null;
        return comma >= semi ? ',' : ';';
    }

    private bool DetermineHeaderIsFirstRow(string sample, char delimiter)
    {
        try
        {
            var fields = TryParseCsvLine(sample, delimiter);
            if (!fields.success) return true;
            if (fields.fields.Count != 3) return true;
            return !int.TryParse(fields.fields[_playerNumberIdIndex], out _);
        }
        catch (Exception ex)
        {
            return true;
        }
    }

    private (int firstNameIndex, int lastNameIndex, int playerNumberIdIndex) DetermineFieldIndices(string sample, char delimiter)
    {
        try
        {
            var firstLineFields = sample.Split(delimiter).ToList();
            var firstNameIndex = firstLineFields.IndexOf(FirstNameField);
            var lastNamedIndex = firstLineFields.IndexOf(LastNameField);
            var playerNumberIdIndex = firstLineFields.IndexOf(PlayersIDField);
            return (firstNameIndex, lastNamedIndex, playerNumberIdIndex);
        }
        catch
        {
            return (-1, -1, -1);
        }
    }

    private (bool success, List<string> fields, string error) TryParseCsvLine(string line, char delimiter)
    {
        try
        {
            var fields = new List<string>();
            var error = string.Empty;

            var sb = new StringBuilder();
            var inQuotes = false;

            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"'); // escaped quote
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == delimiter)
                    {
                        fields.Add(sb.ToString().Trim());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            if (inQuotes)
            {
                error = "Unmatched quote.";
                return (false, fields, error);
            }

            fields.Add(sb.ToString().Trim());

            return (true, fields, error);
        }
        catch (Exception ex)
        {
            return (false, new List<string>(), ex.Message);
        }
    }
}