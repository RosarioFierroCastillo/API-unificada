using MySql.Data.MySqlClient;

namespace API_Archivo.Clases
{
    public class Egresos
    {
        public string concepto { get; set; }
        public string  descripcion { get; set; }
        public string proveedor { get; set; }
        public double monto { get; set; }
        public string fecha { get; set; }


    }
}
