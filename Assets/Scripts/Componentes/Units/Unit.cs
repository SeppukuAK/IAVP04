using UnityEngine;

/// <summary>
/// Clase base de todas las unidades : Aliado, Enemigo y Heroe
/// </summary>
public class Unit : MonoBehaviour {

    /// <summary>
    /// Generador de números random
    /// Es utilizado para moverse aleatoriamente y calcular probabilidades
    /// </summary>
    protected static System.Random rnd = new System.Random();

    /// <summary>
    /// Tile en el que se encuentra la unidad
    /// </summary>
    public Pos Pos { get; set; }

    /// <summary>
    /// Constructura de la Unidad. Asigna su posición lógica
    /// </summary>
    /// <param name="pos"></param>
    public virtual void BuildUnit(Pos pos)
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
    /// <summary>
    /// Devuelve la siguiente posición a la que tiene que moverse la unidad
    /// </summary>
    /// <param name="unitPos"></param>
    /// <returns></returns>
    protected Pos NextBestPos(Pos unitPos)
    {
        Pos nextPos = null;

        //Generamos el random entre 0 y 1
        int random = rnd.Next(0, 2);

        //Caso en el que el movimiento ha salido horizontal
        if (random == 0)
        {
            nextPos = HorizontalMovement(unitPos);

            //Si no he podido moverme horizontalmente, me muevo verticalmente
            if (nextPos == null)
                nextPos = VerticalMovement(unitPos);

        }

        //Caso en el que el movimiento ha salido vertical
        else
        {
            nextPos = VerticalMovement(unitPos);

            //Si no he podido moverme verticalmente, me muevo horizontalmente
            if (nextPos == null)
                nextPos = HorizontalMovement(unitPos);

        }
        return nextPos;
    }

    /// <summary>
    ///  Método que devuelve la posición más cercana dependiendo de si el objetivo está por encima o por debajo
    ///  Si ya está en la posición correspondiente, devuelvo null
    /// </summary>
    /// <param name="unitPos"></param>
    /// <returns></returns>
    protected Pos VerticalMovement(Pos unitPos)
    {
        Pos nextPos = null;

        //El objetivo está arriba
        if (Pos.Y < unitPos.Y)
            nextPos = new Pos(Pos.X, Pos.Y + 1);

        //El objetivo está abajo
        else if (Pos.Y > unitPos.Y)
            nextPos = new Pos(Pos.X, Pos.Y - 1);

        return nextPos;
    }

    /// <summary>
    ///  Método que devuelve la mejor posición dependiendo de si el objetivo está a la izquierda o a la derecha
    ///  Si ya está en la posición correspondiente, devuelvo null
    /// </summary>
    /// <param name="unitPos"></param>
    /// <returns></returns>
    protected Pos HorizontalMovement(Pos unitPos)
    {
        Pos nextPos = null;

        //El objetivo está a la derecha
        if (Pos.X < unitPos.X)
            nextPos = new Pos(Pos.X + 1, Pos.Y);

        //El objetivo está a la izquierda
        else if (Pos.X > unitPos.X)
            nextPos = new Pos(Pos.X - 1, Pos.Y);

        return nextPos;
    }
}
