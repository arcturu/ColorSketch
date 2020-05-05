using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

class Brush
{
    public int PaletteIndex { get; set; }
}

public class InputManager : MonoBehaviour
{
    public new Camera camera;
    public Material BrushMaterial;
    public GameObject PaletteGameObject;
    public Image ColorPreview;
    public SliderManager SliderManager;

    int polygonCount = 0;
    bool isStroking = false;
    List<Vector2> stroke = new List<Vector2>();
    Mesh mesh;
    List<Color> palette;
    List<Image> paletteImages;
    Brush currentBrush;

    // Start is called before the first frame update
    void Start()
    {
        polygonCount = 0;
        isStroking = false;
        stroke = new List<Vector2>();
        palette = new List<Color>
        {
            GetColor(205, 31, 66),
            GetColor(230, 109, 0),
            GetColor(226, 197, 0),
            GetColor(74, 163, 21),
            GetColor(0, 126, 119),
            GetColor(0, 90, 145),
            GetColor(71, 71, 152),
            GetColor(137, 44, 113),
            GetColor(255, 255, 255),
            GetColor(48, 48, 48),
        };
        paletteImages = new List<Image>();
        for (int i = 0; i < 10; i++)
        {
            var child = PaletteGameObject.transform.Find($"Color{i}");
            var image = child.GetComponent<Image>();
            image.color = palette[i];
            paletteImages.Add(image);
        }
        currentBrush = new Brush { PaletteIndex = 0 };
        UpdateColorPreviewAndSetColorPiker();
    }

    // Update is called once per frame
    void Update()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        var pos =new Vector2(ray.origin.x, ray.origin.y); // TODO: まじめな座標変換
        if (Input.GetMouseButtonDown(0))
        {
            if (pos.x >= -0.5 && pos.x < 0.5 && pos.y >= -0.5 && pos.y < 0.5)
            {
                isStroking = true;
                stroke.Clear();
                CreateMesh(stroke);
            }
        }
        if (isStroking && Input.GetMouseButtonUp(0))
        {
            isStroking = false;
            Debug.Log(stroke.Count);
        }
        if (isStroking && Input.GetMouseButton(0))
        {
            stroke.Add(pos);
            UpdateMesh(stroke);
        }
    }

    void CreateMesh(List<Vector2> stroke)
    {
        var go = new GameObject();
        go.name = "Polygon";
        go.transform.parent = this.transform;
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);

        var mesh = new Mesh();
        mesh.vertices = PurifyStroke(stroke).Select(v2 => new Vector3(v2.x, v2.y, -(polygonCount + 1) / 100.0f)).ToArray();
        var indices = new List<int>();
        Triangulate.EarCut(mesh.vertices, indices);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

        var renderer = go.AddComponent<MeshRenderer>();
        renderer.material = BrushMaterial;
        renderer.material.color = palette[currentBrush.PaletteIndex];
        var filter = go.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        this.mesh = mesh;

        polygonCount++;
    }

    void UpdateMesh(List<Vector2> stroke)
    {
        mesh.vertices = PurifyStroke(stroke).Select(v2 => new Vector3(v2.x, v2.y, -(polygonCount + 1) / 100.0f)).ToArray();
        var indices = new List<int>();
        Triangulate.EarCut(mesh.vertices, indices);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
        
    }

    public void OnClickColor0()
    {
        currentBrush.PaletteIndex = 0;
        UpdateColorPreviewAndSetColorPiker();
    }

    public void OnClickColor1()
    {
        currentBrush.PaletteIndex = 1;
        UpdateColorPreviewAndSetColorPiker();
    }

    public void OnClickColor2()
    {
        currentBrush.PaletteIndex = 2;
        UpdateColorPreviewAndSetColorPiker();
    }

    public void OnClickColor3()
    {
        currentBrush.PaletteIndex = 3;
        UpdateColorPreviewAndSetColorPiker();
    }

    public void OnClickColor4()
    {
        currentBrush.PaletteIndex = 4;
        UpdateColorPreviewAndSetColorPiker();
    }

    public void OnClickColor5()
    {
        currentBrush.PaletteIndex = 5;
        UpdateColorPreviewAndSetColorPiker();
    }

    public void OnClickColor6()
    {
        currentBrush.PaletteIndex = 6;
        UpdateColorPreviewAndSetColorPiker();
    }

    public void OnClickColor7()
    {
        currentBrush.PaletteIndex = 7;
        UpdateColorPreviewAndSetColorPiker();
    }

    public void OnClickColor8()
    {
        currentBrush.PaletteIndex = 8;
        UpdateColorPreviewAndSetColorPiker();
    }

    public void OnClickColor9()
    {
        currentBrush.PaletteIndex = 9;
        UpdateColorPreviewAndSetColorPiker();
    }

    public void OnSliderValueChanged(float[] values, int index)
    {
        palette[currentBrush.PaletteIndex] = Color.HSVToRGB(values[0], values[1], values[2]);
        UpdateColorPreview();
    }

    void UpdateColorPreview()
    {
        ColorPreview.color = palette[currentBrush.PaletteIndex];
        paletteImages[currentBrush.PaletteIndex].color = palette[currentBrush.PaletteIndex];
    }

    void UpdateColorPreviewAndSetColorPiker()
    {
        UpdateColorPreview();
        Color.RGBToHSV(palette[currentBrush.PaletteIndex], out float h, out float s, out float v);
        SliderManager.SetValues(new float[3] { h, s, v });
    }

    /// <summary>
    /// stroke 中の縮退辺を削除します。
    /// </summary>
    /// <param name="stroke">点列</param>
    /// <returns>縮退辺のない点列</returns>
    static List<Vector2> PurifyStroke(List<Vector2> stroke)
    {
        const float EPS = 1e-5f;

        var res = new List<Vector2>();
        if (stroke.Count <= 1)
        {
            return stroke;
        }
        res.Add(stroke[0]);
        var last = res[0];

        for (int i = 1; i < stroke.Count; i++)
        {
            if (Vector2.Distance(stroke[i], last) > EPS)
            {
                res.Add(stroke[i]);
                last = stroke[i];
            }
        }

        return res;
    }

    /// <summary>
    /// 0 ~ 255 の RGB 値から Color オブジェクトを生成します。
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    static Color GetColor(int r, int g, int b)
    {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }
}
