using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using System;
using UnityEngine.Events;

/// <summary>
/// Manages the state of the game
/// </summary>
public class GameManager : Singleton<GameManager>, ClickManager
{

    #region unity variables
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
    #endregion

    public enum Player {NONE, PLAYER_WHITE, PLAYER_BLACK};

    private GridManager gridManager;

    #region worker variables
    private Player currentPlayer = Player.PLAYER_BLACK;
    private string baseWhiteScoreText = "Current White Score: ";
    private string baseBlackScoreText = "Current Black Score: ";
    private string baseCurrentPlayerText = "Current Player: ";
    private int currentWhiteScore = 2;
    private int currentBlackScore = 2;
    #endregion

    void Start()
    {
        this.gridManager = new GridManager(Tile, Grid, WhiteTile, BlackTile, BaseTile, ValidTile);
        gridManager.Initiate();
        Button buttonComponent = PlayAgainReset.AddComponent<Button>();
        buttonComponent.onClick.AddListener(resetGame);
    }

    public void OnClick(TileScript caller)
    {
        gridManager.TileClicked(caller);
        endTurn();
    }

    public Player GetCurrentPlayer()
    {
        return this.currentPlayer;
    }

    #region worker methods
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
    /// Changes the current player, sets valid tiles, keeps score and checks for victory conditions
    /// </summary>
    private void endTurn()
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
    #endregion
}