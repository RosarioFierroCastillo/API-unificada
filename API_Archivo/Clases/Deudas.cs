namespace API_Archivo.Clases
{
    public class Deudas

    {
        public int id_deudas { get; set; } //primary key

        public int id_fraccionamiento { get; set; }

        public int id_tesorero { get; set; }

        public float monto { get; set; }

        public string nombre { get; set; }

        public string descripcion { get; set; }

        public int dias_gracia { get; set; }

        public int periodicidad { get; set; }

        public float recargo { get; set; }

        public DateTime proximo_pago { get; set; }
    }
}
