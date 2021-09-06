using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScreenWrapping : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private Camera cam;
    
    [SerializeField] bool isWrappingX = false;
    [SerializeField] bool isWrappingY = false;

    // if at least one renderer is visible, return true
    bool CheckRenderers() { return renderers.Any(_renderer => _renderer.isVisible); } 
    private void Awake() 
    { 
        if(renderers.Count <= 0) renderers.Add(GetComponent<Renderer>());
        cam = FindObjectOfType<Camera>(); 
    } 
    private void Update() => ScreenWrap();

    private void ScreenWrap()
    {
        var isVisible = CheckRenderers();
        if (isVisible)
        {
            isWrappingX = false;
            isWrappingY = false;
            return;
        }
        if (isWrappingX && isWrappingY) return;

        var position = transform.position;
        
        var viewportPosition = cam.WorldToViewportPoint(position);
        var newPosition = position;

        // teleports the object based on the x and y position from the camera's viewport
        if (!isWrappingX && (viewportPosition.x > 1 || viewportPosition.x < 0)) 
        {
            newPosition.x = -newPosition.x;
            isWrappingX = true;
        }
        //if (!isWrappingY && (viewportPosition.y > 1 || viewportPosition.y < 0)) 
        //{
        //    newPosition.y = -newPosition.y;
        //    isWrappingY = true;
        //}
        //transform.position = newPosition;
    }
}
