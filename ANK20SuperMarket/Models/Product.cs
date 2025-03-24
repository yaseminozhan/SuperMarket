namespace ANK20SuperMarket.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public bool InStock { get; set; }


        //Veritabanına sadece resmin adı kaydedileceği için string olarak tanımladık.
        public string? PhotoName { get; set; }
    }
}
