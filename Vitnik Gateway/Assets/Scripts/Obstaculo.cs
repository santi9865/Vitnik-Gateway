using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstaculo : MonoBehaviour
{
    [SerializeField] private TipoObstaculo _tipo;
    public TipoObstaculo Tipo {get => _tipo;}

    [SerializeField] private Vector3 _offset;

    public Vector3 Offset {get => _offset;}
}

public enum TipoObstaculo
{
    Caja, Tronco, TroncoSuspendido, Piedra, CajaAlta
}
