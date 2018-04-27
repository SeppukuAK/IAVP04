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

    public AllyView AllyView { get; set; }
    public HeroView HeroView { get; set; }
    public List<EnemyView> EnemyView { get; set; }

    public void BuildTile(Tile tile)
    {
        this.tile = tile;
        EnemyView = new List<EnemyView>();
    }

    private void OnMouseDown()
    {
        //Se comprueba que en la casilla donde se ha hecho click no está el refugio
        if (!GameManager.Instance.Refuge.Equals(tile.Pos))
        {
            switch (GameManager.Instance.State)
            {
                case SceneState.SETHERO:
                    CreateHero();
                    break;

                case SceneState.SETMAP:
                    //Distinguir entre que tengo que poner
                    if (tile.Hero == null)
                    {
                        //Si no hay nada 
                        if (tile.Ally == null && tile.Enemy.Count == 0)
                        {
                            //hay menos de 5 aliados, pongo un aliado
                            if (GameManager.Instance.Board.NumAllies < GameManager.MAXALLIES)
                                CreateAlly();

                            //hay más de 5 aliados, y menos de 20 enemigos, pongo un enemigo
                            else if (GameManager.Instance.Board.NumEnemies < GameManager.MAXENEMIES)
                                CreateEnemy();

                            //No gano nada si hay 20 enemigos y 5 aliados
                            
                        }

                        //Hay aliado
                        else if (tile.Ally != null)
                        {
                            DeleteAlly();

                            //Hay menos de 20 enemigos, pongo un enemigo
                            if (GameManager.Instance.Board.NumEnemies < GameManager.MAXENEMIES)
                                CreateEnemy();

                            //Hay más de 20 enemigos, pone un tile vacio
                        }

                        else if (tile.Enemy.Count > 0)
                        {
                            DeleteEnemy();
                        }

                    }
                    break;

            }
        }

    }

    /// <summary>
    /// Crea el Heroe, lo instancia en la escena, lo construye
    /// Guarda la referencia en el GameManager y guarda su posición en la lista de Boats
    /// </summary>
    private void CreateHero()
    {
        //Establecemos la lógica
        tile.Hero = new Hero(tile);
        Pos pos = tile.Pos;

        //Construimos el GameObject Hero
        GameObject heroGO = Instantiate(GameManager.Instance.HeroPrefab, new Vector3(pos.X * GameManager.DISTANCE, -pos.Y * GameManager.DISTANCE, 0.0f), Quaternion.identity);
        HeroView = heroGO.GetComponent<HeroView>();
        HeroView.BuildHero(tile.Hero);

        //Guardamos la referencia en GameManger
        GameManager.Instance.Hero = HeroView;

        List<GameObject> list = new List<GameObject>();
        list.Add(heroGO);
        GameManager.Instance.Boats.Add(pos, list);

        //Cambiamos de estado
        GameManager.Instance.State = SceneState.SETMAP;

        GameManager.Instance.ButtonPlay.gameObject.SetActive(true);
    }

    private void CreateAlly()
    {
        //Establecemos la lógica
        tile.Ally = new Ally(tile);
        Pos pos = tile.Pos;

        //Construimos el GameObject Ally
        GameObject allyGO = Instantiate(GameManager.Instance.AllyPrefab, new Vector3(pos.X * GameManager.DISTANCE, -pos.Y * GameManager.DISTANCE, 0.0f), Quaternion.identity);
        AllyView = allyGO.GetComponent<AllyView>();
        AllyView.BuildAlly(tile.Ally);

        //Guardamos la referencia en GameManager
        List<GameObject> list = new List<GameObject>();
        list.Add(allyGO);
        GameManager.Instance.Boats.Add(pos, list);

        //Aumentamos el número de aliados
        GameManager.Instance.Board.NumAllies++;
    }



    private void CreateEnemy()
    {
        //Establecemos la lógica
        tile.Enemy.Add(new Enemy(tile));
        Pos pos = tile.Pos;

        //Construimos el GameObject Ally
        GameObject enemyGO = Instantiate(GameManager.Instance.EnemyPrefab, new Vector3(pos.X * GameManager.DISTANCE, -pos.Y * GameManager.DISTANCE, 0.0f), Quaternion.identity);
        EnemyView enemyView = enemyGO.GetComponent<EnemyView>();
        EnemyView.Add(enemyView);

        EnemyView[0].BuildEnemy(tile.Enemy[0]);

        //Guardamos la referencia en GameManager
        List<GameObject> list = new List<GameObject>();
        list.Add(enemyGO);
        GameManager.Instance.Boats.Add(pos, list);

        //Aumentamos el número de aliados
        GameManager.Instance.Board.NumEnemies++;
    }

    private void DeleteAlly()
    {
        //Borramos referencia lógica
        tile.Ally = null;

        //Borramos referencia visual
        AllyView = null;

        //Borramos de la lista
        GameObject allyGo = GameManager.Instance.Boats[tile.Pos][0];
        GameManager.Instance.Boats.Remove(tile.Pos);

        //Borramos GameObject
        Destroy(allyGo);
        GameManager.Instance.Board.NumAllies--;
    }


    private void DeleteEnemy()
    {
        //Borramos referencia lógica
        tile.Enemy.RemoveAt(0);

        //Borramos referencia visual
        EnemyView.RemoveAt(0);

        //Borramos de la lista
        GameObject enemyGo = GameManager.Instance.Boats[tile.Pos][0];
        GameManager.Instance.Boats.Remove(tile.Pos);

        //Borramos GameObject
        Destroy(enemyGo);
        GameManager.Instance.Board.NumEnemies--;
    }

}