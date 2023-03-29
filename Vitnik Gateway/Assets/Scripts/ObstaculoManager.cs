using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoManager : MonoBehaviour
{
    [SerializeField] private float alturaPrimerPiso;
    [SerializeField] private float alturaSegundoPiso;
    [SerializeField] private float distancia;
    [SerializeField] private float margen;
    [SerializeField] private float unidad;
    [SerializeField] private float distanciaSiguientePista;

    [SerializeField] private float probabilidadSpawnMultiple;
    [SerializeField] private float posicionUltimoGrupo;

    [SerializeField] private GameObject prefabObstaculo;
    [SerializeField] private int tamañoPool;
    [SerializeField] private GameObject contenedorObstaculos;

    private List<GameObject> poolObstaculos;


    void Start()
    {
        poolObstaculos = new List<GameObject>();

        for(int i = 0; i < tamañoPool; i++)
        {
            AgregarObstaculoAlPool("caja " + i);            
        }
    }

    private GameObject AgregarObstaculoAlPool(string nombre)
    {
        GameObject obstaculo = Instantiate(prefabObstaculo, Vector3.zero, prefabObstaculo.transform.rotation);
        obstaculo.SetActive(false);
        obstaculo.transform.SetParent(contenedorObstaculos.transform);
        poolObstaculos.Add(obstaculo);

        obstaculo.name = nombre;

        //Debug.Log("Obstaculo creado.");

        return obstaculo;
    }

    private GameObject ObtenerObstaculoDesactivado()
    {
        foreach(GameObject obstaculo in poolObstaculos)
        {
            if(!obstaculo.activeSelf)
            {
                return obstaculo;
            }
        }

        return null;
    }

    private GameObject PosicionarObstaculo(GameObject obstaculo, Vector3 posicion, Quaternion rotacion)
    {
        obstaculo.transform.position = posicion;
        obstaculo.transform.rotation = rotacion;
        obstaculo.SetActive(true);
        return obstaculo;
    }

    private float DevolverEnUnidad(float valor)
    {
        return valor - (valor % unidad);
    }

    public void SpawnearObstaculos(GameObject pista, GameObject siguientePista)
    {
        BehaviourPista scriptPista = pista.GetComponentInChildren<BehaviourPista>();
        BehaviourPista scriptSiguientePista = siguientePista.GetComponentInChildren<BehaviourPista>();

        List<GrupoObstaculos> gruposObstaculos = new List<GrupoObstaculos>();

        float numeroRandom;

        float margenAleatorio = DevolverEnUnidad(Random.Range(0,margen));

        while((pista.transform.position.z + scriptPista.Longitud / 2) - posicionUltimoGrupo > distancia + margenAleatorio)
        {

            GrupoObstaculos nuevoGrupo = new GrupoObstaculos(posicionUltimoGrupo + distancia + margenAleatorio, new List<LugarObstaculo>(), new List<Transform>());

            if(posicionUltimoGrupo + distancia < siguientePista.transform.position.z - scriptSiguientePista.Longitud / 2 - distanciaSiguientePista)
            {
                nuevoGrupo.Carriles = siguientePista.GetComponentInChildren<BehaviourListaCarriles>().Carriles;
            }
            else
            {
                nuevoGrupo.Carriles = pista.GetComponentInChildren<BehaviourListaCarriles>().Carriles;
            }

            for(int i = 0; i < nuevoGrupo.Carriles.Count * 2; i++)
            {
                LugarObstaculo nuevoLugar = new LugarObstaculo();

                nuevoLugar.Carril = i / 2;

                if( i % 2 == 1)
                {
                    nuevoLugar.Altura = Altura.Arriba;
                }
                else
                {
                    nuevoLugar.Altura = Altura.Abajo;
                }

                nuevoGrupo.Lugares.Add(nuevoLugar);
            }

            LugarObstaculo lugarLibreAleatorio = nuevoGrupo.DevolverLugarLibreAleatorio();

            if(ObtenerObstaculoDesactivado() == null)
            {
                lugarLibreAleatorio.Obstaculo = AgregarObstaculoAlPool("");
            }
            else
            {
                lugarLibreAleatorio.Obstaculo = ObtenerObstaculoDesactivado();
            }

            lugarLibreAleatorio.Obstaculo.SetActive(true);
            
            for(int i = 0; i < nuevoGrupo.Lugares.Count - 1; i++)
            {
                numeroRandom = Random.Range(0f,1f);

                if(numeroRandom < probabilidadSpawnMultiple)
                {
                    lugarLibreAleatorio = nuevoGrupo.DevolverLugarLibreAleatorio();

                    if(ObtenerObstaculoDesactivado() == null)
                    {
                        lugarLibreAleatorio.Obstaculo = AgregarObstaculoAlPool("");
                    }
                    else
                    {
                        lugarLibreAleatorio.Obstaculo = ObtenerObstaculoDesactivado();
                    }

                    lugarLibreAleatorio.Obstaculo.SetActive(true);
                }
            }

            foreach(LugarObstaculo lugar in nuevoGrupo.Lugares)
            {
                if(!lugar.Libre)
                {
                    float posicionVertical;

                    if(lugar.Altura == Altura.Arriba)
                    {
                        posicionVertical = alturaSegundoPiso;
                    }
                    else
                    {
                        posicionVertical = alturaPrimerPiso;
                    }

                    PosicionarObstaculo(lugar.Obstaculo, new Vector3(nuevoGrupo.Carriles[lugar.Carril].position.x, posicionVertical, nuevoGrupo.Posicion) , prefabObstaculo.transform.rotation );
                }
            }

            posicionUltimoGrupo = nuevoGrupo.Posicion;

            gruposObstaculos.Add(nuevoGrupo);

            margenAleatorio = DevolverEnUnidad(Random.Range(0,margen));

            //Debug.Log(distancia + margenAleatorio);
        }

        scriptPista.gruposObstaculos = gruposObstaculos;
    }
}
