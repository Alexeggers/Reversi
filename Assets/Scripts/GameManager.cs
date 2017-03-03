using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using System;
using UnityEngine.Events;

/// <summary>
/// Manages the state of the game
/// </summary>
public class GameManager : Singleton<GameManager>
{

    // Passed to C# by Unity, never instantiated in code
    public GameObject Tile;
    public Transform Grid;
    public Material WhiteTile;
    public Material BlackTile;
    public Material BaseTile;
    public Material ValidTile;
    public Text WhiteScoreText;
    public Text BlackScoreText;
    public Text CurrentPlayerText;
    public GameObject PlayAgainReset;


    public enum Player {NONE, PLAYER_WHITE, PLAYER_BLACK};
    private GridManager gridManager;
    private Player currentPlayer = Player.PLAYER_BLACK;
    private string baseWhiteScoreText = "Current White Score: ";
    private string baseBlackScoreText = "Current Black Score: ";
    private string baseCurrentPlayerText = "Current Player: ";
    private int currentWhiteScore = 2;
    private int currentBlackScore = 2;

    // Use this for initialization
    void Start()
    {
        this.gridManager = new GridManager(Tile, Grid, WhiteTile, BlackTile, BaseTile, ValidTile);
        gridManager.Initiate();

        Button buttonComponent = PlayAgainReset.AddComponent<Button>();
        buttonComponent.onClick.AddListener(resetGame);
    }

    private void resetGame()
    {

        gridManager.ResetBoard();
        resetCurrentPlayer();
        gridManager.SetValidTiles();
        countScores();
    }

    /// <summary>
    /// Changes the currently active player and checks for valid tiles
    /// </summary>
    private void changePlayer()
    {
        if (this.currentPlayer == Player.PLAYER_WHITE)
        {
            this.currentPlayer = Player.PLAYER_BLACK;
            this.CurrentPlayerText.text = baseCurrentPlayerText + "Black";
        }
        else
        {
            this.currentPlayer = Player.PLAYER_WHITE;
            this.CurrentPlayerText.text = baseCurrentPlayerText + "White";
        }
        gridManager.SetValidTiles();
    }

    /// <summary>
    /// Sets the current player to black and alters the UI text accordingly
    /// </summary>
    private void resetCurrentPlayer()
    {
        this.currentPlayer = Player.PLAYER_BLACK;
        this.CurrentPlayerText.text = baseCurrentPlayerText + "Black";
    }

    /// <summary>
    /// Callback for TileScript to notify that a tile was clicked. Returns the new owner of the tile so it can be set in the TileScript
    /// </summary>
    /// <param name="x">X Coordinate of tile</param>
    /// <param name="y">Y Coordinate of tile</param>
    public void TileClicked(int x, int y)
    {
        gridManager.TileClicked(x, y);
    }

    /// <summary>
    /// Callback for the TileScript to notify the game manager that a player has completed his turn
    /// 
    /// Changes the current player, sets valid tiles, keeps score and checks for victory conditions
    /// </summary>
    public void EndTurn()
    {
        changePlayer();
        countScores();
        if (gridManager.GetValidTilesCount() == 0)
        {
            changePlayer();
            if (gridManager.GetValidTilesCount() == 0)
            {
                endOfGame();
            }
        }
    }

    private void endOfGame()
    {
        // TODO Implement victory
        if (this.currentBlackScore < this.currentWhiteScore)
        {
            this.CurrentPlayerText.text = "White wins!";
        }
        else
        {
            this.CurrentPlayerText.text = "Black wins!";
        }
    }

    private void countScores()
    {
        this.currentWhiteScore = gridManager.GetScore(Player.PLAYER_WHITE);
        this.currentBlackScore = gridManager.GetScore(Player.PLAYER_BLACK);
        this.WhiteScoreText.text = baseWhiteScoreText + this.currentWhiteScore;
        this.BlackScoreText.text = baseBlackScoreText + this.currentBlackScore;
    }

    public Player GetCurrentPlayer()
    {
        return this.currentPlayer;
    }
}