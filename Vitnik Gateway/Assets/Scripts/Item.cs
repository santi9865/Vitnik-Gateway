using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int ID {get;}
    public string Nombre {get;}
    public int IDIcono {get;}
    public string Categoria {get;}
    public string Descripcion {get;}
    public int IDModelo{get;}

    public Item(int id, string nombre, string categoria, string descripcion, int idIcono, int idModelo)
    {
        ID = id;
        Nombre = nombre;
        IDIcono = idIcono;
        Categoria = categoria;
        Descripcion = descripcion;
        IDModelo = idModelo;
    }

}
