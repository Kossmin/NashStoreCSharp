namespace DTO.Models
{
    public class RequestGetProductModel
    {
        public int PageIndex { get; set; }
        public string ProductName { get; set; } = "";
        public int CategoryId { get; set; } = 0;
    }
}