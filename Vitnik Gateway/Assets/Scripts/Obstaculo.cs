using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstaculo : MonoBehaviour
{
    [SerializeField] private TipoObstaculo _tipo;
    public TipoObstaculo Tipo {get => _tipo;}

    [SerializeField] private float _offsetHorizontal;
    [SerializeField] private float _offsetVertical;

    public float OffsetHorizontal {get => _offsetHorizontal;}
    public float OffsetVertical {get => _offsetVertical;}
}

public enum TipoObstaculo
{
    Caja, Tronco, TroncoSuspendido, Piedra, CajaAlta
}
