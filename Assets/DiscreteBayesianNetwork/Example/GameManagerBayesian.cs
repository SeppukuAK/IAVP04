/*
 * Copyright (c) by Junjie Chen
 * Please refer to https://unity3d.com/legal/as_terms for the terms and conditions
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Jackyjjc.Bayesianet;

public class GameManagerBayesian : MonoBehaviour
{

    //-----------------INSPECTOR-------------------------
    /// <summary>
    /// Referencia al texto que muestra la decisión del personaje
    /// </summary>
    public Text decisionText;

    /// <summary>
    /// Referencia al texto que muestra la probabilidad de atacar y de correr
    /// </summary>
    public Text probabilityText;

    /// <summary>
    /// Referencia al texto que muestra el número de enemigos
    /// </summary>
    public Text enemyAmountSliderText;

    //Referencia a los Toogle
    public Toggle braveToggle;
    public ToggleGroup coverTypeToggleGroup;

    //-----------------INSPECTOR-------------------------

    //-----------------PRIVATE ATTRIBUTES-------------------------

    /// <summary>
    /// Variable principal de la red Bayesiana
    /// Tiene la tabla
    /// </summary>
    private VariableElimination ve;

    /// <summary>
    /// Número de enemigos actuales
    /// you can map continuous values into discrete ones
    /// </summary>
    private int enemyAmount;

    //-----------------PRIVATE ATTRIBUTES-------------------------

    /// <summary>
    /// Parsea el json y crea la Variable de Eliminación
    /// Llama a MakeDecision()
    /// </summary>
    void Start()
    {
        string networkJson = (Resources.Load("enemy_logic_json") as TextAsset).text;

        ve = new VariableElimination(new BayesianJsonParser().Parse(networkJson));
        MakeDecision();
    }

    /// <summary>
    /// Se le llama al inicio de la escena
    /// Se le llama cada vez que se pulsa un toogle o se desliza el slider
    /// </summary>
    public void MakeDecision()
    {

        ////Lista con las observaciones de la escena
        //List<string> observations = new List<string> {
        //    "brave=" + GetIsBrave(),
        //    "enemy_amount=" + GetEnemyAmount(),
        //    "cover_type=" + GetCoverType()
        //};
        ///*EJEMPLO: [brave=True, enemy_amount= 3, cover_type=Full_cover] */
        ////Usamos la lista para inferir otra variable en la red
        ////Calculamos la probabilidad de luchar en función de la observación
        //double[] fightDistribution = ve.Infer("fight", observations);

        ////Obtenemos si lucha o no
        //bool fight = ve.PickOne(fightDistribution) == 0;

        ////Añadimos si lucha o no a la lista de observaciones para tenerlo en cuenta para las siguientes inferencias
        //observations.Add("fight=" + fight);

        // The API functions are overloaded to fit your needs
        // e.g. you can use a less string-based approach if you want to do things programmatically
        //Creamos la red de decisión
        BayesianNetwork network = ve.GetNetwork();

        //Obtenemos referencia a todas las proposiciones y las crea con los valores observables
        Proposition braveProp = network.FindNode("brave").Instantiate(GetIsBrave());
        Proposition enemyAmountProp = network.FindNode("enemy_amount").Instantiate(GetEnemyAmount());
        Proposition hasCoverProp = network.FindNode("cover_type").Instantiate(GetCoverType());

        BayesianNode fightNode = ve.GetNetwork().FindNode("fight");
        double[] fightDistribution = ve.Infer(fightNode, braveProp, enemyAmountProp, hasCoverProp);
        bool fight = ve.PickOne(fightDistribution) == fightNode.var.GetTokenIndex("True");

        //Obtenemos la referencia a la proposición creada
        Proposition fightProp = network.FindNode("fight").Instantiate(fight.ToString());

        //Inferimos el nuevo nodo y tomamos una decisión
        BayesianNode runAwayNode = ve.GetNetwork().FindNode("run_away");
        double[] runawayDistribution = ve.Infer(runAwayNode, braveProp, enemyAmountProp, hasCoverProp, fightProp);
        bool runaway = ve.PickOne(runawayDistribution) == runAwayNode.var.GetTokenIndex("True");

        //Se puede inferir todo lo que quieras con información parcial o sin información
        ve.Infer("enemy_amount", "fight=True");
        ve.Infer("fight");

        //Escribir la decisión en función de lo que nos salga del coñete
        if (enemyAmount.Equals("NoEnemy"))
        {
            decisionText.text = "Did not see any enemy.";
        }
        else if (fight)
        {
            decisionText.text = "The NPC decided to fight. ";
        }
        else if (!fight && runaway)
        {
            decisionText.text = "The NPC decided to run away.";
        }
        else
        {
            decisionText.text = "The NPC decided to wait for his chance.";
        }

        decisionText.text = "Decision made: " + decisionText.text;

        probabilityText.text = string.Format("true: {0}%\t\tfalse: {1}%\ntrue: {2}%\t\tfalse: {3}%",
            fightDistribution[0] * 100, fightDistribution[1] * 100, runawayDistribution[0] * 100, runawayDistribution[1] * 100);
    }

    /// <summary>
    /// Es llamado cada vez que se desliza el Slider. Actualiza el número de enemigos y vuelve a tomar decisión
    /// </summary>
    /// <param name="sliderValue"></param>
    public void SliderValueChange(float sliderValue)
    {
        enemyAmountSliderText.text = string.Format("The number of enemies: {0}", sliderValue);
        enemyAmount = (int)sliderValue;

        MakeDecision();
    }

    /// <summary>
    /// Devuelve un String con la cantidad de enemigos
    /// </summary>
    /// <returns></returns>
    private string GetEnemyAmount()
    {
        string result;
        if (enemyAmount == 0) result = "NoEnemy";
        else if (enemyAmount <= 2) result = "Underwhelm";
        else if (enemyAmount == 3) result = "Level";
        else result = "Overwhelm";
        return result;
    }

    /// <summary>
    /// Devuelve un string con "No_Cover", "Half_Cover" o "Full_Cover" dependiendo de cuál Toogle está activado
    /// </summary>
    /// <returns></returns>
    private string GetCoverType()
    {
        return coverTypeToggleGroup.ActiveToggles().First().GetComponentInChildren<Text>().text;
    }

    /// <summary>
    /// Devuelve un string con "True" o "False" dependiendo si está activado o no que el NPC sea valiente
    /// </summary>
    /// <returns></returns>
    private string GetIsBrave()
    {
        return braveToggle.isOn.ToString();
    }
}
