using System.Collections.Generic;

public static class LevelValidator
{
    public static bool TryValidate(LevelDefinition level, out string[] errors)
    {
        var issues = new List<string>();

        if (level == null)
        {
            errors = new[] { "Level asset is missing." };
            return false;
        }

        if (string.IsNullOrWhiteSpace(level.id))
            issues.Add("Level id is empty.");

        if (level.size <= 0)
            issues.Add("Level size must be greater than zero.");

        var voidCells = new HashSet<(int x, int y)>();
        foreach (CellOverrideData cell in level.cellOverrides ?? new CellOverrideData[0])
        {
            if (!IsInside(level, cell.x, cell.y))
                issues.Add($"Cell override ({cell.x}, {cell.y}) is outside the grid.");
            else if (cell.type == CellType.Void && !voidCells.Add((cell.x, cell.y)))
                issues.Add($"Void cell ({cell.x}, {cell.y}) is duplicated.");
        }

        var cluesById = new Dictionary<int, ClueData>();
        foreach (ClueData clue in level.clues ?? new ClueData[0])
        {
            if (!cluesById.TryAdd(clue.id, clue))
                issues.Add($"Clue id {clue.id} is duplicated.");
            if (!IsInside(level, clue.x, clue.y))
                issues.Add($"Clue {clue.id} is outside the grid.");
            else if (voidCells.Contains((clue.x, clue.y)))
                issues.Add($"Clue {clue.id} is placed on a Void cell.");
            if (clue.value <= 0)
                issues.Add($"Clue {clue.id} must have a positive value.");
        }

        var coveredCells = new HashSet<(int x, int y)>();
        var solutionClueIds = new HashSet<int>();
        foreach (SolutionRectData rect in level.solution ?? new SolutionRectData[0])
        {
            if (!cluesById.TryGetValue(rect.clueId, out ClueData clue))
            {
                issues.Add($"Solution references missing clue id {rect.clueId}.");
                continue;
            }

            if (!solutionClueIds.Add(rect.clueId))
                issues.Add($"Clue {rect.clueId} has more than one solution rectangle.");
            if (rect.width <= 0 || rect.height <= 0)
                issues.Add($"Solution for clue {rect.clueId} has an invalid size.");
            if (rect.width * rect.height != clue.value)
                issues.Add($"Solution area for clue {rect.clueId} does not equal value {clue.value}.");
            if (clue.mechanic.constraint == ClueConstraint.Horizontal && rect.height != 1)
                issues.Add($"Solution for clue {rect.clueId} must be horizontal.");
            if (clue.mechanic.constraint == ClueConstraint.Vertical && rect.width != 1)
                issues.Add($"Solution for clue {rect.clueId} must be vertical.");
            if (clue.x < rect.x || clue.x >= rect.x + rect.width ||
                clue.y < rect.y || clue.y >= rect.y + rect.height)
                issues.Add($"Solution for clue {rect.clueId} does not contain its clue cell.");

            for (int y = rect.y; y < rect.y + rect.height; y++)
            {
                for (int x = rect.x; x < rect.x + rect.width; x++)
                {
                    if (!IsInside(level, x, y))
                        issues.Add($"Solution for clue {rect.clueId} extends outside the grid at ({x}, {y}).");
                    else if (voidCells.Contains((x, y)))
                        issues.Add($"Solution for clue {rect.clueId} covers Void cell ({x}, {y}).");
                    else if (!coveredCells.Add((x, y)))
                        issues.Add($"Playable cell ({x}, {y}) is covered more than once.");
                }
            }
        }

        foreach (int clueId in cluesById.Keys)
        {
            if (!solutionClueIds.Contains(clueId))
                issues.Add($"Clue {clueId} has no solution rectangle.");
        }

        for (int y = 0; y < level.size; y++)
        {
            for (int x = 0; x < level.size; x++)
            {
                if (!voidCells.Contains((x, y)) && !coveredCells.Contains((x, y)))
                    issues.Add($"Playable cell ({x}, {y}) is not covered by the solution.");
            }
        }

        errors = issues.ToArray();
        return errors.Length == 0;
    }

    private static bool IsInside(LevelDefinition level, int x, int y)
    {
        return x >= 0 && x < level.size && y >= 0 && y < level.size;
    }
}
