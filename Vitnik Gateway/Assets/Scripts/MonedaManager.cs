using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonedaManager : MonoBehaviour
{

    [SerializeField] private GameObject prefabMoneda;
    [SerializeField] private GameObject contenedorMonedas;
    [SerializeField] private float espaciado;
    [SerializeField] private int tamañoPool;

    private GameObject ultimaMonedaSpawneada;

    private List<GameObject> poolMonedas;


    // Start is called before the first frame update
    void Start()
    {
        poolMonedas = new List<GameObject>();

        for(int i = 0 ;i< tamañoPool; i++)
        {
            AgregarMonedaAlPool();
        }
    }

    private GameObject AgregarMonedaAlPool()
    {
        GameObject moneda = Instantiate(prefabMoneda, Vector3.zero, prefabMoneda.transform.rotation);
        moneda.SetActive(false);
        moneda.transform.SetParent(contenedorMonedas.transform);

        poolMonedas.Add(moneda);

        //Debug.Log("Moneda Creada");

        return moneda;
    }

    private GameObject ObtenerMonedaDesactivada()
    {
        foreach(GameObject moneda in poolMonedas)
        {
            if(!moneda.activeSelf)
            {
                return moneda;
            }
        }

        return null;
    }

    private GameObject PosicionarMoneda(GameObject moneda, Vector3 posicion, Quaternion rotacion)
    {
        moneda.transform.position = posicion;
        moneda.transform.rotation = rotacion;
        moneda.SetActive(true);
        return moneda;
    }




}
