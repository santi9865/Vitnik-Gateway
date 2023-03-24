using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaculoManager : MonoBehaviour
{
    [SerializeField] private float alturaPrimerPiso;
    [SerializeField] private float alturaSegundoPiso;
    [SerializeField] private float distancia;
    [SerializeField] private float distanciaSiguientePista;

    [SerializeField] private float probabilidadSpawnMultiple;
    [SerializeField] private Vector3 posicionUltimoObstaculo;

    [SerializeField] private GameObject prefabObstaculo;
    [SerializeField] private int tamañoPool;
    [SerializeField] private GameObject contenedorObstaculos;

    private List<GameObject> poolObstaculos;


    void Start()
    {
        poolObstaculos = new List<GameObject>();

        for(int i = 0; i < tamañoPool; i++)
        {
            AgregarObstaculoAlPool();
        }
    }

    private GameObject AgregarObstaculoAlPool()
    {
            GameObject obstaculo = Instantiate(prefabObstaculo, Vector3.zero, prefabObstaculo.transform.rotation);
            obstaculo.SetActive(false);
            obstaculo.transform.SetParent(contenedorObstaculos.transform);
            poolObstaculos.Add(obstaculo);

            Debug.Log("Obstaculo creado.");

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

    public void SpawnearObstaculo(GameObject pista, GameObject siguientePista)
    {
        BehaviourPista scriptPista = pista.GetComponentInChildren<BehaviourPista>();
        BehaviourPista scriptSiguientePista = siguientePista.GetComponentInChildren<BehaviourPista>();

        List<GameObject> obstaculosAsociados = new List<GameObject>();

        List<Transform> carrilesAConsiderar;
        List<int> lugaresConsiderados = new List<int>();
        int lugarRandom;

        float numeroRandom;

        Transform carrilSeleccionado;
        float posicionVertical;

        GameObject ultimoObstaculoPosicionado;

        while((pista.transform.position.z + scriptPista.Longitud / 2) - posicionUltimoObstaculo.z > distancia)
        {
            if(posicionUltimoObstaculo.z + distancia < siguientePista.transform.position.z - scriptSiguientePista.Longitud / 2 - distanciaSiguientePista)
            {
                carrilesAConsiderar = siguientePista.GetComponentInChildren<BehaviourListaCarriles>().Carriles;
            }
            else
            {
                carrilesAConsiderar = pista.GetComponentInChildren<BehaviourListaCarriles>().Carriles;
            }

            for(int i = 0; i < carrilesAConsiderar.Count * 2; i++)
            {
                lugaresConsiderados.Add(i);
            }

            lugarRandom = Random.Range(0, lugaresConsiderados.Count);

            carrilSeleccionado = carrilesAConsiderar[lugaresConsiderados[lugarRandom] / 2];

            //Debug.Log(lugarRandom);
            //Debug.Log(lugaresConsiderados[lugarRandom] / 2);

            if(lugaresConsiderados[lugarRandom] % 2 == 1)
            {
                posicionVertical = alturaSegundoPiso;
            }
            else
            {
                posicionVertical = alturaPrimerPiso;
            } 

            ultimoObstaculoPosicionado = ObtenerObstaculoDesactivado();

            if(ultimoObstaculoPosicionado == null)
            {
                ultimoObstaculoPosicionado = AgregarObstaculoAlPool();
            }
            
            obstaculosAsociados.Add(ultimoObstaculoPosicionado);

            PosicionarObstaculo(ultimoObstaculoPosicionado, new Vector3(carrilSeleccionado.position.x, posicionVertical, posicionUltimoObstaculo.z + distancia), prefabObstaculo.transform.rotation);

            lugaresConsiderados.RemoveAt(lugarRandom);

            for(int i = 0; i < lugaresConsiderados.Count - 1; i++)
            {
                lugarRandom = Random.Range(0, lugaresConsiderados.Count);

                numeroRandom = Random.Range(0f,1f);

                if(numeroRandom < probabilidadSpawnMultiple)
                {
                    carrilSeleccionado = carrilesAConsiderar[lugaresConsiderados[lugarRandom] / 2];

                    if(lugaresConsiderados[lugarRandom] % 2 == 1)
                    {
                        posicionVertical = alturaSegundoPiso;
                    }
                    else
                    {
                        posicionVertical = alturaPrimerPiso;
                    } 

                    ultimoObstaculoPosicionado = ObtenerObstaculoDesactivado();

                    if(ultimoObstaculoPosicionado == null)
                    {
                        ultimoObstaculoPosicionado = AgregarObstaculoAlPool();
                    }
                    
                    obstaculosAsociados.Add(ultimoObstaculoPosicionado);

                    PosicionarObstaculo(ultimoObstaculoPosicionado, new Vector3(carrilSeleccionado.position.x, posicionVertical, posicionUltimoObstaculo.z + distancia), prefabObstaculo.transform.rotation);
                }

                lugaresConsiderados.RemoveAt(lugarRandom);
            }

            posicionUltimoObstaculo =  ultimoObstaculoPosicionado.transform.position;

            scriptPista.obstaculosAsociados = obstaculosAsociados;
        }
    }
}
