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
        if(Input.GetKeyDown(KeyCode.Escape)) InputBroker(KeyCode.Escape);

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

            case KeyCode.Escape:
                Application.Quit();
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
        for(int y = 0; y < boardHeight; y++)
        {
            List<Tile> row = CollectRow(y);
            row = CombineBackward(row);
            board[y] = InsertNulls(boardWidth - row.Count, row);
        }
    }

    public void MoveTilesLeft()
    {
        for(int y = 0; y < boardHeight; y++)
        {
            List<Tile> row = CollectRow(y);
            row = CombineForward(row);
            board[y] = AppendNulls(boardWidth - row.Count, row);
        }
    }

    public void MoveTilesUp()
    {
        for(int x = 0; x < boardWidth; x++)
        {
            List<Tile> col = CollectColumn(x);
            col = CombineBackward(col);
            col = InsertNulls(boardHeight - col.Count, col);

            for(int y = 0; y < boardHeight; y++)
            {
                board[y][x] = col[y];
            }
        }
    }

    public void MoveTilesDown()
    {
        for(int x = 0; x < boardWidth; x++)
        {
            List<Tile> col = CollectColumn(x);
            col = CombineForward(col);
            col = AppendNulls(boardHeight - col.Count, col);

            for(int y = 0; y < boardHeight; y++)
            {
                board[y][x] = col[y];
            }
        }
    }

    public List<Tile> InsertNulls(int count, List<Tile> tiles)
    {
        for(int i = 0; i < count; i++)
        {
            tiles.Insert(0, null);
        }
        return tiles;
    }

    public List<Tile> AppendNulls(int count, List<Tile> tiles)
    {
        for(int i = 0; i < count; i++)
        {
            tiles.Add(null);
        }
        return tiles;
    }

    public List<Tile> CollectRow(int r)
    {
        List<Tile> row = new List<Tile>();

        for(int x = 0; x < boardWidth; x++)
        {
            // rowlect the actual tiles
            Tile t = board[r][x];
            if(t != null) { row.Add(t); }
        }
        return row;
    }


    // collect the actual tiles
    public List<Tile> CollectColumn(int c)
    {
        List<Tile> col = new List<Tile>();

        for(int y = 0; y < boardHeight; y++)
        {
            Tile t = board[y][c];
            if(t != null) { col.Add(t); }
        }

        return col;
    }


    // loop forwards combining where needed
    public List<Tile> CombineForward(List<Tile> tiles)
    {
        for(int i = 0; i < tiles.Count - 1; i++)
        {
            if(tiles[i].Value == tiles[i + 1].Value)
            {
                tiles[i].Value *= 2;
                Destroy(tiles[i + 1].gameObject);
                tiles.RemoveAt(i + 1);
            }
        }

        return tiles;
    }

    // loop backwards combining where needed
    public List<Tile> CombineBackward(List<Tile> tiles)
    {
        for(int i = tiles.Count - 1; i > 0; i--)
        {
            if(tiles[i].Value == tiles[i - 1].Value)
            {
                tiles[i].Value *= 2;
                Destroy(tiles[i - 1].gameObject);
                tiles.RemoveAt(i - 1);
                // we decrement here because we dont want things to combine more than once
                // this will skip checking the same tile twice
                i--;
            }
        }
        return tiles;
    }
}
