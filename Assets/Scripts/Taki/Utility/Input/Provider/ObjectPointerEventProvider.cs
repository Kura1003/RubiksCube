using R3;
using UnityEngine;

namespace Taki.Utility
{
    [RequireComponent(typeof(Collider))]
    public sealed class ObjectPointerEventProvider : BasePointerEventProvider
    {
        private void OnMouseDown()
        {
            ExecuteOnClicked();
        }

        private void OnMouseEnter()
        {
            ExecuteOnPointerEntered();
        }

        private void OnMouseExit()
        {
            ExecuteOnPointerExited();
        }
    }
}