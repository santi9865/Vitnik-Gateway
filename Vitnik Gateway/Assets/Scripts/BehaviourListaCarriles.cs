using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourListaCarriles : MonoBehaviour
{
    [SerializeField] private List<Carril> carriles;

    public List<Carril> Carriles 
    {
        get
        {
            return carriles;
        }
    }

    void Awake()
    {
    }

    private void BuscarCarrilesEnHijos()
    {
        carriles = new List<Carril>();

        for(int i = 0; i < transform.childCount; i++)
        {
            Carril carrilHijo = transform.GetChild(i).GetComponent<Carril>();

            if(carrilHijo != null)
            {
                carriles.Add(carrilHijo);
            }
        }

        if(carriles.Count == 0)
        {
            carriles = null;
            return;
        }

        //Ordenar de izquierda a derecha

        carriles.Sort(CompararCarriles);
    }

    private int CompararCarriles(Carril a, Carril b)
    {
       return (a.Tipo, b.Tipo) switch
       {
            (TipoCarril.Izquierdo, TipoCarril.Central) => -1,
            (TipoCarril.Central, TipoCarril.Derecho) => -1,
            (TipoCarril.Izquierdo, TipoCarril.Derecho) => -1,
            (TipoCarril TipoA, TipoCarril TipoB) when TipoA == TipoB => 0,
            _ => 1
       };
    }    
}
