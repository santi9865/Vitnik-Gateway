using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogrosPresenter : MonoBehaviour
{
    [SerializeField] private GameObject prefabPanelLogro;
    [SerializeField] private GameObject contenedorLogros;
    [SerializeField] private TMP_Text txtContadorLogros;

    public void PresentarLogros(List<Logro> logros)
    {
        int logrosCompletados = 0;

        foreach(Logro logro in logros)
        {
            CrearPanelLogro(logro);

            if(logro.Progreso >= logro.Meta)
            {
                logrosCompletados++;
            }
        }

        ActualizarContadorLogros(logros.Count, logrosCompletados);
    }

    private void ActualizarContadorLogros(int logrosTotales, int logrosCompletados)
    {
        txtContadorLogros.text = $"{logrosCompletados.ToString()} / {logrosTotales.ToString()}";
    }

    private void CrearPanelLogro(Logro logro)
    {
        Debug.Log("Creando panel " + logro.Nombre);

        GameObject nuevoPanel;
        PanelLogro scriptNuevoPanel;
            nuevoPanel = Instantiate(prefabPanelLogro, contenedorLogros.transform);
            scriptNuevoPanel = nuevoPanel.GetComponent<PanelLogro>();

            scriptNuevoPanel.Actualizar(logro.Nombre, logro.Descripcion, logro.Progreso 
            / (float)logro.Meta, ObtenerSpriteLogroSegunID(logro.ID));
    }

    private Sprite ObtenerSpriteLogroSegunID(int ID)
    {
        return Resources.Load<Sprite>($"Placeholders/IconosLogros/{ID}");
    }
}
