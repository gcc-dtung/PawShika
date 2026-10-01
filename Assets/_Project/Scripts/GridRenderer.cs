using System;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 cellSize;
    [SerializeField] private Vector2 cellGap;
    [SerializeField] private Transform gridTransform;
    
    [SerializeField] private bool drawGrid;

    private void OnDrawGizmos()
    {
        if (!drawGrid) return;
        if (gridTransform == null)
        {
            gridTransform = transform;
        }
        
        Gizmos.color = Color.green;
        
        for (float i = 0; i < gridSize.y; i++)
        {
            for (float j = 0; j < gridSize.x; j++)
            {
                float xPos = (j - (float) (gridSize.x - 1) /2) * (cellSize.x + cellGap.x) + gridTransform.position.x;
                float yPos = (i - (float) (gridSize.y - 1) /2) * (cellSize.y + cellGap.y) + gridTransform.position.y;
                
                Vector2 cellPosition = new Vector2(xPos, yPos);
                Gizmos.DrawWireCube(cellPosition, cellSize);
            }
        }
    }
}
