using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private TMP_Text txtMonedas;
    [SerializeField] private TMP_Text txtDistancia;
    [SerializeField] private BehaviourMiniPantallas behaviourMiniPantallas;

    void Update()
    {
        ActualizarDistancia();
        ActualizarMonedas();
    }

    public void ActualizarMonedas()
    {
        txtMonedas.text = GameManager.Instancia.Monedas.ToString();
    }

    public void ActualizarDistancia()
    {
        txtDistancia.text = GameManager.Instancia.Distancia.ToString("0.0") + " m";
    }

    public void BtnPausa()
    {
        if(GameManager.Instancia.IsPaused)
        {
            behaviourMiniPantallas.CerrarTodo();
        }
        else
        {
            behaviourMiniPantallas.PantallaPausaSetActive(true);
        }
    }
}
