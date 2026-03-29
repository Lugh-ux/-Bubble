using System.Collections.Generic;

namespace BubbleApp.Modelos
{
    public class ModeloPanelPrincipal
    {
        public ModeloUsuario CurrentUser { get; set; }

        public bool MyBubbleActive { get; set; }

        public ModeloBurbuja MyBubble { get; set; }

        public List<ModeloBurbuja> Bubbles { get; set; } = new List<ModeloBurbuja>();
    }
}
