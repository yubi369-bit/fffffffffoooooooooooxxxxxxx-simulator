
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinecraftHealthUI : MonoBehaviour
{
    [Header("플레이어 체력")]
    public Health playerHealth;

    [Header("하트 설정")]
    public float heartSize = 28f;
    public float heartSpacing = 2f;

    [Header("화면 위치")]
    public Vector2 screenOffset = new Vector2(20f, 20f);

    [Header("색상")]
    public Color fullColor = new Color(0.95f, 0.12f, 0.12f);
    public Color emptyColor = new Color(0.28f, 0.28f, 0.28f);
    public Color outlineColor = Color.black;

    private readonly List<Image> hearts = new List<Image>();

    private Sprite fullHeart;
    private Sprite halfHeart;
    private Sprite emptyHeart;

    private RectTransform heartContainer;

    private int lastHp = -1;
    private int lastMaxHp = -1;

    private readonly string[] heartPattern =
    {
        "0011100011100",
        "0111110111110",
        "1111111111111",
        "1111111111111",
        "1111111111111",
        "0111111111110",
        "0011111111100",
        "0001111111000",
        "0000111110000",
        "0000011100000",
        "0000001000000"
    };

    void Start()
    {
        if (playerHealth == null)
        {
            GameObject player =
                GameObject.FindWithTag("Player");

            if (player != null)
                playerHealth = player.GetComponent<Health>();
        }

        CreateSprites();
        CreateContainer();
        RefreshHealth();
    }

    void LateUpdate()
    {
        if (playerHealth == null)
            return;

        if (lastMaxHp != playerHealth.maxHp ||
            lastHp != playerHealth.currentHp)
        {
            RefreshHealth();
        }
    }

    void CreateSprites()
    {
        fullHeart = GenerateHeart(2);
        halfHeart = GenerateHeart(1);
        emptyHeart = GenerateHeart(0);
    }

    Sprite GenerateHeart(int fillType)
    {
        int width = heartPattern[0].Length;
        int height = heartPattern.Length;

        Texture2D texture = new Texture2D(
            width,
            height,
            TextureFormat.RGBA32,
            false
        );

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int patternY = height - 1 - y;

                bool inside =
                    heartPattern[patternY][x] == '1';

                Color color = Color.clear;

                if (inside)
                {
                    bool edge = false;

                    int[,] neighbors =
                    {
                        { -1, 0 },
                        { 1, 0 },
                        { 0, -1 },
                        { 0, 1 }
                    };

                    for (int n = 0; n < 4; n++)
                    {
                        int nx = x + neighbors[n, 0];
                        int ny = patternY + neighbors[n, 1];

                        if (nx < 0 || nx >= width ||
                            ny < 0 || ny >= height ||
                            heartPattern[ny][nx] == '0')
                        {
                            edge = true;
                            break;
                        }
                    }

                    if (edge)
                    {
                        color = outlineColor;
                    }
                    else
                    {
                        bool filled =
                            fillType == 2 ||
                            (fillType == 1 && x < width / 2);

                        color = filled
                            ? fullColor
                            : emptyColor;
                    }
                }

                pixels[y * width + x] = color;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0, 0, width, height),
            new Vector2(0.5f, 0.5f),
            100f
        );
    }

    void CreateContainer()
    {
        GameObject container = new GameObject(
            "Minecraft Hearts",
            typeof(RectTransform)
        );

        container.transform.SetParent(
            transform,
            false
        );

        heartContainer =
            container.GetComponent<RectTransform>();

        // 화면 왼쪽 위 고정
        heartContainer.anchorMin = new Vector2(0f, 1f);
        heartContainer.anchorMax = new Vector2(0f, 1f);
        heartContainer.pivot = new Vector2(0f, 1f);

        // 왼쪽에서 20, 위에서 20만큼 떨어지게
        heartContainer.anchoredPosition = new Vector2(
            screenOffset.x,
            -screenOffset.y
        );
    }

    void BuildHearts()
    {
        foreach (Image image in hearts)
        {
            if (image != null)
                Destroy(image.gameObject);
        }

        hearts.Clear();

        int heartCount = Mathf.CeilToInt(
            Mathf.Max(1, playerHealth.maxHp) / 2f
        );

        float width = heartCount * heartSize +
            (heartCount - 1) * heartSpacing;

        heartContainer.sizeDelta =
            new Vector2(width, heartSize);

        for (int i = 0; i < heartCount; i++)
        {
            GameObject obj = new GameObject(
                "Heart " + (i + 1),
                typeof(RectTransform),
                typeof(Image)
            );

            obj.transform.SetParent(
                heartContainer,
                false
            );

            RectTransform rect =
                obj.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0f, 0f);

            rect.sizeDelta = new Vector2(
                heartSize,
                heartSize
            );

            rect.anchoredPosition = new Vector2(
                i * (heartSize + heartSpacing),
                0f
            );

            Image image = obj.GetComponent<Image>();

            image.sprite = emptyHeart;
            image.raycastTarget = false;

            hearts.Add(image);
        }
    }

    void RefreshHealth()
    {
        if (playerHealth == null)
            return;

        int maxHp = Mathf.Max(1, playerHealth.maxHp);
        int hp = Mathf.Clamp(
            playerHealth.currentHp, 0, maxHp
        );

        if (lastMaxHp != maxHp)
        {
            BuildHearts();
            lastMaxHp = maxHp;
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            int remaining = hp - i * 2;

            if (remaining >= 2)
                hearts[i].sprite = fullHeart;
            else if (remaining == 1)
                hearts[i].sprite = halfHeart;
            else
                hearts[i].sprite = emptyHeart;
        }

        lastHp = hp;
    }

    void OnDestroy()
    {
        DestroySprite(fullHeart);
        DestroySprite(halfHeart);
        DestroySprite(emptyHeart);
    }

    void DestroySprite(Sprite sprite)
    {
        if (sprite == null)
            return;

        if (sprite.texture != null)
            Destroy(sprite.texture);

        Destroy(sprite);
    }
}
