using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourMovimientoJugador : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    private Rigidbody rb;
    [SerializeField] private bool vivo = true;
    [SerializeField] private List<Transform> carriles;
    [SerializeField] private int carrilActual = 1;
    [SerializeField] private BehaviourMiniPantallas scriptMiniPantallas;

    [SerializeField] private Vector3 centroColliderNormal;
    [SerializeField] private Vector3 tamañoColliderNormal;
    [SerializeField] private Vector3 centroColliderDesliz;
    [SerializeField] private Vector3 tamañoColliderDesliz;
    [SerializeField] private float tiempoMaximoDesliz;
    private float timerDesliz;
    private bool deslizando;

    private BehaviourPlayerCollisionDetector detectorColisiones;

    //Debug Variables

    [SerializeField] private float alturaMaxima = 0;

    //End of debug variables

    void Start()
    {
        detectorColisiones = gameObject.GetComponentInChildren<BehaviourPlayerCollisionDetector>();

        rb = gameObject.GetComponent<Rigidbody>();

        if(rb == null)
        {
            Debug.Log("Rigidbody not found.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(vivo)
        {
            MoverseHaciaAdelante();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Saltar();
            }

            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoverseALaDerecha();
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoverseALaIzquierda();
            }
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                Deslizar();
            }

            if(deslizando)
            {
                timerDesliz -= Time.deltaTime;

                if(timerDesliz <= 0)
                {
                    Levantar();
                }
            }


        }
    }

    private void Deslizar()
    {
        //Solo se actualiza el collider si no se está deslizando actualmente.
        if(!deslizando)
        {
            detectorColisiones.ActualizarCollider(centroColliderDesliz, tamañoColliderDesliz);
        }

        timerDesliz = tiempoMaximoDesliz;

        deslizando = true;
    }

    private void Levantar()
    {
        detectorColisiones.ActualizarCollider(centroColliderNormal, tamañoColliderNormal);

        deslizando = false;
    }


    private void MoverseALaDerecha()
    {
        if (carrilActual < carriles.Count - 1)
        {
            gameObject.transform.position = new Vector3(carriles[carrilActual + 1].position.x, 
            gameObject.transform.position.y, gameObject.transform.position.z);
            carrilActual++;
        }
    }

    private void MoverseALaIzquierda()
    {
        if (carrilActual > 0)
        {
            gameObject.transform.position = new Vector3(carriles[carrilActual - 1].position.x, 
            gameObject.transform.position.y, gameObject.transform.position.z);
            carrilActual--;
        }
    }

    private void MoverseHaciaAdelante()
    {
        gameObject.transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void Saltar()
    {
        Levantar();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    //Este m�todo es llamado por el script del detector de colisiones del jugador
    public void ColisionObstaculo(ObstacleType tipoObstaculo)
    {
        switch (tipoObstaculo)
        {
            case ObstacleType.Solid:
                vivo = false;
                scriptMiniPantallas.PantallaGameOverSetActive(true);
                break;
            case ObstacleType.Moneda:
                GameManager.Instancia.AddMonedas(1);
                break;
        }
    }

    public void NuevaSeccionPista(GameObject seccion)
    {
        carriles = seccion.GetComponent<BehaviourListaCarriles>().Carriles;
    }

    //Start of debug methods

    private void DebugAltura(bool outputToConsole)
    {
        if(gameObject.transform.position.y > alturaMaxima)
        {
            alturaMaxima = gameObject.transform.position.y;

            if(outputToConsole)
            {
                Debug.Log(alturaMaxima);
            }
        }
    }

    //End of debug methods
}

