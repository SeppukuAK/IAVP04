using System;

/// <summary>
/// Guarda toda la representación lógica de la matriz de Tiles
/// Inicializa la matriz, coloca el cadaver y los agujeros
/// </summary>
public class Board
{
    //Representación del tablero
    public Tile[,] Matrix { get; set; }

    /// <summary>
    /// Inicializa la matriz a Tiles genéricos
    /// </summary>
    public Board()
    {
        Matrix = new Tile[GameManager.HEIGHT, GameManager.WIDTH];

        //Se inicializan todas las casillas
        for (int y = 0; y < GameManager.HEIGHT; y++)
        {
            for (int x = 0; x < GameManager.WIDTH; x++)
            {
                Matrix[y, x] = new Tile(new Pos(x, y));

            }
        }
    }

    /*
    public void SetHero(int x, int y)
    {
        Matrix[y, x].Hero = true;
        GameManager.Instance.SetSpriteHero(x, y);
    }
    */
}


