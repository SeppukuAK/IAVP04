
public class Enemy : Unit
{

    public override void NextStep()
    {
        Ally ally = GetNearestAlly();

        //Eliminar el enemigo guardado en Tile

        //Calcular la siguiente mejor posición
        Pos nextPos = NextBestPos(ally.Pos);

        //Moverme al siguiente tile
    }

    //TODO: SE PUEDE HACER ALEATORIO
    public Pos NextBestPos(Pos allyPos)
    {
        Pos enemyPos = Pos;
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

    /// <summary>
    /// Devuelve el aliado más cercano a un enemigo
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public Ally GetNearestAlly()
    {
        Ally nearestAlly = null;
        int minDistance = GameManager.WIDTH + GameManager.HEIGHT;

        foreach (Ally ally in GameManager.Instance.Map.Allies.Values)
        {
            int distance = this.Pos.ManhattanDistance(ally.Pos);
            if (distance < minDistance)
            {
                nearestAlly = ally;
                minDistance = distance;
            }

        }
        return nearestAlly;
    }
}
