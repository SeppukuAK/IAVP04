using UnityEngine;

/// <summary>
/// Clase base de todas las unidades : Aliado, Enemigo y Heroe
/// </summary>
public class Unit : MonoBehaviour {

    /// <summary>
    /// Tile en el que se encuentra la unidad
    /// </summary>
    public Pos Pos { get; set; }

    /// <summary>
    /// Constructura de la Unidad. Asigna su posición lógica
    /// </summary>
    /// <param name="pos"></param>
    public void BuildUnit(Pos pos)
    {
        Pos = pos;
    }

    /// <summary>
    /// Es llamado en la corrutina una vez empieza el juego
    /// Los hijos lo implementan
    /// </summary>
    public virtual void NextStep()
    {
    }
}
