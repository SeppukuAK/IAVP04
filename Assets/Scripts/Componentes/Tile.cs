using UnityEngine;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Componente asociado a los gameObject Tile, guarda la representación lógica y física de estos e instancia a las unidades correspondientes
/// </summary>
public class Tile : MonoBehaviour
{
    /// <summary>
    /// Posición lógica del tile
    /// </summary>
    public Pos Pos { get; set; }

    public int NumEnemies { get; set; }
    public bool HeroIsHere { get; set; }
    public bool AllyIsHere { get; set; }

    /// <summary>
    /// Constructora del tile
    /// </summary>
    /// <param name="pos"></param>
    public void BuildTile(Pos pos)
    {
        Pos = pos;
        NumEnemies = 0;
        HeroIsHere = AllyIsHere = false;
    }

    //Coloca la unidad correspondiente
    private void OnMouseDown()
    {
        //Se comprueba que en la casilla donde se ha hecho click no está el refugio
        if (!Map.Instance.Refuge.Equals(Pos))
        {
            switch (GameManager.Instance.State)
            {
                case SceneState.SETHERO:
                    CreateHero();
                    break;

                case SceneState.SETMAP:
                    //Si no hay heroe en esta casilla
                    if (!HeroIsHere)
                    {
                        //Distingo entre que tengo que poner

                        //Si no hay nada 
                        if (!AllyIsHere && NumEnemies == 0)
                        {
                            //hay menos de 5 aliados, pongo un aliado
                            if (Map.Instance.Allies.Count < GameManager.MAXALLIES)
                                CreateAlly();

                            //hay más de 5 aliados, y menos de 20 enemigos, pongo un enemigo
                            else if (Map.Instance.Enemies.Count < GameManager.MAXENEMIES)
                                CreateEnemy();

                            //No hago nada si hay 20 enemigos y 5 aliados

                        }

                        //Hay aliado
                        else if (AllyIsHere)
                        {
                            DeleteAlly();

                            //Hay menos de 20 enemigos, pongo un enemigo
                            if (Map.Instance.Enemies.Count < GameManager.MAXENEMIES)
                                CreateEnemy();

                            //Hay más de 20 enemigos, pone un tile vacio
                        }

                        else if (NumEnemies == 1)
                        {
                            DeleteEnemy();
                        }

                    }
                    break;

            }
        }

    }

    /// <summary>
    /// Crea el Heroe, lo instancia en la escena, lo construye;
    /// Guarda la referencia en el Mapa;
    /// Guarda la información en el Tile
    /// Actualiza el estado del mapa;
    /// Cambia al estado de juego;
    /// </summary>
    private void CreateHero()
    {
        //Construimos el GameObject Hero
        GameObject heroGO = Instantiate(GameManager.Instance.HeroPrefab, new Vector3(Pos.X * GameManager.DISTANCE, -Pos.Y * GameManager.DISTANCE, 0.0f), Quaternion.identity);
        Hero hero = heroGO.GetComponent<Hero>();
        hero.BuildUnit(Pos);

        //Guardamos la referencia en el Mapa
        Map.Instance.Hero = hero;

        //Guardamos la información en el tile
        HeroIsHere = true;

        //Establece la iluminación del barco en función de si hay luz o no
        Map.Instance.LightOn = Map.Instance.LightOn;//Llama a OnMapChange()

        //Cambiamos de estado
        GameManager.Instance.State = SceneState.SETMAP;
        GameManager.Instance.ButtonPlay.gameObject.SetActive(true);
        Map.Instance.StatsPanel.SetActive(true);

    }

    /// <summary>
    /// Crea el Aliado, lo instancia en la escena, lo construye;
    /// Guarda la referencia en el Mapa, añadiendoló a la lista;
    /// Guarda la información en el Tile
    /// Actualiza el estado del mapa;
    /// </summary>
    private void CreateAlly()
    {
        //Construimos el GameObject Ally
        GameObject allyGO = Instantiate(GameManager.Instance.AllyPrefab, new Vector3(Pos.X * GameManager.DISTANCE, -Pos.Y * GameManager.DISTANCE, 0.0f), Quaternion.identity);
        Unit ally = allyGO.GetComponent<Unit>();
        ally.BuildUnit(Pos);

        //Guardamos la referencia en el Mapa
        Map.Instance.Allies.Add(ally);

        //Guardamos la información en el tile
        AllyIsHere = true;

        Map.Instance.OnMapChange();
    }

    /// <summary>
    /// Crea el Enemigo, lo instancia en la escena, lo construye;
    /// Guarda la referencia en el Mapa, añadiendoló a la lista;
    /// Guarda la información en el Tile
    /// Actualiza el estado del mapa;
    /// </summary>
    private void CreateEnemy()
    {
        //Construimos el GameObject Enemy
        GameObject enemyGO = Instantiate(GameManager.Instance.EnemyPrefab, new Vector3(Pos.X * GameManager.DISTANCE, -Pos.Y * GameManager.DISTANCE, 0.0f), Quaternion.identity);
        Enemy enemy = enemyGO.GetComponent<Enemy>();
        enemy.BuildUnit(Pos);

        //Guardamos la referencia en el Mapa
        Map.Instance.Enemies.Add(enemy);

        //Guardamos la información en el tileD:\UNI\TERCER CURSO\IA\PRÁCTICA 4\IAVP04G01\Assets\Scripts\Componentes\Map.cs
        NumEnemies++;

        Map.Instance.OnMapChange();
    }


    /// <summary>
    /// Método para eliminar un enemigo del mapa en el estado de SETMAP
    /// Se usa además cuando un aliado muere durante un combate
    /// </summary>
    public void DeleteAlly()
    {
        //Borramos la información en el tile
        AllyIsHere = false;

        List<Unit> allies = Map.Instance.Allies;

        //Eliminamos de la lista al aliado
        Unit ally = allies.First(s => s.Pos.Equals(this.Pos));
        allies.Remove(ally);

        //Destruimos el objeto
        Destroy(ally.gameObject);

        Map.Instance.OnMapChange();
    }

    /// <summary>
    /// Método para eliminar un enemigo del mapa en el estado de SETMAP
    /// Se llama desde héroe a la hora de destruirse
    /// </summary>
    public void DeleteEnemy()
    {
        //Borramos la información en el tile
        NumEnemies--;

        List<Enemy> enemies = Map.Instance.Enemies;

        //Eliminamos de la lista al enemigo
        Enemy enemy = enemies.First(s => s.Pos.Equals(this.Pos));
        enemies.Remove(enemy);

        //Destruimos el objeto
        Destroy(enemy.gameObject);

        Map.Instance.OnMapChange();

    }

}