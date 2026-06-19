
using UnityEngine;

namespace pingpong.igra
{
    [RequireComponent(typeof(RectTransform))]
    public class NotchSupport : MonoBehaviour
    {
        private void Awake()
        {
            //Get the Safe Area dimensions (Area that doesn't contain a notch)
            var rectTransform = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;
            var anchorMinimum = safeArea.position;
            var anchorMaximum = anchorMinimum + safeArea.size;

            //Remove the notch dimensions from the whole screen dimensions
            anchorMinimum.x /= Screen.width;
            anchorMinimum.y /= Screen.height;
            anchorMaximum.x /= Screen.width;
            anchorMaximum.y /= Screen.height;

            //Then set the new dimensions to the current rectTransform
            rectTransform.anchorMin = anchorMinimum;
            rectTransform.anchorMax = anchorMaximum;
        }
    }
}