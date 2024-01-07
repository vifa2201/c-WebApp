

namespace TrainingApp.Models
{
    //modell för träningpass
    public class TrainingModel
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int Distance { get; set; }
        public string Comment { get; set; } = string.Empty;
public DateTime Date { get; set; } = DateTime.Now;
    }
}