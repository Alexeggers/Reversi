using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Contains the array that has references to the game tiles and manages all operations on them
    /// </summary>
    class GridManager
    {
        private GameManager gameManager = GameManager.Instance;
        private GameObject[,] tileStore;
        private GameObject tile;
        private Transform grid;
        private Material whiteTile;
        private Material blackTile;
        private Material baseTile;
        private Material validTile;
        private float zAxis = 0.1f;
        private int validTilesCount = 4;

        public GridManager(GameObject tile, Transform grid, Material whiteTile, Material blackTile, Material baseTile, Material validTile)
        {
            this.tile = tile;
            this.grid = grid;
            this.whiteTile = whiteTile;
            this.blackTile = blackTile;
            this.baseTile = baseTile;
            this.validTile = validTile;
        }

        #region Public Methods
        /// <summary>
        /// Build the grid and start managing it
        /// </summary>
        public void Initiate()
        {
            CreateGrid();
            SetValidTiles();
        }

        /// <summary>
        /// Delete all children then call initiate()
        /// </summary>
        public void ResetBoard()
        {
            List<GameObject> tempTileStore = new List<GameObject>();
            foreach (GameObject child in tileStore) tempTileStore.Add(child);
            tempTileStore.ForEach(child => GameObject.Destroy(child));
            Initiate();
        }

        /// <summary>
        /// Checks the array for tiles that the current player can click
        /// </summary>
        public void SetValidTiles()
        {
            GameObject tempTile;
            int tempValidTiles = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    tempTile = tileStore[x, y];
                    if (tempTile.GetComponent<TileScript>().GetOwner() != GameManager.Player.NONE)
                    {
                        tileStore[x, y].GetComponent<TileScript>().SetIsValid(false);
                        continue;
                    }

                    if (hasAdjacentCapture(x, y))
                    {
                        setValidity(x, y, true);
                        tempValidTiles++;
                    }
                    else
                    {
                        setValidity(x, y, false);
                    }
                }
            }
            this.validTilesCount = tempValidTiles;
        }

        /// <summary>
        /// Is called when a tile is clicked. Changes color of the tile and checks whether other tiles need to be altered as well
        /// </summary>
        /// <param name="x">X Coordinate of tile</param>
        /// <param name="y">Y Coordinate of tile</param>
        public void TileClicked(int x, int y)
        {
            changeTileOwner(x, y);
            manageTileConversion(x, y);
        }

        /// <summary>
        /// Counts the tiles that are owned by the given player and returns the amount
        /// </summary>
        /// <param name="player">The player who is being checked for</param>
        /// <returns>An int value representing the number of tiles</returns>
        public int GetScore(GameManager.Player player)
        {
            int tempScore = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (this.tileStore[x, y].GetComponent<TileScript>().GetOwner() == player)
                    {
                        tempScore++;
                    }
                }
            }
            return tempScore;
        }
        #endregion

        /// <summary>
        /// Builds the grid tiles from a template tile and stores them in an array
        /// </summary>
        private void CreateGrid()
        {
            tileStore = new GameObject[8, 8];
            Vector3 tempVector;
            GameObject tempGameObject;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    tempVector = new Vector3(x, y, zAxis);
                    tempGameObject = GameObject.Instantiate(tile, grid);
                    tempGameObject.transform.position = tempVector;
                    this.tileStore[x, y] = tempGameObject;
                }
            }
            changeTileOwner(3, 3, GameManager.Player.PLAYER_WHITE);
            changeTileOwner(3, 4, GameManager.Player.PLAYER_BLACK);
            changeTileOwner(4, 3, GameManager.Player.PLAYER_BLACK);
            changeTileOwner(4, 4, GameManager.Player.PLAYER_WHITE);
        }


        /// <summary>
        /// Sets a tile's isValid attribute and gives it the appropriate texture
        /// </summary>
        /// <param name="x">X coordinate of the tile</param>
        /// <param name="y">Y coordinate of the tile</param>
        /// <param name="isValid">Whether it is valid or not</param>
        private void setValidity(int x, int y, bool isValid)
        {
            if (isValid)
            {
                tileStore[x, y].GetComponent<TileScript>().SetIsValid(true);
                tileStore[x, y].GetComponent<MeshRenderer>().material = this.validTile;
            }
            else
            {
                tileStore[x, y].GetComponent<TileScript>().SetIsValid(false);
                tileStore[x, y].GetComponent<MeshRenderer>().material = this.baseTile;
            }
        }

        /// <summary>
        /// Checks whether a tile can capture in any direction
        /// </summary>
        /// <param name="x">X coordinate of the tile</param>
        /// <param name="y">Y coordinate of the tile</param>
        /// <returns>Whether it can</returns>
        private bool hasAdjacentCapture(int x, int y)
        {
            return checkForXCapture(x, y, 1) || checkForXCapture(x, y, -1) || checkForYCapture(x, y, 1) || checkForYCapture(x, y, -1);
        }

        /// <summary>
        /// Checks whether there are potential capture lines on the y axis in a specific direction from the given coordinates
        /// </summary>
        /// <param name="x">X coordinate of the given tile</param>
        /// <param name="y">Y coordinate of the given tile</param>
        /// <param name="operation">Direction the function checks in (positive 1 or negative 1 for up or down respectively)</param>
        /// <returns>Boolean indicating the possibility</returns>
        private bool checkForYCapture(int x, int y, int operation)
        {
            y += operation;
            bool repeat = inBounds(y);
            if (inBounds(y))
            {
                if (tileStore[x, y].GetComponent<TileScript>().GetOwner() == gameManager.GetCurrentPlayer() ||
                    tileStore[x, y].GetComponent<TileScript>().GetOwner() == GameManager.Player.NONE)
                {
                    return false;
                }
            }
            y += operation;
            repeat = repeat && inBounds(y);
            // At this point, the adjacent tile definitely belongs to the opponent
            while (repeat)
            {
                if (tileStore[x, y].GetComponent<TileScript>().GetOwner() == gameManager.GetCurrentPlayer())
                {
                    return true;
                }
                else if (tileStore[x, y].GetComponent<TileScript>().GetOwner() == GameManager.Player.NONE)
                {
                    break;
                }
                y += operation;
                repeat = repeat && inBounds(y);
            }
            return false;
        }

        /// <summary>
        /// Checks whether there are potential capture lines on the x axis in a specific direction from the given coordinates
        /// </summary>
        /// <param name="x">X coordinate of the given tile</param>
        /// <param name="y">Y coordinate of the given tile</param>
        /// <param name="operation">Direction the function checks in (positive 1 or negative 1 for right or left respectively)</param>
        /// <returns>Boolean indicating the possibility</returns>
        private bool checkForXCapture(int x, int y, int operation)
        {
            x += operation;
            bool repeat = inBounds(x);
            if (inBounds(x))
            {
                if (tileStore[x, y].GetComponent<TileScript>().GetOwner() == gameManager.GetCurrentPlayer() ||
                    tileStore[x, y].GetComponent<TileScript>().GetOwner() == GameManager.Player.NONE)
                {
                    return false;
                }
            }
            x += operation;
            repeat = repeat && inBounds(x);
            // At this point, the adjacent tile definitely belongs to the opponent
            while (repeat)
            {
                if (tileStore[x, y].GetComponent<TileScript>().GetOwner() == gameManager.GetCurrentPlayer())
                {
                    return true;
                }
                else if (tileStore[x, y].GetComponent<TileScript>().GetOwner() == GameManager.Player.NONE)
                {
                    break;
                }
                x += operation;
                repeat = repeat && inBounds(x);
            }
            return false;
        }


        private void manageTileConversion(int x, int y)
        {

            xTileConversion(x, y, 1);
            xTileConversion(x, y, -1);
            yTileConversion(x, y, 1);
            yTileConversion(x, y, -1);
        }

        /// <summary>
        /// Goes along the X axis to check how many tiles may need to change ownership
        /// and delegates changing ownership if necessary
        /// </summary>
        /// <param name="x">X coordinate of the tile</param>
        /// <param name="y">Y coordinate of the tile</param>
        /// <param name="operation">int used to traverse the array</param>
        private void xTileConversion(int x, int y, int operation)
        {
            x += operation;
            List<GameObject> tempStorage = new List<GameObject>();
            bool repeat = inBounds(x);
            while (repeat)
            {
                repeat = processTile(x, y, ref tempStorage);
                x += operation;
                repeat = repeat && inBounds(x);
            }
        }

        /// <summary>
        /// Goes along the Y axis to check how many tiles may need to change ownership
        /// and delegates changing ownership if necessary
        /// </summary>
        /// <param name="x">X coordinate of the tile</param>
        /// <param name="y">Y coordinate of the tile</param>
        /// <param name="operation">int used to traverse the array</param>
        private void yTileConversion(int x, int y, int operation)
        {
            y += operation;
            List<GameObject> tempStorage = new List<GameObject>();
            bool repeat = inBounds(y);
            while (repeat)
            {
                repeat = processTile(x, y, ref tempStorage);
                y += operation;
                repeat = repeat && inBounds(y);
            }
        }

        /// <summary>
        /// Checks who owns the tile at the given coordinates and either adds it to the tiles
        /// to be changed if its owned by the opponent, changes all previous tiles' ownership
        /// if it belongs to the current player or does nothing if it is empty
        /// </summary>
        /// <param name="x">X coordinate of the tile</param>
        /// <param name="y">Y coordinate of the tile</param>
        /// <param name="tempStorage">Temporary storage of tiles that potentially switch ownership</param>
        /// <returns>Bool indicating whether further tiles should be checked</returns>
        private bool processTile(int x, int y, ref List<GameObject> tempStorage)
        {
            if (ownedByOpponent(tileStore[x, y]))
            {
                tempStorage.Add(tileStore[x, y]);
                return true;
            }
            else if (ownedByCurrentPlayer(tileStore[x, y]))
            {
                tempStorage.ForEach(changeTileOwner);
                return false;
            }
            else
            {
                return false;
            }
        }

        #region ChangeTileOwner
        /// <summary>
        /// Sets the owner of the tile according to the current player and alters the texture accordingly
        /// </summary>
        /// <param name="x">X Coordinate of tile</param>
        /// <param name="y">Y Coordinate of tile</param>
        private void changeTileOwner(int x, int y)
        {
            GameObject tempTile;
            tempTile = tileStore[x, y];
            if (gameManager.GetCurrentPlayer() == GameManager.Player.PLAYER_BLACK)
            {
                tempTile.GetComponent<MeshRenderer>().material = blackTile;
            }
            else
            {
                tempTile.GetComponent<MeshRenderer>().material = whiteTile;
            }
            tempTile.GetComponent<TileScript>().SetOwner(gameManager.GetCurrentPlayer());
        }

        private void changeTileOwner(GameObject tile)
        {
            if (gameManager.GetCurrentPlayer() == GameManager.Player.PLAYER_BLACK)
            {
                tile.GetComponent<MeshRenderer>().material = blackTile;
            }
            else
            {
                tile.GetComponent<MeshRenderer>().material = whiteTile;
            }
            tile.GetComponent<TileScript>().SetOwner(gameManager.GetCurrentPlayer());
        }

        private void changeTileOwner(int x, int y, GameManager.Player newOwner)
        {
            GameObject tempTile;
            tempTile = tileStore[x, y];
            if (newOwner == GameManager.Player.PLAYER_BLACK)
            {
                tempTile.GetComponent<MeshRenderer>().material = blackTile;
            }
            else
            {
                tempTile.GetComponent<MeshRenderer>().material = whiteTile;
            }
            tempTile.GetComponent<TileScript>().SetOwner(newOwner);
        }
        #endregion

        #region HelperMethods
        public int GetValidTilesCount()
        {
            return this.validTilesCount;
        }

        private bool ownedByOpponent(GameObject tile)
        {
            GameManager.Player owner = tile.GetComponent<TileScript>().GetOwner();
            return owner != gameManager.GetCurrentPlayer() && owner != GameManager.Player.NONE;
        }

        private bool ownedByCurrentPlayer(GameObject tile)
        {
            return tile.GetComponent<TileScript>().GetOwner() == gameManager.GetCurrentPlayer();
        }

        private bool inBounds(int toCheck)
        {
            return toCheck >= 0 && toCheck < 8;
        }
        #endregion
    }
}
