namespace PlantShop.Models
{
    public class PlantResultViewModel
    {
        public string PlantName { get; set; }

        public double Probability { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }
        public string Family { get; set; }

        public string Genus { get; set; }

        public string WikiDescription { get; set; }
        public string WikiUrl { get; set; }
    }
}