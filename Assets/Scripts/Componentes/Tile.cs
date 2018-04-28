using UnityEngine;


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
        if (!GameManager.Instance.Map.Refuge.Equals(Pos))
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
                            if (GameManager.Instance.Map.Allies.Count < GameManager.MAXALLIES)
                                CreateAlly();

                            //hay más de 5 aliados, y menos de 20 enemigos, pongo un enemigo
                            else if (GameManager.Instance.Map.Enemies.Count < GameManager.MAXENEMIES)
                                CreateEnemy();

                            //No gano nada si hay 20 enemigos y 5 aliados
                            
                        }

                        //Hay aliado
                        else if (AllyIsHere)
                        {
                            DeleteAlly();

                            //Hay menos de 20 enemigos, pongo un enemigo
                            if (GameManager.Instance.Map.Enemies.Count < GameManager.MAXENEMIES)
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
        GameManager.Instance.Map.Hero = hero;

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
        GameManager.Instance.Map.Allies.Add(Pos,ally);

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
        GameManager.Instance.Map.Enemies.Add(Pos, enemy);

        //Guardamos la información en el tile
        NumEnemies++;
    }

    private void DeleteAlly()
    {
        //Borramos la información en el tile
        AllyIsHere = false;

        //Encontrar el aliado en la lista del GameManager y borrarlo
        Ally ally = GameManager.Instance.Map.Allies[Pos];
        GameManager.Instance.Map.Allies.Remove(Pos);

        //Destruir la instancia
        Destroy(ally.gameObject);
    }


    private void DeleteEnemy()
    {
        //Borramos la información en el tile
        NumEnemies--;

        //Encontrar el aliado en la lista del GameManager y borrarlo
        Enemy enemy = GameManager.Instance.Map.Enemies[Pos];
        GameManager.Instance.Map.Enemies.Remove(Pos);

        //Destruir la instancia
        Destroy(enemy.gameObject);
    }

}