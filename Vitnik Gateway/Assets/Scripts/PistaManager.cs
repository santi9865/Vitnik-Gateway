using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistaManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> pistas;
    [SerializeField] private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CircularPistas()
    {
        GameObject pistaRemovida = pistas[0];
        Vector3 posicionUltimaPista = pistas[pistas.Count - 1].transform.position;

        pistaRemovida.SetActive(false);
        pistas.RemoveAt(0);

        pistaRemovida.transform.position = posicionUltimaPista + offset;

        pistas.Add(pistaRemovida);

        pistaRemovida.SetActive(true);
    }
}
