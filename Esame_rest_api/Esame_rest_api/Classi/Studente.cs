using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Esame_rest_api.Classi
{
    public class Studente
    {
        
        
   
        [Key] //indico la primary key
        public int id { get; set; }

        public string name { get; set; } = null!;
        
        public int eta { get; set; }

        public List<Corso> corsi { get; set; } = new List<Corso>();
    }
}
