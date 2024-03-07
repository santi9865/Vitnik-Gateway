using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistaManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> pistas;
    //Cantidad de pistas que no sean intersecciones que deben 
    //spawnear antes de que sea posible spawnear una pista de intersección.
    [SerializeField] private int pistasMinimasInterseccion;
    //Probailidad de que se spawnee una pista de intersección 
    //una vez que se cumpla con el mínimo de pistas no intersecciones.
    [SerializeField] private float probabilidadInterseccion;
    [SerializeField] private int pistasPorRama;

    //Cuantas pistas va a haber sin obstáculos desde el comienzo de una ramificacion.
    [SerializeField] private int pistasDeGraciaInterseccion;

    //Que pista de la lista debería pasarse antes de borrar la última.
    [SerializeField] private int estelaDePistas;
    //Cantidad mínima de pistas no rotas que deben spawnear 
    //antes de que sea posible spawnear una pista rota.
    [SerializeField] private int pistasMinimasRotura;

    //Probabilidad de que spawnee una pista rota de inicio.
    [SerializeField] private float probabilidadDePistaRota;
    //Probabilidad de que la rotura spawnee otra pista rota despues de otra pista rota.
    [SerializeField] private float probabilidadContinuarRotura;
    //Cuenta la cantidad de pistas que han transcurrido desde la última intersección.
    private int contadorInterseccion = 0;
    //Cuenta la cantidad de pistas que han transcurrido desde la última pista rota.
    private int contadorRotura = 0;

    [SerializeField] private PistaFactory pistaFactory;

    private List<GameObject> sendaPrincipal;
    private Eje ejePrincipal;
    [SerializeField] private ObstaculoManager obstaculoManager;
    [SerializeField] private MonedaManager monedaManager;

    private int contadorPistas = 8;

    [SerializeField] private bool debug;

    void Start()
    {
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

        pistaDeInflexion.GetComponent<BehaviourPista>().pistasAsociadas = nuevasPistasAsociadas;

        contadorInterseccion = 0;

        obstaculoManager.JugadorDoblo(nuevaRama.TipoRama);
        monedaManager.JugadorDoblo(nuevaRama.TipoRama);
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

        TipoPista tipoNuevaPista;

        //Si el tipo de pista tiene un final abrupto entonces no pone una pista nueva;
        switch(scriptUltimaPista.Tipo)
        {
            case TipoPista.InterT:
            case TipoPista.InterLDerecha:
            case TipoPista.InterLIzquierda:
                return;
            case TipoPista.RectaRotaInicioDerecha:
            case TipoPista.RectaRotaDerecha:
                if(probabilidadContinuarRotura >= Random.Range(0, 1f))
                {
                    tipoNuevaPista = TipoPista.RectaRotaDerecha;
                }
                else
                {
                    tipoNuevaPista = TipoPista.RectaRotaFinDerecha;
                }
                break;
            case TipoPista.RectaRotaInicioIzquierda:
            case TipoPista.RectaRotaIzquierda:
                if(probabilidadContinuarRotura >= Random.Range(0, 1f))
                {
                    tipoNuevaPista = TipoPista.RectaRotaIzquierda;
                }
                else
                {
                    tipoNuevaPista = TipoPista.RectaRotaFinIzquierda;
                }
                break;
            default:
                tipoNuevaPista = TipoPista.Recta;
                if(contadorInterseccion >= pistasMinimasInterseccion)
                {
                    if(probabilidadInterseccion >= Random.Range(0, 1f))
                    {
                        switch(Random.Range(0,1f))
                        {
                            case > 0.90f:
                                tipoNuevaPista = TipoPista.InterCruz;
                                break;
                            case > 0.80f:
                                tipoNuevaPista = TipoPista.InterLDerecha;
                                break;
                            case > 0.70f:
                                tipoNuevaPista = TipoPista.InterLIzquierda;
                                break;
                            case > 0.60f:
                                tipoNuevaPista = TipoPista.InterDerecha;
                                break;
                            case > 0.50f:
                                tipoNuevaPista = TipoPista.InterIzquierda;
                                break;
                            case > 0.40F:
                                tipoNuevaPista = TipoPista.InterLDerecha;
                                break;
                            case > 0.30F:
                                tipoNuevaPista = TipoPista.InterLIzquierda;
                                break;
                            default:
                                tipoNuevaPista = TipoPista.InterT;
                                break;
                        }
                    }
                }
                if(contadorRotura >= pistasMinimasRotura)
                {
                    if(probabilidadDePistaRota >= Random.Range(0, 1f))
                    {
                        switch(Random.Range(0,1f))
                        {
                            case >= 0.5f:
                                tipoNuevaPista = TipoPista.RectaRotaInicioIzquierda;
                                break;
                            default:
                                tipoNuevaPista = TipoPista.RectaRotaInicioDerecha;
                                break;
                        }
                    }
                }

                break;
        }

        GameObject nuevaPista = ObtenerPistaDesactivada(tipoNuevaPista);
        SpawnearPista(nuevaPista, ejePrincipal, ultimaPista.transform.position + (scriptUltimaPista.Longitud / 2) * ejePrincipal.Vectorizado);
        sendaPrincipal.Add(nuevaPista);

        //Se agrega una pista al final, pero se pobla la anterior, porque hace falta saber el tipo de la pista siguiente
        //para poner bien los obstaculos.

        switch(scriptUltimaPista.Tipo)
        {
            case TipoPista.Recta:
            case TipoPista.RectaRotaDerecha:
            case TipoPista.RectaRotaIzquierda:
            case TipoPista.RectaRotaInicioDerecha:
            case TipoPista.RectaRotaInicioIzquierda:
            case TipoPista.RectaRotaFinDerecha:
            case TipoPista.RectaRotaFinIzquierda:
                GameObject penultimaPista = null;
                GameObject antepenultimaPista = null;

                if(sendaPrincipal.Count > 2)
                {
                    penultimaPista = sendaPrincipal[sendaPrincipal.Count - 3];
                }

                if(sendaPrincipal.Count > 3)
                {
                    antepenultimaPista = sendaPrincipal[sendaPrincipal.Count - 4];
                }

                obstaculoManager.SpawnearObstaculos(penultimaPista, ultimaPista, nuevaPista);
                monedaManager.SpawnearMonedasSendaPrincipal(antepenultimaPista, penultimaPista, ultimaPista);
                break;
        }
    }

    //Agrega pistas a las ramas de una pista base.
    private void PoblarRamas(GameObject pistaBase)
    {
        BehaviourPista scripPistaBase = pistaBase.GetComponent<BehaviourPista>();    

        if(scripPistaBase.RamaDerecha != null)
        {
            Rama scriptRamaDerecha = scripPistaBase.RamaDerecha.GetComponent<Rama>();
            scriptRamaDerecha.AlinearSegunPadre(scripPistaBase.EjeMovimiento);

            Vector3 posicionInicial = pistaBase.transform.position + (scripPistaBase.Longitud / 2) * scriptRamaDerecha.EjeMovimiento.Vectorizado;

            scriptRamaDerecha.Pistas.Add(SpawnearPista(ObtenerPistaDesactivada(), scriptRamaDerecha.EjeMovimiento, posicionInicial, true));
            obstaculoManager.RestablecerUltimoObstaculo(posicionInicial, TipoRama.Derecha);

            for(int i = 1; i < pistasPorRama; i++)
            {
                GameObject ultimaPista = scriptRamaDerecha.Pistas[scriptRamaDerecha.Pistas.Count - 1];
                BehaviourPista scriptUltimaPista = ultimaPista.GetComponent<BehaviourPista>();

                TipoPista tipoNuevaPista;

                switch(scriptUltimaPista.Tipo)
                {
                    case TipoPista.InterT:
                    case TipoPista.InterLDerecha:
                    case TipoPista.InterLIzquierda:
                        return;
                    case TipoPista.RectaRotaInicioDerecha:
                    case TipoPista.RectaRotaDerecha:
                        if(probabilidadContinuarRotura >= Random.Range(0, 1f))
                        {
                            tipoNuevaPista = TipoPista.RectaRotaDerecha;
                        }
                        else
                        {
                            tipoNuevaPista = TipoPista.RectaRotaFinDerecha;
                        }
                        break;
                    case TipoPista.RectaRotaInicioIzquierda:
                    case TipoPista.RectaRotaIzquierda:
                        if(probabilidadContinuarRotura >= Random.Range(0, 1f))
                        {
                            tipoNuevaPista = TipoPista.RectaRotaIzquierda;
                        }
                        else
                        {
                            tipoNuevaPista = TipoPista.RectaRotaFinIzquierda;
                        }
                        break;
                    default:
                        tipoNuevaPista = TipoPista.Recta;
                        if(probabilidadDePistaRota >= Random.Range(0, 1f))
                        {
                            switch(Random.Range(0,1f))
                            {
                                case >= 0.5f:
                                    tipoNuevaPista = TipoPista.RectaRotaInicioIzquierda;
                                    break;
                                default:
                                    tipoNuevaPista = TipoPista.RectaRotaInicioDerecha;
                                    break;
                            }
                        }
                        break;
                }

                GameObject nuevaPista = ObtenerPistaDesactivada(tipoNuevaPista);
                SpawnearPista(nuevaPista, scriptRamaDerecha.EjeMovimiento, ultimaPista.transform.position + (scriptUltimaPista.Longitud / 2) * scriptRamaDerecha.EjeMovimiento.Vectorizado, true);
                scriptRamaDerecha.Pistas.Add(nuevaPista);

                switch(scriptUltimaPista.Tipo)
                {
                    case TipoPista.Recta:
                    case TipoPista.RectaRotaDerecha:
                    case TipoPista.RectaRotaIzquierda:
                    case TipoPista.RectaRotaInicioDerecha:
                    case TipoPista.RectaRotaInicioIzquierda:
                    case TipoPista.RectaRotaFinDerecha:
                    case TipoPista.RectaRotaFinIzquierda:
                        GameObject penultimaPista = null;
                        GameObject antepenultimaPista = null;

                        if(scriptRamaDerecha.Pistas.Count > 2)
                        {
                            penultimaPista = scriptRamaDerecha.Pistas[scriptRamaDerecha.Pistas.Count - 3];
                        }

                        if(scriptRamaDerecha.Pistas.Count > 3)
                        {
                            antepenultimaPista = scriptRamaDerecha.Pistas[scriptRamaDerecha.Pistas.Count - 4];
                        }

                        if(i > 2)
                        {
                            obstaculoManager.SpawnearObstaculos(penultimaPista, ultimaPista, nuevaPista, TipoRama.Derecha);
                            monedaManager.SpawnearMonedasRamaDerecha(antepenultimaPista, penultimaPista, ultimaPista);
                        }
                        else if(i == 2)
                        {
                            obstaculoManager.SpawnearObstaculos(penultimaPista, ultimaPista, nuevaPista, TipoRama.Derecha);
                            monedaManager.SpawnearMonedasRamaDerecha(pistaBase, penultimaPista, ultimaPista, true);
                        }
                        break;
                }
            }   
        }

        if(scripPistaBase.RamaIzquierda != null)
        {
            Rama scriptRamaIzquierda = scripPistaBase.RamaIzquierda.GetComponent<Rama>();
            scriptRamaIzquierda.AlinearSegunPadre(scripPistaBase.EjeMovimiento);

            Vector3 posicionInicial = pistaBase.transform.position + (scripPistaBase.Longitud / 2) * scriptRamaIzquierda.EjeMovimiento.Vectorizado;

            scriptRamaIzquierda.Pistas.Add(SpawnearPista(ObtenerPistaDesactivada(), scriptRamaIzquierda.EjeMovimiento, posicionInicial, true));
            obstaculoManager.RestablecerUltimoObstaculo(posicionInicial, TipoRama.Izquierda);

            for(int i = 1; i < pistasPorRama; i++)
            {
                GameObject ultimaPista = scriptRamaIzquierda.Pistas[scriptRamaIzquierda.Pistas.Count - 1];
                BehaviourPista scriptUltimaPista = ultimaPista.GetComponent<BehaviourPista>();

                TipoPista tipoNuevaPista;

                switch(scriptUltimaPista.Tipo)
                {
                    case TipoPista.InterT:
                    case TipoPista.InterLDerecha:
                    case TipoPista.InterLIzquierda:
                        return;
                    case TipoPista.RectaRotaInicioDerecha:
                    case TipoPista.RectaRotaDerecha:
                        if(probabilidadContinuarRotura >= Random.Range(0, 1f))
                        {
                            tipoNuevaPista = TipoPista.RectaRotaDerecha;
                        }
                        else
                        {
                            tipoNuevaPista = TipoPista.RectaRotaFinDerecha;
                        }
                        break;
                    case TipoPista.RectaRotaInicioIzquierda:
                    case TipoPista.RectaRotaIzquierda:
                        if(probabilidadContinuarRotura >= Random.Range(0, 1f))
                        {
                            tipoNuevaPista = TipoPista.RectaRotaIzquierda;
                        }
                        else
                        {
                            tipoNuevaPista = TipoPista.RectaRotaFinIzquierda;
                        }
                        break;
                    default:
                        tipoNuevaPista = TipoPista.Recta;
                        if(probabilidadDePistaRota >= Random.Range(0, 1f))
                        {
                            switch(Random.Range(0,1f))
                            {
                                case >= 0.5f:
                                    tipoNuevaPista = TipoPista.RectaRotaInicioIzquierda;
                                    break;
                                default:
                                    tipoNuevaPista = TipoPista.RectaRotaInicioDerecha;
                                    break;
                            }
                        }
                        break;
                }

                GameObject nuevaPista = ObtenerPistaDesactivada(tipoNuevaPista);
                SpawnearPista(nuevaPista, scriptRamaIzquierda.EjeMovimiento, ultimaPista.transform.position + (scriptUltimaPista.Longitud / 2) * scriptRamaIzquierda.EjeMovimiento.Vectorizado, true);
                scriptRamaIzquierda.Pistas.Add(nuevaPista);

                switch(scriptUltimaPista.Tipo)
                {
                    case TipoPista.Recta:
                    case TipoPista.RectaRotaDerecha:
                    case TipoPista.RectaRotaIzquierda:
                    case TipoPista.RectaRotaInicioDerecha:
                    case TipoPista.RectaRotaInicioIzquierda:
                    case TipoPista.RectaRotaFinDerecha:
                    case TipoPista.RectaRotaFinIzquierda:
                        GameObject penultimaPista = null;
                        GameObject antepenultimaPista = null;

                        if(scriptRamaIzquierda.Pistas.Count > 2)
                        {
                            penultimaPista = scriptRamaIzquierda.Pistas[scriptRamaIzquierda.Pistas.Count - 3];
                        }

                        if(scriptRamaIzquierda.Pistas.Count > 3)
                        {
                            antepenultimaPista = scriptRamaIzquierda.Pistas[scriptRamaIzquierda.Pistas.Count - 4];
                        }

                        if(i > 2)
                        {
                            obstaculoManager.SpawnearObstaculos(penultimaPista, ultimaPista, nuevaPista, TipoRama.Izquierda);
                            monedaManager.SpawnearMonedasRamaIzquierda(antepenultimaPista, penultimaPista, ultimaPista);
                        }
                        else if(i == 2)
                        {
                            obstaculoManager.SpawnearObstaculos(penultimaPista, ultimaPista, nuevaPista, TipoRama.Izquierda);
                            monedaManager.SpawnearMonedasRamaIzquierda(pistaBase, penultimaPista, ultimaPista, true);
                        }
                        break;
                }
            }   
        }
    }

    private GameObject SpawnearPista(GameObject pistaASpawnear, Eje ejeOrientador, Vector3 puntoInicio, bool ignorarContadores = false)
    {
        BehaviourPista scriptPistaASpawnear = pistaASpawnear.GetComponent<BehaviourPista>();

        scriptPistaASpawnear.ReiniciarEje();
        scriptPistaASpawnear.PistaManager = this;
        pistaASpawnear.transform.position = puntoInicio + (scriptPistaASpawnear.Longitud / 2) * ejeOrientador.Vectorizado;
        pistaASpawnear.SetActive(true);
        pistaASpawnear.name = "pista " + contadorPistas;
        contadorPistas ++;

        //Rotar pista según su eje y el eje orientador.
        float anguloRotacion = scriptPistaASpawnear.EjeMovimiento.AngulosA(ejeOrientador);
        pistaASpawnear.transform.Rotate(0,anguloRotacion, 0, Space.World);
        scriptPistaASpawnear.EjeMovimiento = new Eje(ejeOrientador.Direccion, ejeOrientador.Sentido);

        switch(scriptPistaASpawnear.Tipo)
        {
            case TipoPista.InterIzquierda:
            case TipoPista.InterDerecha:
            case TipoPista.InterT:
            case TipoPista.InterLIzquierda:
            case TipoPista.InterLDerecha:
            case TipoPista.InterCruz:
                PoblarRamas(pistaASpawnear);
                contadorInterseccion = 0;

                if(debug)
                {
                    Debug.Log("ContadorInterseccion: " + contadorInterseccion);
                }

                if(!ignorarContadores)
                {
                    contadorRotura++;
                }
                break;
            case TipoPista.RectaRotaDerecha:
            case TipoPista.RectaRotaIzquierda:
            case TipoPista.RectaRotaInicioDerecha:
            case TipoPista.RectaRotaInicioIzquierda:
            case TipoPista.RectaRotaFinDerecha:
            case TipoPista.RectaRotaFinIzquierda:
                contadorRotura = 0;
                if(!ignorarContadores)
                {
                    contadorInterseccion++;
                }
                break;
            default:
                if(!ignorarContadores)
                {
                    contadorInterseccion++;
                    contadorRotura++;
                }
                break;
        }

        return pistaASpawnear;
    }
}
