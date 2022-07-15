using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using MoralisUnity;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NFTPurchaser : MonoBehaviour
{
    public static NFTPurchaser insta;

    private void Awake()
    {
        insta = this;
    }

    private void Start()
    {
        //StartCoroutine(UploadPNG());
    }

    private async UniTask<string> CreateIpfsMetadata()
    {
        // 1. Build Metadata
        object metadata = MoralisTools.BuildMetadata("name", "description", "imageUrl");

        string metadataName = $"{"name"}_{"objectId"}.json";

        // 2. Encoding JSON
        string json = JsonConvert.SerializeObject(metadata);
        string base64Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

        // 3. Save metadata to IPFS
        //string ipfsMetadataPath = await MoralisTools.SaveToIpfs(metadataName, base64Data);

        return json;
    }


    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        form.AddField("meta", "myData");

        using (UnityWebRequest www = UnityWebRequest.Post("https://api.nft.storage/store", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log("Form upload error " + www.downloadHandler.text);
            }
            else
            {
                Debug.Log("Form upload complete! " + www.downloadHandler.text);
            }
        }
    }

    public string metaData;
    IEnumerator UploadPNG()
    {
        // We should only read the screen after all rendering is complete
        yield return new WaitForEndOfFrame();

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        // Read screen contents into the texture
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);

        // Create a Web Form
        WWWForm form = new WWWForm();
       // form.AddField("meta", metaData);
        form.AddBinaryData("file", bytes, "screenShot.png", "image/*");

        var data = new List<IMultipartFormSection> {
             new MultipartFormDataSection("foo", "bar"),
             new MultipartFormFileSection("file", bytes, "test.png", "image/*")
         };

        // Upload to a cgi script
        using (var w = UnityWebRequest.Post("https://api.nft.storage/upload", form))
        {
            w.SetRequestHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkaWQ6ZXRocjoweDZBNDA4Q0ZiOTJDNDlCYjk3ZDhENDg4NUE3MGE3NkNhOWVBYUIyNjIiLCJpc3MiOiJuZnQtc3RvcmFnZSIsImlhdCI6MTY1Nzg3NDg0ODU1MywibmFtZSI6Ik1ldGFKdW5nbGUifQ.DHiD9jVmKkMJQaZtF6WLUO7QpGwnXiAi2s4l_Lt_BRA");
            yield return w.SendWebRequest();
            if (w.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(w.error);
                Debug.Log("Form upload error " + w.downloadHandler.text);
            }
            else
            {
                Debug.Log("Form upload complete! " + w.downloadHandler.text);
            }
        }
    }
}
