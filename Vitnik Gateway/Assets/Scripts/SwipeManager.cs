using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public event Action SwipeLeft;
    public event Action SwipeRight;
    public event Action SwipeUp;
    public event Action SwipeDown;

    private Dictionary<int, Vector2> _posicionesIniciales;

    void Start()
    {
        _posicionesIniciales = new Dictionary<int, Vector2>();
    }

    void Update()
    {
        DetectarTouch();
    }

    private void DetectarTouch()
    {
        if(Input.touchCount > 0)
        {
            foreach(Touch touch in Input.touches)
            {
                switch(touch.phase)
                {
                    case TouchPhase.Began:
                        _posicionesIniciales[touch.fingerId] = touch.position;
                        break;
                    case TouchPhase.Ended:
                        AnalizarTrayectoria(touch);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void AnalizarTrayectoria(Touch touch)
    {
        Vector2 deltaPosition = touch.position - _posicionesIniciales[touch.fingerId];

        if(Mathf.Abs(deltaPosition.x) > Mathf.Abs(deltaPosition.y))
        {
            //Es un movimiento horizontal
            if(deltaPosition.x > 0)
            {
                //Es hacia la derecha.
                SwipeRight.Invoke();
            }
            else
            {
                //Es hacia la izquierda
                SwipeLeft.Invoke();
            }
        }
        else
        {
            //Es un movimiento vertical
            if(deltaPosition.y > 0)
            {
                //Es hacia arriba.
                SwipeUp.Invoke();
            }
            else
            {
                //Es hacia la izquierda
                SwipeDown.Invoke();
            }
        }
    }
}
