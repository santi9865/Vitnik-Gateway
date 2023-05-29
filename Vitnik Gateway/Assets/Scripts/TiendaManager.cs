using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TiendaManager : MonoBehaviour
{
    [SerializeField] private ConexionDatabase conexionDatabaseItems;
    [SerializeField] private ConexionDatabase conexionDatabasePrecios;
    [SerializeField] private ConexionDatabase conexionDatabaseStatsJugador;

    [SerializeField] private GameObject contenidoPestañaPecho;
    [SerializeField] private GameObject contenidoPestañaPiernas;
    [SerializeField] private GameObject contenidoPestañaAccesorios;

    [SerializeField] private GameObject iconoPrefab;

    [SerializeField] private TMP_Text txtDescripcion;
    [SerializeField] private TMP_Text txtNombre;
    [SerializeField] private TMP_Text txtMonedasDisponibles;
    [SerializeField] private TMP_Text txtMontoAPagar;
    [SerializeField] private TMP_Text txtEstado;

    [SerializeField] private Button btnComprar;

    [SerializeField] private BehaviourMiniPantallaConfirmarCompra scriptMiniPantallaConfirmarCompra;

    private List<Precio> precios;
    private List<Item> items;
    private List<int> IDsItemsAdquiridos;
    private List<GameObject> botonesItems;

    private int monedasDisponibles;
    private Item itemSeleccionado;
    private Precio precioSeleccionado;

    private bool miniPantallaActiva;

    void Start()
    {
        CargarMonedasDisponibles();
        CargarPrecios();
        CargarItems();
        CrearIconos();
        ActualizarBotones();
    }

    private void CargarItemsAdquiridos()
    {
        string items = (string)conexionDatabaseStatsJugador.ObtenerPrimerValor("IDItemsAdquiridos");

        if(items == null)
        {
            Debug.Log("Error de lectura de items adquiridos.");
            return;
        }

        IDsItemsAdquiridos = new List<int>();

        while(items.Length > 0)
        {
            int posicionComa = items.IndexOf(',');

            if(posicionComa == -1)
            {
                IDsItemsAdquiridos.Add(System.Convert.ToInt32(items));
                items = "";
            }
            else
            {
                IDsItemsAdquiridos.Add(System.Convert.ToInt32(items.Substring(0, posicionComa)));

                items = items.Remove(0, posicionComa + 1);
            }
        }
    }

    private void CargarMonedasDisponibles()
    {
        monedasDisponibles = System.Convert.ToInt32(conexionDatabaseStatsJugador.ObtenerPrimerValor("Monedas"));

        txtMonedasDisponibles.text = monedasDisponibles.ToString();
    }

    public void ActualizarEstadoMiniPantalla(bool estado)
    {
        miniPantallaActiva = estado;
    }

    public void MostrarMiniPantallaConfirmarCompra()
    {
        if(miniPantallaActiva)
        {
            return;
        }

        if(itemSeleccionado == null || precioSeleccionado == null)
        {
            Debug.Log("No hay item o precio seleccionado.");
            return;
        }

        string pregunta = $"¿Quieres comprar {itemSeleccionado.Nombre} por {precioSeleccionado.Monto} monedas?";

        scriptMiniPantallaConfirmarCompra.Activar();
        scriptMiniPantallaConfirmarCompra.Actualizar(pregunta, monedasDisponibles.ToString());
    }

    public void CompraConfirmada()
    {
        monedasDisponibles -= precioSeleccionado.Monto;

        conexionDatabaseStatsJugador.ModificarValor("Monedas", monedasDisponibles, "ID", 0);
        
        string itemsActuales = conexionDatabaseStatsJugador.ObtenerPrimerValor("IDItemsAdquiridos").ToString();

        itemsActuales += itemSeleccionado.ID + ",";

        conexionDatabaseStatsJugador.ModificarValor("IDItemsAdquiridos", itemsActuales, "ID", 0);

        txtMonedasDisponibles.text = monedasDisponibles.ToString();

        ActualizarBotones();

        btnComprar.interactable = false;

        txtEstado.text = "";
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
                Precio nuevoPrecio;

                nuevoPrecio = new Precio(System.Convert.ToInt32(IDs[i].ToString()), System.Convert.ToInt32(Montos[i].ToString()));
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
                    System.Convert.ToInt32(lecturaItem[0]),
                    (string)lecturaItem[1],
                    (string)lecturaItem[2],
                    (string)lecturaItem[3],
                    System.Convert.ToInt32(lecturaItem[4]),
                    System.Convert.ToInt32(lecturaItem[5])
                );

                items.Add(nuevoItem);
            }
        }
    }

    private void CrearIconos()
    {
        botonesItems = new List<GameObject>();

        Transform nuevoPadre;

        for(int i = 0; i < precios.Count; i++)
        {
            switch(items[i].Categoria)
            {
                case "Pecho":
                    nuevoPadre = contenidoPestañaPecho.transform;
                    break;
                case "Piernas":
                    nuevoPadre = contenidoPestañaPiernas.transform;
                    break;
                case "Accesorio":
                    nuevoPadre = contenidoPestañaAccesorios.transform;
                    break;

                default:
                    nuevoPadre = null;
                    break;
            }
            if(nuevoPadre == null)
            {
                Debug.Log($"Categoria del item de id: {items[i].ID} es inválida ({items[i].Categoria})");
            }
            else
            {
                GameObject nuevoIcono = Instantiate(iconoPrefab, nuevoPadre);

                Sprite nuevoSprite = BuscarImagenPorID(items[i].IDIcono);

                nuevoIcono.GetComponent<IconoTienda>().Actualizar(items[i].ID, precios[i].Monto ,nuevoSprite, this);

                botonesItems.Add(nuevoIcono);
            }
        }
    }

    private void ActualizarBotones()
    {
        CargarItemsAdquiridos();

        foreach(int itemID in IDsItemsAdquiridos)
        {
            GameObject boton = EncontrarBotonPorID(itemID);

            if(boton != null)
            {
                boton.GetComponent<IconoTienda>().Adquirir();
            }
        }

        foreach(GameObject boton in botonesItems)
        {
            IconoTienda scriptBoton = boton.GetComponent<IconoTienda>();

            if(!scriptBoton.Adquirido)
            {
                scriptBoton.CambiarComprabilidad(scriptBoton.Monto <= monedasDisponibles);
            }
        }
    }

    private GameObject EncontrarBotonPorID(int itemIDABuscar)
    {
        foreach(GameObject boton in botonesItems)
        {
            if(boton.GetComponent<IconoTienda>().ItemID == itemIDABuscar)
            {
                return boton;
            }
        }

        return null;
    }

    private Sprite BuscarImagenPorID(int idIcono)
    {
        return Resources.Load<Sprite>($"Placeholders/IconosRopa/{idIcono}");
    }

    private Item BuscarItemPorID(int itemID)
    {
        foreach(Item item in items)
        {
            if(item.ID == itemID)
            {
                return item;
            }
        }

        return null;
    }

    private Precio BuscarPrecioPorID(int itemID)
    {
        foreach(Precio precio in precios)
        {
            if(precio.IDItem == itemID)
            {
                return precio;
            }
        }

        return null;
    }

    public void NuevoIconoSeleccionado(int itemID, bool comprable, bool adquirido)
    {
        if(miniPantallaActiva)
        {
            return;
        }

        itemSeleccionado = BuscarItemPorID(itemID);
        precioSeleccionado = BuscarPrecioPorID(itemID);

        if(itemSeleccionado == null || precioSeleccionado == null)
        {
            Debug.Log("El item seleccionado no fue encontrado.");

            itemSeleccionado = null;
            precioSeleccionado = null;
        }
        else
        {
            txtNombre.text = itemSeleccionado.Nombre;
            txtDescripcion.text = itemSeleccionado.Descripcion;
            txtMontoAPagar.text = precioSeleccionado.Monto.ToString();

            txtEstado.text = "";
            txtEstado.color = Color.white;


            if(adquirido)
            {
                txtEstado.text = "Item ya adquirido!";
                txtEstado.color = Color.green;
            }
            else if(!comprable)
            {
                txtEstado.text = "Fondos insuficientes!";
                txtEstado.color = Color.red;
            }

            btnComprar.interactable = comprable;
        }
    }
}
