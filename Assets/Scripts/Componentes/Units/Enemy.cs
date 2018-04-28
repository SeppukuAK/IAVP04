using DG.Tweening;
using System;
using UnityEngine;

public class Enemy : Unit
{
    static System.Random rnd = new System.Random();

    public override void NextStep()
    {
        Ally ally = GetNearestAlly();

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
    void CheckBattle()
    {

        Tile tile = Map.Instance.Matrix[Pos.Y, Pos.X];

        //Comprobamos si en la casilla en la que se mueve el enemigo hay un aliado o el héroe
        if (tile.AllyIsHere || tile.HeroIsHere)
        {
            //Generamos el random entre 1 y 100
            int random = rnd.Next(1, 101);

            bool alliesWin = (random <= Map.Instance.WinRate * 100);

            //Caso en el que los aliados ganan
            if (alliesWin)
            {
                //Borramos la información en el tile
                tile.NumEnemies--;

                //Encontrar el enemigo en la lista del GameManager y borrarlo
                Map.Instance.Enemies.Remove(this);

                StopAllCoroutines();
                Destroy(this.gameObject);
            }
            //Caso en el que los aliados pierden
            else
            {
                //Se destruye al aliado
                if (tile.AllyIsHere)
                {
                    //Destruir al aliade
                    tile.DeleteAlly();

                    //Se actualiza la probabilidad al disminuir el número de aliados :(
                    Map.Instance.UpdateProbability();
                }
                //Se destruye al héroe
                else if (tile.HeroIsHere)
                {

                    //Se destruye el game Object
                    Destroy(Map.Instance.Hero.gameObject);
                    Map.Instance.Hero = null;

                    //Se elimina la referencia del héroe en el tile en el que estaba
                    tile.HeroIsHere = false;

                    GameManager.Instance.GameOver();
                }
            }

        }


    }

    //TODO: SE PUEDE HACER ALEATORIO HECHO :) MHHHHHMMMMM
    public Pos NextBestPos(Pos allyPos)
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
    Pos VerticalMovement(Pos allyPos)
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
    Pos HorizontalMovement(Pos allyPos)
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
    /// Devuelve el aliado más cercano a un enemigo
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public Ally GetNearestAlly()
    {
        Ally nearestAlly = null;
        int minDistance = GameManager.WIDTH + GameManager.HEIGHT;

        foreach (Ally ally in Map.Instance.Allies)
        {
            int distance = this.Pos.ManhattanDistance(ally.Pos);
            if (distance < minDistance)
            {
                nearestAlly = ally;
                minDistance = distance;
            }

        }
        if (Map.Instance.Hero != null)
        {
            int distanceToHero = Map.Instance.Hero.Pos.ManhattanDistance(this.Pos);

            if (distanceToHero <= minDistance)
                nearestAlly = Map.Instance.Hero;
        }

        return nearestAlly;
    }
}
