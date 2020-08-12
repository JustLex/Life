using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject cellPF;
    public Material cellMat;
    Dictionary<Vector2, GameObject> cells = new Dictionary<Vector2, GameObject>();
    Dictionary<Vector2, int> cellsNCount = new Dictionary<Vector2, int>();

    readonly int[][] adj = new int[][] {
        new int[]{-1, -1},
        new int[]{-1, 0},
        new int[]{-1, 1},
        new int[]{1, 1},
        new int[]{1, 0},
        new int[]{1, -1},
        new int[]{0, -1},
        new int[]{0, 1},
    };

    float period = 0.01f;
    float nextTrigger = 0;


    void DrawDot() {


    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void IncNCount(Vector2 pos) {
        foreach(var n in adj) {
            var nPos = new Vector2(pos.x + n[0], pos.y + n[1]);
            if (!cellsNCount.ContainsKey(nPos))
                cellsNCount.Add(nPos, 1);
            else
                cellsNCount[nPos]++;
        }
    }

    void DecNCount(Vector2 pos) {
        foreach (var n in adj)
        {
            var nPos = new Vector2(pos.x + n[0], pos.y + n[1]);
            cellsNCount[nPos]--;
            if (cellsNCount[nPos] == 0) cellsNCount.Remove(nPos);
        }
    }

    Vector2 pxToGrid(Vector3 pos) {
        var cPos = Camera.main.ScreenToWorldPoint(pos);
        return new Vector2(Mathf.Floor(cPos.x), Mathf.Floor(cPos.y));
    }

    bool running = false;
    Vector3 prev = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)) {
            var inp = Input.mousePosition;
            AddCellScreen(inp);
            if (prev != Vector3.zero) {
                var l = GetPath(prev, pxToGrid(inp));
                foreach (var c in l)
                    if (!cells.ContainsKey(c))
                        AddCell(c);
            }
            prev = pxToGrid(inp);
        }

        if (Input.GetMouseButtonUp(0))
            prev = Vector3.zero;

        if (Input.GetKeyDown(KeyCode.Space))
            running = !running;
        if (Time.time > nextTrigger && running) {
            nextTrigger = Time.time + period;
            Next();
        }
        //if (Input.GetAxis("Input wheel")) return;
        Camera.main.orthographicSize += Input.mouseScrollDelta.y;
        Camera.main.orthographicSize = Mathf.Max(5.0f, Camera.main.orthographicSize);
        Camera.main.transform.position += Camera.main.orthographicSize * Vector3.right * Time.deltaTime * (Input.GetKey(KeyCode.D) ? 1 : 0);
        Camera.main.transform.position += Camera.main.orthographicSize * Vector3.up * Time.deltaTime * (Input.GetKey(KeyCode.W) ? 1 : 0);
        Camera.main.transform.position += Camera.main.orthographicSize * Vector3.left * Time.deltaTime * (Input.GetKey(KeyCode.A) ? 1 : 0);
        Camera.main.transform.position += Camera.main.orthographicSize * Vector3.down * Time.deltaTime * (Input.GetKey(KeyCode.S) ? 1 : 0);
    }

    void AddCellScreen(Vector3 posSc) {
        var pos = pxToGrid(posSc);
        AddCell(new Vector2(Mathf.Floor(pos.x), Mathf.Floor(pos.y)));
    }

    void AddCell(Vector2 pos) {
        if (!cells.ContainsKey(pos)) {
            cells.Add(pos, Draw(pos));
            IncNCount(pos);
        }
    }

    void RMCell(Vector2 pos) { 
        if (cells.ContainsKey(pos)) {
            Destroy(cells[pos]);
            cells.Remove(pos);
            DecNCount(pos);
        }
    }

    List<Vector2> GetPath(Vector2 f, Vector2 t) {
        List<Vector2> ret = new List<Vector2>();
        int xDiff = f.x < t.x ? 1 : -1;
        int yDiff = f.y < t.y ? 1 : -1;
        while (f != t ) {
            if (Mathf.Abs(f.x - t.x) > 0)
                f.x += xDiff;
            if (Mathf.Abs(f.y - t.y) > 0)
                f.y += yDiff;
            ret.Add(f);
        }
        return ret;
    }

    GameObject Draw(Vector2 pos) { 
        return Instantiate(cellPF, new Vector3(pos.x, pos.y, 1),Quaternion.identity, transform);
    }

    void Next() {
        var create = cellsNCount.Where(c => !cells.ContainsKey(c.Key) && c.Value == 3).ToList();
        var remove = cells.Where(c => !cellsNCount.ContainsKey(c.Key) || cellsNCount[c.Key] < 2 || cellsNCount[c.Key] > 3).ToList();

        foreach(var c in create) {
            AddCell(c.Key);
        }

        foreach(var c in remove) {
            RMCell(c.Key);
        }
    }
}
