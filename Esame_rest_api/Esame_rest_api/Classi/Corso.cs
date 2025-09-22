using System.ComponentModel.DataAnnotations;

namespace Esame_rest_api.Classi
{
    public class Corso
    {
        [Key] //indico la primary key
        public int id { get; set; }
        public String nome { get; set; } = null!;

        public int ore { get; set; } 

    }
}
