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

    [SerializeField] private Carril carrilInicial;
    [SerializeField] private GameObject ultimaMonedaSpawneada;

    private float distanciaMaxima;

    private List<GrupoObstaculos> grupos;
    private int grupoActual;
    private LugarObstaculo lugarActual;
    private List<GameObject> poolMonedas;

    private float distanciaUltimaMonedaAlGrupoActual;
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
        List<Carril> listaCarrilesInicial = new List<Carril>();
        listaCarrilesInicial.Add(carrilInicial);
        grupos.Add(new GrupoObstaculos(ultimaMonedaSpawneada.transform.position, listaLugaresInicial, listaCarrilesInicial));

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


    // Calcula la distancia de la última moneda spawneada al grupo actual.
    // Si la moneda está antes del grupo la distancia es positiva, si la moneda está después la distancia es negativa.
    // El antes o despues depende del eje de movimiento. El antes es yendo hacia el después en la dirección y sentido del eje de movimiento.
    private void ActualizarDistanciaAlGrupoActual(Eje eje)
    {
        distanciaUltimaMonedaAlGrupoActual = Vector3.Dot(grupos[grupoActual].Posicion - ultimaMonedaSpawneada.transform.position, eje.Vectorizado);
    }

    private void CercaniaObstaculo()
    {
        if(Mathf.Abs(distanciaUltimaMonedaAlGrupoActual - espaciado) < distanciaMaxima / 2)
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
        if(altura == Altura.Arriba && Mathf.Abs(distanciaUltimaMonedaAlGrupoActual - espaciado) < scriptJugador.DistanciaSalto)
        {
            atravesandoObstaculo = true;
        }
        else if(altura == Altura.Abajo && Mathf.Abs(distanciaUltimaMonedaAlGrupoActual - espaciado) < scriptJugador.DistanciaDesliz)
        {
            atravesandoObstaculo = true;
        }
        else
        {
            atravesandoObstaculo = false;
        }
    }

    private bool EstaLibreArriba(GrupoObstaculos grupo, int carril)
    {
        foreach(LugarObstaculo lugar in grupo.Lugares)
        {
            if(lugar.Carril == carril && lugar.Altura == Altura.Arriba)
            {
                return lugar.Libre;
            }
        }

        return false;
    }

    public void SpawnearMonedas(GameObject pista, GameObject primeraMoneda)
    {
        if(primeraMoneda != null)
        {
            ultimaMonedaSpawneada = primeraMoneda;
        }

        BehaviourPista scriptPista = pista.GetComponent<BehaviourPista>();
        scriptPista.monedas = new List<GameObject>();

        AgregarGrupos(scriptPista.gruposObstaculos);

        while(Vector3.Dot(pista.transform.position + ((scriptPista.Longitud / 2) - (distanciaMaxima / 2)) * scriptPista.EjeMovimiento.Vectorizado - 
        ultimaMonedaSpawneada.transform.position, scriptPista.EjeMovimiento.Vectorizado) > espaciado)
        {
            GameObject nuevaMoneda = ObtenerMonedaDesactivada();

            float distanciaGrupoActualAlSiguienteGrupo;

            ActualizarDistanciaAlGrupoActual(scriptPista.EjeMovimiento);

            if(grupoActualAtravesado)
            {
                if(grupoActual + 1 < grupos.Count)
                {
                    distanciaGrupoActualAlSiguienteGrupo = Vector3.Dot(grupos[grupoActual].Posicion - grupos[grupoActual + 1].Posicion, scriptPista.EjeMovimiento.Vectorizado);

                    lugarActual = grupos[grupoActual + 1].DevolverLugarLibreAleatorio();

                    grupoActual++;
                    grupoActualAtravesado = false;
                }

                ActualizarDistanciaAlGrupoActual(scriptPista.EjeMovimiento);
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
                if(distanciaUltimaMonedaAlGrupoActual - espaciado < -(distanciaMaxima / 2))
                {
                    grupoActualAtravesado = true;
                }
            }

            float altura;

            if(atravesandoObstaculo)
            {
                if(lugarActual.Altura == Altura.Arriba)
                {
                    float distanciaRelativa = (scriptJugador.DistanciaSalto / 2 - (distanciaUltimaMonedaAlGrupoActual - espaciado)) / scriptJugador.DistanciaSalto;

                    altura = (-4 * Mathf.Pow(distanciaRelativa, 2) + 4 * (distanciaRelativa)) * scriptJugador.AlturaMaxima + scriptJugador.AlturaBase;                
                }
                else
                {
                    if(EstaLibreArriba(grupos[grupoActual], lugarActual.Carril))
                    {
                        altura = scriptJugador.AlturaBase;
                    }
                    else
                    {
                        altura = alturaDesliz;
                    }
                }
            }
            else
            {
                altura = scriptJugador.AlturaBase;
            }

            ultimaMonedaSpawneada = PosicionarMoneda(nuevaMoneda, Vector3.Scale(grupos[grupoActual].Carriles[lugarActual.Carril].Posicion.transform.position, scriptJugador.EjeMovimiento.VectorAxisPerpendicular)
            + altura * Vector3.up + Vector3.Scale(ultimaMonedaSpawneada.transform.position, scriptPista.EjeMovimiento.VectorAxisParalelo)
            + espaciado * scriptPista.EjeMovimiento.Vectorizado, prefabMoneda.transform.rotation);

            scriptPista.monedas.Add(ultimaMonedaSpawneada);
            
            ultimaMonedaSpawneada.GetComponent<BehaviourMoneda>().pistaAsociada = scriptPista;

            ultimaMonedaSpawneada.transform.Rotate(0,Eje.EjeZPositivo.AngulosA(scriptJugador.EjeMovimiento) ,0);
        }
    }
}
