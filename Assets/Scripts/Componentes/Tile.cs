
/// <summary>
/// Representación lógica de las casillas. Guardan toda la información contenida en ella
/// </summary>
public class Tile
{

    public bool Ally { get; set; }
    public bool Hero { get; set; }
    public bool Enemy { get; set; }

    public Pos Pos { get; set; }

    public Tile(Pos pos)
    {
        Pos = pos;
        Ally = Hero = Enemy = false;
    }
}
