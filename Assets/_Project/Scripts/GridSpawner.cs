using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] private LevelDefinition level;
    [SerializeField] private Vector2 cellSize;
    [SerializeField] private Vector2 cellGap;
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Transform root;

    private void Start()
    {
        if (!LevelValidator.TryValidate(level, out string[] errors))
        {
            Debug.LogError($"Cannot spawn level '{level?.name ?? "Missing"}':\n{string.Join("\n", errors)}", this);
            return;
        }

        if (cellPrefab == null)
        {
            Debug.LogError("GridSpawner needs a Cell prefab.", this);
            return;
        }

        Transform spawnRoot = root != null ? root : transform;

        for (int y = 0; y < level.size; y++)
        for (int x = 0; x < level.size; x++)
        {
            if (IsVoid(x, y))
                continue;

            float xPos = (x - (level.size - 1) * 0.5f) * (cellSize.x + cellGap.x) + transform.position.x;
            int unityRow = level.size - 1 - y;
            float yPos = (unityRow - (level.size - 1) * 0.5f) * (cellSize.y + cellGap.y) + transform.position.y;

            Cell cell = Instantiate(cellPrefab, new Vector2(xPos, yPos), Quaternion.identity, spawnRoot);
            cell.name = $"Cell_{x}_{y}";
        }
    }

    private bool IsVoid(int x, int y)
    {
        foreach (CellOverrideData cell in level.cellOverrides)
        {
            if (cell.x == x && cell.y == y)
                return cell.type == CellType.Void;
        }

        return false;
    }
}
