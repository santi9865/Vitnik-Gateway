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


    public Logro(int id, string nombre, string descripcion, string tipo, int meta, int idIcono)
    {
        ID = id;
        Nombre = nombre;
        Descripcion = descripcion;
        Tipo = tipo;
        Meta = meta;
        IDIcono = idIcono;
        Progreso = 0;
    }

    public void CambiarProgreso(int valorNuevo)
    {
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
                break;
                
            default:
                break;
        }

        Progreso = valorNuevo;
    }

}


