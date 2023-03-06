using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMiniPantallas : MonoBehaviour
{
    [SerializeField] private GameObject pantallaGameOver;
    [SerializeField] private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PantallaGameOverSetActive(bool nuevoEstado)
    {
        pantallaGameOver.SetActive(nuevoEstado);

        if(nuevoEstado) 
        {
            gameManager.PauseGame();
        }
    }
}
