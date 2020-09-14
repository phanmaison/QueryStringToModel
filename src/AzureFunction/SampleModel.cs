using System;
using System.Collections.Generic;

namespace AzureFunction
{
    public class Model1
    {

        public string String1 { get; set; }

        public int Int1 { get; set; }

        public double Double1 { get; set; }

        public DateTime Date1 { get; set; }

        public List<string> List1 { get; set; }

        public List<Model2> Model2s { get; set; }

    }

    public class Model2
    {
        public string String2 { get; set; }

        public int Int2 { get; set; }

        public double Double2 { get; set; }

        public DateTime Date2 { get; set; }

        public Model3 Model3 { get; set; }

        public List<Model3> Model3s { get; set; }
    }

    public class Model3
    {
        public string String3 { get; set; }

        public int Int3 { get; set; }

        public double Double3 { get; set; }

        public DateTime Date3 { get; set; }

    }
}
