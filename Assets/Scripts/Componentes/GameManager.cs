using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jackyjjc.Bayesianet;

/// <summary>
/// Enumerado que controla los distintos estados del juego
/// </summary>
public enum SceneState { SETHERO, SETMAP,NULL }

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static GameManager Instance = null;

    //------------------CONSTANTES-------------------

    public const int WIDTH = 12;
    public const int HEIGHT = 6;
    public const float DISTANCE = 0.64f;

    //------------------CONSTANTES-------------------

    //------------------INSPECTOR-------------------
    public GameObject TilePrefab;
    public GameObject HeroPrefab;
    public GameObject AllyPrefab;
    public GameObject EnemyPrefab;

    // private GameObject heroGO;
    //------------------INSPECTOR-------------------

    //------------------PROPIEDADES-------------------
    /// <summary>
    /// Estado actual del juego
    /// </summary>
    public SceneState State { get; set; }


    /// <summary>
    /// Posición del refugio
    /// </summary>
    public Pos Refuge { get; set; }
    //------------------PROPIEDADES-------------------


    //----------------ATRIBUTOS PRIVADOS------------------
    /// <summary>
    /// Tablero
    /// </summary>
    public Board Board;

    /// <summary>
    /// Matriz de GO tiles
    /// </summary>
    private GameObject[,] tileMatrix { get; set; }
    public Dictionary<Pos, List<GameObject>> personajes;
    //----------------ATRIBUTOS PRIVADOS------------------


    private void Awake()
    {
        //GameManager es Singleton
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        State = SceneState.NULL;

        //Referente a la Red Bayesiana
        /*
        BayesianNode node1 = new BayesianNode("node1", new string[] { "value1", "value2" }, new double[] { 0.4, 0.6 });
        BayesianNode node2 = new BayesianNode("node2", new string[] { "v1", "v2","v3" }, new double[] { 0.1,0.2,0.7,0.2, 0.6,0.2 },node1);
        BayesianNetwork network = new BayesianNetwork(node1, node2);
        VariableElimination ve = new VariableElimination(network);

        double[] probability = ve.Infer("node1", "node2=v1");
        Debug.Log("Probability of value1: " + probability[0]);
        Debug.Log("Probability of value2: " + probability[1]);
        */

        Board = new Board();

        //Se genera el tablero
        SetSceneBoard();

        Refuge = new Pos(0, 5);

        State = SceneState.SETHERO;
    }


    /// <summary>
    /// Método que coloca la representación lógica del tablero a la representación física
    /// </summary>
    void SetSceneBoard()
    {
        tileMatrix = new GameObject[HEIGHT, WIDTH];

        GameObject GOBoard = new GameObject("Board");

        for (int y = 0; y < HEIGHT; y++)
        {
            for (int x = 0; x < WIDTH; x++)
            {
                //Creamos los Game Object del tablero
                tileMatrix[y, x] = Instantiate(TilePrefab, new Vector3(x * DISTANCE, -y * DISTANCE, 0), Quaternion.identity, GOBoard.transform);

                Tile tileAux = Board.Matrix[y, x];

                //Construimos la casilla
                tileMatrix[y, x].GetComponent<TileView>().BuildTile(tileAux);
            }

        }

    }

    
    public void CreateHero(Hero hero)
    {
        Pos pos = hero.Tile.Pos;
        GameObject heroGO = Instantiate(HeroPrefab, new Vector3(pos.X* DISTANCE, -pos.Y * DISTANCE, 0.0f), Quaternion.identity);
        
        //No puede haber nada antes del héroe
        List<GameObject> listaPersonajes = new List<GameObject>();
        listaPersonajes.Add(heroGO);
        personajes.Add(pos, listaPersonajes);    
        
        HeroView heroView = heroGO.GetComponent<HeroView>();
        heroView.BuildHero(hero);

        State = SceneState.SETMAP;

        //Añadimos la posicion del heroe a la matriz lógica
        //board.SetHero(pos.X, pos.Y);

        //Instanciamos el héroe
    }

    public void CreateAlly(Ally ally)
    {
        Pos pos = ally.Tile.Pos;
        GameObject allyGO = Instantiate(AllyPrefab, new Vector3(pos.X * DISTANCE, -pos.Y * DISTANCE, 0.0f), Quaternion.identity);

        List<GameObject> listaPersonajes = new List<GameObject>();
        listaPersonajes.Add(allyGO);
        personajes.Add(pos, listaPersonajes);
        AllyView allyView = allyGO.GetComponent<AllyView>();
        allyView.BuildAlly(ally);


    }
    public void CreateEnemy(Enemy enemy)
    {
        Pos pos = enemy.Tile.Pos;
        GameObject heroGO = Instantiate(AllyPrefab, new Vector3(pos.X * DISTANCE, -pos.Y * DISTANCE, 0.0f), Quaternion.identity);

        EnemyView enemyView = heroGO.GetComponent<EnemyView>();
        enemyView.BuildEnemy(enemy);
    }

}
