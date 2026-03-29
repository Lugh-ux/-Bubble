namespace BubbleApp.Modelos
{
    public class ModeloBurbuja
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double DistanceKm { get; set; }

        public string DistanceLabel { get; set; }

        public bool IsCurrentUser { get; set; }

        public ModeloUsuario User { get; set; }
    }
}
