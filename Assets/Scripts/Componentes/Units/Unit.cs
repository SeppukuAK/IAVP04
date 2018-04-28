using UnityEngine;

public class Unit : MonoBehaviour {

    /// <summary>
    /// Tile en el que se encuentra la unidad
    /// </summary>
    public Pos Pos { get; set; }

    public void BuildUnit(Pos pos)
    {
        Pos = pos;
    }

    public virtual void NextStep()
    {
    }
}
