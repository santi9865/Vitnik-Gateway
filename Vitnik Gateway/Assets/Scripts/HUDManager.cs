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
    [SerializeField] private Button btnPausa;

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
            GameManager.Instancia.UnPauseGame();
            estadoBtnPausa = false;
            behaviourMiniPantallas.PantallaPausaSetActive(false);
        }
        else
        {
            GameManager.Instancia.PauseGame();
            estadoBtnPausa = true;
            behaviourMiniPantallas.PantallaPausaSetActive(true);
        }
    }

}
