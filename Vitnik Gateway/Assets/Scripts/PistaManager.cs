using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistaManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> pistas;
    [SerializeField] private Vector3 offset;

    private ObstaculoManager obstaculoManager;
    private MonedaManager monedaManager;

    // Start is called before the first frame update
    void Start()
    {
        obstaculoManager = GetComponent<ObstaculoManager>();
        monedaManager = GetComponent<MonedaManager>();
    }

    public void CircularPistas()
    {
        GameObject pistaRemovida = pistas[0];
        Vector3 posicionUltimaPista = pistas[pistas.Count - 1].transform.position;

        pistaRemovida.SetActive(false);
        pistaRemovida.GetComponentInChildren<BehaviourPista>().DesactivarObstaculosAsociados();
        pistaRemovida.GetComponentInChildren<BehaviourPista>().DesactivarMonedasAsociadas();
        pistas.RemoveAt(0);

        pistaRemovida.transform.position = posicionUltimaPista + offset;

        pistas.Add(pistaRemovida);

        pistaRemovida.SetActive(true);

        obstaculoManager.SpawnearObstaculos(pistas[pistas.Count - 2], pistas[pistas.Count - 1], pistas[0]);
        monedaManager.SpawnearMonedas(pistas[pistas.Count - 1]);
    }
}
