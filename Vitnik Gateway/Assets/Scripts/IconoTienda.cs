using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconoTienda : MonoBehaviour
{
    [SerializeField] private TMP_Text txtMonto;
    [SerializeField] private Image imgIcono;
    private string descripcion;
    private int monto;
    private string nombre;

    private BehaviourMiniPantallaDescripcionItemTienda scriptMiniPantallaDescripcion;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActualizarIcono(Image nuevoIcono)
    {
        imgIcono = nuevoIcono;
    }

    public void ActualizarDescripcion(string nuevaDescripcion)
    {
        descripcion = nuevaDescripcion;
    }

    public void ActualizarMonto(int nuevoMonto)
    {
        monto = nuevoMonto;
        txtMonto.text = monto.ToString();
    }

    public void ActualizarNombre(string nuevoNombre)
    {
        nombre = nuevoNombre;
    }

    public void Apretado()
    {

    }
}
