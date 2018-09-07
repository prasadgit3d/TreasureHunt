
using UnityEngine;

namespace SillyGames.SGBase
{
    public static class Texture2DExtensions
    {
        /// <summary>
        /// Scales a texture to the specified resolution while maintaining the parameters of the source.
        /// Picked up from http://jon-martin.com/?p=114. Modified to take the source texture's mipmapping into account too.
        /// </summary>
        /// <returns>The resized texture.</returns>
        /// <param name="source">Source texture.</param>
        /// <param name="targetWidth">Target width.</param>
        /// <param name="targetHeight">Target height.</param>
        public static Texture2D scaleTexture(this Texture2D source, int targetWidth, int targetHeight)
        {
            // sanity - check if scaling is even needed
            if (source.width == targetWidth && source.height == targetHeight)
            {
                //Debug.Log("Utility: scaleTexture: " + source + ": Skipping scaling as source and target resolution are the same!");
                return (source);
            }

            // create a new texture based on the source 
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format,
                                             (source.mipmapCount > 1) ? true : false);	// if the orginal has more than the base texture, assume mipmapping

            //Debug.Log("Utility: scaleTexture: " + source + ": Scaling from " + source.width + "x" + source.height + " to " + result.width + "x" + result.height +
            //          " with mipmapping=" + ((result.mipmapCount > 1) ? true : false));

            Color[] rpixels = result.GetPixels(0);
            float incX = (1.0f / (float)targetWidth);
            float incY = (1.0f / (float)targetHeight);
            for (int px = 0; px < rpixels.Length; ++px)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
            }
            result.SetPixels(rpixels, 0);
            result.Apply();
            return result;
        }

        /// <summary>
        /// Create sprite from Texture2D asset, use same width & height as texture with sprite pivote to center.
        /// </summary>
        public static Sprite createSprite(this Texture2D source)
        {
            return Sprite.Create(source, new Rect(0, 0, source.width, source.height), new Vector2(0.5f, 0.5f));
        }
    }
}