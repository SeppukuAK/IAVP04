using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// TODO: UN MEJOR COMENTARIO
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
    public Text EnemyProbText;
    public Text NumEnemiesText;
    public Text NumAlliesText;

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
    public List<Ally> Allies { get; set; }
    public List<Enemy> Enemies { get; set; }

    /// <summary>
    /// Probabilidad de victoria por parte de los aliados feministas :) mmmmmmmmmhhhhhhhhhhhhhhhggghghhhgughputanmmtequieroooasdmsdmmmmm
    /// </summary>
    public float WinRate { get; set; }

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

        Allies = new List<Ally>();
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

            yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// Es llamado cada vez que se añade o elimina una unidad en el mapa
    /// TODO: SOLO SE LE DEBERÍA LLAMAR SI NO ESTAMOS EN GAMEOVER -> COMPROBAR QUE ESTO ES ASI
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
        EnemyProbText.text = "Probabilidad de ganar los enemigos al entrar en combate: " + (1.0f - WinRate) * 100 + "%";
        NumEnemiesText.text = "Enemigos: " + Enemies.Count;

        int numAllies = Allies.Count;
        if (Hero != null)
            numAllies++;

        NumAlliesText.text = "Aliados: " + numAllies;
    }

    /// <summary>
    /// Es llamado cuando se pulsa el botón de apagar/encender la luz
    /// Apaga y enciende la luz correspondiente y actualiza la probabilidad cada vez que es llamado
    /// </summary>
    public void OnOffLight()
    {
        LightOn = !LightOn;
    }

}


