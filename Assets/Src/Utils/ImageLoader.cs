using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader : MonoBehaviour
{
    [SerializeField]
    string _url = default;

    [SerializeField]
    Vector2 _size = default;

    public static Texture2D GetImage(int id)
    {
        return _images.ContainsKey(id) ? _images[id] : null;
    }

    static Dictionary<int, Texture2D> _images = new Dictionary<int, Texture2D>();
    public IEnumerator StartLoading(IEnumerable<int> ids)
    {
        _images.Clear();

        var folderPath = $"{Application.persistentDataPath}/Images/";
        if(!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        Texture2D tex = null;

        foreach (var item in ids)
        {
            if(_images.ContainsKey(item))
            {
                continue;
            }

            string path = $"{folderPath}/{item}.png";
            if (File.Exists(path))
            {
                tex = new Texture2D((int)_size.x, (int)_size.y);
                if(tex.LoadImage(File.ReadAllBytes(path)))
                {
                    _images.Add(item, tex);
                    continue;
                }
                else
                {
                    File.Delete(path);
                }
            }

            string url = $"{_url}/id/{item}/{_size.x}/{_size.y}";
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Error in downloading img " + request.error);
            }
            else
            {
                tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
                _images.Add(item, tex);
                File.WriteAllBytes(path, tex.EncodeToPNG());
            }
        }
    }
}
