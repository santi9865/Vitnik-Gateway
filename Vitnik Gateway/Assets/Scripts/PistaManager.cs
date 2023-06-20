using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistaManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> pistas;
    [SerializeField] private Vector3 offset;
    [SerializeField] private int pistasMinimasInterseccion;
    [SerializeField] private float probabilidadInterseccion;
    [SerializeField] private int pistasPorRama;

    //Que pista de la lista debería pasarse antes de borrar la última.
    [SerializeField] private int estelaDePistas;

    //Cuenta la cantidad de pistas que han transcurrido desde la última intersección.
    private float contadorInterseccion;

    [SerializeField] private PistaFactory pistaFactory;

    private List<GameObject> sendaPrincipal;
    private Eje ejePrincipal;
    private ObstaculoManager obstaculoManager;
    private MonedaManager monedaManager;

    // Start is called before the first frame update
    void Start()
    {
        obstaculoManager = GetComponent<ObstaculoManager>();
        monedaManager = GetComponent<MonedaManager>();

        ClonarPistas();

        ejePrincipal = new Eje(EjeDireccion.Z, EjeSentido.Positivo);
    }

    private void ClonarPistas()
    {
        sendaPrincipal = new List<GameObject>();

        foreach(GameObject pista in pistas)
        {
            if(pista.activeSelf)
            {
                sendaPrincipal.Add(pista);
            }
        }
    }


    private GameObject ObtenerPistaDesactivada(TipoPista tipo = TipoPista.Recta)
    {
        foreach(GameObject pista in pistas)
        {
            BehaviourPista behaviourPista = pista.GetComponent<BehaviourPista>();

            if(pista.activeSelf == false && behaviourPista.Tipo == tipo)
            {
                return pista;
            }
        }

        GameObject nuevaPista = pistaFactory.CrearPista(tipo);
        pistas.Add(nuevaPista);
        nuevaPista.SetActive(false);

        return nuevaPista;
    }

    public void CambiarSendaPrincipal(GameObject pistaDeInflexion, Rama nuevaRama)
    {
        List<GameObject> nuevasPistasAsociadas = new List<GameObject>();

        int indicePistaInflexion = sendaPrincipal.IndexOf(pistaDeInflexion);

        for(int i = sendaPrincipal.Count - 1; i > indicePistaInflexion; i--)
        {
            nuevasPistasAsociadas.Add(sendaPrincipal[i]);
            sendaPrincipal.RemoveAt(i);
        }

        foreach(GameObject pista in nuevaRama.Pistas)
        {
            sendaPrincipal.Add(pista);
        }

        nuevaRama.Pistas.Clear();

        ejePrincipal = new Eje(nuevaRama.EjeMovimiento.Direccion, nuevaRama.EjeMovimiento.Sentido);

        contadorInterseccion = 0;
    }

    public void CircularPistas(GameObject pistaEntrante)
    {
        int pistasARemover = sendaPrincipal.IndexOf(pistaEntrante) + 1 - estelaDePistas;

        GameObject ultimaPista = sendaPrincipal[sendaPrincipal.Count - 1];
        BehaviourPista scriptUltimaPista = ultimaPista.GetComponent<BehaviourPista>();

        if(pistasARemover > 0)
        {
            for(int i = 0; i < pistasARemover ; i++)
            {
                GameObject pistaRemovida = sendaPrincipal[0];
                BehaviourPista scriptPista = pistaRemovida.GetComponent<BehaviourPista>();

                pistaRemovida.SetActive(false);
                scriptPista.DesactivarObstaculosAsociados();
                scriptPista.DesactivarMonedasAsociadas();
                scriptPista.DesactivarPistasAsociadas();
                scriptPista.LimpiarRamas();
                scriptPista.ReiniciarEje();
                sendaPrincipal.RemoveAt(0);
            }
        }

        //Si el tipo de pista tiene un final abrupto entonces no pone una pista nueva;
        switch(scriptUltimaPista.Tipo)
        {
            case TipoPista.InterT:
            case TipoPista.InterLDerecha:
            case TipoPista.InterLIzquierda:
                return;
        }

        TipoPista tipoNuevaPista = TipoPista.Recta;

        // if(contadorInterseccion >= pistasMinimasInterseccion)
        // {
        //     if(probabilidadInterseccion <= Random.Range(0, 1f))
        //     {
        //         switch(Random.Range(0,1f))
        //         {
        //             case > 0.90f:
        //                 tipoNuevaPista = TipoPista.InterCruz;
        //                 break;
        //             case > 0.80f:
        //                 tipoNuevaPista = TipoPista.InterLDerecha;
        //                 break;
        //             case > 0.70f:
        //                 tipoNuevaPista = TipoPista.InterLIzquierda;
        //                 break;
        //             case > 0.60f:
        //                 tipoNuevaPista = TipoPista.InterDerecha;
        //                 break;
        //             case > 0.50f:
        //                 tipoNuevaPista = TipoPista.InterIzquierda;
        //                 break;
        //             case > 0.40f:
        //                 tipoNuevaPista = TipoPista.InterT;
        //                 break;
        //         }
        //     }
        // }

        GameObject nuevaPista = ObtenerPistaDesactivada(tipoNuevaPista);
        SpawnearPista(nuevaPista, ejePrincipal, ultimaPista.transform.position + (scriptUltimaPista.Longitud / 2) * ejePrincipal.Vectorizado);
        sendaPrincipal.Add(nuevaPista);

        //Se agrega una pista al final, pero se pobla la anterior, porque hace falta saber el tipo de la pista siguiente
        //para poner bien los obstaculos.
        obstaculoManager.SpawnearObstaculos(sendaPrincipal[sendaPrincipal.Count - 3], sendaPrincipal[sendaPrincipal.Count - 2], sendaPrincipal[sendaPrincipal.Count - 1]);
        monedaManager.SpawnearMonedas(sendaPrincipal[sendaPrincipal.Count - 2], null);
    }

    //Agrega pistas a las ramas de una pista base.
    private void PoblarRamas(GameObject pistaBase)
    {
        BehaviourPista scripPistaBase = pistaBase.GetComponent<BehaviourPista>();

        if(scripPistaBase.RamaDerecha != null)
        {
            Rama scriptRamaDerecha = scripPistaBase.RamaDerecha.GetComponent<Rama>();
            scriptRamaDerecha.AlinearSegunPadre(scripPistaBase.EjeMovimiento);

            Vector3 posicionInicial = scripPistaBase.RamaDerecha.transform.position;

            for(int i = 0; i < pistasPorRama; i++)
            {
                GameObject nuevaPista = SpawnearPista(ObtenerPistaDesactivada(), scriptRamaDerecha.EjeMovimiento, posicionInicial);
                posicionInicial = nuevaPista.transform.position;
                scriptRamaDerecha.Pistas.Add(nuevaPista);
            }   
        }

        if(scripPistaBase.RamaIzquierda != null)
        {
            Rama scriptRamaIzquierda = scripPistaBase.RamaIzquierda.GetComponent<Rama>();
            scriptRamaIzquierda.AlinearSegunPadre(scripPistaBase.EjeMovimiento);

            Vector3 posicionInicial = scripPistaBase.RamaIzquierda.transform.position;

            for(int i = 0; i < pistasPorRama; i++)
            {
                GameObject nuevaPista = SpawnearPista(ObtenerPistaDesactivada(), scriptRamaIzquierda.EjeMovimiento, posicionInicial);
                posicionInicial = nuevaPista.transform.position;
                scriptRamaIzquierda.Pistas.Add(nuevaPista);
            } 
        }
    }

    private GameObject SpawnearPista(GameObject pistaASpawnear, Eje ejeOrientador, Vector3 puntoInicio)
    {
        BehaviourPista scripPistaASpawnear = pistaASpawnear.GetComponent<BehaviourPista>();

        scripPistaASpawnear.ReiniciarEje();
        pistaASpawnear.transform.position = puntoInicio + (scripPistaASpawnear.Longitud / 2) * ejeOrientador.Vectorizado;
        pistaASpawnear.SetActive(true);

        //Rotar pista según su eje y el eje orientador.
        float anguloRotacion = scripPistaASpawnear.EjeMovimiento.AngulosA(ejeOrientador);
        pistaASpawnear.transform.Rotate(0,anguloRotacion, 0);
        scripPistaASpawnear.EjeMovimiento = new Eje(ejeOrientador.Direccion, ejeOrientador.Sentido);

        switch(scripPistaASpawnear.Tipo)
        {
            case TipoPista.InterIzquierda:
            case TipoPista.InterDerecha:
            case TipoPista.InterT:
            case TipoPista.InterLIzquierda:
            case TipoPista.InterLDerecha:
            case TipoPista.InterCruz:
                PoblarRamas(pistaASpawnear);
                contadorInterseccion = 0;
                break;
            default:
                contadorInterseccion++;
                break;
        }

        return pistaASpawnear;
    }
}
