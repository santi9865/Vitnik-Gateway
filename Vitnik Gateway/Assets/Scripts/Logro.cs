using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logro
{
    public int ID {get;}
    public string Nombre {get;}
    public string Descripcion {get;}
    public string Tipo{get;}
    public int Meta{get;}
    public int Progreso{get; private set;}
    public int IDIcono{get;}
    public string TipoRecompensa{get;}
    public int ValorRecompensa{get;}

    private LogrosManager _manager;


    public Logro(int id, string nombre, string descripcion, string tipo, int meta,
     int idIcono, int progreso, string tipoRecompensa, int valorRecompensa, LogrosManager manager)
    {
        ID = id;
        Nombre = nombre;
        Descripcion = descripcion;
        Tipo = tipo;
        Meta = meta;
        IDIcono = idIcono;
        Progreso = progreso;
        TipoRecompensa = tipoRecompensa;
        ValorRecompensa = valorRecompensa;
        _manager = manager;
    }

    public bool CambiarProgreso(int valorNuevo)
    {
        if(Progreso >= Meta)
        {
            return false;
        }

        switch(Tipo)
        {
            case "Bool":
                if(valorNuevo > 0)
                {
                    valorNuevo = 1;
                }
                else
                {
                    valorNuevo = 0;
                }
                break;

            case "Numero":
                Progreso = valorNuevo;
                break;
                
            default:
                Progreso = valorNuevo;
                break;
        }

        if(Progreso >= Meta)
        {
            Recompensar();
        }

        return true;
    }

    private void Recompensar()
    {
        _manager.ImpartirRecompensa(TipoRecompensa, ValorRecompensa);
    }
}


