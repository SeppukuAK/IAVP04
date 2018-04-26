using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroView : MonoBehaviour {

    private Hero hero;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void BuildHero(Hero hero)
    {
        this.hero = hero;
    }
}
