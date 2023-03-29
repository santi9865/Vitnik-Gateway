using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourPista : MonoBehaviour
{
    [SerializeField] private PistaManager pistaManager;
    [SerializeField] private float longitud;

    public List<GrupoObstaculos> gruposObstaculos;

    public float Longitud {get => longitud;}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DesactivarObstaculosAsociados()
    {
        if(gruposObstaculos != null)
        {
            foreach(GrupoObstaculos grupo in gruposObstaculos)
            {
                grupo.DesactivarObstaculos();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            pistaManager.CircularPistas();
        }
    }
}
