using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyView : MonoBehaviour {

    public Ally Ally { get; set; }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BuildAlly(Ally ally)
    {
        Ally = ally;
    }


}
