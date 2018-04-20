using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Representación lógica de las casillas. Guardan toda la información contenida en ella
/// Componente asociado a los gameObject Tile, guarda la representación lógica de estos e informa al GameManager cuando son pulsados con el ratón
/// </summary>
public class TileView : MonoBehaviour
{

    /// <summary>
    /// Posición lógica del tile
    /// </summary>
    private Tile tile;

    public void BuildTile(Tile tile)
    {
        this.tile = tile;
    }

    private void OnMouseDown()
    {
        //Se comprueba que en la casilla donde se ha hecho click no está el refugio
        if (!GameManager.Instance.Refuge.Equals(tile.Pos))
        {
            switch (GameManager.Instance.State)
            {
                case SceneState.SETHERO:
                    //GameManager.Instance.SetHero(tile.Pos);
                    break;

                case SceneState.SETALLY:
                    // GameManager.Instance.SetAlly(tile.Pos);
                    break;

                case SceneState.SETENEMY:
                    // GameManager.Instance.SetEnemy(tile.Pos);
                    break;


            }
        }

    }

}