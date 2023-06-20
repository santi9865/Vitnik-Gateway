using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carril : MonoBehaviour
{
    [SerializeField] private TipoCarril _tipo;
    [SerializeField] private bool _habilitado;

    [SerializeField] private GameObject _posicion;

    public TipoCarril Tipo {get => _tipo;}
    public bool Habilitado {get => _habilitado;}
    public GameObject Posicion {get => _posicion;}

    public Carril(TipoCarril tipo, bool habilitado, GameObject posicion)
    {
        _tipo = tipo;
        _habilitado = habilitado;
        _posicion = posicion;
    }
}

public enum TipoCarril
{
    Izquierdo,
    Central,
    Derecho
}
