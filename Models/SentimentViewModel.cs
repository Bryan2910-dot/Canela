namespace Canela.Models
{
    public class SentimentViewModel
    {
        public string Comment { get; set; }
        public string Result { get; set; }
        public string AlertClass { get; set; } // "success" o "danger"
        public float Probability { get; set; }
    }
}