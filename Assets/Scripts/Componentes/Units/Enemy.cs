using UnityEngine;
using DG.Tweening;

/// <summary>
/// Hijo de unidad
/// </summary>
public class Enemy : Unit
{

    public bool DestroyThisEnemy { get; set; }

    /// <summary>
    /// Constructura de la Unidad. Asigna su posición lógica
    /// </summary>
    /// <param name="pos"></param>
    public override void BuildUnit(Pos pos)
    {
        Pos = pos;
        DestroyThisEnemy = false;
    }

    /// <summary>
    /// Es llamdo desde OnTurn() en cada Turno
    /// </summary>
    public override void NextStep()
    {
        Unit ally = GetNearestAlly();

        //Si no hay ningún aliado no hago nada
        if (ally != null)
        {
            //Calcular la siguiente mejor posición
            Pos nextPos = NextBestPos(ally.Pos);

            //TODO: COMPROBAR SI SE PUEDE QUITAR NEXTPOS!=NULL
            //Caso en el que el barco enemigo no coincide con con un aliado

            //Decrementar el número de enemigos del mapa
            Map.Instance.Matrix[Pos.Y, Pos.X].NumEnemies--;

            //Movemos al enemigo lógicamente
            Pos = nextPos;

            //En el nuevo tile, añadimos la "referencia" del nuevo enemigo
            Map.Instance.Matrix[Pos.Y, Pos.X].NumEnemies++;

            //Mover al enemigo        
            transform.DOMove(new Vector3(Pos.X * GameManager.DISTANCE, -Pos.Y * GameManager.DISTANCE, 0), 0.2f);

            CheckBattle();

        }

    }

    /// <summary>
    /// Devuelve el aliado más cercano a un enemigo
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    public Unit GetNearestAlly()
    {
        Unit nearestAlly = null;
        int minDistance = GameManager.WIDTH + GameManager.HEIGHT;

        //TODO: LINQ
        foreach (Unit ally in Map.Instance.Allies)
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
    /// Comprueba si hay batalla y la realiza
    /// </summary>
    private void CheckBattle()
    {
        Tile tile = Map.Instance.Matrix[Pos.Y, Pos.X];

        //Comprobamos si en la casilla en la que se mueve el enemigo hay un aliado o el héroe

        //En el caso en que coinciden aliado y héroe, primero comprobamos si ha chocado con el aliado
        if (tile.AllyIsHere)
        {
            //Generamos el random entre 1 y 100
            int random = rnd.Next(1, 101);

            bool alliesWin = (random <= Map.Instance.WinRate * 100);

            //El enemigo muere
            if (alliesWin)
            {
                //Borramos la información en el tile
                tile.NumEnemies--;

                //Modificamos la puntuación 
                Map.Instance.Score += GameManager.POINTSENEMYDEADBYALLY;

                DestroyThisEnemy = true;

                //Se aumenta el número de enemigos matados por los aliados
                Map.Instance.NumEnemiesKilledByAllies++;

            }

            //El aliado muere
            else
            {
                //Destruir al aliade
                tile.DeleteAlly();

                //Modificamos la puntuación 
                Map.Instance.Score += GameManager.POINTSALLYDEAD;

                //Se actualiza la probabilidad al disminuir el número de aliados 
                Map.Instance.OnMapChange();

                //Se aumenta el número de aliados muertos
                Map.Instance.NumAlliesDead++;

            }

        }

        //Caso en el que se ha chocado con el héroe y el enemigo no ha muerto aún
        if (tile.HeroIsHere && !DestroyThisEnemy)
        {
            //Generamos el random entre 1 y 100
            int random = rnd.Next(1, 101);

            bool alliesWin = (random <= Map.Instance.WinRate * 100);

            //El enemigo muere
            if (alliesWin)
            {
                //Borramos la información en el tile
                tile.NumEnemies--;

                //Modificamos la puntuación 
                Map.Instance.Score += GameManager.POINTSENEMYDEADBYHERO;

                DestroyThisEnemy = true;

                //Se aumenta el número de enemigos matados por el héroe
                Map.Instance.NumEnemiesKilledByHero++;
            }
            //El héroe muere
            else
            {
                //Se elimina la referencia del héroe en el tile en el que estaba
                tile.HeroIsHere = false;

                //Se destruye el game Object
                Destroy(Map.Instance.Hero.gameObject);
                Map.Instance.Hero = null;

                //Modificamos la puntuación 
                Map.Instance.Score += GameManager.POINTSHERODEAD;

                //Se actualiza la probabilidad al disminuir el número de aliados 
                Map.Instance.OnMapChange();
            }
        }
    }
}
