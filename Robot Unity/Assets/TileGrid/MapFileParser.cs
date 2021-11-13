using System.Collections.Generic;
using System;

public class MapFileParser
{
    public static readonly MapFileParser Instance = new MapFileParser();

    private MapFileParser()
    {
    }

    private (int, int) ParseDimension(string line)
    {
        string[] split = line.Trim().ToLower().Split('x');
        if (split.Length != 2)
        {
            throw new InvalidTileGridFileException($"The first line of a TileGrid file should contain the format \"{{rows}}x{{columns}}\" but was {line}.");
        }

        if (!Int32.TryParse(split[0], out int rows) || !Int32.TryParse(split[1], out int columns))
        {
            throw new InvalidTileGridFileException($"The first line of a TileGrid file should contain the format \"{{rows}}x{{columns}}\" but was {line}.");
        }

        return (rows, columns);
    }

    private char[,] ParseGrid(int rows, int columns, List<string> lines)
    {
        char[,] grid = new char[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            string line = lines[row];
            if (line.Length != columns)
            {
                throw new InvalidTileGridFileException($"The file specified provieds a column count of {columns} but row {row} contains {line.Length} characters.");
            }
            for (int col = 0; col < columns; col++)
            {
                grid[row, col] = line[col];
            }
        }

        return grid;
    }

    public virtual char[,] Parse(List<string> lines)
    {
        if (lines.Count == 0)
        {
            throw new InvalidTileGridFileException("Cannot parse an empty file as a TileGrid.");
        }

        (int rows, int columns) = this.ParseDimension(lines[0]);

        if (lines.Count != rows + 1)
        {
            throw new InvalidTileGridFileException($"The file specified provides a row count of {rows} but contains {lines.Count - 1} rows.");
        }

        char[,] grid = this.ParseGrid(rows, columns, lines.GetRange(1, lines.Count - 1));
        return grid;
    }

}