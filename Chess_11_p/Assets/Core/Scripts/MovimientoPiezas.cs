using System.Collections.Generic;
using UnityEngine;

public class MovimientoPiezas : MonoBehaviour
{
    public GameObject[,] tablero;
    private GameObject piezaSeleccionada;
    private List<Vector2Int> movimientosValidos = new List<Vector2Int>();
    private Vector2Int tamañoTablero = new Vector2Int(11, 11);
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int posicionClic = ObtenerPosicionClic();
            if (piezaSeleccionada == null)
            {
                SeleccionarPieza(posicionClic);
            }
            else
            {
                MoverPieza(posicionClic);
            }
        }
    }

    Vector2Int ObtenerPosicionClic()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 puntoImpacto = hit.point;
            return new Vector2Int(Mathf.FloorToInt(puntoImpacto.x), Mathf.FloorToInt(puntoImpacto.z));
        }
        return new Vector2Int(-1, -1);
    }

    void SeleccionarPieza(Vector2Int posicion)
    {
        if (PosicionValida(posicion) && tablero[posicion.x, posicion.y] != null)
        {
            piezaSeleccionada = tablero[posicion.x, posicion.y];
            movimientosValidos = CalcularMovimientosValidos(piezaSeleccionada, posicion);
            MostrarMovimientosValidos();
        }
    }

    void MoverPieza(Vector2Int nuevaPosicion)
    {
        if (movimientosValidos.Contains(nuevaPosicion))
        {
            Vector2Int posicionActual = ObtenerPosicionPieza(piezaSeleccionada);
            tablero[posicionActual.x, posicionActual.y] = null;
            tablero[nuevaPosicion.x, nuevaPosicion.y] = piezaSeleccionada;

            piezaSeleccionada.transform.position = new Vector3(nuevaPosicion.x, 0, nuevaPosicion.y);

            piezaSeleccionada = null;
            LimpiarMovimientosValidos();
        }
        else
        {
            Debug.Log("Esa casilla esta vacia, prueba con otra");
        }
    }

    Vector2Int ObtenerPosicionPieza(GameObject pieza)
    {
        for (int x = 0; x < tamañoTablero.x; x++)
        {
            for (int y = 0; y < tamañoTablero.y; y++)
            {
                if (tablero[x, y] == pieza)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    bool PosicionValida(Vector2Int posicion)
    {
        return posicion.x >= 0 && posicion.x < tamañoTablero.x &&
               posicion.y >= 0 && posicion.y < tamañoTablero.y;
    }

    List<Vector2Int> CalcularMovimientosValidos(GameObject pieza, Vector2Int posicion)
    {

        List<Vector2Int> movimientos = new List<Vector2Int>();

        Vector2Int[] direcciones = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (Vector2Int direccion in direcciones)
        {
            Vector2Int nuevaPos = posicion + direccion;
            if (PosicionValida(nuevaPos) && (tablero[nuevaPos.x, nuevaPos.y] == null || EsEnemigo(nuevaPos)))
            {
                movimientos.Add(nuevaPos);
            }
        }
        return movimientos;
    }

    bool EsEnemigo(Vector2Int posicion)
    {
        GameObject piezaEnPosicion = tablero[posicion.x, posicion.y];
        return piezaEnPosicion != null && piezaEnPosicion.CompareTag("Player2");
    }

    void MostrarMovimientosValidos()
    {
        foreach (Vector2Int posicion in movimientosValidos)
        {
            Debug.Log($"Movimiento: {posicion}");
        }
    }

    void LimpiarMovimientosValidos()
    {
        movimientosValidos.Clear();
    }
}