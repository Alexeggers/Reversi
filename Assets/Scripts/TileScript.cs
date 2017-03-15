using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a reference to its tile and that tile's game-relevant properties
/// </summary>
public class TileScript : MonoBehaviour {

    public GameObject Tile;

    public TileScript Top;
    public TileScript Right;
    public TileScript Bottom;
    public TileScript Left;
    public GameManager.Player Owner = GameManager.Player.NONE;
    public bool isValid = false;

    private ClickManager clickManager;


    public void SetClickManager(ClickManager clickManager)
    {
        this.clickManager = clickManager;
    }

    private void OnMouseUp()
    {
        if (this.isValid) this.clickManager.OnClick(this);
    }
}
