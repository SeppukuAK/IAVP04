using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jackyjjc.Bayesianet;

/// <summary>
/// Enumerado que controla los distintos estados del juego
/// </summary>
public enum SceneState { SETHERO, SETALLY, SETENEMY,NULL }

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
    private Board board;

    /// <summary>
    /// Matriz de GO tiles
    /// </summary>
    private GameObject[,] tileMatrix { get; set; }
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

        board = new Board();

        //Se genera el tablero
        SetSceneBoard();

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

                Tile tileAux = board.Matrix[y, x];

                //Construimos la casilla
                tileMatrix[y, x].GetComponent<TileView>().BuildTile(tileAux);
            }

        }

    }

    /*
    public void SetHero(Pos pos)
    {
        //Añadimos la posicion del heroe a la matriz lógica
        board.SetHero(pos.X, pos.Y);

        //Instanciamos el héroe


    }

    public void SetAlly()
    {

    }
    public void SetEnemy()
    {

    }
    */
}
