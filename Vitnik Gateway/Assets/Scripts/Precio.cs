using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Precio
{
    public int Monto {get;}

    public int IDItem {get;}

    public Precio(int idItem, int monto)
    {
        Monto = monto;
        IDItem = idItem;
    }
}
