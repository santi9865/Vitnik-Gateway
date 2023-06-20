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

    private bool estadoBtnPausa;

    // Start is called before the first frame update
    void Start()
    {
        estadoBtnPausa = false;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if(estadoBtnPausa)
        {
            EventSystem.current.SetSelectedGameObject(null);
            ActualizarEstadoBtnPausa(false);

            if(behaviourMiniPantallas.pantallaOpcionesActive)
            {
                behaviourMiniPantallas.PantallaOpcionesSetActive(false);
            }
            GameManager.Instancia.UnPauseGame();
            behaviourMiniPantallas.PantallaPausaSetActive(false);
        }
        else
        {
            GameManager.Instancia.PauseGame();
            ActualizarEstadoBtnPausa(true);
            behaviourMiniPantallas.PantallaPausaSetActive(true);
        }
    }

    public void ActualizarEstadoBtnPausa(bool nuevoEstado)
    {
        estadoBtnPausa = nuevoEstado;
    }

}
