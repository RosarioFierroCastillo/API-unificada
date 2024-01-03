namespace API_Archivo.Clases
{
    public class Deudoress
    {
        public int id_deuda { get; set; }
        public string concepto { get; set; }
        public string persona { get; set; }
        public float monto { get; set; }
        public DateTime proximo_pago { get; set; }
    }
}
