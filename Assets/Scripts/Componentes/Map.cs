using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Guarda toda la información del mapa: Tiles, Unidades y Luz
/// </summary>
public class Map : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static Map Instance = null;

    //--------------INSPECTOR--------------------

    /// <summary>
    /// Referencia a la luz ambiental
    /// </summary>
    public Light AmbientLight;

    //Textos de estadísticas
    public Text NumAlliesText;
    public Text NumEnemiesText;
    public Text EnemyProbText;
    public Text ScoreText;
    public Text HeroAliveText;
    public Text AlliesDeadText;
    public Text EnemiesKilledByAlliesText;
    public Text EnemiesKilledByHeroText;

    //Estadísticas de las probabilidades
    public Text SituationText;
    public Text SkillText;
    public Text ActionDistributionText;
    public Text DecisionTaken;


    public GameObject StatsPanel;
    //--------------INSPECTOR--------------------

    //--------------PROPERTIES--------------------
    /// <summary>
    /// Representación del tablero
    /// </summary>
    public Tile[,] Matrix { get; set; }

    /// <summary>
    /// Posición del refugio
    /// </summary>
    public Pos Refuge { get; set; }

    //Unidades
    public Hero Hero { get; set; }
    public List<Unit> Allies { get; set; }
    public List<Enemy> Enemies { get; set; }

    /// <summary>
    /// Probabilidad de victoria por parte de los aliados
    /// </summary>
    public float WinRate { get; set; }

    /// <summary>
    /// Puntuación total de la partida
    /// </summary>
    public int Score { get; set; }

    public int NumAlliesDead { get; set; }

    public int NumEnemiesKilledByHero { get; set; }

    public int NumEnemiesKilledByAllies { get; set; }

    /// <summary>
    /// Devuelve si la luz está encendida y permite apagar y encender la luces
    /// Actualiza la probabilidad cada vez que se hace un set
    /// </summary>
    public bool LightOn
    {
        get
        {
            return lightOn;
        }
        set
        {
            lightOn = value;
            AmbientLight.enabled = value;

            if (Hero != null)
                Hero.gameObject.GetComponentInChildren<Light>().enabled = !value;

            OnMapChange();
        }
    }


    //--------------PROPERTIES--------------------

    //--------------ATRIBUTOS--------------------

    private bool lightOn;

    //--------------ATRIBUTOS--------------------

    void Awake()
    {
        Instance = this;

        StatsPanel.SetActive(false);
    }

    void Start()
    {
        //Se genera el tablero
        BuildMap();
        Score = 0;
        NumAlliesDead = NumEnemiesKilledByHero = NumEnemiesKilledByAllies = 0;
    }


    /// <summary>
    /// Inicializa la matriz a Tiles genéricos
    /// </summary>
    public void BuildMap()
    {
        lightOn = true;
        Refuge = new Pos(0, 5);

        Matrix = new Tile[GameManager.HEIGHT, GameManager.WIDTH];

        //Se inicializan todas las casillas
        for (int y = 0; y < GameManager.HEIGHT; y++)
        {
            for (int x = 0; x < GameManager.WIDTH; x++)
            {
                GameObject tileGO = Instantiate(GameManager.Instance.TilePrefab, new Vector3(x * GameManager.DISTANCE, -y * GameManager.DISTANCE, 0), Quaternion.identity, this.transform);
                Tile tile = tileGO.GetComponent<Tile>();
                tile.BuildTile(new Pos(x, y));
                Matrix[y, x] = tile;
            }
        }

        Allies = new List<Unit>();
        Enemies = new List<Enemy>();

        WinRate = 0.0f;
    }


    /// <summary>
    /// Corrutina que llama a avanzar un paso de todos los barcos
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnTurn()
    {
        while (GameManager.Instance.State == SceneState.PLAY)
        {
            for (int i = 0; i < Enemies.Count;)
            {
                Enemies[i].NextStep();

                if (Enemies[i].DestroyThisEnemy)
                {
                    Enemy enemyAux = Enemies[i];
                    //Eliminamos al enemigo de la lista de Enemigos
                    Enemies.Remove(enemyAux);

                    Destroy(enemyAux.gameObject);

                    OnMapChange();
                }
                else
                    i++;

            }

            if (Hero != null)
                Hero.NextStep();

            if (Hero == null || (Hero != null && Hero.Pos.Equals(Refuge)))
                GameManager.Instance.GameOver();

            yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// Es llamado cada vez que se añade o elimina una unidad en el mapa
    /// </summary>
    public void OnMapChange()
    {
        UpdateProbability();

        UpdateTextStats();
    }

    /// <summary>
    /// Actualiza la probabilidad de vencer de los enemigos
    /// </summary>
    private void UpdateProbability()
    {
        if (Allies.Count >= 2)
            WinRate = GameManager.PROBTHREEORMOREALLIES;

        else if (Allies.Count == 1)
            WinRate = GameManager.PROBTWOALLIES;

        //Solo está el heroe
        else
            WinRate = GameManager.PROBONEALLY;

        if (!LightOn)
            WinRate += GameManager.PROBNOLIGHT;

    }

    /// <summary>
    /// Actualiza el texto de las estadísticas
    /// </summary>
    private void UpdateTextStats()
    {
        //Aliados
        int numAllies = Allies.Count;

        if (Hero != null)
            numAllies++;

        NumAlliesText.text = "Aliados: " + numAllies;

        //Enemigos
        NumEnemiesText.text = "Enemigos: " + Enemies.Count;

        //Probabilidad de que ganen los enemigos
        EnemyProbText.text = "Probabilidad de ganar los enemigos al entrar en combate: " + (1.0f - WinRate) * 100 + "%";

        //Puntuación
        ScoreText.text = "Puntuación: " + Score;

        //Héroe vivo
        HeroAliveText.text = "Héroe vivo: " + (Hero != null);

        //Número de aliados muertos
        AlliesDeadText.text = "Aliados muertos: " + NumAlliesDead;

        //Número de enemigos matados por los aliados
        EnemiesKilledByAlliesText.text = "Enemigos matados por los aliados: " + NumEnemiesKilledByAllies;

        //Número de enemigos matados por el héroe
        EnemiesKilledByHeroText.text = "Enemigos matados por el héroe: " + NumEnemiesKilledByHero;

    }

    /// <summary>
    /// Método que actualiza las probabilidades
    /// Se actualiza cada turno
    /// </summary>
    public void UpdateProbabilities(string situation, string skill, double goForward, double goBack, double wait, string decisionTaken)
    {
        //Estadísticas situación
        SituationText.text = "Situación: " + situation;

        //Estadísticas skill
        SkillText.text = "Destreza: " + skill;

        //Distribuciones de las acciones
        ActionDistributionText.text = string.Format("Avanzar: {0}\t\tRetroceder: {1}\nEsperar: {2}",
            goForward, goBack, wait);

        //Acción elegida
        DecisionTaken.text = "El héroe ha decidido " + decisionTaken;

    }

    /// <summary>
    /// Es llamado cuando se pulsa el botón de apagar/encender la luz
    /// Apaga y enciende la luz correspondiente y actualiza la probabilidad cada vez que es llamado
    /// </summary>
    public void OnOffLight()
    {
        LightOn = !LightOn;
    }




    //-------------------------------------------------------------- RED BAYESIANA -------------------------------------------------------------

    /// <summary>
    /// Devuelve un String con la cantidad de enemigos
    /// </summary>
    /// <returns></returns>
    public string GetEnemyAmount()
    {
        string result;

        if (Enemies.Count == 0)
            result = "none";
        else if (Enemies.Count < 5)
            result = "few";
        else
            result = "many";

        return result;
    }

    /// <summary>
    /// Devuelve un String con la cantidad de enemigos
    /// </summary>
    /// <returns></returns>
    public string GetAlliesAmount()
    {
        string result;

        if (Allies.Count == 0)
            result = "none";
        else if (Allies.Count == 1)
            result = "one";
        else
            result = "more";

        return result;
    }

    /// <summary>
    /// Devuelve un string con "yes" o "no" dependiendo si está activada la luz
    /// </summary>
    /// <returns></returns>
    public string GetLight()
    {
        if (LightOn)
            return "yes";
        else
            return "no";
    }
    //-------------------------------------------------------------- RED BAYESIANA -------------------------------------------------------------

}


