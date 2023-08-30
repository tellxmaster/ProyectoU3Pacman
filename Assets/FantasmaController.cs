using UnityEngine;
using Panda;

public class FantasmaController : MonoBehaviour
{
    public Transform jugador;
    public Transform salida; // Punto de salida
    public float velocidad = 5f;
    public float distanciaDeteccion = 10f;
    public float distanciaSalida = 2f; // Distancia para detectar el punto de salida

    private bool haAlcanzadoSalida = false; // Variable de estado

    void Start()
    {
        // Asignar una rotación inicial aleatoria al inicio
        float giroInicial = Random.Range(0f, 360f); // Devuelve un valor entre 0 y 360
        transform.rotation = Quaternion.Euler(0, giroInicial, 0);
    }

    [Task]
    private void JugadorCerca()
    {
        if (jugador != null && Vector3.Distance(transform.position, jugador.position) < distanciaDeteccion)
        {
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    private void SalidaCerca()
    {
        if (haAlcanzadoSalida)
        {
            Task.current.Fail();
            return;
        }

        if (salida != null && Vector3.Distance(transform.position, salida.position) < distanciaSalida)
        {
            haAlcanzadoSalida = true; // Actualiza el estado
            Task.current.Succeed();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    private void CambiarDireccionHaciaJugador()
    {
        if (jugador == null)
        {
            Task.current.Fail();
            return;
        }

        Vector3 direccion = (jugador.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direccion);
        Task.current.Succeed();
    }

    [Task]
    private void CambiarDireccionHaciaSalida()
    {
        if (salida == null)
        {
            Task.current.Fail();
            return;
        }

        Vector3 direccion = (salida.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direccion);
        Task.current.Succeed();
    }

    [Task]
    private void ParedCerca()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
        {
            if (hit.collider.CompareTag("Pared"))
            {
                Task.current.Succeed();
                return;
            }
        }
        Task.current.Fail();
    }

    [Task]
    private void Girar90Grados()
    {
        // Gira 90 grados en una dirección aleatoria (izquierda o derecha)
        float giro = (Random.Range(0, 2) * 2 - 1) * 90; // Devuelve -90 o 90
        transform.rotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, giro, 0));
        Task.current.Succeed();
    }

    [Task]
    private void Mover()
    {
        // Mueve el fantasma en la dirección actual en línea recta
        transform.position += transform.forward * velocidad * Time.deltaTime;
        Task.current.Succeed();
    }
}
