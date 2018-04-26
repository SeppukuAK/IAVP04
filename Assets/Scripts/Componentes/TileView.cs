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
                    tile.Hero = new Hero(tile);
                    GameManager.Instance.CreateHero(tile.Hero);
                    break;

                case SceneState.SETMAP:
                    //Distinguir entre que tengo que poner
                    if (tile.Hero == null)
                    {
                        //Si no hay nada 
                        if (tile.Ally == null && tile.Enemy.Count == 0)
                        {
                            //hay menos de 5 aliados, pongo un aliado
                            if (GameManager.Instance.Board.NumAllies < 5)
                                CreateAlly();

                            //hay más de 5 aliados, pongo un enemigo
                            else  
                                CreateEnemy();
                            
                        }

                        //Hay aliado
                        else if (tile.Ally != null)
                        {
                            //Hay menos de 20 enemigos, pongo un enemigo
                            if (GameManager.Instance.Board.NumEnemies < 20)
                                CreateEnemy();
                            
                            //Hay más de 20 enemigos, pone un tile vacio
                            //TODO: BORRAR ALIADO
                            Destroy(GameManager.Instance)
                        }

                        else if (tile.Enemy.Count > 0)
                        {
                            //TODO: BORRAR ENEMIGO
                        }

                    }
                    break;

            }
        }

    }

    private void CreateAlly()
    {
        tile.Ally = new Ally(tile);
        GameManager.Instance.CreateAlly(tile.Ally);
        GameManager.Instance.Board.NumAllies++;

    }

    private void CreateEnemy()
    {
        tile.Enemy.Add(new Enemy(tile));
        GameManager.Instance.CreateEnemy(tile.Enemy[0]);
        GameManager.Instance.Board.NumEnemies++;

    }

}