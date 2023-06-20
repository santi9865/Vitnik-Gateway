using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrilGrupoObstaculo
{
    private TipoCarril _tipo;
    private bool _habilitado;
    private GameObject _posicion;

    public TipoCarril Tipo {get => _tipo;}
    public bool Habilitado {get => _habilitado;}
    public GameObject Posicion {get => _posicion;}

    public CarrilGrupoObstaculo(TipoCarril tipo, bool habilitado, GameObject posicion)
    {
        _tipo = tipo;
        _habilitado = habilitado;
        _posicion = posicion;
    }
}

