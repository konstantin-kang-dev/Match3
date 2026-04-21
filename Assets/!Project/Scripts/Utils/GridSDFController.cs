using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class GridSDFController : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int cols = 5;
    [SerializeField] private int rows = 9;
    [SerializeField] private float cellSize = 220f;
    [SerializeField] private float cornerRadius = 30f;

    private Material _material;
    private float[] _gridData;
    private Texture2D _gridTex;
    private RectTransform _rt;
    private bool _dirty;

    private static readonly int ColsProp = Shader.PropertyToID("_Cols");
    private static readonly int RowsProp = Shader.PropertyToID("_Rows");
    private static readonly int RadiusProp = Shader.PropertyToID("_Radius");
    private static readonly int RectSizeProp = Shader.PropertyToID("_RectSize");
    private static readonly int GridTexProp = Shader.PropertyToID("_GridTex");

    void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _material = GetComponent<Image>().material;
        _gridData = new float[cols * rows];

        _gridTex = new Texture2D(cols, rows, TextureFormat.R8, false);
        _gridTex.filterMode = FilterMode.Point;
        _gridTex.wrapMode = TextureWrapMode.Clamp;

        // Все ячейки активны по умолчанию
        for (int i = 0; i < _gridData.Length; i++)
            _gridData[i] = 1f;

        // Выставляем размер RectTransform под грид
        _rt.sizeDelta = new Vector2(cols * cellSize, rows * cellSize);

        _dirty = true;
    }

    void LateUpdate()
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

    public bool GetCell(int x, int y) => _gridData[y * cols + x] > 0.5f;

    public void SetGrid(bool[,] grid)
    {
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
                _gridData[y * cols + x] = grid[x, y] ? 1f : 0f;
        _dirty = true;
    }

    private void UploadToGPU()
    {
        var pixels = new Color32[cols * rows];
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
            {
                byte val = _gridData[y * cols + x] > 0.5f ? (byte)255 : (byte)0;
                pixels[y * cols + x] = new Color32(val, 0, 0, 255);
            }
        _gridTex.SetPixels32(pixels);
        _gridTex.Apply();

        float w = cols * cellSize;
        float h = rows * cellSize;

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
        for (int x = 0; x < cols; x++) grid[x, 0] = true;
        for (int x = 0; x < cols; x++) grid[x, 1] = true;
        for (int x = 1; x < cols; x++) grid[x, 2] = true;
        for (int x = 1; x < cols; x++) grid[x, 3] = true;
        for (int x = 2; x < cols; x++) grid[x, 4] = true;
        SetGrid(grid);
        UploadToGPU();
    }
}