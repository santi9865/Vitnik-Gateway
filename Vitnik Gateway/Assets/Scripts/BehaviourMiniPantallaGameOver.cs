using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BehaviourMiniPantallaGameOver : MonoBehaviour
{
    [SerializeField] private TMP_Text distanciaRecorrida;
    [SerializeField] private TMP_Text monedasObtenidas;

    public void Activar()
    {
        gameObject.SetActive(true);
    }

    public void Desactivar()
    {
        gameObject.SetActive(false);
    }

    public void Actualizar()
    {
        distanciaRecorrida.text = "Distancia Recorrida: " + GameManager.Instancia.Distancia.ToString("0.0") + "m";
        monedasObtenidas.text = "Monedas Obtenidas: " + GameManager.Instancia.Monedas;
    }
}
