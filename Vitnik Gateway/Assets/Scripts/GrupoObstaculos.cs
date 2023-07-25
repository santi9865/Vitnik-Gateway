using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrupoObstaculos
{
    public List<LugarObstaculo> Lugares;
    public Vector3 Posicion {get;}
    public List<Carril> Carriles;

    public int CantidadLugaresLibresHabilitados {get => ContarLugaresLibresHabilitados();}
    private List<Carril> _carrilesAConsiderar;

    public GrupoObstaculos(Vector3 posicion, List<LugarObstaculo> lugares, List<Carril> carriles, List<Carril> carrilesAConsiderar = null)
    {
        Posicion = posicion;
        Lugares = lugares;
        Carriles = carriles;
        _carrilesAConsiderar = carrilesAConsiderar;
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

    private int ContarLugaresLibresHabilitados()
    {
        int contadorLugaresLibresHabilitados = 0;

        if(_carrilesAConsiderar != null)
        {
            foreach(LugarObstaculo lugar in Lugares)
            {
                Carril scriptCarril = Carriles[lugar.Carril];
                Carril scriptCarrilAConsiderar = _carrilesAConsiderar[lugar.Carril];

                if(scriptCarril.Habilitado && lugar.Libre && scriptCarrilAConsiderar.Habilitado)
                {
                    contadorLugaresLibresHabilitados++;
                }
            }
        }
        else
        {
            foreach(LugarObstaculo lugar in Lugares)
            {
                Carril scriptCarril = Carriles[lugar.Carril];

                if(scriptCarril.Habilitado && lugar.Libre)
                {
                    contadorLugaresLibresHabilitados++;
                }
            }
        }

        return contadorLugaresLibresHabilitados;
    }

    public LugarObstaculo DevolverLugarLibreAleatorioHabilitado()
    {        
        LugarObstaculo[] lugaresLibres = new LugarObstaculo[CantidadLugaresLibresHabilitados];

        int contadorArray = 0;

        if(_carrilesAConsiderar != null)
        {
            foreach(LugarObstaculo lugar in Lugares)
            {
                Carril scriptCarril = Carriles[lugar.Carril];
                Carril scriptCarrilAConsiderar = _carrilesAConsiderar[lugar.Carril];

                if(scriptCarril.Habilitado && lugar.Libre && scriptCarrilAConsiderar.Habilitado)
                {
                    lugaresLibres[contadorArray] = lugar;
                    contadorArray++;
                }
            }
        }
        else
        {
            foreach(LugarObstaculo lugar in Lugares)
            {
                Carril scriptCarril = Carriles[lugar.Carril];

                if(scriptCarril.Habilitado && lugar.Libre)
                {
                    lugaresLibres[contadorArray] = lugar;
                    contadorArray++;
                }
            }
        }

        return lugaresLibres[Random.Range(0, lugaresLibres.Length)];
    }
}
