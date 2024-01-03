using API_Archivo.Clases;
using CardManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Reflection.PortableExecutable;

namespace API_Archivo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Deudas_UsuarioController : ControllerBase
    {


        [HttpGet]
        [Route("Consultar_Deudores")]

        public List<Deudoress> Consultar_Deudores()
        {



            List<Deudoress> Deuda = new List<Deudoress>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM deudas_ordinarias", conexion);

                //   comando.Parameters.Add("@id_tesorero", MySqlDbType.Int32).Value = id_tesorero;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Deuda.Add(new Deudoress()
                        {
                            id_deuda = reader.GetInt32(0),
                            concepto = reader.GetString(7),
                            persona = reader.GetString(4),
                            monto = reader.GetFloat(5),
                            proximo_pago = reader.GetDateTime(8)

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
        [Route("Restringir_acceso")]
        public bool Eliminar_Persona(int id_deuda)
        {
            bool Persona_eliminada = false;
            int id_persona;

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("SELECT id_persona FROM deudas_ordinarias WHERE id_deuda=@id_deuda", conexion);

                //@Nombre, @Apellido_pat, @Apellido_mat, @Telefono, @Fecha_nacimiento, @Tipo_usuario, @id_fraccionamiento, @id_lote, @Intercomunicador, @Codigo_acceso

                // comando.Parameters.Add("@Nombre", MySqlDbType.VarChar).Value = nombre;
                //  comando.Parameters.Add("@Apellido_pat", MySqlDbType.VarChar).Value = apellido_paterno;
                comando.Parameters.Add("@id_deuda", MySqlDbType.Int32).Value = id_deuda;




                try
                {
                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {

                        Persona_eliminada = true;
                        id_persona = reader.GetInt32(0);
                        AddDevice.Login("admin", "Repara123", "5551", "187.216.118.73");
                        AddDevice.DeleteCardUser(id_persona.ToString());

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

    }
}
