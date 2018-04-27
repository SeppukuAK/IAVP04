using UnityEngine;

public class Ally : MonoBehaviour
{
    /// <summary>
    /// Tile en el que se encuentra el aliado
    /// </summary>
    public Tile Tile { get; set; }

    public void BuildAlly(Tile tile)
    {
        Tile = tile;
    }

    public virtual void NextStep()
    {

    }
}
