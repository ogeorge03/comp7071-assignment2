﻿namespace MVCSampleApp.Models
{
    public class Fascility
    {
        public int Id { get; set; }
        public StAddress St_Address { get; set; }

        public string? Name { get; set; }
        public string? Room { get; set; }
        public string? Description { get; set; }
        public decimal? Room_Rate { get; set; }

    }
}
