using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData {

    private const int textureSize = 512;
    private const TextureFormat textureFormat = TextureFormat.RGB565;

    private float cachedMinHeight, cachedMaxHeight;

    public Layer[] layers;

    public void ApplyToMaterial(Material material)
    {      
        UpdateMeshHeight(material, cachedMinHeight, cachedMaxHeight);
    }

    public void UpdateMeshHeight(Material material, float minHeight, float maxHeight)
    {
        cachedMinHeight = minHeight;
        cachedMaxHeight = maxHeight;

        material.SetInt(Shader.PropertyToID("layersCount"), layers.Length);

        material.SetColorArray(Shader.PropertyToID("baseColors"), layers.Select(x => x.tint).ToArray());
        material.SetFloatArray(Shader.PropertyToID("baseColorsStrength"), layers.Select(x => x.tintStrength).ToArray());

        material.SetFloatArray(Shader.PropertyToID("baseBlends"), layers.Select(x => x.blendStrength).ToArray());

        material.SetFloatArray(Shader.PropertyToID("baseTextureScales"), layers.Select(x => x.textureScale).ToArray());

        material.SetFloatArray(Shader.PropertyToID("baseStartingHeights"), layers.Select(x => x.startingHeight).ToArray());
        material.SetFloat(Shader.PropertyToID("minHeight"), minHeight);
        material.SetFloat(Shader.PropertyToID("maxHeight"), maxHeight);

        Texture2DArray textureArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
        material.SetTexture("baseTextures", textureArray);
    }

    private Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(), i);
        }
        textureArray.Apply();
        return textureArray;
    }

    [System.Serializable]
    public class Layer
    {
        public Texture2D texture;
        public Color tint;
        [Range(0,1)]
        public float tintStrength;
        [Range(0, 1)]
        public float startingHeight;
        [Range(0, 1)]
        public float blendStrength;
        public float textureScale;     
    }

}

//public static class TestExtensions
//{
//    public static System.Collections.Generic.IEnumerable<TResult> MySelect<TSource, TResult>(
//    this System.Collections.Generic.IEnumerable<TSource> source,
//    System.Func<TSource, TResult> selector)
//    {
//        if (source == null)
//        {
//            throw new System.ArgumentNullException("source");
//        }
//        if (selector == null)
//        {
//            throw new System.ArgumentNullException("selector");
//        }
//        return MySelectInner(source, selector);
//    }

//    private static System.Collections.Generic.IEnumerable<TResult> MySelectInner<TSource, TResult>(
//        this System.Collections.Generic.IEnumerable<TSource> source,
//        System.Func<TSource, TResult> selector
//        )
//    {
//        foreach (TSource item in source)
//        {
//            yield return selector(item);
//        }
//    }
//}
