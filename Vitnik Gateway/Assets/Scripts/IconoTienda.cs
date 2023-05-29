using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconoTienda : MonoBehaviour
{
    [SerializeField] private TMP_Text txtMonto;
    [SerializeField] private Image imgIcono;
    [SerializeField] private GameObject imgAdquirido;
    [SerializeField] private GameObject imgIncomprable;

    public int Monto {get; private set;}
    public int ItemID {get; private set;}
    public bool Comprable {get; private set;}
    public bool Adquirido {get; private set;}

    private TiendaManager manager;

    public void Actualizar(int nuevoID, int nuevoMonto, Sprite nuevoSprite, TiendaManager nuevoManager)
    {
        ItemID = nuevoID;
        Monto = nuevoMonto;
        imgIcono.sprite = nuevoSprite;
        manager = nuevoManager;
        Comprable = true;
        Adquirido = false;

        txtMonto.text = Monto.ToString();
    }

    public void Apretado()
    {
        manager.NuevoIconoSeleccionado(ItemID, Comprable, Adquirido);
    }

    public void Adquirir()
    {
        Adquirido = true;
        Comprable = false;
        imgAdquirido.SetActive(true);
    }

    public void CambiarComprabilidad(bool nuevoEstado)
    {
        Comprable = nuevoEstado;

        imgIncomprable.SetActive(!Comprable);
    }
}
