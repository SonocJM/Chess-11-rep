using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoPiezas : MonoBehaviour
{
    [Header("References")]
    public GenerateBoard board; // Referencia al script GenerateBoard
    public Material baseMaterial;
    public Material hoverMaterial;
    public Material clickMaterial;
    public GameObject chessBoard;
    public bool p1Turn;
    public bool usingAbility;
    public bool wizAbility;
    public bool wizAbility2;
    public bool stoppedRotating;
    public bool heroPieceAliveP1;
    public bool heroPieceAliveP2;
    public UIChessBoard ui;
    public MenuVictoria mV;


    private Tile selectedTile = null; // Casilla actualmente seleccionada
    private Tile WizselectedTile = null;
    private List<Tile> highlightedTiles = new List<Tile>(); // Movimientos legales resaltados
    private Camera mainCamera;
    public float rotationSpeed = 90f;
    public int abilityIndex;
    public UIChessBoard uiManager;
    

    private void Awake()
    {
        stoppedRotating = true;
        wizAbility2 = false;
        usingAbility = false;
        p1Turn = true;
        heroPieceAliveP1 = true;
        heroPieceAliveP2 = true;
        
    }
    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleHover();
        HandleClick();
    }
    public void StartSmoothRotateY180(float waitSeconds)
    {
        StartCoroutine(RotateY180Smoothly(waitSeconds));
    }

    private System.Collections.IEnumerator RotateY180Smoothly(float waitSeconds)
    {
        stoppedRotating = false;
        ClearHighlights();
        // Wait for the specified delay before starting the rotation
        yield return new WaitForSeconds(waitSeconds);

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 180f, 0f);

        float elapsedTime = 0f;
        float duration = 180f / rotationSpeed; // Calculate duration based on speed

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            chessBoard.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            yield return null;
        }

        transform.rotation = endRotation; // Ensure exact final rotation
        ClearHighlights();
        stoppedRotating=true;

    }

    private void HandleHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null && !tile.isHighlighted) // Hover solo si no está resaltada
            {
                ResetAllTiles(); // Restablecer otras casillas al material base
                tile.GetComponent<Renderer>().material = hoverMaterial;
            }
        }
        else
        {
            ResetAllTiles();
        }
    }

    private void HandleClick()
    {
        if (Input.GetMouseButtonDown(0) && stoppedRotating) // Botón izquierdo
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                if (tile != null)
                {
                    if(tile.isHighlighted && wizAbility)
                    {
                        WizselectedTile = tile;
                        WizselectedTile.GetComponent<Renderer>().material = board.wizardPortal;
                        wizAbility = false;
                        wizAbility2 = true;
                    }
                    else if(tile.isHighlighted && wizAbility2)
                    {
                        Debug.Log("cast wiz");
                        WizAbilityCast(tile);
                        if (board.p1Cd > 0) { board.p1Cd--; }
                        if (board.p2Cd > 0) { board.p2Cd--; }
                        
                        if (p1Turn)
                        {
                            board.AssignCoolDowns(board.p1T, p1Turn);
                            p1Turn = false;

                        }
                        else
                        {
                            board.AssignCoolDowns(board.p2T, p1Turn);
                            p1Turn = true;
                        }
                        wizAbility2 = false;
                        ui.UpdateUI();
                        ClearHighlights();
                        StartSmoothRotateY180(5);
                    }
                    if (tile.isHighlighted && usingAbility)
                    {
                        UseAbility(tile, null);
                        if (board.p1Cd > 0) { board.p1Cd--; }
                        if (board.p2Cd > 0) { board.p2Cd--; }
                        
                        if (p1Turn)
                        {
                            board.AssignCoolDowns(board.p1T, p1Turn);
                            p1Turn = false;

                        }
                        else 
                        {
                            board.AssignCoolDowns(board.p2T, p1Turn);
                            p1Turn = true; 
                        }
                        ui.UpdateUI();
                        StartSmoothRotateY180(2.5f);
                    }
                    if (selectedTile == null && tile.identity != 0 && !wizAbility && !wizAbility2) // Seleccionar una pieza
                    {
                        WizselectedTile = null;
                        wizAbility = false;
                        if(!tile.p2 && p1Turn)
                        {
                            SelectTile(tile);
                        }
                        if (tile.p2 && !p1Turn)
                        {
                            SelectTile(tile);
                        }
                        
                    }
                    
                    else if ( tile.isHighlighted && !usingAbility ) // Mover pieza
                    {
                        MovePiece(tile);
                        if(p1Turn)
                        {
                            p1Turn = false;
                        }
                        else { p1Turn = true; }
                        if (board.p1Cd > 0) { board.p1Cd--; }
                        if (board.p2Cd > 0) { board.p2Cd--; }
                        ui.UpdateUI();
                        StartSmoothRotateY180(1.5f);
                    }
                    
                    else
                    {
                        // Deseleccionar si se hace clic en un lugar no válido
                        DeselectTile();
                        wizAbility = false;
                        wizAbility2 = false;
                    }
                }
                else
                {
                    DeselectTile();
                    wizAbility = false;
                    wizAbility2 = false;
                    usingAbility = false;
                }
            }
        }
    }
    public void WizardAnimCast(Tile targetTile, Tile currentTile)
    {
        targetTile.SpawnWizAnimation(2);
        currentTile.SpawnWizAnimation(2);

        StartCoroutine(WaitAndDeSpawnWizAnimations(targetTile, currentTile));
    }

    private IEnumerator WaitAndDeSpawnWizAnimations(Tile targetTile, Tile currentTile)
    {
        yield return new WaitForSeconds(3); // Espera 5 segundos
        UpdateAllTiles();
        yield return new WaitForSeconds(2);
        targetTile.DeSpawnWizAnimation(); // Desactiva la animación después de la espera
        currentTile.DeSpawnWizAnimation();
    }

    public void WizAbilityCast(Tile targetTile)
    {
        WizardAnimCast(targetTile, WizselectedTile);
        int tempTeam = targetTile.team;
        int tempIdentity = targetTile.identity;
        bool tempP2 = targetTile.p2;
        
        targetTile.team = WizselectedTile.team;
        targetTile.identity = WizselectedTile.identity;
        targetTile.p2 = WizselectedTile.p2;
        
        



        // Limpiar la casilla original
        WizselectedTile.team = tempTeam;
        WizselectedTile.identity = tempIdentity;
        WizselectedTile.p2 = tempP2;
        // Actualizar las piezas visualmente
        



        WizselectedTile = null; 
        // Limpiar selección
        DeselectTile();
        ClearHighlights();
    }
    public void AbilityButton(bool p2)
    {
        ResetAllTiles();
        ClearHighlights();
        bool canCast = false;

        if (p1Turn != p2 )
        {
            

            int playerTeam;
            if (!p2)
            {
                playerTeam = board.p1T;
                if(board.p1Cd == 0)
                {
                    canCast = true;
                }
                
            }
            else
            {
                playerTeam = board.p2T;
                if (board.p2Cd == 0)
                {
                    canCast = true;
                }
            }

            if(canCast)
            {
                switch (playerTeam)
                {
                    //rogue
                    case 1 or 4:
                        usingAbility = true;
                        for (int i = 0; i < 11; i++)
                        {
                            for (int j = 0; j < 11; j++)
                                
                            {
                                if (board.tiles[i, j].GetComponent<Tile>().p2 != p2 && board.tiles[i, j].GetComponent<Tile>().identity != 6)
                                {
                                    if (board.tiles[i, j].GetComponent<Tile>().identity != 0 && board.tiles[i, j].GetComponent<Tile>().identity != 5 && board.tiles[i, j].GetComponent<Tile>().identity != 7 && board.tiles[i, j].GetComponent<Tile>().identity != 8 && board.tiles[i, j].GetComponent<Tile>().identity != 9)
                                    {
                                        board.tiles[i, j].GetComponent<Renderer>().material = clickMaterial;
                                        board.tiles[i, j].GetComponent<Tile>().isHighlighted = true;
                                        highlightedTiles.Add(board.tiles[i, j].GetComponent<Tile>());
                                    }

                                }
                            }
                        }
                        abilityIndex = 1;

                        break;
                    //necromancer
                    case 2 or 5:
                        usingAbility = true;
                        for (int i = 0; i < 11; i++)
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                if (!p2)
                                {
                                    if (board.tiles[i, j].GetComponent<Tile>().p1DiedHere && board.tiles[i, j].GetComponent<Tile>().identity != 6)
                                    {
                                            board.tiles[i, j].GetComponent<Renderer>().material = clickMaterial;
                                            board.tiles[i, j].GetComponent<Tile>().isHighlighted = true;
                                            highlightedTiles.Add(board.tiles[i, j].GetComponent<Tile>());
                                    }

                                }
                                else
                                {
                                    if (board.tiles[i, j].GetComponent<Tile>().p2DiedHere && board.tiles[i, j].GetComponent<Tile>().identity != 6)
                                    {   
                                        board.tiles[i, j].GetComponent<Renderer>().material = clickMaterial;
                                        board.tiles[i, j].GetComponent<Tile>().isHighlighted = true;
                                        highlightedTiles.Add(board.tiles[i, j].GetComponent<Tile>());    
                                    }
                                }
                            }
                        }
                        abilityIndex = 2;
                        break;
                    //elementalist
                    case 3 or 6:
                        wizAbility = true;
                        selectedTile = null;
                        for (int i = 0; i < 11; i++)
                        {
                            for (int j = 0; j < 11; j++)
                            {
                                if (board.tiles[i, j].GetComponent<Tile>().identity != 6 && board.tiles[i, j].GetComponent<Tile>().identity != 0)
                                {
                                    board.tiles[i, j].GetComponent<Renderer>().material = clickMaterial;
                                    board.tiles[i, j].GetComponent<Tile>().isHighlighted = true;
                                    highlightedTiles.Add(board.tiles[i, j].GetComponent<Tile>());
                                }
                            }
                            wizAbility = true;
                        }
                        break;

                        
                }
            }
            
        }
       
        

    }
    public void UseAbility(Tile targetTile, Tile targetTile2)
    {
        usingAbility = false;
        
        switch(abilityIndex)

        {
            
            //rogue cast
            case 1:
                targetTile.identity = 0;
                targetTile.SpawnWizAnimation(0);
                
                break;
                //necro cast
            case 2:
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        if (p1Turn)
                        {
                            if (board.tiles[i, j].GetComponent<Tile>().p1DiedHere && board.tiles[i, j].GetComponent<Tile>().identity != 6)
                            {
                                board.tiles[i,j].GetComponent<Tile>().identity = 10;
                                board.tiles[i, j].GetComponent<Tile>().p2 = false;
                                board.tiles[i, j].GetComponent<Tile>().team = board.p1T;
                                board.tiles[i, j].GetComponent<Tile>().p1DiedHere = false;
                                board.tiles[i, j].GetComponent<Tile>().clearParticles(0);
                                board.tiles[i, j].GetComponent<Tile>().SpawnWizAnimation(1);

                            }

                        }
                        else
                        {
                            if (board.tiles[i, j].GetComponent<Tile>().p2DiedHere && board.tiles[i, j].GetComponent<Tile>().identity != 6)
                            {
                                board.tiles[i, j].GetComponent<Tile>().identity = 10;
                                board.tiles[i, j].GetComponent<Tile>().p2 = true;
                                board.tiles[i, j].GetComponent<Tile>().team = board.p2T;
                                board.tiles[i, j].GetComponent<Tile>().p2DiedHere = false;
                                board.tiles[i, j].GetComponent<Tile>().clearParticles(0);
                                board.tiles[i, j].GetComponent<Tile>().SpawnWizAnimation(1);
                            }
                        }
                    }
                }
                break;
            
            
            
        }
        UpdateAllTiles();
        DeselectTile();
        ClearHighlights();

    }

    

    private void SelectTile(Tile tile)
    {
        usingAbility = false;
        wizAbility = false;
        wizAbility2 = false;
        selectedTile = tile;
        WizselectedTile = null;
        HighlightLegalMoves(tile); // Iluminar movimientos legales
    }

    private void DeselectTile()
    {
        WizselectedTile = null;
        selectedTile = null;
        ClearHighlights();
    }

    private void HighlightLegalMoves(Tile tile)
    {
        ClearHighlights(); // Limpiar cualquier resaltado previo
        List<Vector2Int> legalMoves = tile.legalMoves; // Obtener movimientos legales

        foreach (Vector2Int move in legalMoves)
        {
            Tile targetTile = board.tiles[move.x, move.y].GetComponent<Tile>(); // Usar board.tiles
            targetTile.GetComponent<Renderer>().material = clickMaterial;
            targetTile.isHighlighted = true;
            highlightedTiles.Add(targetTile);
        }
    }

    private void ClearHighlights()
    {
        foreach (Tile tile in highlightedTiles)
        {
            tile.GetComponent<Renderer>().material = baseMaterial;
            tile.isHighlighted = false;
        }
        highlightedTiles.Clear();
    }

    private void MovePiece(Tile targetTile)
    {
        
        if(targetTile.identity == 6) //detectar quien gano
        {
            if(p1Turn)
            {
                //jugador 1 gano
                mV.SpawnPanel(p1Turn, board.p1T);
            }
            else
            {
                mV.SpawnPanel(p1Turn, board.p2T);
            }
        }
        if (targetTile.identity == 7 || targetTile.identity == 8 || targetTile.identity == 9 ) //desactivar boton de habilidad
        {
            
            uiManager.DisableButton(p1Turn);
            
        }
        //registrar casillas con piezas muertas para el necromancer
        if (board.p1T == 2 || board.p1T == 5)
        {
            Debug.Log("Jugador 1 es necro");
            if (targetTile.identity != 0 && targetTile.identity != 1 && targetTile.identity != 10)
            {
                
                if (!targetTile.p2DiedHere && !targetTile.p1DiedHere)
                {
                    
                    if (!targetTile.p2)
                    {
                        Debug.Log("Jugador 1 perdio una pieza que no es peon");
                        targetTile.p1DiedHere = true;
                        targetTile.ParticleStatus(1);
                    }
                }
            }
        
        }
        if (board.p2T == 2 || board.p2T == 5)
        {
            Debug.Log("Jugador 2 es necro");
            if (targetTile.identity != 0 && targetTile.identity != 1 && targetTile.identity != 10)
            {

                if (!targetTile.p2DiedHere && !targetTile.p1DiedHere)
                {
                    if (targetTile.p2)
                    {
                        Debug.Log("Jugador 2 perdio una pieza que no es peon");
                        targetTile.p2DiedHere = true;
                        if(!board.mirror)
                        {
                            targetTile.ParticleStatus(1);
                        }
                        else
                        {
                            targetTile.ParticleStatus(2);
                        }
                        
                    }
                    
                }
            }

        }
        // no es un zombie
        if(selectedTile.identity != 10)
        {
            targetTile.team = selectedTile.team;
            targetTile.identity = selectedTile.identity;
            targetTile.p2 = selectedTile.p2;
            //limitar al peon
            targetTile.hasMoved = true;
        }//es un zombie pero se mueve a una casilla vacia
        else if (selectedTile.identity == 10 && targetTile.identity == 0)
        {
            targetTile.team = selectedTile.team;
            targetTile.identity = selectedTile.identity;
            targetTile.p2 = selectedTile.p2;
            //limitar al peon
            targetTile.hasMoved = true;
        } // es un zombie pero se come a una pieza
        else if (selectedTile.identity == 10 && targetTile.identity != 0)
        {
            targetTile.identity = 0;
            targetTile.team = 0;
            targetTile.p2 = false;
        }
        
        


        // Limpiar la casilla original
        selectedTile.team = 0;
        selectedTile.identity = 0;
        selectedTile.p2 = false;
        // Actualizar las piezas visualmente
        UpdateAllTiles();

        
        

        // Limpiar selección
        DeselectTile();
        ClearHighlights();
    }

    private void UpdateAllTiles()
    {
        for(int i = 0; i < 11; i++ )
        {
            for (int j = 0; j < 11; j++)
            {
                board.tiles[i, j].GetComponent<Tile>().ChangePiece();
                board.tiles[i, j].GetComponent<Tile>().LegalMovesAssign();
            }
        }
    }

    private void ResetAllTiles()
    {
        foreach (GameObject tileObj in board.tiles) // Usar board.tiles
        {
            Tile tile = tileObj.GetComponent<Tile>();
            if (!tile.isHighlighted)
                tile.GetComponent<Renderer>().material = baseMaterial;
        }
    }
}