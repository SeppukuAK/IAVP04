using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero  {

    public Tile Tile { get; set; }

    public Hero(Tile tile)
    {
        Tile = tile;
    }
}
