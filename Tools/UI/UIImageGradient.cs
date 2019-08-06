using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIImageGradient : MonoBehaviour
{
    [System.Serializable]
    public struct GradientPoint
    {
        public Color Color;
        [Range(0, 1)]
        public float PositionX;
        [Range(0, 1)]
        public float PositionY;

        public GradientPoint(Color color, float positionX, float positionY)
        {
            Color = color;
            PositionX = positionX;
            PositionY = positionY;
        }
    }

    public Image Image;
    [HideInInspector]
    public Sprite Sprite;
    [HideInInspector]
    public Texture2D Texture2D;
    public Vector2 Resolution = new Vector2(100, 100);

    public Color[,] ColorMapTable = new Color[4, 4];

    public Color LinearFrom = Color.white;
    public Color LinearTo = Color.black;
    public float LinearTime;
    public Color LinearResult = Color.gray;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        Image = GetComponent<Image>();

        if (Image != null)
        {
            Texture2D = new Texture2D((int)Resolution.x, (int)Resolution.y);
            Sprite = Sprite.Create(Texture2D, new Rect(0, 0, (int)Resolution.x, (int)Resolution.y), new Vector2(0.5f, 0.5f));
            Image.sprite = Sprite;

            UpdateTexture();
        }
    }

    public void UpdateTexture()
    {
        if (Texture2D == null) Init();

        if (Texture2D != null)
        {
            Color buffer = Color.white;
            for (int x = 0; x < Texture2D.width; x++)
            {
                for (int y = 0; y < Texture2D.height; y++)
                {
                    buffer = BiSplineInterpolation(Convert(ColorMapTable), 1 - (y / (Texture2D.height - 1.0f)), x / (Texture2D.width - 1.0f));
                    Texture2D.SetPixel(x, y, buffer);
                }
            }

            Texture2D.Apply();
        }
    }

    public Color SplineInterp(Color[] p, float x)
    {
        int numSections = p.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(x * (float)numSections), numSections - 1);
        float u = x * (float)numSections - (float)currPt;

        return new Color(SplineMathFunction(p[currPt].r, p[currPt + 1].r, p[currPt + 2].r, p[currPt + 3].r, u),
            SplineMathFunction(p[currPt].g, p[currPt + 1].g, p[currPt + 2].g, p[currPt + 3].g, u),
            SplineMathFunction(p[currPt].b, p[currPt + 1].b, p[currPt + 2].b, p[currPt + 3].b, u));
    }

    public float SplineMathFunction(float a, float b, float c, float d, float u)
    {
        return .5f * (
            (-a + 3f * b - 3f * c + d) * (u * u * u)
            + (2f * a - 5f * b + 4f * c - d) * (u * u)
            + (-a + c) * u
            + 2f * b
        );
    }

    public Color BiSplineInterpolation(Color[][] points, float timeX, float timeY)
    {
        Color[] aux = new Color[points.Length];

        for (int i = 0; i < points.Length; i++)
            aux[i] = SplineInterp(points[i], timeX);

        return SplineInterp(aux, timeY);
    }

    public Color[][] Convert(Color[,] inputArray)
    {
        Color[][] result = new Color[inputArray.GetLength(0)][];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = new Color[inputArray.GetLength(1)];

            for (int j = 0; j < result[i].Length; j++)
            {
                result[i][j] = inputArray[i, j];
            }
        }

        return result;
    }

    void linearSampler()
    {
        LinearResult = Color.Lerp(LinearFrom, LinearTo, LinearTime);
    }

    void Reset()
    {
        Image = GetComponent<Image>();
    }
}
