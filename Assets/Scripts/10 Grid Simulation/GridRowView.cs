using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;

namespace EnhancedScrollerDemos.GridSimulation
{
    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// It stores references to sub cells
    /// </summary>
    public class GridRowView : EnhancedScrollerCellView
    {
        public GameObject cellPrefab;
        private GameObject[] cells;
        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(ref SmallList<Data> data, int columnSize, int startingIndex)
        {
            // if the sub cell is outside the bounds of the data, we pass null to the sub cell
            cells ??= new GameObject[columnSize];
            // loop through the sub cells to display their data (or disable them if they are outside the bounds of the data)
            for (var i = 0; i < columnSize; i++)
            {
                if (cells[i] == null)
                {
                    cells[i] = Instantiate(cellPrefab, transform);
                }
                
                var pictureData = startingIndex + i < data.Count ? data[startingIndex + i].picture : Texture2D.whiteTexture;
                Sprite sprite = Sprite.Create(pictureData, 
                    new Rect(0, 0, pictureData.width, pictureData.height), 
                    new Vector2(0.5f, 0.5f));
                cells[i].GetComponent<Image>().sprite = sprite;
            }
        }
    }
}