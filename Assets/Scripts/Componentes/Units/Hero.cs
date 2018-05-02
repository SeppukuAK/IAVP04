using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Jackyjjc.Bayesianet;


/// <summary>
/// Hijo de Aliado
/// </summary>
public class Hero : Unit
{
    public enum HeroState { GOFORWARD, GOBACK, WAIT }

    private HeroState heroState;
    /// <summary>
    /// Constructura de la Unidad. Asigna su posición lógica
    /// </summary>
    /// <param name="pos"></param>
    public override void BuildUnit(Pos pos)
    {
        Pos = pos;
        //Empieza en el estado de espera
        heroState = HeroState.WAIT;
    }

    public override void NextStep()
    {
        MakeDecision();

        switch (heroState)
        {
            case HeroState.GOFORWARD:
                GoForward();
                break;

            case HeroState.GOBACK:
                GoBack();
                break;

        }

        
    }

    private void MakeDecision()
    {
        VariableElimination ve = GameManager.Instance.VariableElimination;

        // Observaciones Situación
        List<string> observationsSituation = new List<string> {
            "enemies=" + Map.Instance.GetEnemyAmount(),
            "allies=" + Map.Instance.GetAlliesAmount()          
        };

        // Distribucion de la situacion
        double[] situationDistribution = ve.Infer("situation", observationsSituation);

        string situation;
        
        switch (ve.PickOne(situationDistribution))
        {
            case 0:
                situation = "many_enemies";
                break;
            case 1:
                situation = "many_allies";
                break;
            default:
                situation = "neutral";
                break;
        }

        // Observaciones Destreza
        List<string> observationsSkill = new List<string> {
            "light=" + Map.Instance.GetLight(),
            "allies=" + Map.Instance.GetAlliesAmount()
        };


        //Distribución de la destreza
        double[] skillDistribution = ve.Infer("skill", observationsSkill);

        string skill;

        switch (ve.PickOne(skillDistribution))
        {
            case 0:
                skill = "good";
                break;
            case 1:
                skill = "regular";
                break;
            default:
                skill = "bad";
                break;
        }

        // Observaciones Destreza
        List<string> observationsState = new List<string> {
            "situation=" + situation,
            "skill=" + skill,
            "action=go_forward"
        };

        //Se infiere el estado de avance
        double[] goForwardDist = ve.Infer("utility", observationsState);

        observationsState[2] = "action=go_back";

        //Calculamos la distribución
        double[] goBackDist = ve.Infer("utility", observationsState);

        //Cambiamos el estado a esperar
        observationsState[2] = "action=wait";


        // Calculamos la distribución
        double[] waitDist = ve.Infer("utility", observationsState);

        double goForward = goForwardDist[0] * 1.0 + goForwardDist[1] * 0.7 + goForwardDist[2] * 0.0;
        double goBack = goBackDist[0] * 1.0 + goBackDist[1] * 0.7 + goBackDist[2] * 0.0;
        double wait = waitDist[0] * 1.0 + waitDist[1] * 0.7 + waitDist[2] * 0.0;

        //Hallamos el máximo entre las utilidades
        double decision = System.Math.Max(goForward, goBack);
        decision = System.Math.Max(decision, wait);

        string decisionTaken;//string para el canvas

        if (goForward == decision)
        {
            heroState = HeroState.GOFORWARD;
            decisionTaken = "Avanzar";
        }
        else if (goBack == decision)
        {
            heroState = HeroState.GOBACK;
            decisionTaken = "Retroceder";
        }
        else
        {
            heroState = HeroState.WAIT;
            decisionTaken = "Esperar";
        }
           
        //Se actualizan las probabilidades
        Map.Instance.UpdateProbabilities(situation,skill, goForward, goBack, wait, decisionTaken);
    }
    //----------------------------------- MÉTODOS DEL ESTADO GOFORWARD -----------------
    private void GoForward()
    {
        Enemy enemy = GetNearestEnemy();

        if (enemy != null)
            Move(enemy.Pos);    
    }

    /// <summary>
    /// Método que devuelve el enemigo más cercano al héroe
    /// </summary>
    /// <returns></returns>
    private Enemy GetNearestEnemy()
    {
        Enemy nearestEnemy = null;
        int minDistance = GameManager.WIDTH + GameManager.HEIGHT;

        //TODO: LINQ
        foreach (Enemy enemy in Map.Instance.Enemies)
        {
            int distance = this.Pos.ManhattanDistance(enemy.Pos);
            if (distance < minDistance)
            {
                nearestEnemy = enemy;
                minDistance = distance;
            }

        }
        return nearestEnemy;
    }

    //----------------------------------- MÉTODOS DEL ESTADO GOFORWARD -----------------

    private void GoBack()
    {
        //Obtenemos la posición del refugio
        Pos pos = Map.Instance.Refuge;
        Move(pos);
    }

    /// <summary>
    /// Método que se encarga de mover de forma física y lógica al héroe
    /// Comprueba si debe combatir y en ese caso combate
    /// </summary>
    /// <param name="pos"></param>
    private void Move(Pos pos)
    {
        //Calcular la siguiente mejor posición
        Pos nextPos = NextBestPos(pos);

        //Decrementar el número de enemigos del mapa
        Map.Instance.Matrix[Pos.Y, Pos.X].HeroIsHere = false;

        //Movemos al héroe lógicamente
        Pos = nextPos;

        //En el nuevo tile, añadimos la "referencia" del nuevo enemigo
        Map.Instance.Matrix[Pos.Y, Pos.X].HeroIsHere = true;

        //Mover al enemigo        
        transform.DOMove(new Vector3(Pos.X * GameManager.DISTANCE, -Pos.Y * GameManager.DISTANCE, 0), 0.2f);

        CheckBattle();
    }
    /// <summary>
    /// Comprueba si hay batalla y la realiza
    /// </summary>
    private void CheckBattle()
    {
        Tile tile = Map.Instance.Matrix[Pos.Y, Pos.X];


        while (tile.NumEnemies > 0 && tile.HeroIsHere)
        {
            //Generamos el random entre 1 y 100
            int random = rnd.Next(1, 101);

            //Devuelve si el héroe ha ganado o no
            bool heroWin = (random <= Map.Instance.WinRate * 100);

            //El héroe gana
            if (heroWin)
            {
                //Modificamos la puntuación
                Map.Instance.Score += GameManager.POINTSENEMYDEADBYHERO;

                //Se destruye el enemigo
                tile.DeleteEnemy();

                //Informar al mapa de que ha cambiado
                //Se actualiza la probabilidad al disminuir el número de enemigos
                Map.Instance.OnMapChange();

                //Se aumenta el número de enemigos matados por el héroe
                Map.Instance.NumEnemiesKilledByHero++;
            }
            //El héroe muere
            else
            {
                //Quitamos la referencia del héroe
                tile.HeroIsHere = false;
                Map.Instance.Hero = null;

                //Destruimos el objeto
                Destroy(this.gameObject);

                //Modificamos la puntuación 
                Map.Instance.Score += GameManager.POINTSHERODEAD;

                //Se actualiza la probabilidad al disminuir el número de aliados 
                Map.Instance.OnMapChange();
            }
        }
    }
}
