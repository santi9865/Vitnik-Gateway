using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrupoObstaculos
{
    public List<LugarObstaculo> Lugares;
    public Vector3 Posicion {get;}
    public List<Carril> Carriles;

    public GrupoObstaculos(Vector3 posicion, List<LugarObstaculo> lugares, List<Carril> carriles)
    {
        Posicion = posicion;
        Lugares = lugares;
        Carriles = carriles;
    }

    public void DesactivarObstaculos()
    {
        if(Lugares != null)
        {
            foreach(LugarObstaculo lugar in Lugares)
            {
                lugar.ReiniciarObstaculo();
                lugar.DesactivarObstaculo();
            }
        }
    }

    public LugarObstaculo DevolverLugarLibreAleatorio()
    {        
        int contadorLugaresLibres = 0;
        
        foreach(LugarObstaculo lugar in Lugares)
        {
            Carril scriptCarril = Carriles[lugar.Carril];

            if(scriptCarril.Habilitado && lugar.Libre)
            {
                contadorLugaresLibres++;
            }
        }

        LugarObstaculo[] lugaresLibres = new LugarObstaculo[contadorLugaresLibres];

        int contadorArray = 0;

        foreach(LugarObstaculo lugar in Lugares)
        {
            Carril scriptCarril = Carriles[lugar.Carril];

            if(scriptCarril.Habilitado && lugar.Libre)
            {
                lugaresLibres[contadorArray] = lugar;
                contadorArray++;
            }
        }

        return lugaresLibres[Random.Range(0, lugaresLibres.Length)];
    }
}
