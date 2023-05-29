using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IconoInventario : MonoBehaviour
{
    [SerializeField] private Image imgIcono;
    [SerializeField] private GameObject imgEquipado;

    public bool Equipado {get; private set;}
    public int ItemID {get; private set;}

    private InventarioManager _inventarioManager;

    public void Actualizar(int itemID, Sprite sprite, bool equipado, InventarioManager inventarioManager)
    {
        ItemID = itemID;
        imgIcono.sprite = sprite;
        CambiarEquipado(equipado);
        _inventarioManager = inventarioManager;
    }

    public void Apretado()
    {
        CambiarEquipado(true);

        _inventarioManager.NuevoIconoSeleccionado(ItemID);
    }

    public void CambiarEquipado(bool nuevoEstado)
    {
        Equipado = nuevoEstado;
        imgEquipado.SetActive(nuevoEstado);
    }
}
