using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelLogro : MonoBehaviour
{
    [SerializeField] private TMP_Text txtNombre;
    [SerializeField] private TMP_Text txtDescripcion;
    [SerializeField] private Image imgIcono;
    [SerializeField] private Slider sliProgreso;

    public void Actualizar(string nombre, string descripcion, float progreso, Sprite icono)
    {
        txtNombre.text = nombre;
        txtDescripcion.text = descripcion;
        imgIcono.sprite = icono;
        sliProgreso.value = progreso;
    }
}
