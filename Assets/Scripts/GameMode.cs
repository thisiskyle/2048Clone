using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour
{

    // prefabs
    public GameObject tilePrefab = null;
    public GameObject backgroundPrefab = null;
    public GameObject boardGo;

    public GameObject cam = null;

    private int boardWidth = 4;
    private int boardHeight = 4;
    private List<List<Tile>> board;
    private KeyCode? inputBuffer = null;


    
    // Start is called before the first frame update
    void Start()
    {
        InitialSetup();
        cam.transform.position = new Vector3((boardWidth / 2) - 0.5f, (boardHeight / 2) - 0.5f, cam.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        InputHandler();
        UpdateBoard();
        UpdateGraphics();
    }

    private void InitialSetup()
    {
        CreateBoard();

        SpawnNewTile();
        SpawnNewTile();

        UpdateBoard();
        UpdateGraphics();
    }

    private void UpdateBoard()
    {
        for(int y = 0; y < boardHeight; ++y)
        {
            for(int x = 0; x < boardWidth; ++x)
            {
                if(board[y][x] != null)
                {
                    board[y][x].SetBoardPosition(new Vector3(x, y, 0));
                }
            }
        }

    }

    private void UpdateGraphics()
    {
        for(int y = 0; y < boardHeight; ++y)
        {
            for(int x = 0; x < boardWidth; ++x)
            {
                if(board[y][x] != null)
                {
                    board[y][x].UpdateGraphic();
                }
            }
        }
    }

    private bool IsAnimating()
    {
        for(int y = 0; y < boardHeight; ++y)
        {
            for(int x = 0; x < boardWidth; ++x)
            {
                if(board[y][x] != null && board[y][x].IsAnimating)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // handles the keypress and desides wether to call them or store them in the buffer
    private void InputHandler()
    {
        if(!IsAnimating())
        {
            if(inputBuffer != null)
            {
                InputBroker(inputBuffer);
                inputBuffer = null;
            }
            else
            {
                if(Input.GetKeyDown(KeyCode.W)) InputBroker(KeyCode.W);
                else if(Input.GetKeyDown(KeyCode.A)) InputBroker(KeyCode.A);
                else if(Input.GetKeyDown(KeyCode.S)) InputBroker(KeyCode.S);
                else if(Input.GetKeyDown(KeyCode.D)) InputBroker(KeyCode.D);
                else if(Input.GetKeyDown(KeyCode.R)) InputBroker(KeyCode.R);
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.D)) inputBuffer = KeyCode.D;
            else if(Input.GetKeyDown(KeyCode.A)) inputBuffer = KeyCode.A;
            else if(Input.GetKeyDown(KeyCode.W)) inputBuffer = KeyCode.W;
            else if(Input.GetKeyDown(KeyCode.S)) inputBuffer = KeyCode.S;
        }
    }


    // calls the function based on the key sent
    private void InputBroker(KeyCode? key)
    {
        if(key == null) return;

        switch(key)
        {
            case KeyCode.W:
                MoveTilesUp();
                SpawnNewTile();
                break;

            case KeyCode.A:
                MoveTilesLeft();
                SpawnNewTile();
                break;

            case KeyCode.S:
                MoveTilesDown();
                SpawnNewTile();
                break;

            case KeyCode.D:
                MoveTilesRight();
                SpawnNewTile();
                break;

            case KeyCode.R:
                Reset();
                break;
        }
    }



    private void CreateBoard()
    {
        board = new List<List<Tile>>();
        boardGo = new GameObject("Board");


        for(int y = 0; y < boardHeight; ++y)
        {
            board.Add(new List<Tile>());

            for(int x = 0; x < boardWidth; ++x)
            {
                board[y].Add(null);
                Instantiate(backgroundPrefab, new Vector3(x, y, 2), Quaternion.identity).gameObject.transform.SetParent(boardGo.transform);
            }
        }
    }


    private void SpawnTile(int x, int y, int v)
    {
        Tile tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity).GetComponent<Tile>();
        board[y][x] = tile;
        tile.Value = v;
        tile.gameObject.transform.SetParent(boardGo.transform);
    }

    private void Reset()
    {
        board.Clear();
        Destroy(boardGo);
        boardGo = null;
        InitialSetup();
    }


    private void SpawnNewTile()
    {
        int x = Random.Range(0, boardWidth);
        int y = Random.Range(0, boardHeight);

        int count = 0;

        while(board[y][x] != null)
        {
            if(count > boardWidth * boardHeight) return;

            x = Random.Range(0, boardWidth);
            y = Random.Range(0, boardHeight);

            count++;
        }

        SpawnTile(x, y, 2);
    }



    public void MoveTilesRight()
    {

        // for each row....
        for(int y = 0; y < boardHeight; y++)
        {
            // holds the current tiles in the row
            List<Tile> row = new List<Tile>();

            // for each column...
            for(int x = 0; x < boardWidth; x++)
            {
                // collect the actual tiles
                Tile t = board[y][x];
                if(t != null) { row.Add(t); }
            }


            // loop backwards combining where needed
            for(int i = row.Count - 1; i > 0; i--)
            {
                if(row[i].Value == row[i - 1].Value)
                {
                    row[i].Value *= 2;
                    Destroy(row[i - 1].gameObject);
                    row.RemoveAt(i - 1);
                    // we decrement here because we dont want things to combine more than once
                    // this will skip checking the same tile twice
                    i--;
                }
            }

            // create a new row
            board[y] = new List<Tile>();

            // populate new row with nulls
            for(int k = 0; k < boardWidth - row.Count; k++)
            {
                board[y].Add(null);
            }

            // add the tiles to the end of the row
            row.ForEach(item => board[y].Add(item));
        }
    }

    public void MoveTilesLeft()
    {

        // for each row....
        for(int y = 0; y < boardHeight; y++)
        {
            // holds the current tiles in the row
            List<Tile> row = new List<Tile>();

            // for each column...
            for(int x = 0; x < boardWidth; x++)
            {
                // collect the actual tiles
                Tile t = board[y][x];
                if(t != null) { row.Add(t); }
            }

            // loop forwards combining where needed
            for(int i = 0; i < row.Count - 1; i++)
            {
                if(row[i].Value == row[i + 1].Value)
                {
                    row[i].Value *= 2;
                    Destroy(row[i + 1].gameObject);
                    row.RemoveAt(i + 1);
                }
            }

            // create a new row
            board[y] = new List<Tile>();
            // add the tiles to the front of the row
            row.ForEach(item => board[y].Add(item));
            // add nulls to the end of the new row 
            for(int k = 0; k < boardWidth - row.Count; k++)
            {
                board[y].Add(null);
            }
        }
    }

    public void MoveTilesUp()
    {

        // for each column....
        for(int x = 0; x < boardWidth; x++)
        {
            // holds the current tiles in the column
            List<Tile> column = new List<Tile>();

            // for each column...
            for(int y = 0; y < boardHeight; y++)
            {
                // collect the actual tiles
                Tile t = board[y][x];
                if(t != null) { column.Add(t); }
            }


            // loop backwards combining where needed
            for(int i = column.Count - 1; i > 0; i--)
            {
                if(column[i].Value == column[i - 1].Value)
                {
                    column[i].Value *= 2;
                    Destroy(column[i - 1].gameObject);
                    column.RemoveAt(i - 1);
                    // we decrement here because we dont want things to combine more than once
                    // this will skip checking the same tile twice
                    i--;
                }
            }

            int nullCount = boardHeight - column.Count;

            for(int y = 0; y < boardHeight; y++)
            {
                if(y < nullCount)
                {
                    board[y][x] = null;
                }
                else
                {
                    board[y][x] = column[y - nullCount];
                }
            }
        }
    }

    public void MoveTilesDown()
    {

        // for each column....
        for(int x = 0; x < boardWidth; x++)
        {
            // holds the current tiles in the column
            List<Tile> column = new List<Tile>();

            // for each column...
            for(int y = 0; y < boardHeight; y++)
            {
                // collect the actual tiles
                Tile t = board[y][x];
                if(t != null) { column.Add(t); }
            }

            // loop forwards combining where needed
            for(int i = 0; i < column.Count - 1; i++)
            {
                if(column[i].Value == column[i + 1].Value)
                {
                    column[i].Value *= 2;
                    Destroy(column[i + 1].gameObject);
                    column.RemoveAt(i + 1);
                }
            }

            int nullCount = boardHeight - column.Count;

            for(int y = 0; y < boardHeight; y++)
            {
                if(y > column.Count - 1)
                {
                    board[y][x] = null;
                }
                else
                {
                    board[y][x] = column[y];
                }
            }
        }
    }

}
