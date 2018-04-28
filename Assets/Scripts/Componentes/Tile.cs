using UnityEngine;
using System.Collections.Generic;



/// <summary>
/// Representación lógica de las casillas. Guardan toda la información contenida en ella
/// Componente asociado a los gameObject Tile, guarda la representación lógica de estos e informa al GameManager cuando son pulsados con el ratón
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

    public void BuildTile(Pos pos)
    {
        Pos = pos;
        NumEnemies = 0;
        HeroIsHere = AllyIsHere = false;
    }

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
                    //Distinguir entre que tengo que poner
                    if (!HeroIsHere)
                    {
                        //Si no hay nada 
                        if (!AllyIsHere && NumEnemies == 0)
                        {
                            //hay menos de 5 aliados, pongo un aliado
                            if (Map.Instance.Allies.Count < GameManager.MAXALLIES)
                                CreateAlly();

                            //hay más de 5 aliados, y menos de 20 enemigos, pongo un enemigo
                            else if (Map.Instance.Enemies.Count < GameManager.MAXENEMIES)
                                CreateEnemy();

                            //No gano nada si hay 20 enemigos y 5 aliados

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
    /// Crea el Heroe, lo instancia en la escena, lo construye
    /// Guarda la referencia en el GameManager y guarda su posición en la lista de Boats
    /// </summary>
    private void CreateHero()
    {
        //Construimos el GameObject Hero
        GameObject heroGO = Instantiate(GameManager.Instance.HeroPrefab, new Vector3(Pos.X * GameManager.DISTANCE, -Pos.Y * GameManager.DISTANCE, 0.0f), Quaternion.identity);
        Hero hero = heroGO.GetComponent<Hero>();
        hero.BuildUnit(Pos);

        //Guardamos la referencia en GameManger
        Map.Instance.Hero = hero;

        //Cambiamos de estado
        GameManager.Instance.State = SceneState.SETMAP;
        GameManager.Instance.ButtonPlay.gameObject.SetActive(true);

        //Guardamos la información en el tile
        HeroIsHere = true;
    }

    private void CreateAlly()
    {
        //Construimos el GameObject Ally
        GameObject allyGO = Instantiate(GameManager.Instance.AllyPrefab, new Vector3(Pos.X * GameManager.DISTANCE, -Pos.Y * GameManager.DISTANCE, 0.0f), Quaternion.identity);
        Ally ally = allyGO.GetComponent<Ally>();
        ally.BuildUnit(Pos);

        //Guardamos la referencia en GameManger
        Map.Instance.Allies.Add(ally);

        //Guardamos la información en el tile
        AllyIsHere = true;
    }

    private void CreateEnemy()
    {
        //Construimos el GameObject Ally
        GameObject enemyGO = Instantiate(GameManager.Instance.EnemyPrefab, new Vector3(Pos.X * GameManager.DISTANCE, -Pos.Y * GameManager.DISTANCE, 0.0f), Quaternion.identity);
        Enemy enemy = enemyGO.GetComponent<Enemy>();
        enemy.BuildUnit(Pos);

        //Guardamos la referencia en GameManger
        Map.Instance.Enemies.Add(enemy);

        //Guardamos la información en el tile
        NumEnemies++;
    }
    /// <summary>
    /// Método para eliminar un enemigo del mapa en el estado de SETMAP
    /// Se usa además cuando un aliado muere durante un combate
    /// </summary>
    public void DeleteAlly()
    {
        AllyIsHere = false;
        bool allyFound = false;
        List<Ally> allies = Map.Instance.Allies;

        int i = 0;

        //Encontrar el aliado en la lista del GameManager y borrarlo
        while(i < allies.Count && !allyFound)
        {
            if(allies[i].Pos == this.Pos)
            {
                allyFound = true;
                Ally allyAux = allies[i];

                allies.RemoveAt(i);
                //Destruir la instancia
                Destroy(allyAux.gameObject);
            }
            i++;
        }             
        
    }

    /// <summary>
    /// Método para eliminar un enemigo del mapa en el estado de SETMAP
    /// </summary>
    private void DeleteEnemy()
    {
        //Borramos la información en el tile
        NumEnemies--;

        bool enemyFound = false;
        List<Enemy> enemies = Map.Instance.Enemies;

        int i = 0;

        //Encontrar el aliado en la lista del GameManager y borrarlo
        while (i < enemies.Count && !enemyFound)
        {
            if (enemies[i].Pos == this.Pos)
            {
                enemyFound = true;
                Enemy enemyAux = enemies[i];
                enemies.RemoveAt(i);
                //Destruir la instancia
                Destroy(enemyAux.gameObject);
            }
            i++;
        }
    }

}