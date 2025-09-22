
using Esame_rest_api.Classi;
using Microsoft.EntityFrameworkCore;

namespace Esame_rest_api.Context
{
    public class StudentiContext : DbContext
    {
        public StudentiContext(DbContextOptions<StudentiContext> options) : base(options) { }

        public DbSet<Studente> Studenti { get; set; }
        public DbSet<Corso> Corsi { get; set; }
    }
}
