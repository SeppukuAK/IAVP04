using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Guarda toda la representación lógica de la matriz de Tiles
/// Inicializa la matriz, coloca el cadaver y los agujeros
/// </summary>
public class Map : MonoBehaviour
{
    /// <summary>
    /// Posición del refugio
    /// </summary>
    public Pos Refuge { get; set; }

    /// <summary>
    /// Referencia al Heroe
    /// </summary>
    public Hero Hero { get; set; }

    public Dictionary<Pos,Ally> Allies { get; set; }

    public Dictionary<Pos,Enemy> Enemies { get; set; }

    public bool LightOn { get; set; }

    /// <summary>
    /// Representación del tablero
    /// </summary>
    public Tile[,] Matrix { get; set; }

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

            }
        }

        Allies = new Dictionary<Pos, Ally>();
        Enemies = new Dictionary<Pos, Enemy>();
    }

}


