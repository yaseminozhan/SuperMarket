using ANK20SuperMarket.Context;
using ANK20SuperMarket.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ANK20SuperMarket.Controllers
{


    public class MarketController : Controller
    {




        private readonly MarketDbContext _db;

        public MarketController(MarketDbContext db)
        {
            _db = db;
        }

        //Burası bütün ürünlerin listelendiği sayfanın action'ı olsun.

        public IActionResult Index()
        {

            return View(_db.Products.ToList());
        }


        //Burası SADECE ekleme formunu göstermek için
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(ProductVM productVM)
        {
            //Burada  gelen productVM'i veritabanındaki product'a eşleştirelim. Photo özelliği olarak IFormFile olduğu için onun adını alıp aktaracağız. Diğerleri aynı.

            if (productVM.Price <= 0)
            {
                TempData["Error"] = "The price must be positive! The price you have written is " +  productVM.Price;
                return View();  
            }
               
           

            Product product = new Product();
            product.Price = productVM.Price;
            product.Name = productVM.Name;
            product.InStock = productVM.InStock;

            //kişi resim seçmek zorunda değil

            if (productVM.Photo != null)
            {
                product.PhotoName = productVM.Photo.FileName;

                //şimdi resmi kaydedelim....



                //dosyanın kaydedileceği konumu belirleyelim
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", productVM.Photo.FileName);


                //dosya için bir akış ortamı oluşturalım....
                var akisOrtami = new FileStream(path, FileMode.Create);

                //Resmi o klasöre kaydet
                productVM.Photo.CopyTo(akisOrtami);

                //ortamı kapat
                akisOrtami.Close();
            }





            _db.Products.Add(product);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            //hangi ürün için çöp kutusuna tıklanıyorsa, o ürünün id'sini parametre olarak yakalarız. (yukarıdaki int id'den bahsediyorum)
            //O zaman bu id'ye göre ürünü tekrar db'den çekelim, ve emin misiniz view'ına gönderelim.


            return View(_db.Products.Find(id));
        }

        [HttpPost]
        public IActionResult Delete(Product product)
        {

            DeleteImage(product);
            _db.Products.Remove(product);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            //Hangi ürünün yanındaki güncelleme butonuna basılırsa, onun id'sini parametre olarak alacağız.Fakat artık VM ile ilgilendiğimiz için db'den gelen hakiki ürünü viewmodele aktarıp onu view'ına model olarak gönderelim. Böylece kişi formda dolu bilgiler olarak VM'in bilgilerini görür. Güncelleri yazar, yeni resim seçer. Sonra bu güncel VM'in özelliklerini, güncellenmek istenen HAKİKİ ürünün özelliklerine atayalım. Resim işlemleri de dahil. sonra onu update edelim.

            var product = _db.Products.Find(id);

            TempData["id"] = id;

            ProductVM productVM = new ProductVM();
            productVM.InStock = product.InStock;
            productVM.Name = product.Name;
            productVM.Price = product.Price;
            ViewBag.PhotoName = product.PhotoName;



            return View(productVM);

        }

        [HttpPost]
        public IActionResult Update(ProductVM productVM)
        {

            if (productVM.Price <= 0)
            {
                TempData["Error"] = "The price must be positive! The price you have written is " + productVM.Price;
                return View();
            }


            var product = _db.Products.Find(TempData["id"]);



            product.Price = productVM.Price;
            product.Name = productVM.Name;
            product.InStock = productVM.InStock;



            if (productVM.Photo != null)
            {
                DeleteImage(product);

                product.PhotoName = productVM.Photo.FileName;

                //şimdi resmi kaydedelim....



                //dosyanın kaydedileceği konumu belirleyelim
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", productVM.Photo.FileName);


                //dosya için bir akış ortamı oluşturalım....
                var akisOrtami = new FileStream(path, FileMode.Create);

                //Resmi o klasöre kaydet
                productVM.Photo.CopyTo(akisOrtami);

                //ortamı kapat
                akisOrtami.Close();
            }


            _db.Products.Update(product);
            _db.SaveChanges();

            return RedirectToAction("Index");

            //güncelleye basıldığı zaman buraya gelecek ve güncelleyecek


        }

        public void DeleteImage(Product product)
        {
            //Kendisi dışımda bu resmi kullanan başka bir ürün yoksa o resmi sil
            var resmiKullananBaskaVarMi = _db.Products.Any(p => p.PhotoName == product.PhotoName && p.Id != product.Id);

            if (!resmiKullananBaskaVarMi && product.PhotoName!=null)
            {
                //o zaman sil.
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/", product.PhotoName);

                System.IO.File.Delete(path);
            }

        }

        public IActionResult RemovePhoto(int id)
        {
            //gelen id'ye ait olan ürünü db den çek ve resmini kaldırmak istiyor musunuz sayfasına yönlendir.

            return View(_db.Products.Find(id));
        }

        [HttpPost]
        public IActionResult RemovePhoto(Product product)
        {
            DeleteImage(product);
            product.PhotoName = null;
            _db.Products.Update(product);
            _db.SaveChanges();
           

            return RedirectToAction("Index");
        }

    }
}
