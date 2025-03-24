namespace ANK20SuperMarket.Models
{
    public class ProductVM
    {

        public string Name { get; set; }

        public double Price { get; set; }

        public bool InStock { get; set; }

        //Kullanıcıdan resim yüklemesi isteneceği için IFormFile türünde ekledik.
        public IFormFile? Photo { get; set; }
    }
}
