using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a reference to its tile and that tile's game-relevant properties
/// </summary>
public class TileScript : MonoBehaviour {

    public GameObject Tile;

    private GameManager gameManager;
    private GameManager.Player owner = GameManager.Player.NONE;
    private bool isValid = false;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameManager.Player GetOwner()
    {
        return this.owner;
    }

    public void SetOwner(GameManager.Player newOwner)
    {
        this.owner = newOwner;
    }

    public bool GetIsValid()
    {
        return this.isValid;
    }

    public void SetIsValid(bool isValid)
    {
        this.isValid = isValid;
    }

    /// <summary>
    /// On Click handler for tile
    /// </summary>
    void OnMouseDown()
    {
        if (owner == GameManager.Player.NONE && this.isValid)
        {
            gameManager.TileClicked((int)Tile.transform.position.x, (int)Tile.transform.position.y);
            gameManager.EndTurn();
        }
    }
}
