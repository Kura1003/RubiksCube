using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Taki.Utility.Core;

namespace Taki.Utility
{
    public class ButtonEntrypoint : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> _targetParents = new();

        private List<PointerEventHandler> _handlers;

        public void CollectHandlers()
        {
            _handlers = _targetParents
                .Where(parent => parent != null)
                .SelectMany(parent => parent.GetComponentsInChildren<PointerEventHandler>(true))
                .ToList();
        }

        public void InitializeHandlers()
        {
            Thrower.IfNull(_handlers, nameof(_handlers));

            _handlers.ForEach(handler => handler.Initialize());
        }

        public void DisposeHandlers()
        {
            Thrower.IfNull(_handlers, nameof(_handlers));

            _handlers.ForEach(handler => handler.Dispose());
        }
    }
}