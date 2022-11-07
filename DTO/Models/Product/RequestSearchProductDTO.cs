namespace NashPhaseOne.DTO.Models.Product
{
    public class RequestSearchProductDTO
    {
        public int PageIndex { get; set; }
        public string ProductName { get; set; } = "";
        public int? CategoryId { get; set; } = 0;
    }
}