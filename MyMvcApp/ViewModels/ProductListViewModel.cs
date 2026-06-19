namespace MyMvcApp.ViewModels
{
    public class ProductListViewModel
    {
        public string PageTitle { get; set; } = "";
        public List<ProductViewModel> Products { get; set; } = [];
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
    }
}