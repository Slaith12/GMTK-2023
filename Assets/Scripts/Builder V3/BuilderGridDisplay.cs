using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum CellCategory { All, Edges }

public class BuilderGridDisplay : MonoBehaviour
{
    const string CLASS_LIGHTEN = "lighten";
    const string CLASS_OCCUPIED = "occupied";
    const string CLASS_HIGHLIGHT = "highlighted";
    const string CLASS_INVALID = "invalid";

    private VisualElement placementGrid;
    private VisualElement[,] visualCells;
    private List<VisualElement> highlightedCells;

    private void Init(int gridWidth, int gridHeight)
    {
        visualCells = new VisualElement[gridWidth, gridHeight];
        highlightedCells = new List<VisualElement>();

        UIDocument document = GetComponent<UIDocument>();
        VisualElement rootPlacementsElement = document.rootVisualElement.Q(name: "placements");
        placementGrid = rootPlacementsElement.Q(className: "rows");

        int cellY = 0;
        foreach (VisualElement cellRow in placementGrid.Children())
        {
            if (cellY >= gridHeight)
            {
                Debug.LogError("Builder grid height bigger than expected. Please correct value in BuilderGridDisplay script.");
                break;
            }
            int cellX = 0;
            foreach (VisualElement cell in cellRow.Children())
            {
                if (cellX >= gridWidth)
                {
                    Debug.LogError("Builder grid width bigger than expected. Please correct value in BuilderGridDisplay script.");
                    break;
                }
                visualCells[cellX, cellY] = cell;
                cellX++;
            }
            if (cellX < gridWidth)
            {
                Debug.LogError("Builder grid width smaller than expected. Please correct value in BuilderGridDisplay script.");
            }
            cellY++;
        }
        if (cellY < gridHeight)
        {
            Debug.LogError("Builder grid height smaller than expected. Please correct value in BuilderGridDisplay script.");
        }
    }

    public Vector2Int GetClosestCellFromMouse(Vector2 mousePos, CellCategory filter = CellCategory.All)
    {
        Vector2 gridOrigin = placementGrid.worldBound.min; //top-left corner
        Vector2 cellSize = visualCells[0, 0].worldBound.size;
        mousePos.y = Screen.height - mousePos.y; //input system uses y = 0 at bottom of screen, ui uses y = 0 at top of screen

        int cellX = (int)((mousePos.x - gridOrigin.x) / cellSize.x);
        if (cellX < 0)
            cellX = 0;
        else if (cellX >= visualCells.GetLength(0))
            cellX = visualCells.GetLength(0) - 1;

        int cellY = (int)((mousePos.y - gridOrigin.y) / cellSize.y);
        if (cellY < 0)
            cellY = 0;
        else if (cellY >= visualCells.GetLength(1))
            cellY = visualCells.GetLength(1) - 1;

        if(filter == CellCategory.Edges && 
            cellX > 0 && cellX < visualCells.GetLength(0) - 1 &&
            cellY > 0 && cellY < visualCells.GetLength(1) - 1) 
        {
            if (cellY >= visualCells.GetLength(1) / 2)
                cellY = visualCells.GetLength(1) - 1;
            else
                cellY = 0;
        }

        return new Vector2Int(cellX, cellY);
    }

    public bool CellsAvailable(List<Vector2Int> cellPositions)
    {
        foreach(Vector2Int cellPos in cellPositions)
        {
            if (visualCells[cellPos.x, cellPos.y].ClassListContains(CLASS_OCCUPIED))
                return false;
        }
        return true;
    }

    #region Highlight

    public void HighlightCells(List<Vector2Int> cellPositions)
    {
        UnHighlightCells();
        if(cellPositions == null)
        {
            return;
        }

        bool invalid = !CellsAvailable(cellPositions);

        foreach(Vector2Int cellPos in cellPositions)
        {
            VisualElement cell = visualCells[cellPos.x, cellPos.y];
            highlightedCells.Add(cell);
            cell.AddToClassList(CLASS_HIGHLIGHT);
            if (invalid)
                cell.AddToClassList(CLASS_INVALID);
        }
    }

    public void UnHighlightCells()
    {
        foreach(VisualElement cell in highlightedCells)
        {
            cell.RemoveFromClassList(CLASS_HIGHLIGHT);
            cell.RemoveFromClassList(CLASS_INVALID);
        }
        highlightedCells.Clear();
    }

    #endregion

    #region Occupied

    public void MarkOccupied(Vector2Int cellPos)
    {
        VisualElement cell = visualCells[cellPos.x, cellPos.y];
        cell.AddToClassList(CLASS_OCCUPIED);
    }

    public void MarkUnOccupied(Vector2Int cellPos)
    {
        VisualElement cell = visualCells[cellPos.x, cellPos.y];
        cell.RemoveFromClassList(CLASS_OCCUPIED);
    }

    #endregion

    #region Lighten

    public void LightenCells(CellCategory filter = CellCategory.All)
    {
        switch(filter)
        {
            case CellCategory.All:
                for (int i = 0; i < visualCells.GetLength(0); i++)
                {
                    for (int j = 0; j < visualCells.GetLength(1); j++)
                    {
                        visualCells[i, j].AddToClassList(CLASS_LIGHTEN);
                    }
                }
                break;
            case CellCategory.Edges:
                for (int i = 0; i < visualCells.GetLength(0); i++)
                {
                    int lastY = visualCells.GetLength(1) - 1;
                    visualCells[i, 0].AddToClassList(CLASS_LIGHTEN);
                    visualCells[i, lastY].AddToClassList(CLASS_LIGHTEN);
                }
                for (int j = 1; j < visualCells.GetLength(1) - 1; j++)
                {
                    int lastX = visualCells.GetLength(0) - 1;
                    visualCells[0, j].AddToClassList(CLASS_LIGHTEN);
                    visualCells[lastX, j].AddToClassList(CLASS_LIGHTEN);
                }
                break;
        }
    }

    public void UnLightenCells()
    {
        for (int i = 0; i < visualCells.GetLength(0); i++)
        {
            for (int j = 0; j < visualCells.GetLength(1); j++)
            {
                visualCells[i, j].RemoveFromClassList(CLASS_LIGHTEN);
            }
        }
    }

    #endregion

}
