using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Jackyjjc.Bayesianet;

/// <summary>
/// Enumerado que controla los distintos estados del juego
/// </summary>
public enum SceneState { SETHERO, SETMAP, PLAY, GAMEOVER, NULL }

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

    //Probabilidades del combate
    public const float PROBTHREEORMOREALLIES = 0.9f;
    public const float PROBTWOALLIES = 0.5f;
    public const float PROBONEALLY = 0.2f;
    public const float PROBNOLIGHT = -0.1f;

    //------------------CONSTANTES-------------------

    //------------------INSPECTOR-------------------

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
        State = SceneState.NULL;
        ButtonPlay.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        State = SceneState.SETHERO;
    }

    public void InitGame()
    {
        State = SceneState.PLAY;

        ButtonPlay.gameObject.SetActive(false);

        Map.Instance.UpdateProbability();

        StartCoroutine(Map.Instance.OnTurn());
    }



    public void OnOffLight()
    {

        if (Map.Instance.LightOn)
        {
            AmbientLight.enabled = false;
            Map.Instance.LightOn = false;

            if (Map.Instance.Hero != null)
                Map.Instance.Hero.gameObject.GetComponentInChildren<Light>().enabled = true;

        }
        else
        {
            AmbientLight.enabled = true;
            Map.Instance.LightOn = true;

            if (Map.Instance.Hero != null)
                Map.Instance.Hero.gameObject.GetComponentInChildren<Light>().enabled = false;
        }
        Map.Instance.UpdateProbability();
    }

    public void GameOver()
    {
        State = SceneState.GAMEOVER;
        //MOSTRAR PUNTUACIÓN FEDE COMEME LOS HUEVIS

    }
}
