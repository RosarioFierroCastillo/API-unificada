using API_Archivo.Clases;
using API_Archivo.Deudores;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Threading;

namespace API_Archivo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeudasController : ControllerBase
    {
        //AQUÍ VAN TODAS LAS DEUDAS
        [HttpPost]
        [Route("Agregar_Deuda")]

        public bool Agregar_Deuda([FromBody] Deudas request)
        {

            bool fraccionamiento_agregado = false;
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into deudas " +
                    "(id_deudas, id_fraccionamiento, id_tesorero, monto, nombre, descripcion," +
                    "dias_gracia, periodicidad, recargo, proximo_pago) " +
                    "VALUES (@id_deudas, @id_fraccionamiento, @id_tesorero, @monto, @nombre, @descripcion, @dias_gracia, " +
                    "@periodicidad, @recargo, @proximo_pago)", conexion);

                //Nombre_fraccionamiento=@Nombre_fraccionamiento, Direccion=@Direccion, Coordenadas=@Coordenadas, id_administrador=@id_administrador, id_tesorero=@id_tesorero)

                DateTime now = DateTime.Now;
                DateTime Dateproximo_pago = request.proximo_pago.AddDays(request.periodicidad);
                // string fechaProximoPago = Dateproximo_pago.ToString("yyyy-MM-ddTHH:mm:ss");

                comando.Parameters.Add("@id_deudas", MySqlDbType.Int32).Value = request.id_deudas;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = request.id_fraccionamiento;
                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = request.id_tesorero;
                comando.Parameters.Add("@monto", MySqlDbType.Float).Value = request.monto;
                comando.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = request.nombre;
                comando.Parameters.Add("@descripcion", MySqlDbType.VarChar).Value = request.descripcion;
                comando.Parameters.Add("@dias_gracia", MySqlDbType.Int32).Value = request.dias_gracia;
                comando.Parameters.Add("@periodicidad", MySqlDbType.Int32).Value = request.periodicidad;
                comando.Parameters.Add("@recargo", MySqlDbType.Float).Value =   request.recargo;
                comando.Parameters.Add("@proximo_pago", MySqlDbType.Date).Value = Dateproximo_pago;

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        fraccionamiento_agregado = true;
                    }

                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {
                    conexion.Close();
                }

                Deudas obj_deudas = new Deudas();
                if (fraccionamiento_agregado)
                {
                    if (obj_deudas.AsignarDeudaNuevaATodos(request.id_fraccionamiento))
                    {
                        fraccionamiento_agregado = true;
                    }
                    else
                    {
                        fraccionamiento_agregado = false;
                    }
                }

                return fraccionamiento_agregado;
            }
        }

        [HttpGet]
        [Route("Consultar_Deuda")]

        public List<Deudas> Consultar_DeudasOrdinarias(int id_tesorero)
        {

            List<Deudas> Deuda = new List<Deudas>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudas WHERE id_tesorero=@id_tesorero && periodicidad>0", conexion);

                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = id_tesorero;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deuda.Add(new Deudas()
                        {
                            id_deudas = reader.GetInt32(0),
                            monto = reader.GetFloat(3),
                            nombre = reader.GetString(4),
                            descripcion = reader.GetString(5),
                            dias_gracia = reader.GetInt32(6),
                            periodicidad = reader.GetInt32(7),
                            recargo = reader.GetFloat(8),
                            proximo_pago = reader.GetDateTime(9)

                        });
                        // MessageBox.Show();
                    }


                }
                catch (MySqlException ex)
                {

                }
                finally
                {
                    conexion.Close();
                }

                return Deuda;
            }

        }


        [HttpPut]
        [Route("Actualizar_Deuda")]

        public bool actualizar_DeudasOrdinarias([FromBody] Deudas request)
        {
            bool Propiedad_actualizada = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("UPDATE deudas " +
                    "SET monto=@monto, " +
                    "nombre=@nombre, " +
                    "descripcion=@descripcion, " +
                    "dias_gracia=@dias_gracia, " +
                    "periodicidad=@periodicidad, " +
                    "recargo=@recargo " +
                    "WHERE id_deudas=@id_deudas", conexion);

                comando.Parameters.Add("@monto", MySqlDbType.Float).Value = request.monto;
                comando.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = request.nombre;
                comando.Parameters.Add("@descripcion", MySqlDbType.VarChar).Value = request.descripcion;
                comando.Parameters.Add("@dias_gracia", MySqlDbType.Int32).Value = request.dias_gracia;
                comando.Parameters.Add("@periodicidad", MySqlDbType.Int32).Value = request.periodicidad;
                comando.Parameters.Add("@recargo", MySqlDbType.Float).Value = request.recargo;
                comando.Parameters.Add("@id_deudas", MySqlDbType.Int32).Value = request.id_deudas;

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Propiedad_actualizada = true;
                    }

                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {
                    conexion.Close();
                }
                return Propiedad_actualizada;

            }


        }


        [HttpDelete]
        [Route("Eliminar_Deuda")]
        public bool Eliminar_Deudas(int id_deudas)
        {
            bool Deuda_eliminada = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("DELETE FROM deudas WHERE id_deudas=@id_deudas", conexion);

                comando.Parameters.Add("@id_deudas", MySqlDbType.Int32).Value = id_deudas;




                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Deuda_eliminada = true;
                        Deudas obj_deuda = new Deudas();
                        obj_deuda.EliminarDeudasAUsuarios(id_deudas);
                    }

                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {
                    conexion.Close();
                }
                return Deuda_eliminada;

            }
        }



        [HttpPost]
        [Route("Agregar_DeudaExtra")]

        public bool Agregar_DeudaExtra([FromBody] Deudas request)
        {
            Console.WriteLine(request.proximo_pago);

            bool fraccionamiento_agregado = false;
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into deudas " +
                    "(id_deudas, id_fraccionamiento, id_tesorero, monto, nombre, descripcion," +
                    "proximo_pago) " +
                    "VALUES (@id_deudas, @id_fraccionamiento, @id_tesorero, @monto, @nombre, @descripcion," +
                    "@proximo_pago)", conexion);

                //Nombre_fraccionamiento=@Nombre_fraccionamiento, Direccion=@Direccion, Coordenadas=@Coordenadas, id_administrador=@id_administrador, id_tesorero=@id_tesorero)

               // DateTime now = DateTime.Now;
              //  DateTime Dateproximo_pago = DateTime.Now.AddDays(request.periodicidad);
                // string fechaProximoPago = Dateproximo_pago.ToString("yyyy-MM-ddTHH:mm:ss");

                comando.Parameters.Add("@id_deudas", MySqlDbType.Int32).Value = request.id_deudas;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = request.id_fraccionamiento;
                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = request.id_tesorero;
                comando.Parameters.Add("@monto", MySqlDbType.Float).Value = request.monto;
                comando.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = request.nombre;
                comando.Parameters.Add("@descripcion", MySqlDbType.VarChar).Value = request.descripcion;
              //  comando.Parameters.Add("@dias_gracia", MySqlDbType.Int32).Value = request.dias_gracia;
              //  comando.Parameters.Add("@periodicidad", MySqlDbType.Int32).Value = request.periodicidad;
              //  comando.Parameters.Add("@recargo", MySqlDbType.Float).Value = request.recargo;
                comando.Parameters.Add("@proximo_pago", MySqlDbType.Date).Value = request.proximo_pago;

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        fraccionamiento_agregado = true;
                    }

                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {

                }

                Deudas obj_deudas = new Deudas();
                if (fraccionamiento_agregado)
                {
                    if (obj_deudas.AsignarDeudaNuevaATodos(request.id_fraccionamiento))
                    {
                        fraccionamiento_agregado = true;
                    }
                    else
                    {
                        fraccionamiento_agregado = false;
                    }
                }

                return fraccionamiento_agregado;
            }
        }


        [HttpGet]
        [Route("Consultar_DeudaExtra")]

        public List<Deudas> Consultar_DeudaExtra(int id_tesorero)
        {

            List<Deudas> Deuda = new List<Deudas>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudas WHERE id_tesorero=@id_tesorero && periodicidad=0", conexion);

                comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = id_tesorero;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deuda.Add(new Deudas()
                        {
                            id_deudas = reader.GetInt32(0),
                            monto = reader.GetFloat(3),
                            nombre = reader.GetString(4),
                            descripcion = reader.GetString(5),
                            proximo_pago = reader.GetDateTime(9)

                        });
                        // MessageBox.Show();
                    }


                }
                catch (MySqlException ex)
                {

                }
                finally
                {
                    conexion.Close();
                }

                return Deuda;
            }

        }

        //este es el bueno
        [HttpGet]
        [Route("Consultar_DeudorOrdinario")]

        public List<Deudoress> Consultar_DeudoresOrdinarios(int id_fraccionamiento, int id_usuario)
        {
            List<Deudoress> Deuda = new List<Deudoress>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE (id_fraccionamiento=@id_fraccionamiento && id_deudor=@id_deudor && periodicidad>0)", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = id_usuario;

                try
                {
                    conexion.Open();
                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deudoress deuda = new Deudoress()
                        {
                            id_deudor = reader.GetInt32(1),
                            id_deuda = reader.GetInt32(2),
                            id_fraccionamiento = reader.GetInt32(3),
                            lote= !reader.IsDBNull(4) ? reader.GetInt32(4) : 0,
                            tipo_deuda = reader.GetString(5),
                            nombre_deuda = reader.GetString(6),
                            monto = reader.GetFloat(7),
                            recargo= reader.GetFloat(8),
                            dias_gracia=reader.GetInt32(9),
                            proximo_pago = reader.GetDateTime(10),
                            estado = reader.GetString(11),
                            periodicidad = reader.GetInt32(12)
                        };
                        DateTime fechaLimitePago = deuda.proximo_pago.AddDays(deuda.periodicidad);

                        // Agregar impresiones para depurar
                        Console.WriteLine($"Fecha límite para el pago: {fechaLimitePago}, Fecha actual: {DateTime.Today}");

                        // Verificar si la deuda está vencida utilizando DateTime.Compare
                        if (DateTime.Compare(fechaLimitePago, DateTime.Today) < 0)
                        {
                            Deuda.Add(deuda);
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    conexion.Close();
                }

                return Deuda;
            }
        }

        //Pagar deuda ordinaria
        //Pagar deuda ordinaria
        //Pagar deuda ordinaria
        [HttpPost]
        [Route("Pagar_DeudaOrdinaria")]

        public bool Pagar_DeudaOrdinaria(int id_deudor, int id_deuda, int id_fraccionamiento, string proximo_pago)
        {
            bool Propiedad_actualizada = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("UPDATE deudores " +
                    "SET proximo_pago=@proximo_pago " +
                    "WHERE id_deudor=@id_deudor && id_deuda=@id_deuda && id_fraccionamiento=@id_fraccionamiento", conexion);

                comando.Parameters.Add("@proximo_pago", MySqlDbType.Date).Value = proximo_pago;
                comando.Parameters.Add("@id_deudor", MySqlDbType.Int64).Value = id_deudor;
                comando.Parameters.Add("@id_deuda", MySqlDbType.Int64).Value = id_deuda;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int64).Value = id_fraccionamiento;
                

                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Propiedad_actualizada = true;
                    }

                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {
                    conexion.Close();
                }
                return Propiedad_actualizada;

            }


        }

        [HttpGet]
        [Route("Consultar_DeudorExtraordinario")]

        public List<Deudoress> Consultar_DeudoresExtraordinarios(int id_fraccionamiento, int id_usuario)
        {
            List<Deudoress> Deuda = new List<Deudoress>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE (id_fraccionamiento=@id_fraccionamiento && id_deudor=@id_deudor && periodicidad=0 && estado!='pagado')", conexion);

                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
                comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = id_usuario;

                try
                {
                    conexion.Open();
                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deudoress deuda = new Deudoress()
                        {
                            id_deudor = reader.GetInt32(1),
                            id_deuda = reader.GetInt32(2),
                            id_fraccionamiento = reader.GetInt32(3),
                            lote = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0,
                            tipo_deuda = reader.GetString(5),
                            nombre_deuda = reader.GetString(6),
                            monto = reader.GetFloat(7),
                            recargo = reader.GetFloat(8),
                            dias_gracia = reader.GetInt32(9),
                            proximo_pago = reader.GetDateTime(10),
                            estado = reader.GetString(11),
                            periodicidad = reader.GetInt32(12)

                            
                    };

                        Deuda.Add(deuda);

                        // Agregar impresiones para depurar

                    }
                }
                catch (MySqlException ex)
                {
                    // Manejar la excepción
                }
                finally
                {
                    conexion.Close();
                }

                return Deuda;
            }
        }

        [HttpPost]
        [Route("Pagar_DeudaExtraordinaria")]

        public bool Pagar_DeudaExtraordinaria(int id_deudor, int id_deuda, int id_fraccionamiento, string proximo_pago)
        {
            bool Propiedad_actualizada = false;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("UPDATE deudores " +
                    "SET estado='pagado'" +
                    "WHERE id_deudor=@id_deudor && id_deuda=@id_deuda && id_fraccionamiento=@id_fraccionamiento", conexion);

                comando.Parameters.Add("@proximo_pago", MySqlDbType.Date).Value = proximo_pago;
                comando.Parameters.Add("@id_deudor", MySqlDbType.Int64).Value = id_deudor;
                comando.Parameters.Add("@id_deuda", MySqlDbType.Int64).Value = id_deuda;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int64).Value = id_fraccionamiento;


                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        Propiedad_actualizada = true;
                    }

                }
                catch (MySqlException ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
                finally
                {
                    conexion.Close();
                }
                return Propiedad_actualizada;

            }


        }






        [HttpGet]
        [Route("Consultar_Deudores")]

        public List<Deudoress> Consultar_Deudores()
        {



            List<Deudoress> Deuda = new List<Deudoress>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudas WHERE id_tesorero=@id_tesorero && periodicidad>0", conexion);

                //   comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = id_tesorero;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deuda.Add(new Deudoress()
                        {
                            //concepto = reader.GetString(4),
                            persona = reader.GetString(5),
                            monto = reader.GetInt32(6),
                            proximo_pago = reader.GetDateTime(9)

                        });
                        // MessageBox.Show();
                    }


                }
                catch (MySqlException ex)
                {

                }
                finally
                {
                    conexion.Close();
                }

                return Deuda;
            }

        }



    }
}


//public List<Deudoress> Consultar_DeudoresOrdinarios(int id_fraccionamiento,int id_usuario)
//{

//    List<Deudoress> Deuda = new List<Deudoress>();

//    using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
//    {

//        MySqlCommand comando = new MySqlCommand("SELECT * FROM deudores WHERE (id_fraccionamiento=@id_fraccionamiento && id_deudor=@id_deudor && periodicidad>0)", conexion);

//         comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.Int32).Value = id_fraccionamiento;
//         comando.Parameters.Add("@id_deudor", MySqlDbType.Int32).Value = id_usuario;


//        try
//        {

//            conexion.Open();

//            MySqlDataReader reader = comando.ExecuteReader();

//            while (reader.Read())
//            {
//                Deuda.Add(new Deudoress()
//                {
//                    id_deudor = reader.GetInt32(1),
//                    id_deuda = reader.GetInt32(2),
//                    id_fraccionamiento = reader.GetInt32(3),
//                    tipo_deuda = reader.GetString(5),
//                    nombre_deuda = reader.GetString(6),
//                    monto = reader.GetFloat(7),
//                    proximo_pago = reader.GetDateTime(8),
//                    estado = reader.GetString(9),
//                    periodicidad = reader.GetInt32(10)

//                });
//                Console.WriteLine(Deuda[0].monto);
//            }


//        }
//        catch (MySqlException ex)
//        {

//        }
//        finally
//        {
//            conexion.Close();
//        }
//        List<Deudoress> DeudaFiltrada = Deuda
//        .Where(d => d.id_fraccionamiento == id_fraccionamiento && d.id_deudor == id_usuario && d.periodicidad > 0 && (d.proximo_pago.AddDays(d.periodicidad) <= DateTime.Today))
//        .ToList();
//        Console.WriteLine(DeudaFiltrada.Count);

//        return Deuda;
//    }

//}