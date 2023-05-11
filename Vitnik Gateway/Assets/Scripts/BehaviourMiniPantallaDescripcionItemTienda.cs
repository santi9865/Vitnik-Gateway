using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMiniPantallaDescripcionItemTienda : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Mostrar(string descripcion, string monto)
    {
        gameObject.SetActive(true);
    }

    public void Ocultar()
    {
        gameObject.SetActive(false);
    }
}
