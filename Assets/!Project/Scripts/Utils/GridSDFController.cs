using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GridSDFController : MonoBehaviour
{
    private static readonly int ColsProp = Shader.PropertyToID("_Cols");
    private static readonly int RowsProp = Shader.PropertyToID("_Rows");
    private static readonly int RadiusProp = Shader.PropertyToID("_Radius");
    private static readonly int RectSizeProp = Shader.PropertyToID("_RectSize");
    private static readonly int GridTexProp = Shader.PropertyToID("_GridTex");

    [Header("Grid Settings")] [SerializeField]
    private int cols = 5;

    [SerializeField] private int rows = 9;
    [SerializeField] private float cellSize = 220f;
    [SerializeField] private float cornerRadius = 30f;
    private bool _dirty;
    private float[] _gridData;
    private Texture2D _gridTex;

    private Material _material;
    private RectTransform _rt;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _material = GetComponent<Image>().material;
        _gridData = new float[cols * rows];

        _gridTex = new Texture2D(cols, rows, TextureFormat.R8, false);
        _gridTex.filterMode = FilterMode.Point;
        _gridTex.wrapMode = TextureWrapMode.Clamp;

        
        for (var i = 0; i < _gridData.Length; i++)
            _gridData[i] = 1f;

        
        _rt.sizeDelta = new Vector2(cols * cellSize, rows * cellSize);

        _dirty = true;
    }

    private void LateUpdate()
    {
        if (!_dirty) return;
        _dirty = false;
        UploadToGPU();
    }

    public void SetCell(int x, int y, bool active)
    {
        if (x < 0 || x >= cols || y < 0 || y >= rows) return;
        _gridData[y * cols + x] = active ? 1f : 0f;
        _dirty = true;
    }

    public bool GetCell(int x, int y)
    {
        return _gridData[y * cols + x] > 0.5f;
    }

    public void SetGrid(bool[,] grid)
    {
        for (var y = 0; y < rows; y++)
        for (var x = 0; x < cols; x++)
            _gridData[y * cols + x] = grid[x, y] ? 1f : 0f;
        _dirty = true;
    }

    private void UploadToGPU()
    {
        var pixels = new Color32[cols * rows];
        for (var y = 0; y < rows; y++)
        for (var x = 0; x < cols; x++)
        {
            var val = _gridData[y * cols + x] > 0.5f ? (byte)255 : (byte)0;
            pixels[y * cols + x] = new Color32(val, 0, 0, 255);
        }

        _gridTex.SetPixels32(pixels);
        _gridTex.Apply();

        var w = cols * cellSize;
        var h = rows * cellSize;

        _material.SetTexture(GridTexProp, _gridTex);
        _material.SetFloat(ColsProp, cols);
        _material.SetFloat(RowsProp, rows);
        _material.SetFloat(RadiusProp, cornerRadius);
        _material.SetVector(RectSizeProp, new Vector4(w, h, 0, 0));
    }

    [ContextMenu("Test Pattern")]
    private void TestPattern()
    {
        if (_material == null) _material = GetComponent<Image>().material;
        if (_gridData == null) _gridData = new float[cols * rows];
        if (_gridTex == null)
        {
            _gridTex = new Texture2D(cols, rows, TextureFormat.R8, false);
            _gridTex.filterMode = FilterMode.Point;
            _gridTex.wrapMode = TextureWrapMode.Clamp;
        }

        if (_rt == null) _rt = GetComponent<RectTransform>();

        var grid = new bool[cols, rows];
        for (var x = 0; x < cols; x++) grid[x, 0] = true;
        for (var x = 0; x < cols; x++) grid[x, 1] = true;
        for (var x = 1; x < cols; x++) grid[x, 2] = true;
        for (var x = 1; x < cols; x++) grid[x, 3] = true;
        for (var x = 2; x < cols; x++) grid[x, 4] = true;
        SetGrid(grid);
        UploadToGPU();
    }
}