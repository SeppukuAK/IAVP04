using System;

/// <summary>
/// Guarda toda la representación lógica de la matriz de Tiles
/// Inicializa la matriz, coloca el cadaver y los agujeros
/// </summary>
public class Board
{
    public int NumAllies;
    public int NumEnemies;

    //Representación del tablero
    public Tile[,] Matrix;

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

        NumAllies = NumEnemies = 0;
    }

    /*
    public void SetHero(int x, int y)
    {
        Matrix[y, x].Hero = true;
        GameManager.Instance.SetSpriteHero(x, y);
    }
    */
}


