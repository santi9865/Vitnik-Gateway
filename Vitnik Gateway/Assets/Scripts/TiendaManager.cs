using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TiendaManager : MonoBehaviour
{
    [SerializeField] private ConexionDatabase conexionDatabaseItems;
    [SerializeField] private ConexionDatabase conexionDatabasePrecios;
    [SerializeField] private ConexionDatabase conexionDatabaseStatsJugador;

    [SerializeField] private GameObject contenidoPestañaPecho;
    [SerializeField] private GameObject contenidoPestañaPiernas;
    [SerializeField] private GameObject contenidoPestañaAccesorios;

    [SerializeField] private GameObject iconoPrefab;


    private List<Precio> precios;
    private List<Item> items;

    void Start()
    {

    }

    private void CargarPrecios()
    {
        List<object> IDs;
        List<object> Montos;

        IDs = conexionDatabasePrecios.ObtenerValoresColumna("IDItem");
        Montos = conexionDatabasePrecios.ObtenerValoresColumna("Monto");

        if(IDs == null || Montos == null)
        {   
            Debug.Log("Error al leer IDs o Montos.");
            return;
        }

        if(IDs.Count != Montos.Count)
        {
            Debug.Log("La cantidad de IDS y Montos no coincide.");
        }
        else
        {
            precios = new List<Precio>();

            for(int i = 0; i < IDs.Count; i++)
            {
                Precio nuevoPrecio = new Precio((int)IDs[i], (int)Montos[i]);

                precios.Add(nuevoPrecio);
            }
        }
    }

    private void CargarItems()
    {
        if(precios == null)
        {
            Debug.Log("Error al cargar precios.");
        }
        else
        {
            items = new List<Item>();

            for(int i = 0; i < precios.Count; i++)
            {
                List<object> lecturaItem = conexionDatabaseItems.ObtenerValoresSegunID(precios[i].IDItem);
                Item nuevoItem = new Item
                (
                    (int)lecturaItem[0],
                    (string)lecturaItem[1],
                    (string)lecturaItem[2],
                    (string)lecturaItem[3],
                    (int)lecturaItem[5],
                    (int)lecturaItem[6]
                );

                items.Add(nuevoItem);
            }
        }
    }

    private void CrearIconos()
    {

    }
}
