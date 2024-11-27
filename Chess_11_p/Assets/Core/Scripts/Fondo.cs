using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Fondo : MonoBehaviour
{
    public int rows = 11; // Number of rows in the grid (updated to 11)
    public int columns = 20; // Number of columns in the grid (updated to 20)
    public float swapInterval = 1f; // Time interval between color swaps
    public float spacing = 10f; // Spacing between tiles in the grid (adjustable)
    public float transitionTime = 1f; // Duration of the smooth color transition
    public int numberOfHighlightedTiles = 5; // Number of tiles to highlight at once

    private Image[,] images; // 2D array to hold Image references
    private Color[,] originalColors; // 2D array to store original colors of the tiles

    private Color[] colors = { Color.green, Color.red, new Color(0.5f, 0f, 0.5f) }; // Green, Red, Purple (RGB)

    void Start()
    {
        images = new Image[rows, columns]; // Initialize the image array
        originalColors = new Color[rows, columns]; // Initialize the array to store original colors
        GenerateGrid(); // Generate the grid of images
        StartCoroutine(SwapColors()); // Start the color swapping coroutine
    }

    void GenerateGrid()
    {
        // Loop through and generate the grid of images
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Instantiate a new image for each grid position
                GameObject imgObj = new GameObject("Image", typeof(RectTransform), typeof(Image));
                imgObj.transform.SetParent(transform);

                // Calculate the position of each image based on its row and column
                float xPos = col * (110f + spacing); // Adjust position based on image size + spacing
                float yPos = -row * (110f + spacing); // Adjust position based on image size + spacing
                imgObj.transform.localPosition = new Vector3(xPos, yPos, 0);

                // Get the Image component
                Image img = imgObj.GetComponent<Image>();
                images[row, col] = img; // Store the reference to the image

                // Set the initial color based on the chessboard pattern
                if ((row + col) % 2 == 0)
                {
                    img.color = Color.white; // Even sum -> white
                    originalColors[row, col] = Color.white; // Store the original color
                }
                else
                {
                    img.color = Color.black; // Odd sum -> black
                    originalColors[row, col] = Color.black; // Store the original color
                }

                // Optionally, set the size of each image
                imgObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100); // Size of each rectangle
            }
        }
    }

    IEnumerator SwapColors()
    {
        while (true)
        {
            // Pick a random number of tiles to highlight
            int highlightedTilesCount = Mathf.Min(numberOfHighlightedTiles, rows * columns);

            // Select random tiles
            List<Image> selectedTiles = new List<Image>();
            HashSet<int> selectedIndexes = new HashSet<int>();

            while (selectedTiles.Count < highlightedTilesCount)
            {
                int randomRow = Random.Range(0, rows);
                int randomCol = Random.Range(0, columns);
                int index = randomRow * columns + randomCol;

                if (!selectedIndexes.Contains(index))
                {
                    selectedIndexes.Add(index);
                    selectedTiles.Add(images[randomRow, randomCol]);
                }
            }

            // Pick a random target color from the array of colors (green, red, purple)
            Color targetColor = colors[Random.Range(0, colors.Length)];

            // Smoothly transition all selected tiles to the new color
            foreach (Image tile in selectedTiles)
            {
                StartCoroutine(SmoothColorTransition(tile, tile.color, targetColor));
            }

            // Wait for a specified duration before changing them back to the original colors
            yield return new WaitForSeconds(2f); // You can adjust this wait time

            // Smoothly transition all selected tiles back to their original colors
            foreach (Image tile in selectedTiles)
            {
                // Find the row and column from the 'images' array directly
                // We know 'tile' belongs to the `images` array, so we can get the row and col based on that

                // You already have the tile references in `images[randomRow, randomCol]`
                // So, just use the corresponding row and column values
                int row = -1;
                int col = -1;

                // Find the row and column of the current tile
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        if (images[r, c] == tile)
                        {
                            row = r;
                            col = c;
                            break;
                        }
                    }
                    if (row != -1) break;
                }

                // Ensure that the row and column were found correctly
                if (row != -1 && col != -1)
                {
                    // Transition the tile back to its original color
                    StartCoroutine(SmoothColorTransition(tile, tile.color, originalColors[row, col]));
                }
            }

            // Wait for the next interval before changing another set of tiles
            yield return new WaitForSeconds(swapInterval);
        }
    }

    // Coroutine to smoothly transition between colors
    IEnumerator SmoothColorTransition(Image img, Color startColor, Color targetColor)
    {
        float elapsedTime = 0f;

        while (elapsedTime < transitionTime)
        {
            img.color = Color.Lerp(startColor, targetColor, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure the target color is set exactly at the end
        img.color = targetColor;
    }
}