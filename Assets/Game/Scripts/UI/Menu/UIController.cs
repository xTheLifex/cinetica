using System;
using UnityEngine;
using UnityEngine.UIElements;
using Logger = Circuits.Utility.Logger;

namespace Circuits.UI
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