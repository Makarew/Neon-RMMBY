using UnityEngine;

namespace RMMBY
{
    public class ModToggleButton : MonoBehaviour
    {
        private ModMenuHandler sys;

        private bool isHighlighted;
        private GameObject highlight;
        private GameObject unHighlight;

        private void Start()
        {
            sys = GameObject.FindObjectOfType<ModMenuHandler>();
            highlight = transform.Find("Highlight").gameObject;
            if (gameObject.name.StartsWith("Button"))
                unHighlight = transform.Find("UnHighlight").gameObject;
            else if (gameObject.name.StartsWith("Option"))
                unHighlight = transform.Find("ChoiceBack").gameObject;
        }

        private void Update()
        {
            if (!isHighlighted && sys.selectedObject == gameObject)
            {
                isHighlighted = true;
                highlight.SetActive(true);
                unHighlight.SetActive(false);
            }
            else if (isHighlighted && sys.selectedObject != gameObject)
            {
                highlight.SetActive(false);
                isHighlighted = false;
                unHighlight.SetActive(true);
            }
        }
    }
}