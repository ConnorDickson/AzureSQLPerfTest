using System;

namespace AzureSQLPerfTest.Models
{
    public enum FavouriteColour
    {
        Red, Blue, Orange, Pink, Black, Green
    }

    public class Test
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public FavouriteColour? FavouriteColour { get; set; }
    }
}
