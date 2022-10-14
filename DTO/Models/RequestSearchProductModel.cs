namespace DTO.Models
{
    public class RequestSearchProductModel
    {
        public int PageIndex { get; set; }
        public string ProductName { get; set; } = "";
        public int CategoryId { get; set; } = 0;
    }
}