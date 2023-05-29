using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BehaviourMiniPantallaConfirmarCompra : MonoBehaviour
{

    [SerializeField] private TMP_Text txtPregunta;
    [SerializeField] private TMP_Text txtMonedasDisponibles;

    public void Activar()
    {
        gameObject.SetActive(true);
    }

    public void Desactivar()
    {
        gameObject.SetActive(false);
    }

    public void Actualizar(string pregunta, string monedasDisponibles)
    {
        txtPregunta.text = pregunta;
        txtMonedasDisponibles.text = monedasDisponibles;
    }
}
