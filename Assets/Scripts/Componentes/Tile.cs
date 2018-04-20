
/// <summary>
/// Representación lógica de las casillas. Guardan toda la información contenida en ella
/// </summary>
public class Tile
{

    public bool Aliado { get; set; }
    public bool Heroe { get; set; }

    public bool Enemigo { get; set; }

    public Pos Pos { get; set; }

    public Tile(Pos pos)
    {
        Pos = pos;
        Aliado  = Heroe = Enemigo = false;
    }
}
