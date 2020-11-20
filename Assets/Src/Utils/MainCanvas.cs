using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    [SerializeField]
    Canvas _canvas = default;
    [SerializeField]
    Transform _transform = default;

    public static Canvas Canvas;
    public static Transform CanvasTransform;
    void Start()
    {
        Canvas = _canvas;
        CanvasTransform = _transform;
    }

    private void OnDestroy()
    {
        Canvas = null;
        CanvasTransform = null;
    }
}
