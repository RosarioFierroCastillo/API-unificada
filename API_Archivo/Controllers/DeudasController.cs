using API_Archivo.Clases;
using API_Archivo.Deudores;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
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
                DateTime Dateproximo_pago = DateTime.Now.AddDays(request.periodicidad);
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
                        Deudores.Deudores.Agregar_Deudores(request.id_deudas, request.id_fraccionamiento, request.monto, request.nombre, Dateproximo_pago);
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
            bool Persona_eliminada = false;

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
                        Persona_eliminada = true;
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
                return Persona_eliminada;

            }
        }



        [HttpPost]
        [Route("Agregar_DeudaExtra")]

        public bool Agregar_DeudaExtra([FromBody] Deudas request)
        {

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
                            concepto = reader.GetString(4),
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
