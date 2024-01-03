using MySql.Data.MySqlClient;

namespace API_Archivo.Clases
{
    public class Invitaciones
    {


        public int id_invitacion { get; set; }
        public string token { get; set; }
        public string correo_electronico { get; set; }
        public int id_fraccionamiento { get; set; }

        public bool Generar_invitacion(string token, string correo_invitado, int id_fraccionamiento)
        {
            bool invitacion_agregada = false;
            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {
                int rowsaffected = 0;
                MySqlCommand comando = new MySqlCommand("insert into invitaciones (token, correo_invitado, id_fraccionamiento) VALUES (@token, @correo_invitado, @id_fraccionamiento)", conexion);



                comando.Parameters.Add("@token", MySqlDbType.VarChar).Value = token;
                comando.Parameters.Add("@correo_invitado", MySqlDbType.VarChar).Value = correo_invitado;
                comando.Parameters.Add("@id_fraccionamiento", MySqlDbType.VarChar).Value = id_fraccionamiento;


                try
                {
                    conexion.Open();
                    rowsaffected = comando.ExecuteNonQuery();

                    if (rowsaffected >= 1)
                    {
                        invitacion_agregada = true;
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
                return invitacion_agregada;
            }
        }//


        public List<Invitaciones> Cosultar_invitacion(string token)
        {
            List<Invitaciones> Lista_invitacion = new List<Invitaciones>();

            using (MySqlConnection conexion = new MySqlConnection(Global.cadena_conexion))
            {

                MySqlCommand comando = new MySqlCommand("SELECT * FROM invitaciones WHERE token=@token", conexion);

                comando.Parameters.Add("@token", MySqlDbType.VarChar).Value = token;


                try
                {

                    conexion.Open();

                    MySqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        Lista_invitacion.Add(new Invitaciones() { id_invitacion = reader.GetInt32(0), token = reader.GetString(1), correo_electronico = reader.GetString(2), id_fraccionamiento = reader.GetInt32(3) });
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

                return Lista_invitacion;
            }
        }
    }
}
