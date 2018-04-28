using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Jackyjjc.Bayesianet;

/// <summary>
/// Enumerado que controla los distintos estados del juego
/// </summary>
public enum SceneState { SETHERO, SETMAP, PLAY, NULL }

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    public static GameManager Instance = null;

    //------------------CONSTANTES-------------------

    public const float DISTANCE = 0.64f;
    public const int WIDTH = 12;
    public const int HEIGHT = 6;
    public const int MAXENEMIES = 20;
    public const int MAXALLIES = 5;

    //------------------CONSTANTES-------------------

    //------------------INSPECTOR-------------------
    public Map Map;

    public GameObject TilePrefab;
    public GameObject HeroPrefab;
    public GameObject AllyPrefab;
    public GameObject EnemyPrefab;

    public Light AmbientLight;

    public Button ButtonPlay;

    //------------------INSPECTOR-------------------

    //------------------PROPIEDADES-------------------
    /// <summary>
    /// Estado actual del juego
    /// </summary>
    public SceneState State { get; set; }

    //------------------PROPIEDADES-------------------


    //----------------ATRIBUTOS PRIVADOS------------------

    //----------------ATRIBUTOS PRIVADOS------------------

    private void Awake()
    {
        //GameManager es Singleton
        Instance = this;

        ButtonPlay.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        State = SceneState.NULL;

        //Se genera el tablero
        Map.BuildMap();

        State = SceneState.SETHERO;
    }

    public void InitGame()
    {
        State = SceneState.PLAY;

        ButtonPlay.gameObject.SetActive(false);

        StartCoroutine(OnTurn());
    }

    /// <summary>
    /// Corrutina que llama a avanzar un paso de todos los barcos
    /// </summary>
    /// <returns></returns>
    IEnumerator OnTurn()
    {
        while (State == SceneState.PLAY)
        {
            foreach(Enemy enemy in Map.Enemies.Values)
                enemy.NextStep();

            Map.Hero.NextStep();
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void OnOffLight()
    {

        if (Map.LightOn)
        {
            AmbientLight.enabled = false;
            Map.LightOn = false;

            if (Map.Hero != null)
                Map.Hero.gameObject.GetComponentInChildren<Light>().enabled = true;

        }
        else
        {
            AmbientLight.enabled = true;
            Map.LightOn = true;

            if (Map.Hero != null)
                Map.Hero.gameObject.GetComponentInChildren<Light>().enabled = false;
        }

    }

}
