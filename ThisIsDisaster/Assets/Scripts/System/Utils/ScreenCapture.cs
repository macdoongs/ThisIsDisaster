using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCapture : MonoBehaviour {
    
    Texture2D snapShot = null;
    public UnityEngine.UI.RawImage[] rawImages;
    private bool take = false;

    public float xOffset;
    public float yOffset;

    private Vector2 initialSize;
    
    private void Start()
    {
        initialSize = rawImages[0].rectTransform.sizeDelta;
    }

    private void Update()
    {

    }

    IEnumerator Delay(float delay) {
        yield return new WaitForSeconds(delay);
        take = true;
    }

    public void Capature(float delay = 0f) {
        if (delay == 0f)
        {
            take = true;
        }
        else {
            StartCoroutine(Delay(delay));
        }
    }

    private void OnPostRender()
    {
        if (take)
        {
            int width = (int)(Screen.width * xOffset);
            int height = (int)(Screen.height * yOffset);
            take = false;
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, true);
            int sx = Screen.width;
            int sy = Screen.height;

            int x = (int)((sx - width) * 0.5f);
            int y = (int)((sy - height) * 0.5f);

            texture.ReadPixels(new Rect(x, y, width, height), 0, 0);

            texture.Apply();
            snapShot = texture;
            foreach (var rawImage in rawImages)
            {
                rawImage.texture = texture;
                rawImage.SetNativeSize();
            }
            int std = 0;
            float rate = 0f;
            Vector2 size = initialSize;
            if (height > width)
            {
                std = height;
                rate = initialSize.y / (float)height;
                size = new Vector2(width * rate, initialSize.y);
            }
            else {
                std = width;
                rate = initialSize.x / (float)width;
                size = new Vector2(initialSize.x, height * rate);
            }

            foreach (var rawImage in rawImages) {
                rawImage.rectTransform.sizeDelta = size;
            }
        }
    }
}
