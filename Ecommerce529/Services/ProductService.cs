namespace Ecommerce529.Services
{
    public enum ProductImageType
    {
        MainImage  , 
        SubImage
    }
    public class ProductService
    {
        public string SaveFile(IFormFile ImageFile , ProductImageType productImageType = ProductImageType.MainImage)
        {
            try
            {
                //var fileName =  Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);  // folwer.jpg
                var fileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                //var filePath = "D:\\EraaSoft\\529\\project\\Ecommerce529\\Ecommerce529\\wwwroot\\images\\brand_images\\";
                var filePath = ""; 
                if (productImageType == ProductImageType.MainImage)
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\", fileName);
                }
                else if (productImageType == ProductImageType.SubImage)
                {
                    filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\product_sub_images\\", fileName);
                }

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }
                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errors {ex.Message}");
                return null;
            }

        }
        public bool RemoveFile(string fileName ,  ProductImageType productImageType = ProductImageType.MainImage)
        {
            try
            {
                var oldPath = ""; 
                if (productImageType == ProductImageType.MainImage)
                {
                    oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\", fileName);
                }
                else if (productImageType == ProductImageType.SubImage)
                {
                    oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\product_images\\product_sub_images\\", fileName);
                }

                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
                return true; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errors {ex.Message}");
                return false;
            }

        }
    }
}
