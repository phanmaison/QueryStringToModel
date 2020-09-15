using System;
using System.Collections.Generic;

namespace AzureFunction.Models
{
    public class TopModel
    {
        public string String1 { get; set; }

        public int Int1 { get; set; }

        public double Double1 { get; set; }

        public DateTime Date1 { get; set; }

        public List<string> ListString1 { get; set; }

        public List<int> ListInt1 { get; set; }
        public List<DateTime> ListDate1 { get; set; }

        public List<SubModel1> Model2s { get; set; }

    }

    public class SubModel1
    {
        public string String2 { get; set; }

        public int Int2 { get; set; }

        public double Double2 { get; set; }

        public DateTime Date2 { get; set; }

        public SubModel2 Model3 { get; set; }

        public List<SubModel2> Model3s { get; set; }
    }

    public class SubModel2
    {
        public string String3 { get; set; }

        public int Int3 { get; set; }

        public double Double3 { get; set; }

        public DateTime Date3 { get; set; }
    }
}
