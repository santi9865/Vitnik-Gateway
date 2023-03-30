using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carril : MonoBehaviour
{
    [SerializeField] private TipoCarril _tipo;
    [SerializeField] private bool _habilitado;

    public TipoCarril Tipo {get => _tipo;}
    public bool Habilitado {get => _habilitado;}
}

public enum TipoCarril
{
    Izquierdo,
    Central,
    Derecho
}
