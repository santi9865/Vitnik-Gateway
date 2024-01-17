using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventarioManager : MonoBehaviour
{
    [SerializeField]private ConexionDatabase databaseItems;
    [SerializeField]private ConexionDatabase databaseStatsJugador;

    [SerializeField]private GameObject contenedorPecho;
    [SerializeField]private GameObject contenedorPiernas;
    [SerializeField]private GameObject contenedorAccesorios;

    [SerializeField]private GameObject prefabIconoInventario;

    [SerializeField]private TMP_Text txtNombre;
    [SerializeField]private TMP_Text txtDescripción;

    private List<Item> itemsAdquiridos;
    private List<Item> itemsEquipados;
    private List<GameObject> iconos;

    void Start()
    {
        CargarItemsAdquiridosJugador();
        CargarItemsEquipadosJugador();
        CrearIconos();
    }

    private void CargarItemsAdquiridosJugador()
    {
        string IDItemsAdquiridos = databaseStatsJugador.ObtenerPrimerValor("IDItemsAdquiridos").ToString();

        if(IDItemsAdquiridos == null || IDItemsAdquiridos == "")
        {
            Debug.Log("El jugador no tiene items adquiridos o ocurrió un error al obtenerlos.");
        }
        else
        {
            itemsAdquiridos = new List<Item>();
            int posicionComa = 0;
            int itemID = 0;

            while(IDItemsAdquiridos.Length > 0)
            {
                posicionComa = IDItemsAdquiridos.IndexOf(',');

                if(posicionComa == -1)
                {
                    itemID = System.Convert.ToInt32(IDItemsAdquiridos);
                    IDItemsAdquiridos = "";
                }
                else
                {
                    itemID = System.Convert.ToInt32(IDItemsAdquiridos.Substring(0, posicionComa));
                    IDItemsAdquiridos = IDItemsAdquiridos.Remove(0, posicionComa + 1);
                }

                List<object> lecturaItem = databaseItems.ObtenerValoresSegunID(itemID);
            
                Item nuevoItem = new Item
                (
                    System.Convert.ToInt32(lecturaItem[0]),
                    (string)lecturaItem[1],
                    (string)lecturaItem[2],
                    (string)lecturaItem[3],
                    System.Convert.ToInt32(lecturaItem[4]),
                    System.Convert.ToInt32(lecturaItem[5])
                );

                itemsAdquiridos.Add(nuevoItem);
            }
        }
    }

    private void CargarItemsEquipadosJugador()
    {
        if(itemsAdquiridos == null)
        {
            Debug.Log("Error al cargar los items equipados, los items adquiridos no están cargados.");
        }
        else
        {
            itemsEquipados = new List<Item>();

            string IDItemsEquipados = databaseStatsJugador.ObtenerPrimerValor("IDItemsEquipados").ToString();

            int posicionComa = 0;
            int itemID = 0;

            while(IDItemsEquipados.Length > 0)
            {
                posicionComa = IDItemsEquipados.IndexOf(',');

                if(posicionComa == -1)
                {
                    itemID = System.Convert.ToInt32(IDItemsEquipados);
                    IDItemsEquipados = "";
                }
                else
                {
                    itemID = System.Convert.ToInt32(IDItemsEquipados.Substring(0, posicionComa));
                    IDItemsEquipados = IDItemsEquipados.Remove(0, posicionComa + 1);
                }

                itemsEquipados.Add(BuscarItemAdquiridoPorID(itemID));
            }
        }
    }

    private Item BuscarItemAdquiridoPorID(int itemID)
    {
        foreach(Item item in itemsAdquiridos)
        {
            if(item.ID == itemID)
            {
                return item;
            }
        }

        return null;
    }

    public void NuevoIconoSeleccionado(int itemID)
    {
        Item nuevoItem = BuscarItemAdquiridoPorID(itemID);

        if(nuevoItem == null)
        {
            Debug.Log("El item selecccionado no fue encontrado.");
        }
        else
        {
            txtNombre.text = nuevoItem.Nombre;
            txtDescripción.text = nuevoItem.Descripcion;

            NuevoItemEquipado(BuscarItemAdquiridoPorID(itemID));
            ActualizarIconos();
        }
    }

    private void CrearIconos()
    {
        if(itemsAdquiridos == null)
        {
            Debug.Log("Error al crear íconos, la lista de items adquiridos está vacía.");
        }
        else
        {
            iconos = new List<GameObject>();

            foreach(Item item in itemsAdquiridos)
            {
                GameObject contenedor;

                switch(item.Categoria)
                {
                    case "Pecho":
                        contenedor = contenedorPecho;
                        break;
                    case "Piernas":
                        contenedor = contenedorPiernas;
                        break;
                    case "Accesorio":
                        contenedor = contenedorAccesorios;
                        break;
                    default:
                        contenedor = contenedorPecho;
                        break;
                }

                GameObject nuevoIcono = Instantiate(prefabIconoInventario, contenedor.transform);

                iconos.Add(nuevoIcono);

                IconoInventario scriptIcono = nuevoIcono.GetComponent<IconoInventario>();

                scriptIcono.Actualizar(item.ID, BuscarImagenPorID(item.IDIcono), ItemEstaEquipado(item.ID), this);
            }
        }
    }

    private bool ItemEstaEquipado(int itemID)
    {
        if(itemsEquipados == null)
        {
            Debug.Log("lista nula");
        }


        foreach(Item item in itemsEquipados)
        {
            if(item == null)
            {
                Debug.Log("item nulo");
            }

            if(item.ID == itemID)
            {
                return true;
            }
        }

        return false;
    }

    private Sprite BuscarImagenPorID(int idIcono)
    {
        return Resources.Load<Sprite>($"Placeholders/IconosRopa/{idIcono}");
    }

    private GameObject BuscarIconoPorID(int itemID)
    {
        foreach(GameObject icono in iconos)
        {
            if(icono.GetComponent<IconoInventario>().ItemID == itemID )
            {
                return icono;
            }
        }

        return null;
    }

    private void NuevoItemEquipado(Item nuevoItem)
    {
        string nuevosItemsEquipados = "";

        for(int i = 0; i < itemsEquipados.Count; i++)
        {
            if(itemsEquipados[i].Categoria == nuevoItem.Categoria)
            {
                itemsEquipados[i] = nuevoItem;
            }

            nuevosItemsEquipados += itemsEquipados[i].ID.ToString() + ",";
        }

        databaseStatsJugador.ModificarValor("IDItemsEquipados", nuevosItemsEquipados, "ID", 0);
    }

    private void ActualizarIconos()
    {
        foreach(GameObject icono in iconos)
        {
            IconoInventario scriptIcono = icono.GetComponent<IconoInventario>();

            scriptIcono.CambiarEquipado(ItemEstaEquipado(scriptIcono.ItemID));
        }
    }
}
