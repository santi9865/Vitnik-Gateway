using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourPista : MonoBehaviour
{
    [SerializeField] private PistaManager pistaManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {


        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Alguien entro al trigger");
            pistaManager.CircularPistas();
        }
    }
}
