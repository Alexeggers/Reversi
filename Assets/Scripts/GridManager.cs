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
        private enum Direction { TOP, RIGHT, BOTTOM, LEFT };

        #region worker variables
        private List<GameObject> tiles;
        private Transform grid;
        private GameObject tileTemplate;
        private Material whiteTile;
        private Material blackTile;
        private Material baseTile;
        private Material validTile;
        private float zAxis = 0.1f;
        private int validTilesCount = 4;
        #endregion

        public GridManager(GameObject tileTemplate, Transform grid, Material whiteTile, Material blackTile, Material baseTile, Material validTile)
        {
            this.tileTemplate = tileTemplate;
            this.grid = grid;
            this.whiteTile = whiteTile;
            this.blackTile = blackTile;
            this.baseTile = baseTile;
            this.validTile = validTile;
        }

        /// <summary>
        /// Build the grid and start managing it
        /// </summary>
        public void Initiate()
        {
            buildTiles();
            SetValidTiles();
        }

        /// <summary>
        /// Is called when a tile is clicked. Changes color of the tile and checks whether other tiles need to be altered as well
        /// </summary>
        /// <param name="x">X Coordinate of tile</param>
        /// <param name="y">Y Coordinate of tile</param>
        public void TileClicked(TileScript caller)
        {
            changeTileOwner(caller.gameObject, gameManager.GetCurrentPlayer());
            convertLine(caller.Top, Direction.TOP);
            convertLine(caller.Right, Direction.RIGHT);
            convertLine(caller.Bottom, Direction.BOTTOM);
            convertLine(caller.Left, Direction.LEFT);
        }

        /// <summary>
        /// Delete all children then call initiate()
        /// </summary>
        public void ResetBoard()
        {
            tiles.ForEach(child => GameObject.Destroy(child));
            Initiate();
        }

        /// <summary>
        /// Checks the array for tiles that the current player can click
        /// </summary>
        public void SetValidTiles()
        {
            int tempValidTilesCount = 0;
            tiles.ForEach(t =>
            {
                if (isValid(t))
                {
                    t.GetComponent<TileScript>().isValid = true;
                    t.GetComponent<MeshRenderer>().material = this.validTile;
                    tempValidTilesCount++;
                }
                else
                {
                    t.GetComponent<TileScript>().isValid = false;
                    if (t.GetComponent<TileScript>().Owner == GameManager.Player.NONE)
                    {
                        t.GetComponent<MeshRenderer>().material = this.baseTile;
                    }
                }
            });
            this.validTilesCount = tempValidTilesCount;
        }

        /// <summary>
        /// Counts the tiles that are owned by the given player and returns the amount
        /// </summary>
        /// <param name="player">The player who is being checked for</param>
        /// <returns>An int value representing the number of tiles</returns>
        public int GetScore(GameManager.Player player)
        {
            int tempScore = 0;
            tiles.ForEach(t =>
            {
                if (t.GetComponent<TileScript>().Owner == player) tempScore++;
            });
            return tempScore;
        }

        #region private worker methods

        #region tile builders
        private void handleNeighbours(int x, int y, GameObject tile, GameObject[,] tempGrid)
        {
            // top
            if (inBounds(y - 1))
            {
                tempGrid[x, y - 1].GetComponent<TileScript>().Bottom = tile.GetComponent<TileScript>();
                tile.GetComponent<TileScript>().Top = tempGrid[x, y - 1].GetComponent<TileScript>();
            }
            // left
            if (inBounds(x - 1))
            {
                tempGrid[x - 1, y].GetComponent<TileScript>().Right = tile.GetComponent<TileScript>();
                tile.GetComponent<TileScript>().Left = tempGrid[x - 1, y].GetComponent<TileScript>();
            }
        }

        private void buildTiles()
        {
            GameObject[,] tempGrid = new GameObject[8, 8];
            tiles = new List<GameObject>();
            Vector3 tempVector;
            GameObject tempTile;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    tempVector = new Vector3(x, y, zAxis);
                    tempTile = GameObject.Instantiate(tileTemplate, grid);
                    tempTile.transform.position = tempVector;
                    tempTile.GetComponent<TileScript>().SetClickManager(this.gameManager);
                    handleNeighbours(x, y, tempTile, tempGrid);
                    tiles.Add(tempTile);
                    tempGrid[x, y] = tempTile;
                }
            }
            changeTileOwner(tempGrid[3, 3], GameManager.Player.PLAYER_WHITE);
            changeTileOwner(tempGrid[3, 4], GameManager.Player.PLAYER_BLACK);
            changeTileOwner(tempGrid[4, 3], GameManager.Player.PLAYER_BLACK);
            changeTileOwner(tempGrid[4, 4], GameManager.Player.PLAYER_WHITE);
        }
        #endregion

        #region validity checkers
        private bool isValid(GameObject tile)
        {
            TileScript tileScript = tile.GetComponent<TileScript>();
            if (tileScript.Owner != GameManager.Player.NONE) return false;
            if (tileScript.Top != null && belongsToOpponent(tileScript.Top))
            {
                if (checkLineForValidity(tileScript.Top, Direction.TOP)) return true;
            }
            if (tileScript.Right != null && belongsToOpponent(tileScript.Right))
            {
                if (checkLineForValidity(tileScript.Right, Direction.RIGHT)) return true;
            }
            if (tileScript.Bottom != null && belongsToOpponent(tileScript.Bottom))
            {
                if (checkLineForValidity(tileScript.Bottom, Direction.BOTTOM)) return true;
            }
            if (tileScript.Left != null && belongsToOpponent(tileScript.Left))
            {
                if (checkLineForValidity(tileScript.Left, Direction.LEFT)) return true;
            }
            return false;
        }

        private bool checkLineForValidity(TileScript toCheck, Direction direction)
        {
            // Given top as initial value to satisfy compiler that it will always have a value
            TileScript target = toCheck.Top;
            switch (direction)
            {
                case Direction.TOP:
                    target = toCheck.Top;
                    break;
                case Direction.RIGHT:
                    target = toCheck.Right;
                    break;
                case Direction.BOTTOM:
                    target = toCheck.Bottom;
                    break;
                case Direction.LEFT:
                    target = toCheck.Left;
                    break;
            }
            if (target == null) {
                return false;
            }
            else if (target.Owner == gameManager.GetCurrentPlayer())
            {
                return true;
            }
            else if (belongsToOpponent(target))
            {
                return checkLineForValidity(target, direction);
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region tile converters
        private bool convertLine(TileScript toConvert, Direction direction)
        {
            if (toConvert == null) return false;
            TileScript target = toConvert.Top;
            switch (direction)
            {
                case Direction.TOP:
                    target = toConvert.Top;
                    break;
                case Direction.RIGHT:
                    target = toConvert.Right;
                    break;
                case Direction.BOTTOM:
                    target = toConvert.Bottom;
                    break;
                case Direction.LEFT:
                    target = toConvert.Left;
                    break;
            }
            if (belongsToOpponent(toConvert) && convertLine(target, direction))
            {
                changeTileOwner(toConvert.gameObject, gameManager.GetCurrentPlayer());
                return true;
            }
            else if (toConvert.Owner == gameManager.GetCurrentPlayer())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the owner of the tile according to the current player and alters the texture accordingly
        /// </summary>
        /// <param name="x">X Coordinate of tile</param>
        /// <param name="y">Y Coordinate of tile</param>
        private void changeTileOwner(GameObject tile, GameManager.Player newOwner)
        {
            if (newOwner == GameManager.Player.PLAYER_BLACK)
            {
                tile.GetComponent<MeshRenderer>().material = blackTile;
            }
            else
            {
                tile.GetComponent<MeshRenderer>().material = whiteTile;
            }
            tile.GetComponent<TileScript>().Owner = newOwner;
        }
        #endregion

        #endregion

        #region helper methods
        public int GetValidTilesCount()
        {
            return this.validTilesCount;
        }

        /*
        private bool ownedByCurrentPlayer(GameObject tile)
        {
            return tile.GetComponent<TileScript>().GetOwner() == gameManager.GetCurrentPlayer();
        }
        */
        private bool inBounds(int toCheck)
        {
            return toCheck >= 0 && toCheck < 8;
        }

        private bool belongsToOpponent(TileScript tileScript)
        {
            GameManager.Player tileOwner = tileScript.Owner;
            if (tileOwner != GameManager.Player.NONE && tileOwner != gameManager.GetCurrentPlayer()) return true;
            return false;
        }
        #endregion
    }
}
