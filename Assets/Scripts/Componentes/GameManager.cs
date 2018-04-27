using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Jackyjjc.Bayesianet;

/// <summary>
/// Enumerado que controla los distintos estados del juego
/// </summary>
public enum SceneState { SETHERO, SETMAP, PLAY, NULL }

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static GameManager Instance = null;

    //------------------CONSTANTES-------------------

    public const float DISTANCE = 0.64f;
    public const int WIDTH = 12;
    public const int HEIGHT = 6;
    public const int MAXENEMIES = 20;
    public const int MAXALLIES = 5;

    //------------------CONSTANTES-------------------

    //------------------INSPECTOR-------------------
    public GameObject TilePrefab;
    public GameObject HeroPrefab;
    public GameObject AllyPrefab;
    public GameObject EnemyPrefab;

    public Light AmbientLight;

    public Button ButtonPlay;

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

    /// <summary>
    /// Referencia al Heroe
    /// </summary>
    public HeroView Hero { get; set; }

    public List<AllyView> Allies { get; set; }

    public List<EnemyView> Enemies { get; set; }

    public bool LightOn { get; set; }

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

    /// <summary>
    /// Diccionario que guarda todas las posiciones ocupadas por barcos y todos los barcos en esas posiciones
    /// </summary>
    public Dictionary<Pos, List<GameObject>> Boats;

    //----------------ATRIBUTOS PRIVADOS------------------

    private void Awake()
    {
        //GameManager es Singleton
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        ButtonPlay.gameObject.SetActive(false);
        State = SceneState.NULL;

        LightOn = true;

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

        Boats = new Dictionary<Pos, List<GameObject>>();
        Enemies = new List<EnemyView>();
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


    public void InitGame()
    {
        State = SceneState.PLAY;

        ButtonPlay.gameObject.SetActive(false);

        StartCoroutine(OnTurn());
    }

    /// <summary>
    /// Corrutina que llama a avanzar un paso de todos los barcos
    /// </summary>
    /// <returns></returns>
    IEnumerator OnTurn()
    {
        while (State == SceneState.PLAY)
        {
            for (int i = 0; i < Enemies.Count; i++)
                Enemies[i].NextStep();

            Hero.NextStep();
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void OnOffLight()
    {

        if (LightOn)
        {
            AmbientLight.enabled = false;
            LightOn = false;

            if (Hero != null)
                Hero.gameObject.GetComponentInChildren<Light>().enabled = true;

        }
        else
        {
            AmbientLight.enabled = true;
            LightOn = true;

            if (Hero != null)
                Hero.gameObject.GetComponentInChildren<Light>().enabled = false;
        }

    }

    /// <summary>
    /// Devuelve el aliado más cercano a un enemigo
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public Ally GetNearestAlly(Enemy enemy)
    {
        Ally ally = Allies[0].Ally;
        int minDistance = enemy.Tile.Pos.ManhattanDistance(Allies[0].Ally.Tile.Pos);
        for(int i = 1; i < Allies.Count; i++)
        {
            int distance = enemy.Tile.Pos.ManhattanDistance(Allies[i].Ally.Tile.Pos);
            if (distance < minDistance)
            {
                ally = Allies[i].Ally;
            }
        }

        return ally;
    }

}
