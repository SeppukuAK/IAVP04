using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour {

    private Enemy enemy;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuildEnemy(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void NextStep()
    {
    }
}
