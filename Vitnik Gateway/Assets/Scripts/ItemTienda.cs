using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTienda
{
    public int ID {get;}
    public string Nombre {get;}
    public int Costo {get;}
    public int IDIcono {get;}
    public string Categoria {get;}
    public string Descripcion {get;}
    public int IDModelo{get;}

    public ItemTienda(int id, string nombre, int costo, int idIcono, string categoria, string descripcion, int idModelo)
    {
        ID = id;
        Nombre = nombre;
        Costo = costo;
        IDIcono = idIcono;
        Categoria = categoria;
        Descripcion = descripcion;
        IDModelo = idModelo;
    }

}
