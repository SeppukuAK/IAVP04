using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Guarda toda la representación lógica de la matriz de Tiles
/// Inicializa la matriz, coloca el cadaver y los agujeros
/// </summary>
public class Map : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static Map Instance = null;

    /// <summary>
    /// Posición del refugio
    /// </summary>
    public Pos Refuge { get; set; }

    /// <summary>
    /// Referencia al Heroe
    /// </summary>
    public Hero Hero { get; set; }

    public List<Ally> Allies { get; set; }

    public List<Enemy> Enemies { get; set; }

    public bool LightOn { get; set; }

    /// <summary>
    /// Probabilidad de victoria por parte de los aliados feministas :) mmmmmmmmmhhhhhhhhhhhhhhhggghghhhgughputanmmtequieroooasdmsdmmmmm
    /// </summary>
    public float WinRate { get; set; }


    /// <summary>
    /// Representación del tablero
    /// </summary>
    public Tile[,] Matrix { get; set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        WinRate = 0.0f;

        //Se genera el tablero
        BuildMap();
    }


    /// <summary>
    /// Inicializa la matriz a Tiles genéricos
    /// </summary>
    public void BuildMap()
    {
        LightOn = true;
        Refuge = new Pos(0, 5);

        Matrix = new Tile[GameManager.HEIGHT, GameManager.WIDTH];

        //Se inicializan todas las casillas
        for (int y = 0; y < GameManager.HEIGHT; y++)
        {
            for (int x = 0; x < GameManager.WIDTH; x++)
            {
                GameObject tileGO = Instantiate(GameManager.Instance.TilePrefab, new Vector3(x * GameManager.DISTANCE, -y * GameManager.DISTANCE, 0), Quaternion.identity, this.transform);
                Tile tile = tileGO.GetComponent<Tile>();
                tile.BuildTile(new Pos(x, y));
                Matrix[y, x] = tile;
            }
        }

        Allies = new List<Ally>();
        Enemies = new List<Enemy>();
    }


    /// <summary>
    /// Corrutina que llama a avanzar un paso de todos los barcos
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnTurn()
    {
        while (GameManager.Instance.State == SceneState.PLAY)
        {
            foreach (Enemy enemy in Enemies)
                enemy.NextStep();

            if(Hero!=null)
                Hero.NextStep();

            yield return new WaitForSeconds(1.0f);
        }
    }
    public void UpdateProbability()
    {
        if (Allies.Count >= 3)
            WinRate = GameManager.PROBTHREEORMOREALLIES;

        else if(Allies.Count == 2)
            WinRate = GameManager.PROBTWOALLIES;

        else if(Allies.Count == 1)
            WinRate = GameManager.PROBONEALLY;

        if(!LightOn)
            WinRate += GameManager.PROBNOLIGHT;
    }
}


