namespace SistemaStock.Models.ViewModels
{
    public class StockProductoViewModel
    {
        public int id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? CategoriaNombre { get; set; }

        public List<StockDepositoViewModel> Depositos { get; set; } = new();

        public int StockTotal => Depositos.Sum(d => d.Cantidad);
    }
}
