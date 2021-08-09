using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{

    public int Value { get; set; }
    public ColorDatabase colors;
    public bool IsAnimating { get; private set; }

    public float animationTimeTotal = 1.0f;
    public float animationTimeCurrent = 0;
    public float animationThreshold = 0.9f;
    

    private Vector3 boardPosition;
    


    public void UpdateGraphic()
    {
        if(boardPosition != transform.position)
        {
            IsAnimating = true;
        }

        if(IsAnimating)
        {
            animationTimeCurrent += Time.deltaTime;

            if(animationTimeCurrent > 0 && animationTimeCurrent < animationThreshold)
            {
                animationTimeCurrent += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, boardPosition, SmootherStep(animationTimeCurrent / animationTimeTotal));
            }
            else
            {
                LockBoardPosition();
            }
        }

        transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text = $"{Value}";
        transform.GetChild(1).gameObject.GetComponent<Renderer>().material.color = colors.Query(Value);
    }

    public void SetBoardPosition(Vector3 p)
    {
        boardPosition = p;
    }

    public void LockBoardPosition()
    {
        transform.position = boardPosition;
        IsAnimating = false;
        animationTimeCurrent = 0;
    }

    // used to create a smooth in smooth out lerp
    public float SmootherStep(float ratio)
    {
      return ratio * ratio * ratio * (ratio * (6f * ratio - 15f) + 10f);
    }
}
