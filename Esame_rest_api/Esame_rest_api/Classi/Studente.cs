namespace Esame_rest_api.Classi
{
    public class Studente
    {
        public int id { get; set; }

        public string name { get; set; } = null!;
        
        public int eta { get; set; }

        public List<Corso> corsi { get; set; } = new List<Corso>();
    }
}
