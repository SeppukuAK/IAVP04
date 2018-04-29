using UnityEngine;
using DG.Tweening;

/// <summary>
/// Hijo de unidad
/// </summary>
public class Enemy : Unit
{
    /// <summary>
    /// Generador de números random
    /// Es utilizado para moverse aleatoriamente y calcular probabilidades
    /// </summary>
    static System.Random rnd = new System.Random();

    /// <summary>
    /// Es llamdo desde OnTurn() en cada Turno
    /// </summary>
    public override void NextStep()
    {
        Ally ally = GetNearestAlly();

        //Si no hay ningún aliado no hago nada
        if (ally != null)
        {
            //Calcular la siguiente mejor posición
            Pos nextPos = NextBestPos(ally.Pos);

            //Caso en el que el barco enemigo no coincide con con un aliado
            if (nextPos != null)
            {
                //Decrementar el número de enemigos del mapa
                Map.Instance.Matrix[Pos.Y, Pos.X].NumEnemies--;

                //Movemos al enemigo lógicamente
                Pos = nextPos;

                //En el nuevo tile, añadimos la "referencia" del nuevo enemigo
                Map.Instance.Matrix[Pos.Y, Pos.X].NumEnemies++;

                //Mover al enemigo        
                transform.DOMove(new Vector3(Pos.X * GameManager.DISTANCE, -Pos.Y * GameManager.DISTANCE, 0), 0.2f);
                //transform.position = new Vector3(Pos.X * GameManager.DISTANCE, -Pos.Y * GameManager.DISTANCE, 0);
                CheckBattle();
            }
        }

    }

    /// <summary>
    /// Devuelve el aliado más cercano a un enemigo
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public Ally GetNearestAlly()
    {
        Ally nearestAlly = null;
        int minDistance = GameManager.WIDTH + GameManager.HEIGHT;

        //TODO: LINQ
        foreach (Ally ally in Map.Instance.Allies)
        {
            int distance = this.Pos.ManhattanDistance(ally.Pos);
            if (distance < minDistance)
            {
                nearestAlly = ally;
                minDistance = distance;
            }

        }

        //Si está el heroe y es el más cercano, se dirige hacia él
        if (Map.Instance.Hero != null)//TODO: COMPROBAR QUE ESTO ES CORRECTO
        {
            int distanceToHero = Map.Instance.Hero.Pos.ManhattanDistance(this.Pos);

            if (distanceToHero <= minDistance)
                nearestAlly = Map.Instance.Hero;
        }

        return nearestAlly;
    }


    /// <summary>
    /// Devuelve la siguiente posición a la que tiene que moverse el enemigo
    /// </summary>
    /// <param name="allyPos"></param>
    /// <returns></returns>
    private Pos NextBestPos(Pos allyPos)
    {
        Pos nextPos = null;

        //Generamos el random entre 0 y 1
        int random = rnd.Next(0, 2);

        //Caso en el que el movimiento ha salido horizontal
        if (random == 0)
        {
            nextPos = HorizontalMovement(allyPos);

            //Si no he podido moverme horizontalmente, me muevo verticalmente
            if (nextPos == null)
                nextPos = VerticalMovement(allyPos);

        }

        //Caso en el que el movimiento ha salido vertical
        else
        {
            nextPos = VerticalMovement(allyPos);

            //Si no he podido moverme verticalmente, me muevo horizontalmente
            if (nextPos == null)
                nextPos = HorizontalMovement(allyPos);

        }
        return nextPos;
    }

    /// <summary>
    ///  Método que devuelve la posición más cercana dependiendo de si el aliado está por encima o por debajo
    ///  Si ya está en la posición correspondiente, devuelvo null
    /// </summary>
    /// <param name="allyPos"></param>
    /// <returns></returns>
    private Pos VerticalMovement(Pos allyPos)
    {
        Pos nextPos = null;

        //El aliado está arriba
        if (Pos.Y < allyPos.Y)
            nextPos = new Pos(Pos.X, Pos.Y + 1);

        //El aliado está abajo
        else if (Pos.Y > allyPos.Y)
            nextPos = new Pos(Pos.X, Pos.Y - 1);

        return nextPos;
    }

    /// <summary>
    ///  Método que devuelve la mejor posición dependiendo de si el aliado está a la izquierda o a la derecha
    ///  Si ya está en la posición correspondiente, devuelvo null
    /// </summary>
    /// <param name="allyPos"></param>
    /// <returns></returns>
    private Pos HorizontalMovement(Pos allyPos)
    {
        Pos nextPos = null;

        //El aliado está a la derecha
        if (Pos.X < allyPos.X)
            nextPos = new Pos(Pos.X + 1, Pos.Y);

        //El aliado está a la izquierda
        else if (Pos.X > allyPos.X)
            nextPos = new Pos(Pos.X - 1, Pos.Y);

        return nextPos;
    }

    /// <summary>
    /// Compruba si hay batalla y la realiza
    /// </summary>
    private void CheckBattle()
    {
        Tile tile = Map.Instance.Matrix[Pos.Y, Pos.X];

        //Comprobamos si en la casilla en la que se mueve el enemigo hay un aliado o el héroe
        if (tile.AllyIsHere || tile.HeroIsHere)
        {
            //Generamos el random entre 1 y 100
            int random = rnd.Next(1, 101);

            bool alliesWin = (random <= Map.Instance.WinRate * 100);

            //El enemigo muere
            if (alliesWin)
            {
                //Borramos la información en el tile
                tile.NumEnemies--;

                //Eliminamos al enemigo de la lista de Enemigos
                Map.Instance.Enemies.Remove(this);

                Map.Instance.OnMapChange();

                StopAllCoroutines();
                Destroy(this.gameObject);
            }

            //El aliado muere
            else
            {
                //Se destruye al aliado
                if (tile.AllyIsHere)
                {
                    //Destruir al aliade
                    tile.DeleteAlly();

                    //Se actualiza la probabilidad al disminuir el número de aliados :(
                    Map.Instance.OnMapChange();
                }
                //Se destruye al héroe
                else if (tile.HeroIsHere)
                {
                    //Se elimina la referencia del héroe en el tile en el que estaba
                    tile.HeroIsHere = false;

                    //Se destruye el game Object
                    Destroy(Map.Instance.Hero.gameObject);
                    Map.Instance.Hero = null;

                    GameManager.Instance.GameOver();
                }
            }

        }


    }



}
