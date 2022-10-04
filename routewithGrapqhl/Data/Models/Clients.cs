using System.ComponentModel.DataAnnotations;

namespace API.Data.Models
{
    public class Clients
    {
        [Key]
        public int id { get; set; }

        public string Nombre { get; set; }

        public string Codigo { get; set; }
        public string NombreExtranjero { get; set; }
        public string Grupo { get; set; }
        public string RFC { get; set; }
        public string Calle { get; set; }
        public string Colonia { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }

        public bool Activo { get; set; }

    }
}
