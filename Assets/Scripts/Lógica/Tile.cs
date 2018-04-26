using System.Collections.Generic;

/// <summary>
/// Representación lógica de las casillas. Guardan toda la información contenida en ella
/// </summary>
public class Tile
{
    public Ally Ally { get; set; }
    public Hero Hero { get; set; }
    public List<Enemy> Enemy { get; set; }
    public Pos Pos { get; set; }

    public Tile(Pos pos)
    {
        Pos = pos;
        Hero = null;
        Ally = null;
        Enemy = new List<Enemy>();
    }
}
