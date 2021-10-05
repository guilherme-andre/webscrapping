namespace WebScraping.Models
{
    public class Result
    {
        public string Address { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }

        public Result(string address, string title, string price)
        {
            Address = address;
            Title = title;
            Price = price;
        }
    }
}
