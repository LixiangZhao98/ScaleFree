using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class LoadField : MonoBehaviour
{
static public float[] LoadTexture3D(string filename)
    {
        
        Texture3D loadedTexture3D = AssetDatabase.LoadAssetAtPath<Texture3D>(filename);
        if (loadedTexture3D == null)
        {
            UnityEngine.Debug.LogError("Failed to load Texture3D asset at: " + filename);
            return null;
        }

        Color[] pixels = loadedTexture3D.GetPixels();
        // UnityEngine.Debug.Log("Successfully loaded Texture3D asset from: " + filename + " Total pixels: " + pixels.Length);


        float[] fs=new float[pixels.Length];
        for (int i = 0; i < pixels.Length; i++)
        {
            fs[i] = pixels[i].r;
        }
        return fs;
    }
    
    static public float[] LoadBin(string filename)
    {

        return BytesToFloats(LoadBytes(filename));
    }
    
    
    
    static byte[] LoadBytes(string filename)
    {
        try
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] byteArray = new byte[fs.Length];
                fs.Read(byteArray, 0, byteArray.Length);
                return byteArray;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);

            return new byte[0];
        }
    }

    
    static float[] BytesToFloats(byte[] bs)
    {
        float[] floatArray = new float[bs.Length / sizeof(float)];
        for (int i = 0; i < floatArray.Length; i++)
        {
            byte[] byteArray = new byte[sizeof(float)];
            for (int j = 0; j < sizeof(float); j++)
            {
                byteArray[j] = bs[i * sizeof(float) + j];
            }

            floatArray[i] = BitConverter.ToSingle(byteArray, 0);
        }

        return floatArray;
    }
    
}
