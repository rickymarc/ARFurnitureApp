using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ScriptScreenshot : MonoBehaviour
{
    protected const string storeMedia= "android.provider.MediaStore$Images$Media";
    protected static AndroidJavaObject m_Activity;
    protected static string SaveImageToGallery(Texture2D a_Texture, string a_Title, string a_Description)
    {
        using (AndroidJavaClass mediaClass = new AndroidJavaClass(storeMedia))
        {
            using (AndroidJavaObject contentResolver = Activity.Call<AndroidJavaObject>("getContentResolver"))
            {
                AndroidJavaObject image = Texture2DToAndroidBitmap(a_Texture);
                return mediaClass.CallStatic<string>("insertImage", contentResolver, image, a_Title, a_Description);
            }
        }
    }

    protected static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D a_Texture)
    {
        byte[] encodedTexture = a_Texture.EncodeToPNG();
        using (AndroidJavaClass bitmapFactory = new AndroidJavaClass("android.graphics.BitmapFactory"))
        {
            return bitmapFactory.CallStatic<AndroidJavaObject>("decodeByteArray", encodedTexture, 0, encodedTexture.Length);
        }
    }

    protected static AndroidJavaObject Activity
    {
        get
        {
            if (m_Activity == null)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                m_Activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return m_Activity;
        }
    }

    public void CaptureScreenshot()
    {
        StartCoroutine(CaptureScreenshotCoroutine(Screen.width, Screen.height));
    }

    private IEnumerator CaptureScreenshotCoroutine(int width, int height)
    {
        yield return new WaitForEndOfFrame();
        Texture2D texture1 = new Texture2D(width, height);
        texture1.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture1.Apply();

        yield return texture1;
        string path = SaveImageToGallery(texture1, "Name", "Description");
        Debug.Log("Path:\n" + path);
    }
}
