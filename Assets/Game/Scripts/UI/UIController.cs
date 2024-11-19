using UnityEngine;
using UnityEngine.UIElements;
using Logger = Cinetica.Utility.Logger;

namespace Cinetica.UI
{
    public class UIController : MonoBehaviour
    {
        protected UIDocument _document;
        protected Logger _logger = new Logger("UI Controller Instance");
        
        public UIDocument GetDocument() => _document;
        
        public virtual void Awake()
        {
            _document = GetComponent<UIDocument>();
            if (_document == null)
            {
                Destroy(this);
            }
        }
    }
}