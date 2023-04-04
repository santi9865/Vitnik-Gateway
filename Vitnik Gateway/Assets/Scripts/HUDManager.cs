using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{

    [SerializeField] private TMP_Text txtMonedas;
    [SerializeField] private TMP_Text txtDistancia;

    // Start is called before the first frame update
    void Start()
    {
        
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
}
