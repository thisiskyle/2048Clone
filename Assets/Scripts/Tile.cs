using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile
{
    public GameObject graphic;
    public int Value { get; set; }
    

    private Vector3 boardPosition;



    public Tile(GameObject g)
    {
        graphic = g;
    }


    public void UpdateGraphic()
    {
        graphic.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = $"{Value}";
        graphic.transform.position = boardPosition;
    }

    public void SetBoardPosition(Vector3 p)
    {
        boardPosition = p;
    }
}
