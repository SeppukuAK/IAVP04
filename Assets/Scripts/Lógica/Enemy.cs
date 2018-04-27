using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{

    public Tile Tile;
    public Enemy(Tile tile)
    {
        Tile = tile;
    }


    public void NextStep()
    {
        Ally ally = GameManager.Instance.GetNearestAlly(this);

        //Eliminar el enemigo guardado en Tile

        //Calcular la siguiente mejor posición
        Pos nextPos = NextBestPos(ally.Tile.Pos);

        //Moverme al siguiente tile
    }

    //TODO: SE PUEDE HACER ALEATORIO
    public Pos NextBestPos(Pos allyPos)
    {
        Pos enemyPos = Tile.Pos;
        Pos nextPos = null;

        //El aliado está a la derecha
        if (enemyPos.X < allyPos.X)
            nextPos = new Pos(enemyPos.X + 1, enemyPos.Y);

        //El aliado está a la izquierda
        else if (enemyPos.X > allyPos.X)
            nextPos = new Pos(enemyPos.X - 1, enemyPos.Y);

        //El aliado está arriba
        else if (enemyPos.Y < allyPos.Y)
            nextPos = new Pos(enemyPos.X, enemyPos.Y+1);

        //El aliado está abajo
        else if (enemyPos.Y > allyPos.Y)
            nextPos = new Pos(enemyPos.X + 1, enemyPos.Y-1);

        return nextPos;
    }
}
