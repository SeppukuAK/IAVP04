using UnityEngine;
using Jackyjjc.Bayesianet;

/// <summary>
/// Enumerado que controla los distintos estados del juego
/// </summary>
public enum SceneState {SETHERO, SETALLY,SETENEMY}

public class GameManager : MonoBehaviour {

    /// <summary>
    /// Singleton
    /// </summary>
    public static GameManager Instance;
    
    /// <summary>
    /// Estado actual del juego
    /// </summary>
    public SceneState State { get; set; }

    /// <summary>
    /// Posición del refugio
    /// </summary>
    public Pos Refuge { get; set; }
    
    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        //Lo primero que se coloca es el heroe
        State = SceneState.SETHERO;

        BayesianNode node1 = new BayesianNode("node1", new string[] { "value1", "value2" }, new double[] { 0.4, 0.6 });
        BayesianNode node2 = new BayesianNode("node2", new string[] { "v1", "v2","v3" }, new double[] { 0.1,0.2,0.7,0.2, 0.6,0.2 },node1);
        BayesianNetwork network = new BayesianNetwork(node1, node2);
        VariableElimination ve = new VariableElimination(network);

        double[] probability = ve.Infer("node1", "node2=v1");
        Debug.Log("Probability of value1: " + probability[0]);
        Debug.Log("Probability of value2: " + probability[1]);

    }

    // Update is called once per frame
    void Update () {
		
	}

    /// <summary>
    /// Método que coloca la representación lógica del tablero a la representación física
    /// </summary>
    public void SetSceneBoard()
    {

    }

    public void SetHero()
    {

    }

    public void SetAlly()
    {

    }
    public void SetEnemy()
    {

    }
}
