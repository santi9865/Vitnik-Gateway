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

    public void SpawnearObstaculo(GameObject pista, GameObject siguientePista)
    {
        BehaviourPista scriptPista = pista.GetComponentInChildren<BehaviourPista>();
        BehaviourPista scriptSiguientePista = siguientePista.GetComponentInChildren<BehaviourPista>();

        List<Transform> carrilesAConsiderar;
        List<int> lugaresConsiderados = new List<int>();
        int lugarRandom;

        float numeroRandom;

        Transform carrilSeleccionado;
        float posicionVertical;

        GameObject ultimoObstaculoCreado;

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

            Debug.Log(lugarRandom);
            Debug.Log(lugaresConsiderados[lugarRandom] / 2);

            if(lugaresConsiderados[lugarRandom] % 2 == 1)
            {
                posicionVertical = alturaSegundoPiso;
            }
            else
            {
                posicionVertical = alturaPrimerPiso;
            } 

            ultimoObstaculoCreado = Instantiate(prefabObstaculo, new Vector3(carrilSeleccionado.position.x, posicionVertical, posicionUltimoObstaculo.z + distancia), prefabObstaculo.transform.rotation);

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

                    ultimoObstaculoCreado = Instantiate(prefabObstaculo, new Vector3(carrilSeleccionado.position.x, posicionVertical, posicionUltimoObstaculo.z + distancia), prefabObstaculo.transform.rotation);
                }

                lugaresConsiderados.RemoveAt(lugarRandom);
            }

            posicionUltimoObstaculo =  ultimoObstaculoCreado.transform.position;
        }
    }
}
