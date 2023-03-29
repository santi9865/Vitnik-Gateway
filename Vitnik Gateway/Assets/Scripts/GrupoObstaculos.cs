using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrupoObstaculos
{
    public List<LugarObstaculo> Lugares;
    public float Posicion {get;}
    public List<Transform> Carriles;

    public GrupoObstaculos(float posicion, List<LugarObstaculo> lugares, List<Transform> carriles)
    {
        Posicion = posicion;
        Lugares = lugares;
        Carriles = carriles;
    }

    public void DesactivarObstaculos()
    {
        foreach(LugarObstaculo lugar in Lugares)
        {
            lugar.DesactivarObstaculo();
        }
    }

    public LugarObstaculo DevolverLugarLibreAleatorio()
    {        

        int contadorLugaresLibres = 0;
        foreach(LugarObstaculo lugar in Lugares)
        {
            if(lugar.Libre)
            {
                contadorLugaresLibres++;
            }
        }

        LugarObstaculo[] lugaresLibres = new LugarObstaculo[contadorLugaresLibres];

        int contadorArray = 0;

        foreach(LugarObstaculo lugar in Lugares)
        {
            if(lugar.Libre)
            {
                lugaresLibres[contadorArray] = lugar;
                contadorArray++;
            }
        }

        return lugaresLibres[Random.Range(0, lugaresLibres.Length)];
    }

}
