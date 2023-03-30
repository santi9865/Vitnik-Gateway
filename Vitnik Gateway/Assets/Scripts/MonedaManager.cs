using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonedaManager : MonoBehaviour
{

    [SerializeField] private GameObject prefabMoneda;
    [SerializeField] private GameObject contenedorMonedas;
    [SerializeField] private int tamañoPool;
    [SerializeField] private float espaciado;
    [SerializeField] private float alturaDesliz;

    [SerializeField] private BehaviourMovimientoJugador scriptJugador;

    [SerializeField] private GameObject carrilInicial;
    [SerializeField] private GameObject ultimaMonedaSpawneada;

    private float distanciaMaxima;

    private List<GrupoObstaculos> grupos;
    private int grupoActual;
    private LugarObstaculo lugarActual;
    private List<GameObject> poolMonedas;

    private float distanciaAlGrupoActual;
    private bool grupoActualAtravesado;
    private bool atravesandoObstaculo;
    private bool cercaAObstaculo;

    // Start is called before the first frame update
    void Start()
    {
        poolMonedas = new List<GameObject>();

        for(int i = 0 ;i< tamañoPool; i++)
        {
            AgregarMonedaAlPool();
        }

        if(scriptJugador.DistanciaSalto > scriptJugador.DistanciaDesliz)
        {
            distanciaMaxima = scriptJugador.DistanciaSalto;
        }
        else
        {
            distanciaMaxima = scriptJugador.DistanciaDesliz;
        }

        InicializarVariables();
    }

    private void InicializarVariables()
    {
        grupoActual = 0;
        grupos = new List<GrupoObstaculos>();
        lugarActual = new LugarObstaculo(0, Altura.Abajo, true, null);
        List<LugarObstaculo> listaLugaresInicial = new List<LugarObstaculo>();
        listaLugaresInicial.Add(lugarActual);
        List<GameObject> listaCarrilesInicial = new List<GameObject>();
        listaCarrilesInicial.Add(carrilInicial);
        grupos.Add(new GrupoObstaculos(0, listaLugaresInicial, listaCarrilesInicial));

        grupoActualAtravesado = true;
        atravesandoObstaculo = false;
        cercaAObstaculo = false;
    }

    private GameObject AgregarMonedaAlPool()
    {
        GameObject moneda = Instantiate(prefabMoneda, Vector3.zero, prefabMoneda.transform.rotation);
        moneda.SetActive(false);
        moneda.transform.SetParent(contenedorMonedas.transform);

        poolMonedas.Add(moneda);

        //Debug.Log("Moneda Creada");

        return moneda;
    }

    private GameObject ObtenerMonedaDesactivada()
    {
        foreach(GameObject moneda in poolMonedas)
        {
            if(!moneda.activeSelf)
            {
                return moneda;
            }
        }

        return AgregarMonedaAlPool();
    }

    private GameObject PosicionarMoneda(GameObject moneda, Vector3 posicion, Quaternion rotacion)
    {
        moneda.transform.position = posicion;
        moneda.transform.rotation = rotacion;
        moneda.SetActive(true);
        return moneda;
    }

    private void ActualizarDistanciaAlGrupoActual()
    {
        distanciaAlGrupoActual = ultimaMonedaSpawneada.transform.position.z + espaciado - grupos[grupoActual].Posicion;
    }

    private void CercaniaObstaculo()
    {
        if(Mathf.Abs(distanciaAlGrupoActual) < distanciaMaxima / 2)
        {
            cercaAObstaculo = true;
        }
        else
        {
            cercaAObstaculo = false;
            atravesandoObstaculo = false;
        }
    }

    private void AgregarGrupos(List<GrupoObstaculos> gruposNuevos)
    {
        GrupoObstaculos ultimoGrupo = grupos[grupoActual];

        grupos.Clear();
        grupos.Add(ultimoGrupo);

        foreach(GrupoObstaculos grupo in gruposNuevos)
        {
            grupos.Add(grupo);
        }

        grupoActual = 0;
    }

    private void Atravesando(Altura altura)
    {
        if(altura == Altura.Arriba && Mathf.Abs(distanciaAlGrupoActual) < scriptJugador.DistanciaSalto)
        {
            atravesandoObstaculo = true;
        }
        else if(altura == Altura.Abajo && Mathf.Abs(distanciaAlGrupoActual) < scriptJugador.DistanciaDesliz)
        {
            atravesandoObstaculo = true;
        }
        else
        {
            atravesandoObstaculo = false;
        }
    }


    public void SpawnearMonedas(GameObject pista)
    {
        BehaviourPista scriptPista = pista.GetComponentInChildren<BehaviourPista>();
        scriptPista.monedas = new List<GameObject>();

        AgregarGrupos(scriptPista.gruposObstaculos);

        while(ultimaMonedaSpawneada.transform.position.z + espaciado < pista.transform.position.z + scriptPista.Longitud / 2 - distanciaMaxima / 2)
        {
            GameObject nuevaMoneda = ObtenerMonedaDesactivada();

            float distanciaAlSiguienteGrupo;

            ActualizarDistanciaAlGrupoActual();

            if(grupoActualAtravesado)
            {
                if(grupoActual + 1 < grupos.Count)
                {
                    grupoActual++;

                    distanciaAlSiguienteGrupo = grupos[grupoActual].Posicion - grupos[grupoActual - 1].Posicion;

                    lugarActual = grupos[grupoActual].DevolverLugarLibreAleatorio();

                    grupoActualAtravesado = false;
                }

                ActualizarDistanciaAlGrupoActual();
            }
            else
            {
                CercaniaObstaculo();
            }

            if(cercaAObstaculo)
            {
                Atravesando(lugarActual.Altura);
            }
            else
            {
                if(distanciaAlGrupoActual > distanciaMaxima / 2)
                {
                    grupoActualAtravesado = true;
                }
            }

            if(atravesandoObstaculo)
            {
                float altura;

                if(lugarActual.Altura == Altura.Arriba)
                {
                    float distanciaRelativa = ((ultimaMonedaSpawneada.transform.position.z + espaciado) - (grupos[grupoActual].Posicion - scriptJugador.DistanciaSalto / 2)) / scriptJugador.DistanciaSalto;

                    altura = (-4 * Mathf.Pow(distanciaRelativa, 2) + 4 * (distanciaRelativa)) * scriptJugador.AlturaMaxima + scriptJugador.AlturaBase;                
                }
                else
                {
                    //Debug.Log("abajo");
                    altura = alturaDesliz;
                }

                ultimaMonedaSpawneada = PosicionarMoneda(nuevaMoneda, new Vector3(grupos[grupoActual].Carriles[lugarActual.Carril].transform.position.x, altura, ultimaMonedaSpawneada.transform.position.z + espaciado), prefabMoneda.transform.rotation);
            }
            else
            {
                //Debug.Log("normal");
                ultimaMonedaSpawneada = PosicionarMoneda(nuevaMoneda, new Vector3(grupos[grupoActual].Carriles[lugarActual.Carril].transform.position.x, scriptJugador.AlturaBase, ultimaMonedaSpawneada.transform.position.z + espaciado), prefabMoneda.transform.rotation);
            }

            scriptPista.monedas.Add(ultimaMonedaSpawneada);
        }
    }
}
