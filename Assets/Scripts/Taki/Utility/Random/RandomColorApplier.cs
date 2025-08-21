using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Taki.Utility
{
    public class RandomColorApplier : MonoBehaviour
    {
        [SerializeField]
        private List<Renderer> _renderersToApply = new();

        [SerializeField]
        private List<Graphic> _graphicsToApply = new();

        private void Awake()
        {
            ApplyColor();
        }

        public void ApplyColor()
        {
            Color colorToApply = RandomUtility.GetColor();

            _renderersToApply.Where(rend => rend != null)
                             .ToList()
                             .ForEach(rend =>
                             {
                                 rend.material.color = colorToApply;
                             });

            _graphicsToApply.Where(graphic => graphic != null)
                            .ToList()
                            .ForEach(graphic =>
                            {
                                graphic.color = colorToApply;
                            });
        }
    }
}