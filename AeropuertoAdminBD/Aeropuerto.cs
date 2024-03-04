using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using File = System.IO.File;

namespace AeropuertoAdminBD
{
    /* El código siguiente define una clase parcial llamada "Aeropuerto" que hereda de la clase "Form"
    en C#. 
    Autor: Juan Diego Quintanilla Escalante
    Fecha: 29/11/2023
    */
    public partial class Aeropuerto : Form
    {
        //Cadena para guardar el nombre del server, se define en el constructor
        public string server = "";
        //Cadena para guardar el nombre de la base de datos, se define en el constructor
        public string database = "";
        //Cadena para guardar el ID de un registro que se quiera modificar/eliminar
        public string currentID = "";
        //Cadena para guardar una imagen en bytes, se usa para hacer la lectura y mostrar la imagen en un pictureBox
        public byte[] bytesImagen;
        //Lista con IDs de itinerarios
        List<int> ListIDItinerario = new List<int>();
        //Lista con IDs de tarjeta pasajero
        List<int> listIDTrjetaPasajero = new List<int>();
        //Lista con IDs de asientos para modificacion; guarda ids de asiento que aun no estan ocupados en cierto itinerario
        List<int> listIDAsiento1 = new List<int>();
        //Lista con IDs de asientos de datagridview "Boleto"; guarda ids de asientos de los regsitros boleto
        List<int> listIDAsiento2 = new List<int>();
        //Lista con IDs de vuelos
        List<int> listIDVuelo = new List<int>();


        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Aeropuerto" />
        /// Define los nombres de el servidor y base de datos
        /// y asigna eventos a campos de texto y numericos para la restriccion de caracteres
        /// </summary>
        public Aeropuerto()
        {
            InitializeComponent();
            server = "server=LAPTOP-SMQPE5UL\\SQLEXPRESS; ";
            database = "database=Aeropuerto; ";

            tb_Nombre.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            tb_ModeloAvion.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            tb_NombrePiloto.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            tb_NombreCiudad.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            tb_PaisCiudad.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            nud_DuracionVuelo.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            tb_NombrePasajero.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            tb_NacionalidadPasajero.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            tb_NumPassPasajero.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            tb_NomTitularTarjPasaj.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);
            nud_NumAsiento.KeyPress += new KeyPressEventHandler(TextBox_KeyPress);

            consultaDatosAerolinea();
        }


        /// La función TC_Ventanas_SelectedIndexChanged maneja el evento de cambio de selección de un
        /// control de pestaña y realiza diferentes acciones según el índice seleccionado.
        /// </summary>
        /// <param name="sender">El parámetro del remitente es el objeto que generó el evento. En este
        /// caso se trata del control TC_Ventanas, que es un TabControl.</param>
        /// <param name="EventArgs">EventArgs es un argumento de evento que proporciona información
        /// sobre un evento. Se usa comúnmente en controladores de eventos para manejar eventos y
        /// realizar acciones específicas según el evento que ocurrió. En este fragmento de código, el
        /// parámetro EventArgs se utiliza para controlar el evento SelectedIndexChanged de un
        /// TabControl (TC_Ventanas).</param>
        private void TC_Ventanas_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (TC_Ventanas.SelectedIndex)
            {
                case 0:
                    //AEROLINEA
                    btn_backAerolinea_Click(this, EventArgs.Empty);
                    consultaDatosAerolinea();
                    break;
                case 1:
                    //AVIÓN
                    cb_AerolineasAvion.Items.Clear();
                    LlenarCBavion();
                    btn_backAvion_Click(this, EventArgs.Empty);
                    ConsultaDatosAvion();
                    break;
                case 2:
                    //PILOTO
                    cb_GeneroPiloto.Items.Clear();
                    cb_GeneroPiloto.Items.Add("Masculino");
                    cb_GeneroPiloto.Items.Add("Femenino");
                    cb_GeneroPiloto.Items.Add("Otros");
                    btn_backPiloto_Click(this, EventArgs.Empty);
                    consultaDatosPiloto();
                    break;
                case 3:
                    //CIUDAD
                    btn_backCiudad_Click(this, EventArgs.Empty);
                    consultaDatosCiudad();
                    break;
                case 4:
                    //VUELO
                    cb_CiudOrigVuelo.Items.Clear();
                    cb_CiudDestVuelo.Items.Clear();
                    LlenarCBvuelo();
                    btn_BackVuelo_Click(this, EventArgs.Empty);
                    consultaDatosVuelo();
                    break;
                case 5:
                    //ITINERARIO
                    cb_VueloItinerario.Items.Clear();
                    cb_PilotoItinerario.Items.Clear();
                    cb_AvionItinerario.Items.Clear();
                    LlenarCBItinerario();
                    btn_BackItinerario_Click(this, EventArgs.Empty);
                    consultaDatosItinerario();
                    break;
                case 6:
                    //PASAJERO
                    cb_GeneroPasajero.Items.Clear();
                    cb_GeneroPasajero.Items.Add("Masculino");
                    cb_GeneroPasajero.Items.Add("Femenino");
                    cb_GeneroPasajero.Items.Add("Otros");
                    btn_BackPasajero_Click(this, EventArgs.Empty);
                    ConsultaDatosPasajero();
                    break;
                case 7:
                    //TARJETA PASAJERO
                    cb_PasajeroTarjPasaj.Items.Clear();
                    LlenarCBTarjPasaj();
                    btn_BackTarjPasajero_Click(this, EventArgs.Empty);
                    ConsultaDatosTarjPasajero();
                    break;
                case 8:
                    //ASIENTO
                    cb_ItinerarioAsiento.Items.Clear();
                    cb_LetraAsiento.Items.Clear();
                    cb_LetraAsiento.Items.Add("A");
                    cb_LetraAsiento.Items.Add("B");
                    ListIDItinerario.Clear();
                    LlenarCBAsiento();
                    btn_BackAsiento_Click(this, EventArgs.Empty);
                    ConsultaDatosAsiento();
                    break;
                case 9:
                    //VENTA
                    cb_ItinerarioVentaBoleto.Items.Clear();
                    cb_TarjetaVentaBoleto.Items.Clear();
                    ListIDItinerario.Clear();
                    listIDTrjetaPasajero.Clear();
                    LlenarCBVenta_Boleto();
                    btn_BackVentaBoleto_Click(this, EventArgs.Empty);
                    ConsultaVenta();
                    break;
                case 10:
                    //BOLETO
                    lbl_Modifica1.Visible = false;
                    btn_Eliminar_Boleto.Enabled = false;
                    btn_Modificar_Boleto.Enabled = false;
                    cb_AsientoBoleto.Enabled = false;
                    cb_PasajeroBoleto.Enabled = false;
                    chckb_EstadoBoletos.Enabled = false;
                    currentID = "";
                    ConsultaBoleto();
                    break;
            }
        }


        /// <summary>
        /// La función Query ejecuta una consulta SQL y devuelve 1 si tiene éxito, -1 si hay una
        /// excepción.
        /// </summary>
        /// <param name="query">El parámetro "query" es una cadena que representa la consulta SQL que
        /// desea ejecutar en la base de datos. Puede ser cualquier declaración SQL válida como
        /// SELECCIONAR, INSERTAR, ACTUALIZAR, ELIMINAR, etc.</param>
        /// <returns>
        /// El método devuelve un valor entero. Si la consulta se ejecuta correctamente, devolverá 1. Si
        /// hay una excepción o error durante la ejecución de la consulta, devolverá -1.
        /// </returns>
        public int Query(string query)
        {
            using (SqlConnection Conn = new SqlConnection(server + database + "integrated security = true;"))
            {
                Conn.Open();
                SqlCommand Comm = new SqlCommand(query, Conn);
                try
                {
                    Comm.ExecuteNonQuery();
                    Conn.Close();
                    return 1;
                }
                catch (Exception ex)
                {
                    return -1;
                }
            }
        }


        /// <summary>
        /// La función Query2 ejecuta una consulta SQL, inserta un registro en una tabla de base de
        /// datos y devuelve el ID del registro insertado.
        /// </summary>
        /// <param name="query">El parámetro de query es una cadena que representa la consulta SQL
        /// que desea ejecutar. Puede ser cualquier declaración SQL válida, como INSERT, UPDATE, DELETE
        /// o SELECT.</param>
        /// <returns>
        /// El método devuelve un valor entero, que es el ID del registro insertado. Si la inserción es
        /// exitosa, se devuelve el ID del registro insertado. Si hay un error durante la inserción, el
        /// método devuelve -1.
        /// </returns>
        public int Query2(string query)
        {
            int insertedID = -1; // Valor por defecto en caso de error

            using (SqlConnection Conn = new SqlConnection(server + database + "integrated security = true;"))
            {
                Conn.Open();
                SqlCommand Comm = new SqlCommand(query, Conn);
                try
                {
                    Comm.ExecuteNonQuery();
                    // Después de la inserción, obtén el ID usando SCOPE_IDENTITY()
                    Comm.CommandText = "SELECT SCOPE_IDENTITY()";
                    insertedID = Convert.ToInt32(Comm.ExecuteScalar());

                    Conn.Close();
                    return insertedID;
                }
                catch (Exception ex)
                {
                    insertedID = -1;
                    return insertedID;
                }
            }
        }

        /// <summary>
        /// La función evita que el usuario ingrese ciertos caracteres (-, (, ), .) en un TextBox.
        /// </summary>
        /// <param name="sender">El parámetro "remitente" se refiere al objeto que generó el evento. En
        /// este caso, sería el control TextBox al que está adjunto el evento KeyPress.</param>
        /// <param name="KeyPressEventArgs">KeyPressEventArgs es un argumento de evento que contiene
        /// información sobre un evento de pulsación de tecla, como la tecla que se presionó.</param>
        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '-' || e.KeyChar == '(' || e.KeyChar == ')' || e.KeyChar == '.')
            {
                e.Handled = true;
            }
        }

        ///
        /// Metodos para ventanas de tablas del esquema InfoAerolinea
        ///
        #region InfoAerolinea

        ///
        /// Metodos para la manipulación de la tabla AEROLINEA
        ///
        #region METODOS VENTANA AEROLINEA

        /// <summary>
        /// La función "consultaDatosAerolinea" recupera datos de una tabla de base de datos (Aerolinea) 
        /// y llena un DataGridView con los datos recuperados.
        /// </summary>
        public void consultaDatosAerolinea()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoAerolinea.Aerolinea";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            int i = 0;
            dgv_Aerolinea.Rows.Clear();
            while (reader.Read())
            {
                dgv_Aerolinea.Rows.Add();
                dgv_Aerolinea.Rows[i].Cells[0].Value = reader["idAerolinea"].ToString();
                dgv_Aerolinea.Rows[i].Cells[1].Value = reader["Nom_Aerolinea"].ToString();
                dgv_Aerolinea.Rows[i].Cells[2].Value = reader["FlotaTotal"].ToString();
                dgv_Aerolinea.Rows[i].Cells[3].Value = reader["AñoFundacion"].ToString();
                dgv_Aerolinea.Rows[i].Cells[4].Value = reader["NumVuelos"].ToString();
                dgv_Aerolinea.Rows[i].Cells[5].Value = reader["Logotipo"].ToString();
                i++;
            }
        }


        /// <summary>
        /// Esta función se activa cuando se hace clic en una celda de un DataGridView y recupera y
        /// muestra información de la fila seleccionada, incluida una imagen almacenada en una base de
        /// datos.
        /// </summary>
        /// <param name="sender">El objeto que generó el evento, en este caso, el control
        /// DataGridView.</param>
        /// <param name="DataGridViewCellMouseEventArgs">El parámetro DataGridViewCellMouseEventArgs es
        /// un argumento de evento que proporciona información sobre el evento de clic del mouse que
        /// ocurrió en una celda DataGridView. Contiene propiedades como RowIndex, ColumnIndex y Button,
        /// que se pueden usar para determinar la celda en la que se hizo clic y el botón del mouse que
        /// se usó.</param>
        private void dgv_Aerolinea_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lbl_Modifica1.Visible = true;
                btn_Agregar_Aerolinea.Enabled = false;
                btn_Eliminar_Aerolinea.Enabled = true;
                btn_Modificar_Aerolinea.Enabled = true;
                btn_backAerolinea.Visible = true;

                DataGridViewRow row = dgv_Aerolinea.Rows[e.RowIndex];
                tb_Nombre.Text = row.Cells["Nom_Aerolinea"].Value.ToString();
                tb_AnioF.Text = row.Cells["AnioFundacion"].Value.ToString();
                tb_Image.Text = "-- Nueva imagen (Opcional) --";
                currentID = row.Cells["idAerolínea"].Value.ToString();

                // Leer los bytes de la imagen desde la base de datos
                byte[] bytesImagen = ObtenerBytesDesdeBaseDeDatos(currentID);

                // Mostrar la imagen en el PictureBox
                if (bytesImagen != null)
                {
                    // Creamos un flujo de memoria para manipular los bytes de la imagen
                    MemoryStream stream = new MemoryStream(bytesImagen);
                    // Asignamos la imagen al PictureBox
                    pb_Logo.Image = System.Drawing.Image.FromStream(stream);
                    // Establecemos el modo de visualización de la imagen (en este caso, se estira para llenar el PictureBox)
                    pb_Logo.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
        }

        /// <summary>
        /// La función "ObtenerBytesDesdeBaseDeDatos" recupera una matriz de bytes que representa una
        /// imagen de una base de datos según el ID proporcionado.
        /// </summary>
        /// <param name="id">El parámetro "id" es el identificador de la aerolínea cuyo logotipo
        /// queremos recuperar de la base de datos.</param>
        /// <returns>
        /// El método devuelve una matriz de bytes, que representa los datos de la imagen recuperados de
        /// la base de datos.
        /// </returns>
        private byte[] ObtenerBytesDesdeBaseDeDatos(string id)
        {
            // Establecemos una conexión a la base de datos utilizando una cadena de conexión
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                // Abrimos la conexión
                conexion.Open();

                // Consulta SQL para obtener el logotipo de la aerolínea con el id proporcionado
                string consulta = "SELECT Logotipo FROM InfoAerolinea.Aerolinea WHERE idAerolinea = @ID";
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    // Asignamos el parámetro @ID con el valor proporcionado
                    cmd.Parameters.AddWithValue("@ID", id);
                    // Ejecutamos la consulta y devolvemos el resultado (un array de bytes que representa la imagen)
                    return (byte[])cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// La función btn_backAerolinea_Click se utiliza para restablecer y ocultar ciertos elementos
        /// en un formulario en C#.
        /// </summary>
        /// <param name="sender">El objeto que generó el evento (en este caso, el botón en el que se
        /// hizo clic).</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento.
        /// Contiene información sobre el evento que ocurrió, como el origen del evento y cualquier dato
        /// adicional asociado con el evento. En este caso, el parámetro EventArgs se utiliza para
        /// manejar el evento de clic de un botón.</param>
        public void btn_backAerolinea_Click(object sender, EventArgs e)
        {
            lbl_Modifica1.Visible = false;
            btn_Agregar_Aerolinea.Enabled = true;
            btn_Eliminar_Aerolinea.Enabled = false;
            btn_Modificar_Aerolinea.Enabled = false;
            btn_backAerolinea.Visible = false;
            tb_Nombre.Text = "";
            tb_AnioF.Text = "";
            tb_Image.Text = "";
            pb_Logo.Image = null;
            openFileDialog1.Reset();
        }


        /// <summary>
        /// La función `btn_Agregar_Aerolinea_Click` se utiliza para agregar una nueva aerolínea a una
        /// base de datos, con el nombre de la aerolínea, año de fundación e imagen del logotipo.
        /// </summary>
        /// <param name="sender">El parámetro "sender" es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos del evento que
        /// se pasan a un método de controlador de eventos. Contiene información sobre el evento
        /// ocurrido. En este caso, el método controlador de eventos es btn_Agregar_Aerolinea_Click, que
        /// se activa cuando se presiona el botón "btn_Agregar_Aer</param>
        private void btn_Agregar_Aerolinea_Click(object sender, EventArgs e)
        {
            if (tb_Nombre.Text != "" && tb_AnioF.Text != "" && tb_Image.Text != "")
            {
                string query = "INSERT INTO InfoAerolinea.Aerolinea (Nom_Aerolinea, FlotaTotal, AñoFundacion, NumVuelos, Logotipo) VALUES ('" + tb_Nombre.Text + "', 0, " + tb_AnioF.Text + ", 0, @Logotipo)";
                using (SqlConnection Conn = new SqlConnection(server + database + "integrated security = true;"))
                {
                    Conn.Open();
                    using (SqlCommand Comm = new SqlCommand(query, Conn))
                    {
                        // Asignar el parámetro de la imagen
                        Comm.Parameters.AddWithValue("@Logotipo", bytesImagen);

                        // Ejecutar la consulta
                        Comm.ExecuteNonQuery();
                    }
                }

                consultaDatosAerolinea();
                btn_backAerolinea_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }



        /// <summary>
        /// La función siguiente es un controlador de eventos para un clic en un botón que actualiza la
        /// información de una aerolínea en una base de datos en función de los valores ingresados en
        /// los cuadros de texto.
        /// </summary>
        /// <param name="sender">El parámetro "sender" es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos de un evento.
        /// Se utiliza para pasar información adicional sobre el evento al método del controlador de
        /// eventos. En este caso el método controlador de eventos es btn_Modificar_Click, el cual se
        /// dispara cuando se presiona el botón con el nombre "btn_Modificar"</param>
        private void btn_Modificar_Click(object sender, EventArgs e)
        {
            if (tb_Nombre.Text != "" && tb_AnioF.Text != "")
            {
                string query = "";
                if (tb_Image.Text != "-- Nueva imagen (Opcional) --")
                {
                    query = "UPDATE InfoAerolinea.Aerolinea SET Nom_Aerolinea = '" + tb_Nombre.Text + "', AñoFundacion = '" + tb_AnioF.Text + "', Logotipo = @Logotipo WHERE idAerolinea = " + currentID;

                    using (SqlConnection Conn = new SqlConnection(server + database + "integrated security = true;"))
                    {
                        Conn.Open();
                        using (SqlCommand Comm = new SqlCommand(query, Conn))
                        {
                            // Asignar el parámetro de la imagen
                            Comm.Parameters.AddWithValue("@Logotipo", bytesImagen);

                            // Ejecutar la consulta
                            Comm.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    query = "UPDATE InfoAerolinea.Aerolinea SET Nom_Aerolinea = '" + tb_Nombre.Text + "', AñoFundacion = '" + tb_AnioF.Text + "' WHERE idAerolinea = " + currentID;
                    Query(query);
                }

                consultaDatosAerolinea();
                btn_backAerolinea_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }


        /// <summary>
        /// Esta función elimina un registro de una tabla de base de datos según el valor de ID actual y
        /// muestra un mensaje si no se selecciona ningún registro.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic (btn_Eliminar).</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos del evento. Se
        /// utiliza para pasar información adicional sobre un evento cuando se genera. En este caso,
        /// EventArgs.Empty se utiliza para pasar un objeto de datos de evento vacío al método del
        /// controlador de eventos.</param>
        private void btn_Eliminar_Click(object sender, EventArgs e)
        {
            if (tb_Nombre.Text != "" && tb_AnioF.Text != "")
            {
                string query = "DELETE FROM InfoAerolinea.Aerolinea WHERE idAerolinea = " + currentID;
                Query(query);
                consultaDatosAerolinea();

                btn_backAerolinea_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Selecciona el registro a eliminar";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// Esta función abre un cuadro de diálogo de archivo para permitir al usuario seleccionar un
        /// archivo de imagen, asigna el nombre del archivo seleccionado a un cuadro de texto y
        /// convierte la imagen a bytes.
        /// </summary>
        /// <param name="sender">El objeto que provocó el evento. En este caso, es el botón en el que se
        /// hizo clic.</param>
        /// <param name="EventArgs">EventArgs es una clase base para clases que contienen datos de
        /// eventos. Proporciona un constructor sin parámetros y está pensado para usarse como clase
        /// base para clases de argumentos de eventos. En este caso, se utiliza para manejar el evento
        /// cuando se hace clic en el botón "btn_Imagen".</param>
        private void btn_Imagen_Click(object sender, EventArgs e)
        {
            // Establecemos el directorio inicial y los filtros de archivos en el diálogo de apertura de archivos
            openFileDialog1.InitialDirectory = "C://";
            openFileDialog1.Filter = "Archivos jpg (*.jpg)|*.jpg|Archivos (*.png)|*.png";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            // Se muestra el cuadro de diálogo de apertura de archivos y se verifica si se seleccionó un archivo
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Se asigna el nombre del archivo seleccionado al TextBox tb_Image
                tb_Image.Text = openFileDialog1.FileName;

                // Convertir la imagen a bytes
                bytesImagen = File.ReadAllBytes(openFileDialog1.FileName);
            }
        }


        /// <summary>
        /// La función tb_AnioF_KeyPress restringe la entrada en un TextBox para permitir solo dígitos y
        /// limita la longitud del texto a 4 caracteres.
        /// </summary>
        /// <param name="sender">El parámetro "sender" se refiere al objeto que generó el evento. En
        /// este caso, se trata del control TextBox denominado "tb_AnioF".</param>
        /// <param name="KeyPressEventArgs">KeyPressEventArgs es un argumento de evento que contiene
        /// información sobre un evento de pulsación de tecla, como la tecla que se presionó.</param>
        private void tb_AnioF_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verificamos si la tecla presionada no es un dígito o control
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                // Si no es un dígito o control, la tecla no es manejada (ignorada)
                e.Handled = true;
            }

            // Verificamos si la longitud del texto en el TextBox es igual o mayor a 4 y si la tecla presionada no es la tecla de retroceso (Backspace)
            if (tb_AnioF.Text.Length >= 4 && e.KeyChar != (char)Keys.Back)
            {
                // Si se cumple la condición, la tecla no es manejada (ignorada)
                e.Handled = true;
            }
        }
        #endregion

        ///
        /// Metodos para la manipulación de la tabla PILOTO
        ///
        #region METODOS VENTANA PILOTO

        /// <summary>
        /// La función "consultaDatosPiloto" recupera datos de una tabla de base de datos llamada
        /// "InfoAerolinea.Piloto" y llena un control DataGridView con los datos recuperados.
        /// </summary>
        public void consultaDatosPiloto()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoAerolinea.Piloto";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            int i = 0;
            dgv_Piloto.Rows.Clear();
            while (reader.Read())
            {
                dgv_Piloto.Rows.Add();
                DateTime fecha = (DateTime)reader["FechaNacimiento"];
                dgv_Piloto.Rows[i].Cells[0].Value = reader["idPiloto"].ToString();
                dgv_Piloto.Rows[i].Cells[1].Value = reader["Nom_Piloto"].ToString();
                dgv_Piloto.Rows[i].Cells[2].Value = reader["Genero"].ToString();
                dgv_Piloto.Rows[i].Cells[3].Value = fecha.ToString("dd/MM/yyyy");
                dgv_Piloto.Rows[i].Cells[4].Value = reader["NumLicencia"].ToString();
                i++;
            }
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en un botón y agrega un piloto a una base de
        /// datos si se completan todos los campos obligatorios y el piloto cumple con el requisito de
        /// edad.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y se utiliza como clase base para todas las clases de datos de eventos. En
        /// este caso, se utiliza como parámetro para el método del controlador de eventos
        /// btn_Agregar_Piloto_Click.</param>
        private void btn_Agregar_Piloto_Click(object sender, EventArgs e)
        {
            if (tb_NombrePiloto.Text != "" && cb_GeneroPiloto.Text != "" && dtp_FechaNacPiloto.Text != "" && tb_NumLicPiloto.Text != "")
            {
                // Calcular la edad a partir de la fecha de nacimiento
                DateTime fechaNacimiento = dtp_FechaNacPiloto.Value;
                int edad = CalcularEdad(fechaNacimiento);

                // Verificar si la edad es mayor o igual a 27 años
                if (edad >= 27)
                {
                    if(!checkLicenciaPiloto(tb_NumLicPiloto.Text))
                    {
                        if (tb_NumLicPiloto.Text.Length == 10)
                        {
                            // Realizar la inserción en la base de datos
                            string date = fechaNacimiento.ToString("yyyy-MM-dd");
                            string query = "INSERT INTO InfoAerolinea.Piloto (Nom_Piloto, Genero, FechaNacimiento, NumLicencia) VALUES ('" + tb_NombrePiloto.Text + "','" + cb_GeneroPiloto.Text + "','" + date + "'," + tb_NumLicPiloto.Text + ")";
                            Query(query);
                            consultaDatosPiloto();
                            btn_backPiloto_Click(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        string mensaje = "El piloto debe tener licencia unica";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show(mensaje, "Mensaje de Error", buttons);
                    }
                }
                else
                {
                    string mensaje = "El piloto debe tener al menos 27 años de edad.";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje de Error", buttons);
                }
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función verifica si existe una licencia de piloto en una tabla de base de datos.
        /// </summary>
        /// <param name="text">El parámetro "text" es una cadena que representa el número de licencia
        /// de un piloto.</param>
        /// <returns>
        /// El método devuelve un valor booleano.
        /// </returns>
        public bool checkLicenciaPiloto(string text)
        {
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                conexion.Open();

                string consulta = "SELECT COUNT(*) FROM InfoAerolinea.Piloto WHERE NumLicencia = " + text;
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// La función calcula la edad en función de la fecha de nacimiento indicada y la fecha actual.
        /// </summary>
        /// <param name="DateTime">DateTime es un tipo de datos en C# que representa una fecha y hora
        /// específicas. En este caso, el parámetro "fechaNacimiento" es de tipo DateTime y representa
        /// la fecha de nacimiento de una persona.</param>
        /// <returns>
        /// El método devuelve un valor entero, que representa la edad calculada en función de la fecha
        /// de nacimiento determinada.
        /// </returns>
        private int CalcularEdad(DateTime fechaNacimiento)
        {
            DateTime fechaActual = DateTime.Now;
            int edad = fechaActual.Year - fechaNacimiento.Year;

            // Resta un año si la fecha actual es anterior al día de nacimiento
            if (fechaActual < fechaNacimiento.AddYears(edad))
            {
                edad--;
            }

            return edad;
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en el botón "Modificar Piloto" y verifica si los
        /// campos requeridos están llenos, verifica la restricción de edad, verifica si la licencia del
        /// piloto es única y actualiza la información del piloto en la base de datos si se cumplen
        /// todas las condiciones.
        /// </summary>
        /// <param name="sender">El objeto que generó el evento (en este caso, el botón en el que se
        /// hizo clic).</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y se utiliza como clase base para todas las clases de datos de eventos. En
        /// este caso, se utiliza como parámetro para el método del controlador de eventos
        /// btn_Modifica_Piloto_Click.</param>
        private void btn_Modifica_Piloto_Click(object sender, EventArgs e)
        {
            if (tb_NombrePiloto.Text != "" && cb_GeneroPiloto.Text != "" && dtp_FechaNacPiloto.Text != "" && tb_NumLicPiloto.Text != "")
            {
                // Verificar si la fecha de nacimiento cumple con la restricción
                DateTime fechaNacimiento = dtp_FechaNacPiloto.Value;
                int edad = CalcularEdad(fechaNacimiento);

                if (edad >= 27)
                {
                    if (!checkLicenciaPiloto(tb_NumLicPiloto.Text))
                    {
                        if (tb_NumLicPiloto.Text.Length == 10)
                        {
                            // Si cumple con la restricción, proceder con la actualización
                            string date = fechaNacimiento.ToString("yyyy-MM-dd");
                            string query = "UPDATE InfoAerolinea.Piloto SET Nom_Piloto = '" + tb_NombrePiloto.Text + "', Genero = '" + cb_GeneroPiloto.Text + "', FechaNacimiento='" + date + "', NumLicencia='" + tb_NumLicPiloto.Text + "'  WHERE idPiloto = " + currentID;
                            Query(query);
                            consultaDatosPiloto();
                            btn_backPiloto_Click(this, EventArgs.Empty);
                        } 
                    }
                    else
                    {
                        string mensaje = "El piloto debe tener licencia unica";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show(mensaje, "Mensaje de Error", buttons);
                    }
                }
                else
                {
                    // Mostrar un mensaje de error si no cumple con la restricción
                    string mensaje = "El piloto debe tener al menos 27 años de edad.";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje de Error", buttons);
                }
            }
            else
            {
                // Mostrar un mensaje de error si faltan datos
                string mensaje = "Modifica correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje de Error", buttons);
            }
        }

        /// <summary>
        /// La función btn_Elimina_Piloto_Click se utiliza para eliminar un registro de la tabla Piloto
        /// en la base de datos de InfoAerolinea, según el valor de currentID.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic (btn_Elimina_Piloto).</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y se utiliza como clase base para todas las clases de datos de eventos. En
        /// este caso, se utiliza como parámetro para el método del controlador de eventos
        /// btn_Elimina_Piloto_Click.</param>
        private void btn_Elimina_Piloto_Click(object sender, EventArgs e)
        {
            if (tb_NombrePiloto.Text != "" && cb_GeneroPiloto.Text != "" && dtp_FechaNacPiloto.Text != "" && tb_NumLicPiloto.Text != "")
            {
                string query = "DELETE FROM InfoAerolinea.Piloto WHERE idPiloto = " + currentID;
                Query(query);
                consultaDatosPiloto();

                btn_backPiloto_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Selecciona el registro a eliminar";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// Esta función restablece los campos del formulario y desactiva ciertos botones cuando se hace
        /// clic en el botón "Atrás".
        /// </summary>
        /// <param name="sender">El objeto que generó el evento (en este caso, el botón en el que se
        /// hizo clic).</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento.
        /// Contiene información sobre el evento que ocurrió, como el origen del evento y cualquier dato
        /// adicional asociado con el evento. En este caso, el parámetro EventArgs no se utiliza en el
        /// método.</param>
        private void btn_backPiloto_Click(object sender, EventArgs e)
        {
            lbl_Modifica1.Visible = false;
            btn_Agregar_Piloto.Enabled = true;
            btn_Elimina_Piloto.Enabled = false;
            btn_Modifica_Piloto.Enabled = false;
            btn_backPiloto.Visible = false;

            tb_NombrePiloto.Text = "";
            cb_GeneroPiloto.SelectedIndex = -1;
            dtp_FechaNacPiloto.ResetText();
            tb_NumLicPiloto.Text = "";
            currentID = "";
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en una celda de un control DataGridView, recupera
        /// los valores de la fila en la que se hizo clic y completa varios controles con esos valores.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el control DataGridView el que generó el evento.</param>
        /// <param name="DataGridViewCellMouseEventArgs">El parámetro DataGridViewCellMouseEventArgs es
        /// un argumento de evento que proporciona información sobre el evento de clic del mouse que
        /// ocurrió en una celda DataGridView. Contiene propiedades como RowIndex (el índice de la fila
        /// en la que se hizo clic), ColumnIndex (el índice de la columna en la que se hizo clic) y
        /// Button (el botón del mouse en el que se hizo clic).</param>
        private void dgv_Piloto_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lbl_Modifica1.Visible = true;
                btn_Agregar_Piloto.Enabled = false;
                btn_Elimina_Piloto.Enabled = true;
                btn_Modifica_Piloto.Enabled = true;
                btn_backPiloto.Visible = true;

                DataGridViewRow row = dgv_Piloto.Rows[e.RowIndex];
                tb_NombrePiloto.Text = row.Cells["NombrePiloto"].Value.ToString();
                cb_GeneroPiloto.Text = row.Cells["Genero"].Value.ToString();
                dtp_FechaNacPiloto.Text = row.Cells["FechaNacimiento"].Value.ToString();
                tb_NumLicPiloto.Text = row.Cells["NumLicencia"].Value.ToString();
                currentID = row.Cells["idPiloto"].Value.ToString();
            }
        }

        /// <summary>
        /// La función restringe la entrada en un cuadro de texto para permitir solo dígitos y limita la
        /// longitud de la entrada a 10 caracteres.
        /// </summary>
        /// <param name="sender">El parámetro "sender" se refiere al objeto que generó el evento, en
        /// este caso, el control de cuadro de texto "tb_NumLicPiloto".</param>
        /// <param name="KeyPressEventArgs">KeyPressEventArgs es un argumento de evento que proporciona
        /// datos para el evento KeyPress. Contiene información sobre la tecla que se presionó, como la
        /// representación de caracteres de la tecla.</param>
        private void tb_NumLicPiloto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (tb_NumLicPiloto.Text.Length >= 10 && e.KeyChar != (char)Keys.Back)
            {
                if(tb_NumLicPiloto.Text.Length < 10)
                {
                    e.Handled = false;
                }
                e.Handled = true;
            }
        }
        #endregion

        ///
        /// Metodos para la manipulación de la tabla CIUDAD
        ///
        #region METODOS VENTANA CIUDAD

        /// <summary>
        /// Esta función agrega una nueva ciudad a una base de datos si aún no existe.
        /// </summary>
        /// <param name="sender">El parámetro "sender" se refiere al objeto que generó el evento. En
        /// este caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y se utiliza como clase base para todas las clases de datos de eventos. En
        /// este caso se utiliza como parámetro del método controlador de eventos
        /// btn_Agregar_Ciudad_Click.</param>
        private void btn_Agregar_Ciudad_Click(object sender, EventArgs e)
        {
            if (tb_NombreCiudad.Text != "" && tb_PaisCiudad.Text != "")
            {
                // Verificar si ya existe una entrada con el mismo Nombre de Ciudad y País
                string ciudad = tb_NombreCiudad.Text;
                string pais = tb_PaisCiudad.Text;
                bool ciudadYaExiste = VerificarCiudadExistente(ciudad, pais);

                if (!ciudadYaExiste)
                {
                    // Si no existe, entonces proceder con la inserción
                    string query = "INSERT INTO InfoAerolinea.Ciudad (Nom_Ciudad, Pais) VALUES ('" + ciudad + "','" + pais + "')";
                    Query(query);
                    consultaDatosCiudad();
                    btn_backCiudad_Click(this, EventArgs.Empty);
                }
                else
                {
                    string mensaje = "Ese registro ya existe en la base de datos.";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje", buttons);
                }
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función VerificarCiudadExistente verifica si una ciudad existe en un país determinado en
        /// una base de datos.
        /// </summary>
        /// <param name="ciudad">El parámetro "ciudad" representa el nombre de la ciudad que desea
        /// verificar si existe en la base de datos.</param>
        /// <param name="pais">El parámetro "pais" representa el país de la ciudad que se está
        /// verificando.</param>
        /// <returns>
        /// El método devuelve un valor booleano.
        /// </returns>
        private bool VerificarCiudadExistente(string ciudad, string pais)
        {
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                conexion.Open();

                string consulta = "SELECT COUNT(*) FROM InfoAerolinea.Ciudad WHERE Nom_Ciudad = @Ciudad AND Pais = @Pais";
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    cmd.Parameters.AddWithValue("@Ciudad", ciudad);
                    cmd.Parameters.AddWithValue("@Pais", pais);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }

        /// <summary>
        /// La función btn_Modifica_Ciudad_Click se utiliza para actualizar la información de una ciudad
        /// en una base de datos, comprobando si la ciudad y el país ya existen antes de realizar la
        /// actualización.
        /// </summary>
        /// <param name="sender">El parámetro "sender" es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es un argumento de evento que contiene información sobre
        /// un evento. Se usa comúnmente en controladores de eventos para manejar eventos generados por
        /// interacciones del usuario o acciones del sistema.</param>
        private void btn_Modifica_Ciudad_Click(object sender, EventArgs e)
        {
            if (tb_NombreCiudad.Text != "" && tb_PaisCiudad.Text != "")
            {
                // Verificar si ya existe una entrada con el mismo Nombre de Ciudad y País
                string nuevaCiudad = tb_NombreCiudad.Text;
                string nuevoPais = tb_PaisCiudad.Text;
                bool ciudadYaExiste = VerificarCiudadExistente(nuevaCiudad, nuevoPais);

                if (!ciudadYaExiste)
                {
                    // Si no existe, entonces proceder con la actualización
                    string query = "UPDATE InfoAerolinea.Ciudad SET Nom_Ciudad = '" + nuevaCiudad + "', Pais = '" + nuevoPais + "' WHERE idCiudad = " + currentID;
                    Query(query);
                    consultaDatosCiudad();
                    btn_backCiudad_Click(this, EventArgs.Empty);
                }
                else
                {
                    string mensaje = "La Ciudad y País ya existen en la base de datos.";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje", buttons);
                }
            }
            else
            {
                string mensaje = "Modifica correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// Esta función elimina un registro de la tabla "Ciudad" en la base de datos "InfoAerolinea" si
        /// los campos nombre y país no están vacíos, de lo contrario muestra un mensaje de error.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso es el botón en el que se hizo clic (btn_Elimina_Ciudad).</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos de un evento.
        /// Se utiliza comúnmente como parámetro en los métodos del controlador de eventos para
        /// proporcionar información sobre el evento que ocurrió. En este caso, el método controlador de
        /// eventos es btn_Elimina_Ciudad_Click, el cual se activa cuando se presiona el botón</param>
        private void btn_Elimina_Ciudad_Click(object sender, EventArgs e)
        {
            if (tb_NombreCiudad.Text != "" && tb_PaisCiudad.Text != "")
            {
                string query = "DELETE FROM InfoAerolinea.Ciudad WHERE idCiudad = " + currentID;
                Query(query);
                consultaDatosCiudad();
                btn_backCiudad_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Selecciona bien el registro a eliminar";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// Esta función restablece los campos del formulario y desactiva ciertos botones cuando se hace
        /// clic en el botón "Atrás".
        /// </summary>
        /// <param name="sender">El objeto que generó el evento (en este caso, el botón en el que se
        /// hizo clic).</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento.
        /// Contiene información sobre el evento que ocurrió, como el origen del evento y cualquier dato
        /// adicional asociado con el evento. En este caso, el parámetro EventArgs no se utiliza en el
        /// método.</param>
        private void btn_backCiudad_Click(object sender, EventArgs e)
        {
            lbl_Modifica1.Visible = false;
            btn_Agregar_Ciudad.Enabled = true;
            btn_Elimina_Ciudad.Enabled = false;
            btn_Modifica_Ciudad.Enabled = false;
            btn_backCiudad.Visible = false;
            tb_NombreCiudad.Text = "";
            tb_PaisCiudad.Text = "";
            currentID = "";
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en una celda de un DataGridView y actualiza los
        /// elementos de la interfaz de usuario en función de los datos de la fila seleccionada.
        /// </summary>
        /// <param name="sender">El parámetro "sender" se refiere al objeto que generó el evento. En
        /// este caso se trata del control DataGridView denominado "dgv_Ciudad".</param>
        /// <param name="DataGridViewCellMouseEventArgs">El parámetro DataGridViewCellMouseEventArgs es
        /// un argumento de evento que proporciona información sobre el evento de clic del mouse que
        /// ocurrió en una celda DataGridView. Contiene propiedades como RowIndex, ColumnIndex y Button,
        /// que se pueden usar para determinar la celda específica en la que se hizo clic y el botón del
        /// mouse que se usó.</param>
        private void dgv_Ciudad_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lbl_Modifica1.Visible = true;
                btn_Agregar_Ciudad.Enabled = false;
                btn_Elimina_Ciudad.Enabled = true;
                btn_Modifica_Ciudad.Enabled = true;
                btn_backCiudad.Visible = true;

                DataGridViewRow row = dgv_Ciudad.Rows[e.RowIndex];
                tb_NombreCiudad.Text = row.Cells["NombreCiudad"].Value.ToString();
                tb_PaisCiudad.Text = row.Cells["Pais"].Value.ToString();
                currentID = row.Cells["idCiudad"].Value.ToString();
            }
        }

        /// <summary>
        /// La función "consultaDatosCiudad" recupera datos de una tabla llamada "Ciudad" en una base de
        /// datos SQL Server y llena un DataGridView con los datos recuperados.
        /// </summary>
        public void consultaDatosCiudad()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoAerolinea.Ciudad";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            int i = 0;
            dgv_Ciudad.Rows.Clear();
            while (reader.Read())
            {
                dgv_Ciudad.Rows.Add();
                dgv_Ciudad.Rows[i].Cells[0].Value = reader["idCiudad"].ToString();
                dgv_Ciudad.Rows[i].Cells[1].Value = reader["Nom_Ciudad"].ToString();
                dgv_Ciudad.Rows[i].Cells[2].Value = reader["Pais"].ToString();
                i++;
            }
        }

        #endregion

        ///
        /// Metodos para la manipulación de la tabla AVION
        ///
        #region METODOS VENTANA AVION

        /// <summary>
        /// La función "LlenarCBavion" llena un ComboBox con los nombres de las aerolíneas y sus años de
        /// fundación.
        /// </summary>
        public void LlenarCBavion()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT AñoFundacion, Nom_Aerolinea FROM InfoAerolinea.Aerolinea";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            while (reader.Read())
            {
                cb_AerolineasAvion.Items.Add(reader["Nom_Aerolinea"].ToString() + " (" + reader["AñoFundacion"].ToString() + ")");
            }

            conexion.Close();
        }

        /// <summary>
        /// La función `btn_Agregar_Avion_Click` se utiliza para agregar un nuevo avión a una base de
        /// datos, según la entrada proporcionada por el usuario.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y se utiliza como clase base para todas las clases de datos de eventos. En
        /// este caso, se utiliza como parámetro para el método del controlador de eventos
        /// btn_Agregar_Avion_Click.</param>
        private void btn_Agregar_Avion_Click(object sender, EventArgs e)
        {
            if (tb_ModeloAvion.Text != "" && cb_AerolineasAvion.Text != "" && tb_AnioFabAvion.Text != "")
            {
                string query = "";
                //regex para obtener solo el nombre de la aerolinea del combobox
                string nomAero = Regex.Match(cb_AerolineasAvion.Text, @"^[^(]+").Value;
                //Funcion para obtener el idAerolinea a traves del nombre
                string id = getAerolinea(nomAero, false);

                DateTime fechaActual = DateTime.Now;
                int Anio = fechaActual.Year;
                if (Int32.Parse(tb_AnioFabAvion.Text)>=1905 && Int32.Parse(tb_AnioFabAvion.Text) <= Anio)
                {

                    if (chb_EstadoAvion.Checked)
                    {
                        query = "INSERT INTO InfoAerolinea.Avion (idAerolinea, Capacidad, Modelo, AñoFabricacion, EstadoUso) VALUES (" + id + ", 8, '" + tb_ModeloAvion.Text + "', " + tb_AnioFabAvion.Text + ", 1)";
                    }
                    else
                    {
                        query = "INSERT INTO InfoAerolinea.Avion (idAerolinea, Capacidad, Modelo, AñoFabricacion, EstadoUso) VALUES (" + id + ", 8, '" + tb_ModeloAvion.Text + "', " + tb_AnioFabAvion.Text + ", 0)";
                    }
                    Query(query);
                    ConsultaDatosAvion();
                    btn_backAvion_Click(this, EventArgs.Empty);
                }
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función "getAerolinea" recupera información sobre una aerolínea basándose en su nombre o
        /// ID.
        /// </summary>
        /// <param name="val">El parámetro "val" es una cadena que representa el valor utilizado en la
        /// consulta SQL. Puede ser el nombre de una aerolínea o el ID de una aerolínea, según el valor
        /// del parámetro "bandera".</param>
        /// <param name="flag">El parámetro "bandera" es una bandera booleana que determina el
        /// comportamiento del método. Si se establece en verdadero, el método recuperará el nombre y el
        /// año de fundación de una aerolínea según su ID. Si se establece en falso, el método
        /// recuperará la identificación de una aerolínea según</param>
        /// <returns>
        /// El método devuelve un valor de cadena.
        /// </returns>
        public string getAerolinea(string val, bool flag)
        {
            string AeroValue = "";
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                conexion.Open();
                string consulta = "";
                if (!flag)
                {
                    consulta = "SELECT idAerolinea FROM InfoAerolinea.Aerolinea WHERE Nom_Aerolinea = '"+val+"'";
                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        AeroValue = reader["idAerolinea"].ToString();
                    }
                }
                else
                {
                    consulta = "SELECT Nom_Aerolinea, AñoFundacion FROM InfoAerolinea.Aerolinea WHERE idAerolinea = '" + val + "'";
                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        AeroValue = reader["Nom_Aerolinea"].ToString() + " (" + reader["AñoFundacion"].ToString()+")";
                    }
                }
            }
            return AeroValue;
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en un botón y actualiza la información de un
        /// avión en una base de datos según la entrada del usuario.
        /// </summary>
        /// <param name="sender">El parámetro "sender" se refiere al objeto que generó el evento. En
        /// este caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y normalmente se utiliza cuando no se necesitan datos adicionales para un
        /// evento. En este caso, el evento es el evento de clic de un botón, por lo que el parámetro
        /// EventArgs no se utiliza en el fragmento de código proporcionado.</param>
        private void btn_Modifica_Avion_Click(object sender, EventArgs e)
        {
            if (tb_ModeloAvion.Text != "" && cb_AerolineasAvion.Text != "" && tb_AnioFabAvion.Text != "")
            {
                string query = "";
                //regex para obtener solo el nombre de la aerolinea del combobox
                string nomAero = Regex.Match(cb_AerolineasAvion.Text, @"^[^(]+").Value;
                //Funcion para obtener el idAerolinea a traves del nombre
                string id = getAerolinea(nomAero, false);

                DateTime fechaActual = DateTime.Now;
                int Anio = fechaActual.Year;
                if (Int32.Parse(tb_AnioFabAvion.Text) >= 1905 && Int32.Parse(tb_AnioFabAvion.Text) <= Anio)
                {
                    if (chb_EstadoAvion.Checked)
                    {
                        query = "UPDATE InfoAerolinea.Avion SET idAerolinea = " + id + ", Modelo = '" + tb_ModeloAvion.Text + "', AñoFabricacion = " + tb_AnioFabAvion.Text + ", EstadoUso = 1 WHERE idAvion = " + currentID;
                    }
                    else
                    {
                        query = "UPDATE InfoAerolinea.Avion SET idAerolinea = " + id + ", Modelo = '" + tb_ModeloAvion.Text + "', AñoFabricacion = " + tb_AnioFabAvion.Text + ", EstadoUso = 0 WHERE idAvion = " + currentID;
                    }

                    Query(query);
                    ConsultaDatosAvion();

                    btn_backAvion_Click(this, EventArgs.Empty);
                }
            }
            else
            {
                string mensaje = "Modifica correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// Esta función elimina un registro de la tabla "Avion" en la base de datos "InfoAerolinea" si
        /// los campos requeridos no están vacíos, de lo contrario muestra un mensaje de error.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic (btn_Elimina_Avion).</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y normalmente se utiliza cuando no se necesitan datos adicionales para un
        /// evento. En este caso, se utiliza como parámetro para el método del controlador de eventos
        /// btn_Elimina_Avion_Click.</param>
        private void btn_Elimina_Avion_Click(object sender, EventArgs e)
        {
            if (tb_ModeloAvion.Text != "" && cb_AerolineasAvion.Text != "" && tb_AnioFabAvion.Text != "")
            {
                string query = "DELETE FROM InfoAerolinea.Avion WHERE idAvion = " + currentID;
                Query(query);
                ConsultaDatosAvion();

                btn_backAvion_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Selecciona bien el registro a eliminar";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// Esta función restablece los campos del formulario y desactiva ciertos botones cuando se hace
        /// clic en el botón "Atrás".
        /// </summary>
        /// <param name="sender">El objeto que generó el evento (en este caso, el botón en el que se
        /// hizo clic).</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento.
        /// Contiene información sobre el evento que ocurrió, como el origen del evento y cualquier dato
        /// adicional asociado con el evento. En este caso, el parámetro EventArgs no se utiliza en el
        /// método.</param>
        private void btn_backAvion_Click(object sender, EventArgs e)
        {
            lbl_Modifica1.Visible = false;
            btn_Agregar_Avion.Enabled = true;
            btn_Elimina_Avion.Enabled = false;
            btn_Modifica_Avion.Enabled = false;
            btn_backAvion.Visible = false;
            tb_ModeloAvion.Text = "";
            tb_AnioFabAvion.Text = "";
            cb_AerolineasAvion.SelectedIndex = -1;
            chb_EstadoAvion.Checked = false;
            currentID = "";
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en una celda de un DataGridView y actualiza los
        /// elementos de la interfaz de usuario con los datos correspondientes de la fila seleccionada.
        /// </summary>
        /// <param name="sender">El parámetro del remitente es el objeto que generó el evento. En este
        /// caso se trata del control DataGridView denominado dgv_Avion.</param>
        /// <param name="DataGridViewCellMouseEventArgs">El parámetro DataGridViewCellMouseEventArgs es
        /// un argumento de evento que proporciona información sobre el evento de clic del mouse en una
        /// celda DataGridView. Contiene propiedades como el índice de fila, el índice de columna y el
        /// botón del mouse en el que se hizo clic.</param>
        private void dgv_Avion_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lbl_Modifica1.Visible = true;
                btn_Agregar_Avion.Enabled = false;
                btn_Elimina_Avion.Enabled = true;
                btn_Modifica_Avion.Enabled = true;
                btn_backAvion.Visible = true;

                DataGridViewRow row = dgv_Avion.Rows[e.RowIndex];
                tb_ModeloAvion.Text = row.Cells["Modelo"].Value.ToString();
                tb_AnioFabAvion.Text = row.Cells["AñoFabricacion"].Value.ToString();
                chb_EstadoAvion.Checked = (row.Cells["EstadoUso"].Value.ToString() == "True") ? true : false;

                string NombAero = row.Cells["idAerolineaAv"].Value.ToString();
                for (int i = 0; i < cb_AerolineasAvion.Items.Count; i++)
                {
                    string item = cb_AerolineasAvion.Items[i].ToString();

                    if (item.Equals(NombAero))
                    {
                        int indiceEncontrado = i;
                        cb_AerolineasAvion.SelectedIndex = indiceEncontrado;
                        break;
                    }
                }

                currentID = row.Cells["idAvion"].Value.ToString();
            }
        }

        /// <summary>
        /// La función "ConsultaDatosAvion" recupera datos de una tabla de base de datos de SQL Server y
        /// llena un control DataGridView con los datos recuperados.
        /// </summary>
        public void ConsultaDatosAvion()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoAerolinea.Avion";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            int i = 0;
            dgv_Avion.Rows.Clear();
            while (reader.Read())
            {
                dgv_Avion.Rows.Add();
                dgv_Avion.Rows[i].Cells[0].Value = reader["idAvion"].ToString();
                dgv_Avion.Rows[i].Cells[1].Value = getAerolinea(reader["idAerolinea"].ToString(), true);
                dgv_Avion.Rows[i].Cells[2].Value = reader["Capacidad"].ToString();
                dgv_Avion.Rows[i].Cells[3].Value = reader["Modelo"].ToString();
                dgv_Avion.Rows[i].Cells[4].Value = reader["AñoFabricacion"].ToString();
                dgv_Avion.Rows[i].Cells[5].Value = reader["EstadoUso"].ToString();
                i++;
            }
        }

        /// <summary>
        /// La función restringe la entrada en un cuadro de texto para permitir solo dígitos y limita la
        /// longitud a 4 caracteres.
        /// </summary>
        /// <param name="sender">El parámetro del sender se refiere al objeto que generó el evento.
        /// En este caso, es el control del cuadro de texto (tb_AnioFabAvion) el que generó el evento
        /// KeyPress.</param>
        /// <param name="KeyPressEventArgs">KeyPressEventArgs es un argumento de evento que proporciona
        /// datos para el evento KeyPress. Contiene información sobre la tecla que se presionó, como la
        /// representación de caracteres de la tecla.</param>
        private void tb_AnioFabAvion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (tb_AnioFabAvion.Text.Length >= 4 && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }
        #endregion

        ///
        /// Metodos para la manipulación de la tabla VUELO
        ///
        #region METODOS VENTANA VUELO

        /// <summary>
        /// La función "LlenarCBvuelo" llena dos combobox con nombres de ciudades y países de
        /// una base de datos.
        /// </summary>
        public void LlenarCBvuelo()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT Nom_Ciudad, Pais FROM InfoAerolinea.Ciudad";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            while (reader.Read())
            {
                cb_CiudDestVuelo.Items.Add(reader["Nom_Ciudad"].ToString() + " - " + reader["Pais"].ToString());
                cb_CiudOrigVuelo.Items.Add(reader["Nom_Ciudad"].ToString() + " - " + reader["Pais"].ToString());
            }

            conexion.Close();
        }

        /// <summary>
        /// La función `btn_Agregar_Vuelo_Click` se utiliza para agregar un vuelo a una tabla de base de
        /// datos si los campos requeridos se completan correctamente.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos de un evento.
        /// Se utiliza para pasar información adicional sobre el evento al método del controlador de
        /// eventos. En este caso, el método controlador de eventos es el método
        /// btn_Agregar_Vuelo_Click, el cual se activa cuando se activa el método "Agregar Vuel</param>
        private void btn_Agregar_Vuelo_Click(object sender, EventArgs e)
        {
            if (cb_CiudDestVuelo.Text != "" && cb_CiudOrigVuelo.Text != "" && mtb_CostoBaseVuelo.Text != "" && nud_DuracionVuelo.Text != "")
            {
                if(cb_CiudDestVuelo.SelectedIndex != cb_CiudOrigVuelo.SelectedIndex && Int32.Parse(nud_DuracionVuelo.Text) > 1 && Int32.Parse(nud_DuracionVuelo.Text) < 24 )
                {
                    //Funcion para obtener el idCiudad a traves del contenido del combobox
                    string idCddst = getCiudad(cb_CiudDestVuelo.Text, false);
                    //Funcion para obtener el idCiudad a traves del contenido del combobox
                    string idCdorg = getCiudad(cb_CiudOrigVuelo.Text, false);

                    string query = "INSERT INTO InfoAerolinea.Vuelo (idOrigen, idDestino, DuracionHoras, CostoBase) VALUES (" + idCdorg + "," + idCddst + "," + nud_DuracionVuelo.Text + ", " + mtb_CostoBaseVuelo.Text + ")";
                    Query(query);
                    consultaDatosVuelo();
                    btn_BackVuelo_Click(this, EventArgs.Empty);
                }
                else
                {
                    string mensaje = "Ingresa correctamente los datos";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje", buttons);
                }
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función getCiudad recupera idCiudad o Nom_Ciudad y Pais de la tabla InfoAerolinea.Ciudad
        /// en una base de datos SQL según los parámetros proporcionados.
        /// </summary>
        /// <param name="val">El parámetro "val" es una cadena que representa el nombre de la ciudad y
        /// el país o el ID de la ciudad, según el valor del parámetro "bandera".</param>
        /// <param name="flag">El parámetro de bandera es un valor booleano que determina el
        /// comportamiento del método. Si la bandera es verdadera, el método recupera el nombre de la
        /// ciudad y el país según el ID de la ciudad proporcionado. Si la bandera es falsa, el método
        /// recupera el ID de la ciudad según el nombre de la ciudad y el nombre del país.</param>
        /// <returns>
        /// El método `getCiudad` devuelve un valor de cadena.
        /// </returns>
        public string getCiudad(string val, bool flag)
        {
            string CdCalue = "";
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                conexion.Open();
                string consulta = "";
                if (!flag)
                {
                    //separa el contenido del string
                    string[] partes = val.Split('-');
                    string ciud = partes[0].Trim();
                    string pais = partes[1].Trim();

                    consulta = "SELECT idCiudad FROM InfoAerolinea.Ciudad WHERE Nom_Ciudad = '" + ciud + "' AND Pais = '"+ pais + "'";
                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        CdCalue = reader["idCiudad"].ToString();
                    }
                }
                else
                {
                    consulta = "SELECT Nom_Ciudad, Pais FROM InfoAerolinea.Ciudad WHERE idCiudad = " + val;
                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        CdCalue = reader["Nom_Ciudad"].ToString() + " - " + reader["Pais"].ToString();
                    }
                }
            }
            return CdCalue;
        }

        /// <summary>
        /// La función btn_Modificar_Vuelo_Click se utiliza para actualizar la información de vuelos en
        /// una base de datos en base a la entrada del usuario.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y normalmente se utiliza cuando un evento no requiere que se pase ninguna
        /// información adicional al controlador de eventos. En este caso, el controlador de eventos
        /// para el evento btn_Modificar_Vuelo_Click no requiere ninguna información adicional,</param>
        private void btn_Modificar_Vuelo_Click(object sender, EventArgs e)
        {
            if (cb_CiudDestVuelo.Text != "" && cb_CiudOrigVuelo.Text != "" && mtb_CostoBaseVuelo.Text != "" && nud_DuracionVuelo.Text != "")
            {
                if (cb_CiudDestVuelo.SelectedIndex != cb_CiudOrigVuelo.SelectedIndex && Int32.Parse(nud_DuracionVuelo.Text) > 1 && Int32.Parse(nud_DuracionVuelo.Text) < 24)
                {
                    //Funcion para obtener el idCiudad a traves del contenido del combobox
                    string idCddst = getCiudad(cb_CiudDestVuelo.Text, false);
                    //Funcion para obtener el idCiudad a traves del contenido del combobox
                    string idCdorg = getCiudad(cb_CiudOrigVuelo.Text, false);

                    string query = "UPDATE InfoAerolinea.Vuelo SET idOrigen = " + idCdorg + ", idDestino = " + idCddst + ", DuracionHoras = " + nud_DuracionVuelo.Text + ", CostoBase = " + mtb_CostoBaseVuelo.Text + " WHERE idVuelo = " + currentID;
                    Query(query);
                    consultaDatosVuelo();
                    btn_BackVuelo_Click(this, EventArgs.Empty);
                }
                else
                {
                    string mensaje = "Modifica correctamente los datos";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje", buttons);
                }
            }
            else
            {
                string mensaje = "Modifica correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// Esta función elimina un registro de vuelo de una tabla de base de datos si se completan
        /// todos los campos obligatorios; de lo contrario, muestra un mensaje de error.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos de un evento.
        /// Se utiliza comúnmente como parámetro en los métodos del controlador de eventos para
        /// proporcionar información sobre el evento que ocurrió. En este caso, el método controlador de
        /// eventos es btn_Eliminar_Vuelo_Click, que se activa cuando el "</param>
        private void btn_Eliminar_Vuelo_Click(object sender, EventArgs e)
        {
            if (cb_CiudDestVuelo.Text != "" && cb_CiudOrigVuelo.Text != "" && mtb_CostoBaseVuelo.Text != "" && nud_DuracionVuelo.Text != "")
            {
                string query = "DELETE FROM InfoAerolinea.Vuelo WHERE idVuelo = " + currentID;
                Query(query);
                consultaDatosVuelo();

                btn_BackVuelo_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Selecciona bien el registro a eliminar";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función btn_BackVuelo_Click se utiliza para restablecer y borrar los campos de entrada y
        /// desactivar ciertos botones en un sistema de gestión de vuelos.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic (btn_BackVuelo).</param>
        /// <param name="EventArgs">EventArgs es una clase que contiene datos de evento para un evento.
        /// Proporciona información sobre el evento que ocurrió, como el origen del evento y cualquier
        /// dato adicional asociado con el evento. En este caso, el parámetro EventArgs no se utiliza en
        /// el método.</param>
        private void btn_BackVuelo_Click(object sender, EventArgs e)
        {
            lbl_Modifica1.Visible = false;
            btn_Agregar_Vuelo.Enabled = true;
            btn_Eliminar_Vuelo.Enabled = false;
            btn_Modificar_Vuelo.Enabled = false;
            btn_BackVuelo.Visible = false;
            cb_CiudOrigVuelo.SelectedIndex = -1;
            cb_CiudDestVuelo.SelectedIndex = -1;
            mtb_CostoBaseVuelo.Text = "";
            nud_DuracionVuelo.Text = "0";
            currentID = "";
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en una celda en un DataGridView, recupera los
        /// valores de la fila en la que se hizo clic y completa varios controles con esos valores.
        /// </summary>
        /// <param name="sender">El objeto que generó el evento, en este caso, el control
        /// DataGridView.</param>
        /// <param name="DataGridViewCellMouseEventArgs">El parámetro DataGridViewCellMouseEventArgs es
        /// un argumento de evento que contiene información sobre el evento de clic del mouse que
        /// ocurrió en una celda DataGridView. Proporciona propiedades como el índice de fila y el
        /// índice de columna de la celda en la que se hizo clic, así como el botón del mouse en el que
        /// se hizo clic.</param>
        private void dgv_Vuelo_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string NombCd = "";
            if (e.RowIndex >= 0)
            {
                lbl_Modifica1.Visible = true;
                btn_Agregar_Vuelo.Enabled = false;
                btn_Eliminar_Vuelo.Enabled = true;
                btn_Modificar_Vuelo.Enabled = true;
                btn_BackVuelo.Visible = true;

                DataGridViewRow row = dgv_Vuelo.Rows[e.RowIndex];
                mtb_CostoBaseVuelo.Text = row.Cells["CostoBase"].Value.ToString();
                nud_DuracionVuelo.Text = row.Cells["DuracionHoras"].Value.ToString();
                currentID = row.Cells["idVuelo"].Value.ToString();

                NombCd = row.Cells["idOrigen"].Value.ToString();
                for (int i = 0; i < cb_CiudOrigVuelo.Items.Count; i++)
                {
                    string item = cb_CiudOrigVuelo.Items[i].ToString();

                    if (item.Equals(NombCd))
                    {
                        int indiceEncontrado = i;
                        cb_CiudOrigVuelo.SelectedIndex = indiceEncontrado;
                        break;
                    }
                }
                NombCd = row.Cells["idDestino"].Value.ToString();
                for (int i = 0; i < cb_CiudDestVuelo.Items.Count; i++)
                {
                    string item = cb_CiudDestVuelo.Items[i].ToString();

                    if (item.Equals(NombCd))
                    {
                        int indiceEncontrado = i;
                        cb_CiudDestVuelo.SelectedIndex = indiceEncontrado;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// La función "consultaDatosVuelo" recupera información de vuelos de una base de datos y llena
        /// un DataGridView con los resultados.
        /// </summary>
        public void consultaDatosVuelo()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoAerolinea.Vuelo";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            int i = 0;
            dgv_Vuelo.Rows.Clear();
            while (reader.Read())
            {
                dgv_Vuelo.Rows.Add();
                dgv_Vuelo.Rows[i].Cells[0].Value = reader["idVuelo"].ToString();
                dgv_Vuelo.Rows[i].Cells[1].Value = getCiudad(reader["idOrigen"].ToString(), true);
                dgv_Vuelo.Rows[i].Cells[2].Value = getCiudad(reader["idDestino"].ToString(), true);
                dgv_Vuelo.Rows[i].Cells[3].Value = reader["DuracionHoras"].ToString();
                dgv_Vuelo.Rows[i].Cells[4].Value = reader["CostoBase"].ToString();
                i++;
            }
        }

        /// <summary>
        /// La función mtb_CostoBaseVuelo_KeyPress restringe la entrada en un cuadro de texto para
        /// permitir solo dígitos y un máximo de un punto decimal con hasta tres decimales.
        /// </summary>
        /// <param name="sender">El parámetro del sender se refiere al objeto que generó el evento.
        /// En este caso se trata del control TextBox denominado "mtb_CostoBaseVuelo".</param>
        /// <param name="KeyPressEventArgs">KeyPressEventArgs es un argumento de evento que proporciona
        /// datos para el evento KeyPress. Contiene información sobre la tecla que se presionó, como la
        /// representación de caracteres de la tecla.</param>
        private void mtb_CostoBaseVuelo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // Asegura que solo haya un punto decimal
            if ((e.KeyChar == '.') && ((sender as System.Windows.Forms.TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            // Permite solo tres decimales
            if ((sender as System.Windows.Forms.TextBox).Text.Contains(".") && (sender as System.Windows.Forms.TextBox).Text.Split('.')[1].Length >= 3 && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }
        #endregion

        ///
        /// Metodos para la manipulación de la tabla ITINERARIO
        ///
        #region METODOS VENTANA ITINERARIO

        /// <summary>
        /// La función "LlenarCBItinerario" llena combobox con datos de la base de datos.
        /// </summary>
        public void LlenarCBItinerario()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();

            string query = "SELECT idPiloto, Nom_Piloto FROM InfoAerolinea.Piloto";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();
            while (reader.Read())
            {
                cb_PilotoItinerario.Items.Add(reader["idPiloto"].ToString()+" - "+ reader["Nom_Piloto"].ToString());
            }
            reader.Close();

            query = "SELECT idAvion,idAerolinea,Modelo,EstadoUso FROM InfoAerolinea.Avion";
            Comm = new SqlCommand(query, conexion);
            reader = Comm.ExecuteReader();
            while (reader.Read())
            {
                if(reader["EstadoUso"].ToString() == "True")
                {
                    int indiceParentesisInicio = getAerolinea(reader["idAerolinea"].ToString(), true).IndexOf('(');
                    string Aerolinea = getAerolinea(reader["idAerolinea"].ToString(), true).Substring(0, indiceParentesisInicio).Trim();
                    cb_AvionItinerario.Items.Add(reader["idAvion"].ToString()+" - "+ reader["Modelo"].ToString() + " - "+ Aerolinea);
                }
            }
            reader.Close();

            query = "SELECT idVuelo, idOrigen, idDestino FROM InfoAerolinea.Vuelo";
            Comm = new SqlCommand(query, conexion);
            reader = Comm.ExecuteReader();
            while (reader.Read())
            {
                int indiceGuionInicio = getCiudad(reader["idOrigen"].ToString(), true).IndexOf('-');
                string cdOrg = getCiudad(reader["idOrigen"].ToString(), true).Substring(0, indiceGuionInicio).Trim();
                indiceGuionInicio = getCiudad(reader["idDestino"].ToString(), true).IndexOf('-');
                string cdDest = getCiudad(reader["idDestino"].ToString(), true).Substring(0, indiceGuionInicio).Trim();

                cb_VueloItinerario.Items.Add(cdOrg + " - " + cdDest);
                listIDVuelo.Add(Convert.ToInt32(reader["idVuelo"].ToString()));
            }
            reader.Close();

            conexion.Close();
        }

        /// <summary>
        /// La función btn_Agregar_Itinerario_Click se utiliza para agregar un itinerario a una tabla de
        /// base de datos según la entrada del usuario.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y se utiliza como clase base para todas las clases de datos de eventos. En
        /// este caso, se utiliza como parámetro del método controlador de eventos
        /// btn_Agregar_Itinerario_Click.</param>
        private void btn_Agregar_Itinerario_Click(object sender, EventArgs e)
        {
            if (cb_PilotoItinerario.Text != "" && cb_AvionItinerario.Text != "" && cb_VueloItinerario.Text != "" && mtb_HoraItinerario.Text != "" && dtp_FechaVueloItinerario.Text != "")
            {
                DateTime fechaVuel = dtp_FechaVueloItinerario.Value;
                string date = fechaVuel.ToString("yyyy-MM-dd");
                string PilotoID = getPiloto(cb_PilotoItinerario.Text, false);
                string AvionID = getAvion(cb_AvionItinerario.Text, false);
                string VueloID = getVuelo(cb_VueloItinerario.Text,false);

                string query = "INSERT INTO InfoAerolinea.Itinerario (idPiloto, idAvion, idVuelo, HoraSalida, FechaVuelo) VALUES (" + PilotoID + "," + AvionID + "," + VueloID + ", '" + mtb_HoraItinerario.Text + "', '"+ date + "')";
                if (Query(query) == -1)
                {
                    string mensaje = "Ocurrió una excepción: \nNo se puede agergar dos vuelos del mismo piloto en el mismo dia";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Error", buttons);
                }
                consultaDatosItinerario();
                btn_BackItinerario_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función btn_Modificar_Itinerario_Click se utiliza para actualizar la información de un
        /// itinerario en la base de datos.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y normalmente se utiliza cuando no se necesitan datos adicionales para un
        /// evento.</param>
        private void btn_Modificar_Itinerario_Click(object sender, EventArgs e)
        {
            if (cb_PilotoItinerario.Text != "" && cb_AvionItinerario.Text != "" && cb_VueloItinerario.Text != "" && mtb_HoraItinerario.Text != "" && dtp_FechaVueloItinerario.Text != "")
            {
                DateTime fechaVuel = dtp_FechaVueloItinerario.Value;
                string date = fechaVuel.ToString("yyyy-MM-dd");
                string PilotoID = getPiloto(cb_PilotoItinerario.Text, false);
                string AvionID = getAvion(cb_AvionItinerario.Text, false);
                string VueloID = getVuelo(cb_VueloItinerario.Text, false);

                string query = "UPDATE InfoAerolinea.Itinerario SET idPiloto = " + PilotoID + ", idAvion = " + AvionID + ",idVuelo = " + VueloID + ", HoraSalida = '" + mtb_HoraItinerario.Text + "', FechaVuelo = '"+ date + "' WHERE idItinerario = " + currentID;
                Query(query);
                consultaDatosItinerario();
                btn_BackItinerario_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Modifica correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función `getPiloto` recupera el nombre de un piloto en función de un valor dado, ya sea
        /// extrayéndolo de una cadena o consultando una base de datos.
        /// </summary>
        /// <param name="val">El parámetro "val" es una cadena que representa el valor del piloto. Puede
        /// ser el ID del piloto o una cadena que contenga el ID del piloto seguido de un guión y el
        /// nombre del piloto.</param>
        /// <param name="flag">El parámetro de bandera es un valor booleano que determina si se recupera
        /// o no la información del piloto de la base de datos. Si el indicador es verdadero, la
        /// información del piloto se recuperará de la base de datos utilizando el parámetro val
        /// proporcionado como ID del piloto. Si la bandera es falsa, la información del piloto se
        /// extraerá del parámetro val.</param>
        /// <returns>
        /// El método devuelve un valor de cadena.
        /// </returns>
        public string getPiloto(string val, bool flag)
        {
            string PilotoValue = "";
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                if (!flag)
                {
                    int indiceGuionInicio = val.IndexOf('-');
                    PilotoValue = val.Substring(0, indiceGuionInicio).Trim();
                }
                else
                {
                    conexion.Open();
                    string consulta = "SELECT idPiloto, Nom_Piloto FROM InfoAerolinea.Piloto WHERE idPiloto = " + val;
                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        PilotoValue = reader["idPiloto"].ToString() + " - " + reader["Nom_Piloto"].ToString();
                    }
                }
            }
            return PilotoValue;
        }

        /// <summary>
        /// La función "getAvion" recupera información sobre un avión en función de un valor determinado
        /// y una bandera.
        /// </summary>
        /// <param name="val">El parámetro "val" es una cadena que representa el valor del Avion. Puede
        /// ser idAvion o una cadena que contenga idAvion, Modelo e idAerolinea separados por
        /// guiones.</param>
        /// <param name="flag">El parámetro de bandera es un valor booleano que determina si se recupera
        /// o no el valor de Avion de la base de datos. Si el indicador es verdadero, el valor de Avion
        /// se recuperará de la base de datos utilizando el parámetro val proporcionado como idAvion. Si
        /// la bandera es falsa, se extraerá el valor de Avion</param>
        /// <returns>
        /// El método devuelve un valor de cadena llamado "AvionValue".
        /// </returns>
        public string getAvion(string val, bool flag)
        {
            string AvionValue = "";
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                if (!flag)
                {
                    int indiceGuionInicio = val.IndexOf('-');
                    AvionValue = val.Substring(0, indiceGuionInicio).Trim();
                }
                else
                {
                    conexion.Open();
                    string consulta = "SELECT idAvion,idAerolinea,Modelo FROM InfoAerolinea.Avion WHERE idAvion="+val;
                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        int indiceParentesisInicio = getAerolinea(reader["idAerolinea"].ToString(), true).IndexOf('(');
                        string Aerolinea = getAerolinea(reader["idAerolinea"].ToString(), true).Substring(0, indiceParentesisInicio).Trim();
                        AvionValue = reader["idAvion"].ToString() + " - " + reader["Modelo"].ToString() + " - " + Aerolinea;
                    }
                }
            }
            return AvionValue;
        }

        /// <summary>
        /// La función "getVuelo" recupera información sobre un Vuelo en función de un valor determinado
        /// y una bandera.
        /// </summary>
        /// <param name="val">El parámetro "val" es una cadena que representa el valor del Vuelo. Puede
        /// ser idVuelo o una cadena que contenga informacion del Vuelo</param>
        /// <param name="flag">El parámetro de bandera es un valor booleano que determina si se recupera
        /// o no el valor de Vuelo de la base de datos. Si el indicador es verdadero, el valor de Vuelo
        /// se recuperará de la base de datos utilizando el parámetro val proporcionado como idVuelo. Si
        /// la bandera es falsa, se extraerá el valor de Vuelo de una lista con ids</param>
        /// <returns>
        /// El método devuelve un valor de cadena llamado "VueloValue".
        /// </returns>
        public string getVuelo(string val, bool flag)
        {
            string VueloValue = "";
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                if (!flag)
                {
                    /*
                    conexion.Open();
                    //separa el contenido del string
                    string[] partes = val.Split('-');
                    string ciud1 = partes[0].Trim();
                    string ciud2 = partes[1].Trim();
                    string consulta = "SELECT V.idVuelo FROM InfoAerolinea.Vuelo V JOIN InfoAerolinea.Ciudad O ON V.idOrigen = O.idCiudad JOIN InfoAerolinea.Ciudad D ON V.idDestino = D.idCiudad WHERE O.Nom_Ciudad = '"+ ciud1 + "' AND D.Nom_Ciudad = '"+ ciud2 + "'";

                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        VueloValue = reader["idVuelo"].ToString();
                    }
                    */
                    VueloValue = listIDVuelo[cb_VueloItinerario.SelectedIndex].ToString();
                }
                else
                {
                    conexion.Open();
                    string consulta = "SELECT idOrigen, idDestino FROM InfoAerolinea.Vuelo WHERE idVuelo=" + val;
                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        int indiceGuionInicio = getCiudad(reader["idOrigen"].ToString(), true).IndexOf('-');
                        string cdOrg = getCiudad(reader["idOrigen"].ToString(), true).Substring(0, indiceGuionInicio).Trim();
                        indiceGuionInicio = getCiudad(reader["idDestino"].ToString(), true).IndexOf('-');
                        string cdDest = getCiudad(reader["idDestino"].ToString(), true).Substring(0, indiceGuionInicio).Trim();

                        VueloValue = cdOrg + " - " + cdDest;
                    }
                }
            }
            return VueloValue;
        }

        /// <summary>
        /// Esta función elimina un registro de Itinerario de una tabla de la base de datos si se completan
        /// todos los campos obligatorios; de lo contrario, muestra un mensaje de error.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos de un evento.
        /// Se utiliza comúnmente como parámetro en los métodos del controlador de eventos para
        /// proporcionar información sobre el evento que ocurrió. En este caso, el método controlador de
        /// eventos es btn_Eliminar_Itinerario_Click, que se activa cuando el "</param>
        private void btn_Eliminar_Itinerario_Click(object sender, EventArgs e)
        {
            if (cb_PilotoItinerario.Text != "" && cb_AvionItinerario.Text != "" && cb_VueloItinerario.Text != "" && mtb_HoraItinerario.Text != "" && dtp_FechaVueloItinerario.Text != "")
            {
                string query = "DELETE FROM InfoAerolinea.Itinerario WHERE idItinerario = " + currentID;
                Query(query);
                consultaDatosItinerario();
                btn_BackItinerario_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Selecciona bien el registro a eliminar";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función btn_BackItinerario_Click se utiliza para restablecer y borrar los campos de entrada y
        /// desactivar ciertos botones en la ventana Itinerario.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic (btn_BackItinerario).</param>
        /// <param name="EventArgs">EventArgs es una clase que contiene datos de evento para un evento.
        /// Proporciona información sobre el evento que ocurrió, como el origen del evento y cualquier
        /// dato adicional asociado con el evento. En este caso, el parámetro EventArgs no se utiliza en
        /// el método.</param>
        private void btn_BackItinerario_Click(object sender, EventArgs e)
        {
            lbl_Modifica1.Visible = false;
            btn_Agregar_Itinerario.Enabled = true;
            btn_Eliminar_Itinerario.Enabled = false;
            btn_Modificar_Itinerario.Enabled = false;
            btn_BackItinerario.Visible = false;

            cb_PilotoItinerario.SelectedIndex = -1;
            cb_AvionItinerario.SelectedIndex = -1;
            cb_VueloItinerario.SelectedIndex = -1;
            mtb_HoraItinerario.Text = "00:00:00";
            dtp_FechaVueloItinerario.Text = "";
            currentID = "";
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en una celda en un DataGridView, recupera los
        /// valores de la fila en la que se hizo clic y completa varios controles con esos valores.
        /// </summary>
        /// <param name="sender">El objeto que generó el evento, en este caso, el control
        /// DataGridView.</param>
        /// <param name="DataGridViewCellMouseEventArgs">El parámetro DataGridViewCellMouseEventArgs es
        /// un argumento de evento que contiene información sobre el evento de clic del mouse que
        /// ocurrió en una celda DataGridView. Proporciona propiedades como el índice de fila y el
        /// índice de columna de la celda en la que se hizo clic, así como el botón del mouse en el que
        /// se hizo clic.</param>
        private void dgv_Itinerario_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string txtPiloto = "";
            string txtAvion = "";
            string txtVuelo = "";
            if (e.RowIndex >= 0)
            {
                lbl_Modifica1.Visible = true;
                btn_Agregar_Itinerario.Enabled = false;
                btn_Eliminar_Itinerario.Enabled = true;
                btn_Modificar_Itinerario.Enabled = true;
                btn_BackItinerario.Visible = true;

                DataGridViewRow row = dgv_Itinerario.Rows[e.RowIndex];
                mtb_HoraItinerario.Text = row.Cells["HoraSalida"].Value.ToString();
                dtp_FechaVueloItinerario.Text = row.Cells["FechaVuelo"].Value.ToString();
                currentID = row.Cells["idItinerario"].Value.ToString();

                txtPiloto = row.Cells["idPilotoIT"].Value.ToString();
                for (int i = 0; i < cb_PilotoItinerario.Items.Count; i++)
                {
                    string item = cb_PilotoItinerario.Items[i].ToString();

                    if (item.Equals(txtPiloto))
                    {
                        int indiceEncontrado = i;
                        cb_PilotoItinerario.SelectedIndex = indiceEncontrado;
                        break;
                    }
                }
                txtAvion = row.Cells["idAvionIT"].Value.ToString();
                for (int i = 0; i < cb_AvionItinerario.Items.Count; i++)
                {
                    string item = cb_AvionItinerario.Items[i].ToString();

                    if (item.Equals(txtAvion))
                    {
                        int indiceEncontrado = i;
                        cb_AvionItinerario.SelectedIndex = indiceEncontrado;
                        break;
                    }
                }
                txtVuelo = row.Cells["idVueloIT"].Value.ToString();
                for (int i = 0; i < cb_VueloItinerario.Items.Count; i++)
                {
                    string item = cb_VueloItinerario.Items[i].ToString();

                    if (item.Equals(txtVuelo))
                    {
                        int indiceEncontrado = i;
                        cb_VueloItinerario.SelectedIndex = indiceEncontrado;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// La función "consultaDatosItinerario" recupera información de vuelos de la base de datos y llena
        /// un DataGridView con los resultados.
        /// </summary>
        public void consultaDatosItinerario()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoAerolinea.Itinerario";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            int i = 0;
            dgv_Itinerario.Rows.Clear();
            while (reader.Read())
            {
                dgv_Itinerario.Rows.Add();
                DateTime fecha = (DateTime)reader["FechaVuelo"];
                dgv_Itinerario.Rows[i].Cells[0].Value = reader["idItinerario"].ToString();
                dgv_Itinerario.Rows[i].Cells[1].Value = getPiloto(reader["idPiloto"].ToString(), true);
                dgv_Itinerario.Rows[i].Cells[2].Value = getAvion(reader["idAvion"].ToString(), true);
                dgv_Itinerario.Rows[i].Cells[3].Value = getVuelo(reader["idVuelo"].ToString(), true);
                dgv_Itinerario.Rows[i].Cells[4].Value = reader["HoraSalida"].ToString();
                dgv_Itinerario.Rows[i].Cells[5].Value = fecha.ToString("dd/MM/yyyy");
                i++;
            }
        }

        /// <summary>
        /// La función mtb_HoraItinerario_Validating verifica que el texto de mtb_HoraItinerario sea
        /// el de una hora valida
        /// </summary>
        /// <param name="sender">El parámetro del sender se refiere al objeto que generó el evento.
        /// En este caso se trata del control TextBox denominado "mtb_HoraItinerario".</param>
        /// <param name="KeyPressEventArgs">KeyPressEventArgs es un argumento de evento que proporciona
        /// datos para el evento KeyPress. Contiene información sobre la tecla que se presionó, como la
        /// representación de caracteres de la tecla.</param>
        public void mtb_HoraItinerario_Validating(object sender, CancelEventArgs e)
        {
            string[] partes = mtb_HoraItinerario.Text.Split(':');

            if (partes.Length == 3)
            {
                int horas, minutos, segundos;
                if (int.TryParse(partes[0], out horas) &&
                    int.TryParse(partes[1], out minutos) &&
                    int.TryParse(partes[2], out segundos))
                {
                    if (horas > 24 || minutos > 59 || segundos > 59)
                    {
                        MessageBox.Show("Por favor, ingrese una hora válida.");
                        e.Cancel = true;
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, ingrese una hora válida.");
                    e.Cancel = true;
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingrese una hora válida.");
                e.Cancel = true;
            }
        }
        #endregion

        #endregion

        ///
        /// Metodos para ventanas de tablas del esquema InfoPasajero
        ///
        #region InfoPasajero

        ///
        /// Metodos para la manipulación de la tabla PASAJERO
        ///
        #region METODOS VENTANA PASAJERO

        /// <summary>
        /// La función btn_Agregar_Pasajero_Click se utiliza para agregar un Pasajero a la tabla de
        /// la base de datos según la entrada del usuario.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y se utiliza como clase base para todas las clases de datos de eventos. En
        /// este caso, se utiliza como parámetro del método controlador de eventos
        /// btn_Agregar_Pasajero_Click.</param>
        private void btn_Agregar_Pasajero_Click(object sender, EventArgs e)
        {
            if(tb_NombrePasajero.Text!="" && dtp_FechaNacPasajero.Text != "" && tb_NacionalidadPasajero.Text != "" && cb_GeneroPasajero.Text != ""&& tb_NumPassPasajero.Text != "" && tb_TelefonoPasajero.Text != "" && tb_ContEmerPasajero.Text != "" && mtb_EmailPasajero.Text != "" && mtb_EmailPasajero.Text.Length < 195)
            {
                string telpas = tb_TelefonoPasajero.Text;
                string contem = tb_ContEmerPasajero.Text;
                //EXP regular para el correo
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                Regex regex = new Regex(pattern);
                if (regex.IsMatch(mtb_EmailPasajero.Text) && tb_NumPassPasajero.Text.Length > 9 && telpas.Replace("-", "").Length > 9 && contem.Replace("-", "").Length > 9)
                {
                    DateTime fechaNacimiento = dtp_FechaNacPasajero.Value;
                    DateTime fechaActual = DateTime.Now;
                    if (fechaActual > fechaNacimiento && fechaNacimiento>new DateTime(1923, 1, 1))
                    {
                        string date = fechaNacimiento.ToString("yyyy-MM-dd");
                        string query = "INSERT INTO InfoPasajero.Pasajero (Nom_Pasajero, FechaNacimiento, Nacionalidad, Genero, NumPasaporte, Telefono, ContactoEmergencia, Email) VALUES ('" + tb_NombrePasajero.Text + "','" + date + "','" + tb_NacionalidadPasajero.Text + "','" + cb_GeneroPasajero.Text + "','" + tb_NumPassPasajero.Text + "'," + telpas.Replace("-", "") + "," + contem.Replace("-", "") + ",'" + mtb_EmailPasajero.Text + "')";
                        Query(query);
                        ConsultaDatosPasajero();
                        btn_BackPasajero_Click(this, EventArgs.Empty);
                    }
                    else
                    {
                        string mensaje = "Ingresa correctamente la fecha de nacimiento";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show(mensaje, "Mensaje", buttons);
                    }
                }
                else
                {
                    string mensaje = "Ingresa correctamente los datos";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje", buttons);
                }
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función btn_Modificar_Pasajero_Click se utiliza para actualizar la información de un
        /// Pasajero en la base de datos.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y normalmente se utiliza cuando no se necesitan datos adicionales para un
        /// evento.</param>
        private void btn_Modificar_Pasajero_Click(object sender, EventArgs e)
        {
            if (tb_NombrePasajero.Text != "" && dtp_FechaNacPasajero.Text != "" && tb_NacionalidadPasajero.Text != "" && cb_GeneroPasajero.Text != "" && tb_NumPassPasajero.Text != "" && tb_TelefonoPasajero.Text != "" && tb_ContEmerPasajero.Text != "" && mtb_EmailPasajero.Text != "" && mtb_EmailPasajero.Text.Length < 195)
            {
                string telpas = tb_TelefonoPasajero.Text;
                string contem = tb_ContEmerPasajero.Text;
                //EXP regular para el correo
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                Regex regex = new Regex(pattern);
                if (regex.IsMatch(mtb_EmailPasajero.Text) &&tb_NumPassPasajero.Text.Length > 9 && telpas.Replace("-", "").Length > 9 && contem.Replace("-", "").Length > 9)
                {
                    DateTime fechaNacimiento = dtp_FechaNacPasajero.Value;
                    DateTime fechaActual = DateTime.Now;
                    if (fechaActual > fechaNacimiento && fechaNacimiento > new DateTime(1923, 1, 1))
                    {
                        string date = fechaNacimiento.ToString("yyyy-MM-dd");
                        string query = "UPDATE InfoPasajero.Pasajero SET Nom_Pasajero = '" + tb_NombrePasajero.Text + "', FechaNacimiento = '" + date + "', Nacionalidad='" + tb_NacionalidadPasajero.Text + "', Genero='" + cb_GeneroPasajero.Text + "', NumPasaporte='" + tb_NumPassPasajero.Text + "', Telefono=" + telpas.Replace("-", "") + ", ContactoEmergencia=" + contem.Replace("-", "") + ", Email='" + mtb_EmailPasajero.Text + "' WHERE idPasajero = " + currentID;
                        Query(query);
                        ConsultaDatosPasajero();
                        btn_BackPasajero_Click(this, EventArgs.Empty);
                    }
                    else
                    {
                        string mensaje = "Ingresa correctamente la fecha de nacimiento";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show(mensaje, "Mensaje", buttons);
                    }
                }
                else
                {
                    string mensaje = "Ingresa correctamente los datos";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje", buttons);
                }
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// Esta función elimina un registro de Pasajero de la tabla de la base de datos si se completan
        /// todos los campos obligatorios; de lo contrario, muestra un mensaje de error.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos de un evento.
        /// Se utiliza comúnmente como parámetro en los métodos del controlador de eventos para
        /// proporcionar información sobre el evento que ocurrió.</param>
        private void btn_Eliminar_Pasajero_Click(object sender, EventArgs e)
        {
            if (tb_NombrePasajero.Text != "" && dtp_FechaNacPasajero.Text != "" && tb_NacionalidadPasajero.Text != "" && cb_GeneroPasajero.Text != "" && tb_NumPassPasajero.Text != "" && tb_TelefonoPasajero.Text != "" && tb_ContEmerPasajero.Text != "" && mtb_EmailPasajero.Text != "")
            {
                string query = "DELETE FROM InfoPasajero.Pasajero WHERE idPasajero = " + currentID;
                Query(query);
                ConsultaDatosPasajero();
                btn_BackPasajero_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función btn_BackPasajero_Click se utiliza para restablecer y borrar los campos de entrada y
        /// desactivar ciertos botones en la ventana Pasajero.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic (btn_BackPasajero).</param>
        /// <param name="EventArgs">EventArgs es una clase que contiene datos de evento para un evento.
        /// Proporciona información sobre el evento que ocurrió, como el origen del evento y cualquier
        /// dato adicional asociado con el evento. En este caso, el parámetro EventArgs no se utiliza en
        /// el método.</param>
        private void btn_BackPasajero_Click(object sender, EventArgs e)
        {
            lbl_Modifica1.Visible = false;
            btn_Agregar_Pasajero.Enabled = true;
            btn_Eliminar_Pasajero.Enabled = false;
            btn_Modificar_Pasajero.Enabled = false;
            btn_BackPasajero.Visible = false;

            tb_NombrePasajero.Text = ""; 
            dtp_FechaNacPasajero.Text = "";
            tb_NacionalidadPasajero.Text = "";
            cb_GeneroPasajero.Text = "";
            tb_NumPassPasajero.Text = "";
            tb_TelefonoPasajero.Text = "";
            tb_ContEmerPasajero.Text = "";
            mtb_EmailPasajero.Text = "";

            currentID = "";
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en una celda en un DataGridView, recupera los
        /// valores de la fila en la que se hizo clic y completa varios controles con esos valores.
        /// </summary>
        /// <param name="sender">El objeto que generó el evento, en este caso, el control
        /// DataGridView.</param>
        /// <param name="DataGridViewCellMouseEventArgs">El parámetro DataGridViewCellMouseEventArgs es
        /// un argumento de evento que contiene información sobre el evento de clic del mouse que
        /// ocurrió en una celda DataGridView. Proporciona propiedades como el índice de fila y el
        /// índice de columna de la celda en la que se hizo clic, así como el botón del mouse en el que
        /// se hizo clic.</param>
        private void dgv_Pasajero_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lbl_Modifica1.Visible = true;
                btn_Agregar_Pasajero.Enabled = false;
                btn_Eliminar_Pasajero.Enabled = true;
                btn_Modificar_Pasajero.Enabled = true;
                btn_BackPasajero.Visible = true;

                DataGridViewRow row = dgv_Pasajero.Rows[e.RowIndex];
                tb_NombrePasajero.Text = row.Cells["Nom_Pasajero"].Value.ToString();
                dtp_FechaNacPasajero.Text = row.Cells["FechaNacimientoPS"].Value.ToString();
                tb_NacionalidadPasajero.Text = row.Cells["Nacionalidad"].Value.ToString();
                cb_GeneroPasajero.Text = row.Cells["GeneroPS"].Value.ToString();
                tb_NumPassPasajero.Text = row.Cells["NumPasaporte"].Value.ToString();
                tb_TelefonoPasajero.Text = row.Cells["Telefono"].Value.ToString();
                tb_ContEmerPasajero.Text = row.Cells["ContactoEmergencia"].Value.ToString();
                mtb_EmailPasajero.Text = row.Cells["Email"].Value.ToString();

                currentID = row.Cells["idPasajero"].Value.ToString();
            }
        }

        /// <summary>
        /// La función "ConsultaDatosPasajero" recupera información de vuelos de la base de datos y llena
        /// un DataGridView con los resultados.
        /// </summary>
        public void ConsultaDatosPasajero()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoPasajero.Pasajero";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            int i = 0;
            dgv_Pasajero.Rows.Clear();
            while (reader.Read())
            {
                dgv_Pasajero.Rows.Add();
                DateTime fecha = (DateTime)reader["FechaNacimiento"];
                dgv_Pasajero.Rows[i].Cells[0].Value = reader["idPasajero"].ToString();
                dgv_Pasajero.Rows[i].Cells[1].Value = reader["Nom_Pasajero"].ToString();
                dgv_Pasajero.Rows[i].Cells[2].Value = fecha.ToString("dd/MM/yyyy");
                dgv_Pasajero.Rows[i].Cells[3].Value = reader["Edad"].ToString();
                dgv_Pasajero.Rows[i].Cells[4].Value = reader["Nacionalidad"].ToString();
                dgv_Pasajero.Rows[i].Cells[5].Value = reader["Genero"].ToString();
                dgv_Pasajero.Rows[i].Cells[6].Value = reader["NumPasaporte"].ToString();
                dgv_Pasajero.Rows[i].Cells[7].Value = reader["Telefono"].ToString();
                dgv_Pasajero.Rows[i].Cells[8].Value = reader["ContactoEmergencia"].ToString();
                dgv_Pasajero.Rows[i].Cells[9].Value = reader["Email"].ToString();
                i++;
            }
        }

        /// <summary>
        /// La función restringe la entrada en un cuadro de texto para permitir solo dígitos y limita la
        /// longitud a 11 caracteres.
        /// </summary>
        /// <param name="sender">El parámetro del sender se refiere al objeto que generó el evento.
        /// En este caso, es el control del cuadro de texto (tb_NumPassPasajero) el que generó el evento
        /// KeyPress.</param>
        /// <param name="KeyPressEventArgs">KeyPressEventArgs es un argumento de evento que proporciona
        /// datos para el evento KeyPress. Contiene información sobre la tecla que se presionó, como la
        /// representación de caracteres de la tecla.</param>
        private void tb_NumPassPasajero_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            // Limitar la longitud a 11 caracteres
            if (tb_NumPassPasajero.Text.Length >= 11 && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }
        #endregion

        ///
        /// Metodos para la manipulación de la tabla TARJETA PASAJERO
        ///
        #region METODOS VENTANA TARJETA PASAJERO

        /// <summary>
        /// La función "LlenarCBTarjPasaj" llena un combobox con datos de la base de datos.
        /// </summary>
        public void LlenarCBTarjPasaj()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();

            string query = "SELECT Nom_Pasajero, NumPasaporte FROM InfoPasajero.Pasajero";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();
            while (reader.Read())
            {
                string digitnumpass = reader["NumPasaporte"].ToString().Substring(reader["NumPasaporte"].ToString().Length - 4);
                cb_PasajeroTarjPasaj.Items.Add(digitnumpass + " - " + reader["Nom_Pasajero"].ToString());
            }
            reader.Close();
            conexion.Close();
        }

        /// <summary>
        /// La función btn_Agregar_TarjPasajero_Click se utiliza para agregar una Tarjeta Pasajero a la tabla de
        /// la base de datos según la entrada del usuario.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y se utiliza como clase base para todas las clases de datos de eventos. En
        /// este caso, se utiliza como parámetro del método controlador de eventos
        /// btn_Agregar_TarjPasajero_Click.</param>
        private void btn_Agregar_TarjPasajero_Click(object sender, EventArgs e)
        {
            if(cb_PasajeroTarjPasaj.Text!=""&&tb_NomTitularTarjPasaj.Text!=""&&tb_NumTarjPasaj.Text!="" &&dtp_FechaVencTarjPasaj.Text!=""&&tb_CVVTarjPasaj.Text!="")
            {
                DateTime fechaVen = dtp_FechaVencTarjPasaj.Value;
                //le apone el dia en el primero
                DateTime fechaVenF = new DateTime(fechaVen.Year, fechaVen.Month, 1);
                string date = fechaVenF.ToString("yyyy-MM-dd");
                string PasajeroID = getPasajero(cb_PasajeroTarjPasaj.Text, false);

                string query = "INSERT INTO InfoPasajero.TarjetaPasajero (idPasajero, NombreTitular, Banco, NumTarjeta, FechaVencimiento, CVV) VALUES (" + PasajeroID + ",'" + tb_NomTitularTarjPasaj.Text + "', 'Desconocido', " + tb_NumTarjPasaj.Text + ", '" + date + "', " + tb_CVVTarjPasaj.Text + ")";
                if(Query(query) == -1)
                {
                    string mensaje = "Ocurrió una excepción: \n No se puede agergar ";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Error", buttons);
                }
                ConsultaDatosTarjPasajero();
                btn_BackTarjPasajero_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función btn_Modificar_TarjPasajero_Click se utiliza para actualizar la información de 
        /// una Tarjeta Pasajero en la base de datos.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y normalmente se utiliza cuando no se necesitan datos adicionales para un
        /// evento.</param>
        private void btn_Modificar_TarjPasajero_Click(object sender, EventArgs e)
        {
            if (cb_PasajeroTarjPasaj.Text != "" && tb_NomTitularTarjPasaj.Text != "" && tb_NumTarjPasaj.Text != "" && dtp_FechaVencTarjPasaj.Text != "" && tb_CVVTarjPasaj.Text != "")
            {
                DateTime fechaVen = dtp_FechaVencTarjPasaj.Value;
                DateTime fechaVenF = new DateTime(fechaVen.Year, fechaVen.Month, 1);
                string date = fechaVenF.ToString("yyyy-MM-dd");
                string PasajeroID = getPasajero(cb_PasajeroTarjPasaj.Text, false);

                string query = "UPDATE InfoPasajero.TarjetaPasajero SET idPasajero = " + PasajeroID + ", NombreTitular = '" + tb_NomTitularTarjPasaj.Text + "', NumTarjeta = " + tb_NumTarjPasaj.Text + ", FechaVencimiento = '" + date + "', CVV = " + tb_CVVTarjPasaj.Text + " WHERE idTarjetaPasajero = " + currentID;
                Query(query);
                ConsultaDatosTarjPasajero();
                btn_BackTarjPasajero_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función "getPasajero" recupera información sobre un Pasajero basándose en su nombre o
        /// ID.
        /// </summary>
        /// <param name="val">El parámetro "val" es una cadena que representa el valor utilizado en la
        /// consulta SQL. Puede ser el una cadena con informacion del pasajero o el ID de un pasajero, según el valor
        /// del parámetro "flag".</param>
        /// <param name="flag">El parámetro "flag" es una bandera booleana que determina el
        /// comportamiento del método. Si se establece en verdadero, el método recuperará una
        /// cadena con informacion del pasajero . Si se establece en falso, el método
        /// recuperará el id de un Pasajero según</param>
        /// <returns>
        /// El método devuelve un valor de cadena.
        /// </returns>
        public string getPasajero(string val, bool flag)
        {
            string PasajeroValue = "";
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                if (!flag)
                {
                    conexion.Open();
                    //separa el contenido del string
                    string[] partes = val.Split('-');
                    string digitpass = partes[0].Trim();
                    string nompas = partes[1].Trim();
                    string consulta = "SELECT idPasajero FROM InfoPasajero.Pasajero WHERE NumPasaporte LIKE '%"+ digitpass + "' AND Nom_Pasajero = '"+ nompas + "'";

                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        PasajeroValue = reader["idPasajero"].ToString();
                    }
                }
                else
                {
                    conexion.Open();
                    string consulta = "SELECT Nom_Pasajero, NumPasaporte FROM InfoPasajero.Pasajero WHERE idPasajero=" + val;
                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        string digitnumpass = reader["NumPasaporte"].ToString().Substring(reader["NumPasaporte"].ToString().Length - 4);

                        PasajeroValue = digitnumpass + " - " + reader["Nom_Pasajero"].ToString();
                    }
                }
            }
            return PasajeroValue;
        }

        /// <summary>
        /// Esta función elimina un registro de Tarjeta Pasajero de la tabla de la base de datos 
        /// si se completan todos los campos obligatorios; de lo contrario, muestra un mensaje de error.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos de un evento.
        /// Se utiliza comúnmente como parámetro en los métodos del controlador de eventos para
        /// proporcionar información sobre el evento que ocurrió.</param>
        private void btn_Eliminar_TarjPasajero_Click(object sender, EventArgs e)
        {
            if (cb_PasajeroTarjPasaj.Text != "" && tb_NomTitularTarjPasaj.Text != "" && tb_NumTarjPasaj.Text != "" && dtp_FechaVencTarjPasaj.Text != "" && tb_CVVTarjPasaj.Text != "")
            {
                string query = "DELETE FROM InfoPasajero.TarjetaPasajero WHERE idTarjetaPasajero = " + currentID;
                Query(query);
                ConsultaDatosTarjPasajero();
                btn_BackTarjPasajero_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función btn_BackTarjPasajero_Click se utiliza para restablecer y borrar los campos de entrada y
        /// desactivar ciertos botones en la ventana Tarjeta Pasajero.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic (btn_BackTarjPasajero).</param>
        /// <param name="EventArgs">EventArgs es una clase que contiene datos de evento para un evento.
        /// Proporciona información sobre el evento que ocurrió, como el origen del evento y cualquier
        /// dato adicional asociado con el evento. En este caso, el parámetro EventArgs no se utiliza en
        /// el método.</param>
        private void btn_BackTarjPasajero_Click(object sender, EventArgs e)
        {
            lbl_Modifica1.Visible = false;
            btn_Agregar_TarjPasajero.Enabled = true;
            btn_Eliminar_TarjPasajero.Enabled = false;
            btn_Modificar_TarjPasajero.Enabled = false;
            btn_BackTarjPasajero.Visible = false;

            cb_PasajeroTarjPasaj.SelectedIndex = -1;
            tb_NomTitularTarjPasaj.Text = "";
            tb_NumTarjPasaj.Text = "";
            dtp_FechaVencTarjPasaj.Text = "";
            tb_CVVTarjPasaj.Text = "";

            currentID = "";
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en una celda en un DataGridView, recupera los
        /// valores de la fila en la que se hizo clic y completa varios controles con esos valores.
        /// </summary>
        /// <param name="sender">El objeto que generó el evento, en este caso, el control
        /// DataGridView.</param>
        /// <param name="DataGridViewCellMouseEventArgs">El parámetro DataGridViewCellMouseEventArgs es
        /// un argumento de evento que contiene información sobre el evento de clic del mouse que
        /// ocurrió en una celda DataGridView. Proporciona propiedades como el índice de fila y el
        /// índice de columna de la celda en la que se hizo clic, así como el botón del mouse en el que
        /// se hizo clic.</param>
        private void dgv_TarjetaPasajero_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lbl_Modifica1.Visible = true;
                btn_Agregar_TarjPasajero.Enabled = false;
                btn_Eliminar_TarjPasajero.Enabled = true;
                btn_Modificar_TarjPasajero.Enabled = true;
                btn_BackTarjPasajero.Visible = true;

                DataGridViewRow row = dgv_TarjetaPasajero.Rows[e.RowIndex];
                tb_NomTitularTarjPasaj.Text = row.Cells["NombreTitular"].Value.ToString();
                tb_NumTarjPasaj.Text = row.Cells["NumTarjeta"].Value.ToString();
                dtp_FechaVencTarjPasaj.Text = row.Cells["FechaVencimiento"].Value.ToString();
                tb_CVVTarjPasaj.Text = row.Cells["CVV"].Value.ToString();

                string txtPasaj = row.Cells["PasajeroTP"].Value.ToString();
                for (int i = 0; i < cb_PasajeroTarjPasaj.Items.Count; i++)
                {
                    string item = cb_PasajeroTarjPasaj.Items[i].ToString();

                    if (item.Equals(txtPasaj))
                    {
                        int indiceEncontrado = i;
                        cb_PasajeroTarjPasaj.SelectedIndex = indiceEncontrado;
                        break;
                    }
                }

                currentID = row.Cells["idTarjetaPasajero"].Value.ToString();
            }
        }

        /// <summary>
        /// La función "ConsultaDatosTarjPasajero" recupera información de vuelos de la base de datos y llena
        /// un DataGridView con los resultados.
        /// </summary>
        public void ConsultaDatosTarjPasajero()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoPasajero.TarjetaPasajero";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            int i = 0;
            dgv_TarjetaPasajero.Rows.Clear();
            while (reader.Read())
            {
                dgv_TarjetaPasajero.Rows.Add();
                DateTime fecha = (DateTime)reader["FechaVencimiento"];
                dgv_TarjetaPasajero.Rows[i].Cells[0].Value = reader["idTarjetaPasajero"].ToString();
                dgv_TarjetaPasajero.Rows[i].Cells[1].Value = getPasajero(reader["idPasajero"].ToString(), true);
                dgv_TarjetaPasajero.Rows[i].Cells[2].Value = reader["NombreTitular"].ToString();
                dgv_TarjetaPasajero.Rows[i].Cells[3].Value = reader["Banco"].ToString();
                dgv_TarjetaPasajero.Rows[i].Cells[4].Value = reader["NumTarjeta"].ToString();
                dgv_TarjetaPasajero.Rows[i].Cells[5].Value = fecha.ToString("MM/yyyy");
                dgv_TarjetaPasajero.Rows[i].Cells[6].Value = reader["CVV"].ToString();
                i++;
            }
        }

        /// <summary>
        /// La función restringe la entrada en un cuadro de texto para permitir solo dígitos y limita la
        /// longitud a 16 caracteres.
        /// </summary>
        /// <param name="sender">El parámetro del sender se refiere al objeto que generó el evento.
        /// En este caso, es el control del cuadro de texto (tb_NumPassPasajero) el que generó el evento
        /// KeyPress.</param>
        /// <param name="KeyPressEventArgs">KeyPressEventArgs es un argumento de evento que proporciona
        /// datos para el evento KeyPress. Contiene información sobre la tecla que se presionó, como la
        /// representación de caracteres de la tecla.</param>
        private void tb_NumTarjPasaj_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (tb_NumTarjPasaj.Text.Length >= 16 && e.KeyChar != (char)Keys.Back)
            {
                if (tb_NumTarjPasaj.Text.Length < 16)
                {
                    e.Handled = false;
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// La función restringe la entrada en un cuadro de texto para permitir solo dígitos y limita la
        /// longitud a 3 caracteres.
        /// </summary>
        /// <param name="sender">El parámetro del sender se refiere al objeto que generó el evento.
        /// En este caso, es el control del cuadro de texto (tb_NumPassPasajero) el que generó el evento
        /// KeyPress.</param>
        /// <param name="KeyPressEventArgs">KeyPressEventArgs es un argumento de evento que proporciona
        /// datos para el evento KeyPress. Contiene información sobre la tecla que se presionó, como la
        /// representación de caracteres de la tecla.</param>
        private void tb_CVVTarjPasaj_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (tb_CVVTarjPasaj.Text.Length >= 3 && e.KeyChar != (char)Keys.Back)
            {
                if (tb_CVVTarjPasaj.Text.Length < 3)
                {
                    e.Handled = false;
                }
                e.Handled = true;
            }
        }

        #endregion

        ///
        /// Metodos para la manipulación de la tabla ASIENTO
        ///
        #region METODOS VENTANA ASIENTO

        /// <summary>
        /// La función "LlenarCBAsiento" llena un combobox con datos de la base de datos.
        /// </summary>
        public void LlenarCBAsiento()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();

            string query = "SELECT idItinerario, idVuelo, HoraSalida, FechaVuelo FROM InfoAerolinea.Itinerario";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();
            while (reader.Read())
            {
                string horaCompleta = reader["HoraSalida"].ToString();
                string horaSinSegundos = horaCompleta.Substring(0, 5);

                DateTime fecha = (DateTime)reader["FechaVuelo"];
                string nuevaFecha = fecha.ToString("MM-dd-yyyy");

                cb_ItinerarioAsiento.Items.Add("(" + getVuelo(reader["idVuelo"].ToString(), true) + ") - " + horaSinSegundos + " - " + nuevaFecha);
                ListIDItinerario.Add(Convert.ToInt32(reader["idItinerario"].ToString()));
            }
        }

        /// <summary>
        /// La función btn_BackAsiento_Click se utiliza para restablecer y borrar los campos de entrada y
        /// desactivar ciertos botones en la ventana Tarjeta Pasajero.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic (btn_BackAsiento).</param>
        /// <param name="EventArgs">EventArgs es una clase que contiene datos de evento para un evento.
        /// Proporciona información sobre el evento que ocurrió, como el origen del evento y cualquier
        /// dato adicional asociado con el evento. En este caso, el parámetro EventArgs no se utiliza en
        /// el método.</param>
        private void btn_BackAsiento_Click(object sender, EventArgs e)
        {
            lbl_Modifica1.Visible = false;
            btn_Agregar_Asiento.Enabled = true;
            btn_Eliminar_Asiento.Enabled = false;
            btn_Modificar_Asiento.Enabled = false;
            btn_BackAsiento.Visible = false;

            cb_ItinerarioAsiento.SelectedIndex = -1;
            cb_LetraAsiento.SelectedIndex = -1;
            nud_NumAsiento.Text = "1";
            chbx_OcupadoAsiento.Checked = false;

            currentID = "";
        }

        /// <summary>
        /// La función btn_Agregar_Asiento_Click se utiliza para agregar un Asiento a la tabla de
        /// la base de datos según la entrada del usuario.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y se utiliza como clase base para todas las clases de datos de eventos. En
        /// este caso, se utiliza como parámetro del método controlador de eventos
        /// btn_Agregar_Asiento_Click.</param>
        private void btn_Agregar_Asiento_Click(object sender, EventArgs e)
        {
            if (cb_ItinerarioAsiento.Text != "" && cb_LetraAsiento.Text != "" && nud_NumAsiento.Text != "")
            {
                string idIT = ListIDItinerario[cb_ItinerarioAsiento.SelectedIndex].ToString();

                if (VerificarCupoItinerario(idIT))
                {
                    string query = chbx_OcupadoAsiento.Checked ?
                        "INSERT INTO InfoPasajero.Asiento (idItinerario, Num_Asiento, Letra, Ocupado) VALUES (" + idIT + ", " + nud_NumAsiento.Text + ", '" + cb_LetraAsiento.Text + "', 1)":
                        "INSERT INTO InfoPasajero.Asiento (idItinerario, Num_Asiento, Letra, Ocupado) VALUES (" + idIT + ", " + nud_NumAsiento.Text + ", '" + cb_LetraAsiento.Text + "', 0)";
                    
                    if (Query(query) == -1)
                    {
                        string mensaje = "Ocurrió una excepción: \nEse asiento ya existe";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show(mensaje, "Error", buttons);
                    }

                    ConsultaDatosAsiento();
                    btn_BackAsiento_Click(this, EventArgs.Empty);
                }
                else
                {
                    string mensaje = "Ese itinerario ya esta lleno";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje", buttons);
                }
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función btn_Modificar_Asiento_Click se utiliza para actualizar la información de 
        /// un Asiento en la base de datos.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y normalmente se utiliza cuando no se necesitan datos adicionales para un
        /// evento.</param>
        private void btn_Modificar_Asiento_Click(object sender, EventArgs e)
        {
            if (cb_ItinerarioAsiento.Text != "" && cb_LetraAsiento.Text != "" && nud_NumAsiento.Text != "")
            {
                string idIT = ListIDItinerario[cb_ItinerarioAsiento.SelectedIndex].ToString();

                if (VerificarCupoItinerario(idIT) && VerificarBoletoItinerario(currentID))
                {
                    string query = chbx_OcupadoAsiento.Checked ?
                        "UPDATE InfoPasajero.Asiento SET idItinerario = " + idIT + ", Num_Asiento = " + nud_NumAsiento.Text + ", Letra = '" + cb_LetraAsiento.Text + "', Ocupado = 1 WHERE idAsiento = " + currentID:
                        "UPDATE InfoPasajero.Asiento SET idItinerario = " + idIT + ", Num_Asiento = " + nud_NumAsiento.Text + ", Letra = '" + cb_LetraAsiento.Text + "', Ocupado = 0 WHERE idAsiento = " + currentID;

                    if (Query(query) == -1)
                    {
                        string mensaje = "Ocurrió una excepción: \nEse asiento ya existe, no se modificó";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show(mensaje, "Error", buttons);
                    }

                    ConsultaDatosAsiento();
                    btn_BackAsiento_Click(this, EventArgs.Empty);
                }
                else
                {
                    string mensaje = "No se pudo modificar porque el nuevo itinerario ya esta lleno \no el asiento ya esta asociado a una venta-boleto";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje", buttons);
                }
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// Esta función elimina un registro de Asiento de la tabla de la base de datos 
        /// si se completan todos los campos obligatorios; de lo contrario, muestra un mensaje de error.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos de un evento.
        /// Se utiliza comúnmente como parámetro en los métodos del controlador de eventos para
        /// proporcionar información sobre el evento que ocurrió.</param>
        private void btn_Eliminar_Asiento_Click(object sender, EventArgs e)
        {
            if (cb_ItinerarioAsiento.Text != "" && cb_LetraAsiento.Text != "" && nud_NumAsiento.Text != "")
            {
                string query = "DELETE FROM InfoPasajero.Asiento WHERE idAsiento = " + currentID;
                if (Query(query) == -1)
                {
                    string mensaje = "Ocurrió una excepción: \nHay un boleto asociado a ese asiento";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Error", buttons);
                }
                ConsultaDatosAsiento();
                btn_BackAsiento_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función "ConsultaDatosAsiento" recupera información de vuelos de la base de datos y llena
        /// un DataGridView con los resultados.
        /// </summary>
        private void ConsultaDatosAsiento()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoPasajero.Asiento";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            int i = 0;
            dgv_Asiento.Rows.Clear();
            while (reader.Read())
            {
                dgv_Asiento.Rows.Add();
                dgv_Asiento.Rows[i].Cells[0].Value = reader["idAsiento"].ToString();
                dgv_Asiento.Rows[i].Cells[1].Value = getItinerario(reader["idItinerario"].ToString());
                dgv_Asiento.Rows[i].Cells[2].Value = reader["Num_Asiento"].ToString();
                dgv_Asiento.Rows[i].Cells[3].Value = reader["Letra"].ToString();
                dgv_Asiento.Rows[i].Cells[4].Value = reader["Ocupado"].ToString();
                i++;
            }
        }

        /// <summary>
        /// La función "getItinerario" recupera información de un itinerario basándose en su ID.
        /// </summary>
        /// <param name="itinerariotext">El parámetro "itinerariotext" es una cadena que representa el ID del itinerario
        /// utilizado en la consulta SQL.</param>
        /// <returns>
        /// El método devuelve un valor de cadena con información del itinerario.
        /// </returns>
        private string getItinerario(string itinerariotext)
        {
            string ItinerarioValue = "";
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                conexion.Open();
                string query = "SELECT idVuelo, HoraSalida, FechaVuelo FROM InfoAerolinea.Itinerario WHERE idItinerario=" + itinerariotext;
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    string horaCompleta = reader["HoraSalida"].ToString();
                    string horaSinSegundos = horaCompleta.Substring(0, 5);

                    DateTime fecha = (DateTime)reader["FechaVuelo"];
                    string nuevaFecha = fecha.ToString("MM-dd-yyyy");

                    ItinerarioValue = "(" + getVuelo(reader["idVuelo"].ToString(), true) + ") - " + horaSinSegundos + " - " + nuevaFecha;
                }
            }
            return ItinerarioValue;
        }

        /// <summary>
        /// La función "VerificarCupoItinerario" verifica la disponibilidad de cupo en un itinerario.
        /// </summary>
        /// <param name="itinerario">El parámetro "itinerario" es una cadena que representa el ID del itinerario
        /// utilizado en la consulta SQL.</param>
        /// <returns>
        /// El método devuelve un valor booleano indicando si hay cupo disponible en el itinerario.
        /// </returns>
        private bool VerificarCupoItinerario(string itinerario)
        {
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                conexion.Open();
                string consulta = "SELECT COUNT(*) FROM InfoPasajero.Asiento WHERE idItinerario = " + itinerario;
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    int count = (int)cmd.ExecuteScalar();
                    if (count == 0 || count < 8)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// La función "VerificarBoletoItinerario" verifica si hay un boleto asociado a un asiento en un itinerario.
        /// </summary>
        /// <param name="Asiento">El parámetro "Asiento" es una cadena que representa el ID del asiento
        /// utilizado en la consulta SQL.</param>
        /// <returns>
        /// El método devuelve un valor booleano indicando si hay un boleto asociado al asiento en el itinerario.
        /// </returns>
        private bool VerificarBoletoItinerario(string Asiento)
        {
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                conexion.Open();
                string consulta = "SELECT COUNT(*) FROM InfoPasajero.Boleto WHERE idAsiento = " + Asiento;
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    int count = (int)cmd.ExecuteScalar();
                    if (count == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en una celda en un DataGridView, recupera los
        /// valores de la fila en la que se hizo clic y completa varios controles con esos valores.
        /// </summary>
        /// <param name="sender">El objeto que generó el evento, en este caso, el control
        /// DataGridView.</param>
        /// <param name="DataGridViewCellMouseEventArgs">El parámetro DataGridViewCellMouseEventArgs es
        /// un argumento de evento que contiene información sobre el evento de clic del mouse que
        /// ocurrió en una celda DataGridView. Proporciona propiedades como el índice de fila y el
        /// índice de columna de la celda en la que se hizo clic, así como el botón del mouse en el que
        /// se hizo clic.</param>
        private void dgv_Asiento_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lbl_Modifica1.Visible = true;
                btn_Agregar_Asiento.Enabled = false;
                btn_Eliminar_Asiento.Enabled = true;
                btn_Modificar_Asiento.Enabled = true;
                btn_BackAsiento.Visible = true;

                DataGridViewRow row = dgv_Asiento.Rows[e.RowIndex];

                cb_ItinerarioAsiento.SelectedIndex = -1;
                cb_LetraAsiento.SelectedIndex = (row.Cells["Letra"].Value.ToString() == "A") ? 0 : 1;
                nud_NumAsiento.Text = row.Cells["Num_Asiento"].Value.ToString();
                chbx_OcupadoAsiento.Checked = (row.Cells["Ocupado"].Value.ToString() == "True") ? true : false;
                string txtiti = row.Cells["idItinerarioAS"].Value.ToString();
                for (int i = 0; i < cb_ItinerarioAsiento.Items.Count; i++)
                {
                    string item = cb_ItinerarioAsiento.Items[i].ToString();

                    if (item.Equals(txtiti))
                    {
                        int indiceEncontrado = i;
                        cb_ItinerarioAsiento.SelectedIndex = indiceEncontrado;
                        break;
                    }
                }

                currentID = row.Cells["idAsiento"].Value.ToString();
            }
        }

        #endregion

        ///
        /// Metodos para la manipulación de la tabla VENTA-BOLETO
        ///
        #region METODOS VENTANAS VENTA-BOLETO

        /// <summary>
        /// La función btn_Agregar_VentaBoleto_Click se utiliza para agregar una venta de boleto a la tabla de
        /// la base de datos según la entrada del usuario.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="e">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y se utiliza como clase base para todas las clases de datos de eventos. En
        /// este caso, se utiliza como parámetro del método controlador de eventos
        /// btn_Agregar_VentaBoleto_Click.</param>
        private void btn_Agregar_VentaBoleto_Click(object sender, EventArgs e)
        {
            if (cb_TarjetaVentaBoleto.SelectedIndex!=-1&&cb_ItinerarioVentaBoleto.SelectedIndex!=-1&& nud_NumVentaBoleto.Text != "")
            {
                // Obtiene los IDs del itinerario y la tarjeta de pasajero seleccionados.
                string idIT = ListIDItinerario[cb_ItinerarioVentaBoleto.SelectedIndex].ToString();
                string idTP = listIDTrjetaPasajero[cb_TarjetaVentaBoleto.SelectedIndex].ToString();

                string query = chbx_EstadoPagoVentaBoleto.Checked ?
                        "INSERT INTO InfoPasajero.Venta (idTarjetaPasajero, idItinerario, MontoTotal, EstadoPago) VALUES (" + idTP + ", " + idIT + ", " + 0 + ", 1)" :
                        "INSERT INTO InfoPasajero.Venta (idTarjetaPasajero, idItinerario, MontoTotal, EstadoPago) VALUES (" + idTP + ", " + idIT + ", " + 0 + ", 0)";

                // Verifica si hay suficientes asientos disponibles y si los campos de IVA, Tasa de Seguro y Tasa de Servicio no están vacíos.
                if (VerificarDispAsientos(idIT, int.Parse(nud_NumVentaBoleto.Text)) && nud_Iva.Text != "" && tb_TasaSeg.Text != "" && tb_TasaServ.Text != "" )
                {
                    // Ejecuta la consulta para insertar la venta, CONSIGUIENDO SU ID.
                    int idVenta = Query2(query);
                    // Obtiene el costo base del vuelo.
                    string CostVuel = getCostoBaseVuelo(idIT);
                    // Verifica si la inserción de boletos fue exitosa.
                    if(idVenta!=-1)
                    {
                        // Inserta los boletos asociados a la venta de boleto.
                        if (!insertarBoletos(idVenta, idIT, idTP, CostVuel, int.Parse(nud_NumVentaBoleto.Text)))
                        {
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            MessageBox.Show(idVenta.ToString() + " " + CostVuel, "Error", buttons);
                        }
                    }else
                    {
                        string mensaje = "Ocurrió una excepción: \nNo se pudo insertar porque no es posible tener dos ventas\ndel mismo pasajero en el mismo itinerario";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show(mensaje, "Error", buttons);
                    }
                }
                else
                {
                    string mensaje = "Ocurrió una excepción: \nNo se pudo insertar porque no hay suficientes asientos disponibles";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Error", buttons);
                }

                ConsultaVenta();
                btn_BackVentaBoleto_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función "getCostoBaseVuelo" obtiene el costo base de un vuelo asociado a un itinerario.
        /// </summary>
        /// <param name="idIT">El parámetro "idIT" es una cadena que representa el ID del itinerario
        /// utilizado en la consulta SQL.</param>
        /// <returns>
        /// La función devuelve una cadena con el costo base del vuelo.
        /// </returns>
        private string getCostoBaseVuelo(string idIT)
        {
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                conexion.Open();
                string consulta = "SELECT V.CostoBase FROM InfoAerolinea.Itinerario I JOIN InfoAerolinea.Vuelo V ON I.idVuelo = V.idVuelo WHERE I.idItinerario = "+ idIT;
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    string CV = reader["CostoBase"].ToString();
                    reader.Close();
                    return CV;
                }
            }
        }

        /// <summary>
        /// La función "VerificarDispAsientos" verifica la disponibilidad de un número específico de asientos en un itinerario.
        /// </summary>
        /// <param name="idIT">El parámetro "idIT" es una cadena que representa el ID del itinerario
        /// utilizado en la consulta SQL.</param>
        /// <param name="numAsientos">El parámetro "numAsientos" es un entero que representa el número de asientos
        /// a verificar para disponibilidad.</param>
        /// <returns>
        /// La función devuelve un valor booleano indicando si hay disponibilidad de asientos.
        /// </returns>
        private bool VerificarDispAsientos(string idIT, int NumAsientos)
        {
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                conexion.Open();
                string consulta = "SELECT COUNT(*) FROM InfoPasajero.Asiento WHERE idItinerario = " + idIT + " AND Ocupado = 'False'";
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    int count = (int)cmd.ExecuteScalar();
                    if (count >= NumAsientos)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// La función "insertarBoletos" inserta boletos asociados a una venta de boleto en la base de datos.
        /// </summary>
        /// <param name="idVenta">El parámetro "idVenta" es un entero que representa el ID de la venta de boleto.</param>
        /// <param name="idIT">El parámetro "idIT" es una cadena que representa el ID del itinerario.</param>
        /// <param name="idTP">El parámetro "idTP" es una cadena que representa el ID de la tarjeta de pasajero.</param>
        /// <param name="costVuel">El parámetro "costVuel" es una cadena que representa el costo base del vuelo.</param>
        /// <param name="numBoletos">El parámetro "numBoletos" es un entero que representa el número de boletos a insertar.</param>
        /// <returns>
        /// La función devuelve un valor booleano indicando si la inserción de boletos fue exitosa.
        /// </returns>
        private bool insertarBoletos(int idVenta, string idIT, string idTP, string costVuel, int numboletos)
        {
            //Calculo de valores para impuestos y costo total
            float costV = float.Parse(costVuel);
            float Impuestos = costV * float.Parse("."+nud_Iva.Value.ToString());
            float TarAd = float.Parse(tb_TasaSeg.Text)+ float.Parse(tb_TasaServ.Text);
            float CostTot = costV + Impuestos + TarAd;

            //variables para la conexion a la BD
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "";
            SqlCommand Comm;
            SqlDataReader reader;
            string idPasajero = "-1";
            string idAsiento = "";

            query = "SELECT idPasajero FROM InfoPasajero.TarjetaPasajero WHERE idTarjetaPasajero = " + idTP;
            Comm = new SqlCommand(query, conexion);
            reader = Comm.ExecuteReader();
            reader.Read();
            idPasajero = reader["idPasajero"].ToString();
            reader.Close();

            //Ciclo para la insercion de n boletos
            for (int i = 0;i<numboletos;i++)
            {
                //obtencion de un asiento disponible
                query = "SELECT idAsiento, Ocupado FROM InfoPasajero.Asiento WHERE idItinerario = " + idIT;
                Comm = new SqlCommand(query, conexion);
                reader = Comm.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Ocupado"].ToString() == "False")
                    {
                        idAsiento = reader["idAsiento"].ToString();
                        break;
                    }
                }
                reader.Close();

                //inserción de el boleto
                query = "INSERT INTO InfoPasajero.Boleto (idVenta, idPasajero, idAsiento, Impuestos, TarifasAdicionales, CostoTotal, Estado) VALUES ("+ idVenta + ", NULL, " + idAsiento + ", "+Impuestos+", "+TarAd+", "+CostTot+", 1)";
                Comm = new SqlCommand(query, conexion);
                try
                {
                    Comm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("Ocurrió un error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                
                return false;
                }
            }
            conexion.Close();
            return true;
        }

        /// <summary>
        /// La función btn_Modificar_VentaBoleto_Click se utiliza para actualizar la información de 
        /// una Venta en la base de datos.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y normalmente se utiliza cuando no se necesitan datos adicionales para un
        /// evento.</param>
        private void btn_Modificar_VentaBoleto_Click(object sender, EventArgs e)
        {
            if (cb_TarjetaVentaBoleto.SelectedIndex != -1 && cb_ItinerarioVentaBoleto.SelectedIndex != -1 && nud_NumVentaBoleto.Text != "")
            {
                // Obtiene los IDs del itinerario y la tarjeta de pasajero seleccionados.
                string idIT = ListIDItinerario[cb_ItinerarioVentaBoleto.SelectedIndex].ToString();
                string idTP = listIDTrjetaPasajero[cb_TarjetaVentaBoleto.SelectedIndex].ToString();
                //cuenta los bletos con el id del registro seleccionado
                int numBoletos = CountBoletosCurrentID();

                //obtiene la cadena del itinerario de el registro seleccionado
                DataGridViewRow filaSeleccionada = dgv_Venta.SelectedRows[0];
                string itinerarioSelectedRow = filaSeleccionada.Cells["idItinerarioVB"].Value.ToString();

                // Verifica si hay suficientes asientos disponibles o si el itinerario no se va modificar .
                if (VerificarDispAsientos(idIT, numBoletos)|| itinerarioSelectedRow== cb_ItinerarioVentaBoleto.Text)
                {
                    string query = chbx_EstadoPagoVentaBoleto.Checked ?
                        "UPDATE InfoPasajero.Venta SET idTarjetaPasajero="+idTP+", idItinerario="+idIT+", MontoTotal=0, EstadoPago=1 WHERE idVenta="+currentID:
                        "UPDATE InfoPasajero.Venta SET idTarjetaPasajero=" + idTP + ", idItinerario=" + idIT + ", MontoTotal=0, EstadoPago=0 WHERE idVenta=" + currentID;

                    if (Query(query)!=-1)
                    {
                        //borra los boletos que tenia insertados la venta anteriormente
                        BorrarBoletos(currentID);
                        //reinicia el campo MontoTotal
                        query = "UPDATE InfoPasajero.Venta SET MontoTotal=0 WHERE idVenta=" + currentID;
                        Query(query);

                        // Obtiene el costo base del vuelo.
                        string CostVuel = getCostoBaseVuelo(idIT);

                        // Inserta los nuevos boletos con datos nuevos
                        if (!insertarBoletos(int.Parse(currentID), idIT, idTP, CostVuel, numBoletos))
                        {
                            MessageBoxButtons buttons = MessageBoxButtons.OK;
                            MessageBox.Show(idVenta.ToString() + " " + CostVuel, "Error", buttons);
                        }
                    }
                    else
                    {
                        string mensaje = "Ocurrió una excepción: \nNo se pudo actualizar";
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show(mensaje, "Error", buttons);
                    }

                    ConsultaVenta();
                    btn_BackVentaBoleto_Click(this, EventArgs.Empty);
                }
                else
                {
                    string mensaje = "Ocurrió una excepción: \nNo se pudo modificar porque no hay suficientes asientos disponibles";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Error", buttons);
                }
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función "BorrarBoletos" elimina los boletos asociados a una venta de boleto específica de la base de datos.
        /// </summary>
        /// <param name="currentID">El parámetro "currentID" es una cadena que representa el ID de la venta de boleto (registro seleccionado).</param>
        private void BorrarBoletos(string currentID)
        {
            try
            {
                SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
                conexion.Open();
                string query = "DELETE FROM InfoPasajero.Boleto WHERE idVenta = " + currentID;
                SqlCommand Comm = new SqlCommand(query, conexion);
                Comm.ExecuteNonQuery();
                conexion.Close();
            }
            catch (Exception ex)
            {
                // Mostrar un mensaje de error en una ventana de diálogo
                this.Invoke((MethodInvoker)delegate
                {
                    MessageBox.Show("Ocurrió un error al borrar boletos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            }
        }

        /// <summary>
        /// La función "CountBoletosCurrentID" cuenta el número de boletos asociados a una venta de boleto específica (registro seleccionado).
        /// </summary>
        /// <returns>
        /// La función devuelve un entero que representa el número de boletos asociados a la venta de boleto actual.
        /// </returns>
        private int CountBoletosCurrentID()
        {
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                conexion.Open();
                string consulta = "SELECT COUNT(*) FROM InfoPasajero.Boleto WHERE idVenta = " + currentID;
                using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                {
                    int count = (int)cmd.ExecuteScalar();
                    return count;
                }
            }
        }

        /// <summary>
        /// Esta función elimina un registro de Venta de la tabla de la base de datos 
        /// si se completan todos los campos obligatorios; de lo contrario, muestra un mensaje de error.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos de un evento.
        /// Se utiliza comúnmente como parámetro en los métodos del controlador de eventos para
        /// proporcionar información sobre el evento que ocurrió.</param>
        private void btn_Eliminar_VentaBoleto_Click(object sender, EventArgs e)
        {
            if (cb_TarjetaVentaBoleto.SelectedIndex != -1 && cb_ItinerarioVentaBoleto.SelectedIndex != -1 && nud_NumVentaBoleto.Text != "")
            {
                string query = "DELETE FROM InfoPasajero.Venta WHERE idVenta = " + currentID;
                if(Query(query)==-1)
                {
                    string mensaje = "Ocurrió una excepción\nAún existen boletos asociados a esa venta";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje", buttons);
                }
                ConsultaVenta();
                btn_BackVentaBoleto_Click(this, EventArgs.Empty);
            }
            else
            {
                string mensaje = "Ingresa correctamente los datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// La función btn_BackVentaBoleto_Click se utiliza para restablecer y borrar los campos de entrada y
        /// desactivar ciertos botones en la ventana Venta.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic (btn_BackVentaBoleto).</param>
        /// <param name="EventArgs">EventArgs es una clase que contiene datos de evento para un evento.
        /// Proporciona información sobre el evento que ocurrió, como el origen del evento y cualquier
        /// dato adicional asociado con el evento. En este caso, el parámetro EventArgs no se utiliza en
        /// el método.</param>
        private void btn_BackVentaBoleto_Click(object sender, EventArgs e)
        {
            lbl_Modifica1.Visible = false;
            btn_Agregar_VentaBoleto.Enabled = true;
            btn_Eliminar_VentaBoleto.Enabled = false;
            btn_Modificar_VentaBoleto.Enabled = false;
            btn_BackVentaBoleto.Visible = false;

            cb_ItinerarioVentaBoleto.SelectedIndex = -1;
            cb_TarjetaVentaBoleto.SelectedIndex = -1;
            nud_NumVentaBoleto.Text = "1";
            chbx_EstadoPagoVentaBoleto.Checked = false;
            nud_NumVentaBoleto.Visible = true;
            btn_datosboletoVentaBoleto.Visible = true;
            lbl_numbol.Visible = true;

            currentID = "";
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en una celda en un DataGridView, recupera los
        /// valores de la fila en la que se hizo clic y completa varios controles con esos valores.
        /// </summary>
        /// <param name="sender">El objeto que generó el evento, en este caso, el control
        /// DataGridView.</param>
        /// <param name="DataGridViewCellMouseEventArgs">El parámetro DataGridViewCellMouseEventArgs es
        /// un argumento de evento que contiene información sobre el evento de clic del mouse que
        /// ocurrió en una celda DataGridView. Proporciona propiedades como el índice de fila y el
        /// índice de columna de la celda en la que se hizo clic, así como el botón del mouse en el que
        /// se hizo clic.</param>
        private void dgv_Venta_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                lbl_Modifica1.Visible = true;
                btn_Agregar_VentaBoleto.Enabled = false;
                btn_Eliminar_VentaBoleto.Enabled = true;
                btn_Modificar_VentaBoleto.Enabled = true;
                btn_BackVentaBoleto.Visible = true;

                DataGridViewRow row = dgv_Venta.Rows[e.RowIndex];

                string txtVenta = row.Cells["idTarjetaPasajeroVB"].Value.ToString();
                for (int i = 0; i < cb_TarjetaVentaBoleto.Items.Count; i++)
                {
                    string item = cb_TarjetaVentaBoleto.Items[i].ToString();

                    if (item.Equals(txtVenta))
                    {
                        int indiceEncontrado = i;
                        cb_TarjetaVentaBoleto.SelectedIndex = indiceEncontrado;
                        break;
                    }
                }
                txtVenta = row.Cells["idItinerarioVB"].Value.ToString();
                for (int i = 0; i < cb_ItinerarioVentaBoleto.Items.Count; i++)
                {
                    string item = cb_ItinerarioVentaBoleto.Items[i].ToString();

                    if (item.Equals(txtVenta))
                    {
                        int indiceEncontrado = i;
                        cb_ItinerarioVentaBoleto.SelectedIndex = indiceEncontrado;
                        break;
                    }
                }

                nud_NumVentaBoleto.Visible = false;
                btn_datosboletoVentaBoleto.Visible = false;
                lbl_numbol.Visible = false;
                chbx_EstadoPagoVentaBoleto.Checked = (row.Cells["EstadoPago"].Value.ToString() == "True") ? true : false;

                currentID = row.Cells["idVenta"].Value.ToString();
            }
        }

        /// <summary>
        /// La función "LlenarCBVenta_Boleto" llena combobox con datos de la base de datos.
        /// </summary>
        public void LlenarCBVenta_Boleto()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();

            string query = "SELECT idItinerario, idVuelo, HoraSalida, FechaVuelo FROM InfoAerolinea.Itinerario";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();
            while (reader.Read())
            {
                string horaCompleta = reader["HoraSalida"].ToString();
                string horaSinSegundos = horaCompleta.Substring(0, 5);

                DateTime fecha = (DateTime)reader["FechaVuelo"];
                string nuevaFecha = fecha.ToString("MM-dd-yyyy");

                cb_ItinerarioVentaBoleto.Items.Add("(" + getVuelo(reader["idVuelo"].ToString(), true) + ") - " + horaSinSegundos + " - " + nuevaFecha);
                ListIDItinerario.Add(Convert.ToInt32(reader["idItinerario"].ToString()));
            }
            reader.Close();

            query = "SELECT idTarjetaPasajero, NombreTitular, NumTarjeta FROM InfoPasajero.TarjetaPasajero";
            Comm = new SqlCommand(query, conexion);
            reader = Comm.ExecuteReader();
            while (reader.Read())
            {
                string NumTarjeta = reader["NumTarjeta"].ToString();
                string ultimosTresDigitos = NumTarjeta.Substring(NumTarjeta.Length - 3);
                cb_TarjetaVentaBoleto.Items.Add(reader["NombreTitular"].ToString() + " - " + ultimosTresDigitos);
                listIDTrjetaPasajero.Add(Convert.ToInt32(reader["idTarjetaPasajero"].ToString()));
            }
            reader.Close();
        }

        /// <summary>
        /// La función "ConsultaVenta" recupera información de Venta de la base de datos y llena
        /// un DataGridView con los resultados.
        /// </summary>
        public void ConsultaVenta()
        {
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoPasajero.Venta";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();

            int i = 0;
            dgv_Venta.Rows.Clear();
            while (reader.Read())
            {
                DateTime fecha = (DateTime)reader["FechaVenta"];
                dgv_Venta.Rows.Add();
                dgv_Venta.Rows[i].Cells[0].Value = reader["idVenta"].ToString();
                dgv_Venta.Rows[i].Cells[1].Value = getTarjetaPasajero(reader["idTarjetaPasajero"].ToString());
                dgv_Venta.Rows[i].Cells[2].Value = getItinerario(reader["idItinerario"].ToString());
                dgv_Venta.Rows[i].Cells[3].Value = fecha.ToString("dd/MM/yyyy");
                dgv_Venta.Rows[i].Cells[4].Value = reader["MontoTotal"].ToString();
                dgv_Venta.Rows[i].Cells[5].Value = reader["EstadoPago"].ToString();
                i++;
            }
            reader.Close();
            conexion.Close();
        }

        /// <summary>
        /// La función "getTarjetaPasajero" obtiene información de una tarjeta de pasajero basada en su ID.
        /// </summary>
        /// <param name="idTP">El parámetro "idTP" es una cadena que representa el ID de la tarjeta de pasajero.</param>
        /// <returns>
        /// La función devuelve una cadena con información de la tarjeta de pasajero.
        /// </returns>
        private string getTarjetaPasajero(string idTP)
        {
            string TarjPas = "";
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT NombreTitular, NumTarjeta FROM InfoPasajero.TarjetaPasajero WHERE idTarjetaPasajero="+idTP;
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();
            reader.Read();
            string NumTarjeta = reader["NumTarjeta"].ToString();
            string ultimosTresDigitos = NumTarjeta.Substring(NumTarjeta.Length - 3);
            TarjPas = reader["NombreTitular"].ToString() + " - " + ultimosTresDigitos;
            reader.Close();
            conexion.Close();
            return TarjPas;
        }

        /// <summary>
        /// El evento btn_datosboletoVentaBoleto_Click se utiliza para mostrar o cerrar el panel de Impuestos y Tarifas Adicionales.
        /// </summary>
        /// <param name="sender">El parámetro "sender" es el objeto que generó el evento, en este caso, el botón.</param>
        /// <param name="e">El parámetro "e" proporciona datos para el evento, pero en este caso, no se utiliza.</param>
        private void btn_datosboletoVentaBoleto_Click(object sender, EventArgs e)
        {
            if(btn_datosboletoVentaBoleto.Text == "Impuestos y Tarifas Adicionales")
            {
                btn_datosboletoVentaBoleto.Text = "Cerrar";
                panel_IyTFABoleto.Visible = true;
            }else
            {
                btn_datosboletoVentaBoleto.Text = "Impuestos y Tarifas Adicionales";
                panel_IyTFABoleto.Visible =false;
            }
        }

        /// <summary>
        /// El evento tb_TasaSeg_KeyPress se utiliza para permitir solo dígitos y un máximo de 10 caracteres en el campo de Tasa de Seguridad.
        /// </summary>
        /// <param name="sender">El parámetro "sender" es el objeto que generó el evento, en este caso, el cuadro de texto.</param>
        /// <param name="e">El parámetro "e" proporciona datos para el evento, en este caso, el carácter que se presionó.</param>
        private void tb_TasaSeg_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (tb_TasaSeg.Text.Length >= 10 && e.KeyChar != (char)Keys.Back)
            {
                if (tb_TasaSeg.Text.Length < 10)
                {
                    e.Handled = false;
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// El evento tb_TasaServ_KeyPress se utiliza para permitir solo dígitos y un máximo de 10 caracteres en el campo de Tasa de Servicio.
        /// </summary>
        /// <param name="sender">El parámetro "sender" es el objeto que generó el evento, en este caso, el cuadro de texto.</param>
        /// <param name="e">El parámetro "e" proporciona datos para el evento, en este caso, el carácter que se presionó.</param>
        private void tb_TasaServ_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            if (tb_TasaServ.Text.Length >= 10 && e.KeyChar != (char)Keys.Back)
            {
                if (tb_TasaServ.Text.Length < 10)
                {
                    e.Handled = false;
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Boletos
        /// </summary>        

        /// <summary>
        /// La función "ConsultaBoleto" recupera información de Boleto de la base de datos y llena
        /// un DataGridView con los resultados.
        /// </summary>
        public void ConsultaBoleto()
        {
            listIDAsiento2.Clear();
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoPasajero.Boleto";
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();
            int i = 0;
            dgv_Boleto.Rows.Clear();
            while (reader.Read())
            {
                dgv_Boleto.Rows.Add();
                dgv_Boleto.Rows[i].Cells[0].Value = reader["idBoleto"].ToString();
                dgv_Boleto.Rows[i].Cells[1].Value = reader["idVenta"].ToString();
                if(reader["idPasajero"].ToString()=="")
                {
                    dgv_Boleto.Rows[i].Cells[2].Value = "Por Capturar";
                }else
                {
                    dgv_Boleto.Rows[i].Cells[2].Value = getPasajero(reader["idPasajero"].ToString(), true);
                    //dgv_Boleto.Rows[i].Cells[1].Value =reader["idPasajero"].ToString();
                }
                listIDAsiento2.Add(Convert.ToInt32(reader["idAsiento"].ToString()));
                dgv_Boleto.Rows[i].Cells[3].Value = getAsiento(reader["idAsiento"].ToString());
                dgv_Boleto.Rows[i].Cells[4].Value = reader["Impuestos"].ToString();
                dgv_Boleto.Rows[i].Cells[5].Value = reader["TarifasAdicionales"].ToString();
                dgv_Boleto.Rows[i].Cells[6].Value = reader["CostoTotal"].ToString();
                dgv_Boleto.Rows[i].Cells[7].Value = reader["Estado"].ToString();
                i++;
            }
            reader.Close();


            conexion.Close();
        }

        /// DEPRECADO
        private string getVenta(string val, bool flag)
        {
            string VentaString = "";
            using (SqlConnection conexion = new SqlConnection(server + database + "integrated security = true;"))
            {
                if (!flag)
                {
                    conexion.Open();
                    //separa el contenido del string
                    string[] partes = val.Split('-');
                    string fecha = partes[0].Trim();
                    string montototal = partes[1].Trim();
                    string consulta = "";

                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        VentaString = reader["idPasajero"].ToString();
                    }
                }
                else
                {
                    conexion.Open();
                    string consulta = "SELECT FechaVenta, MontoTotal FROM InfoPasajero.Venta WHERE idVenta=" + val;
                    using (SqlCommand cmd = new SqlCommand(consulta, conexion))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        reader.Read();
                        DateTime fecha = (DateTime)reader["FechaVenta"];
                        string fechaCompleta = fecha.ToString("dd/MM/yyyy HH:mm:ss");
                        
                        VentaString = fechaCompleta + " - " + reader["MontoTotal"].ToString();
                    }
                }
            }
            return VentaString;
        }

        /// <summary>
        /// La función "getAsiento" obtiene información de un Asiento basado en su ID.
        /// </summary>
        /// <param name="idTP">El parámetro "idasiento" es una cadena que representa el ID de el Asiento.</param>
        /// <returns>
        /// La función devuelve una cadena con información de el Asiento.
        /// </returns>
        private string getAsiento(string idasiento)
        {
            string asiento = "";
            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();
            string query = "SELECT * FROM InfoPasajero.Asiento WHERE idAsiento=" + idasiento;
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();
            reader.Read();
            string itinerario = getItinerario(reader["idItinerario"].ToString());
            Match match = Regex.Match(itinerario, @"\(([^)]*)\)");
            itinerario = match.Groups[1].Value;
            asiento = itinerario +" - "+ reader["Letra"].ToString() + "-" + reader["Num_Asiento"].ToString();
            reader.Close();
            conexion.Close();
            return asiento;
        }

        /// <summary>
        /// Esta función se activa cuando se hace clic en una celda en un DataGridView, recupera los
        /// valores de la fila en la que se hizo clic y completa varios controles con esos valores.
        /// </summary>
        /// <param name="sender">El objeto que generó el evento, en este caso, el control
        /// DataGridView.</param>
        /// <param name="DataGridViewCellMouseEventArgs">El parámetro DataGridViewCellMouseEventArgs es
        /// un argumento de evento que contiene información sobre el evento de clic del mouse que
        /// ocurrió en una celda DataGridView. Proporciona propiedades como el índice de fila y el
        /// índice de columna de la celda en la que se hizo clic, así como el botón del mouse en el que
        /// se hizo clic.</param>
        private void dgv_Boleto_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                cb_AsientoBoleto.Items.Clear();
                cb_PasajeroBoleto.Items.Clear();
                lbl_Modifica1.Visible = true;
                btn_Eliminar_Boleto.Enabled = true;
                btn_Modificar_Boleto.Enabled = true;
                cb_AsientoBoleto.Enabled = true;
                cb_PasajeroBoleto.Enabled = true;
                chckb_EstadoBoletos.Enabled = true;

                DataGridViewRow row = dgv_Boleto.Rows[e.RowIndex];
                chckb_EstadoBoletos.Checked = (row.Cells["Estado"].Value.ToString() == "True") ? true : false;

                string idAsiento = listIDAsiento2[e.RowIndex].ToString();

                currentID = row.Cells["idBoleto"].Value.ToString();
                LlenarCBBoletoAsientoPasajero(idAsiento);

                string txtPasaj = row.Cells["idPasajeroVB"].Value.ToString();
                for (int i = 0; i < cb_PasajeroBoleto.Items.Count; i++)
                {
                    string item = cb_PasajeroBoleto.Items[i].ToString();

                    if (item.Equals(txtPasaj))
                    {
                        int indiceEncontrado = i;
                        cb_PasajeroBoleto.SelectedIndex = indiceEncontrado;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// La función "LlenarCBBoletoAsientoPasajero" llena combobox con datos de la base de datos.
        /// En el combobox del asiento, solo llena los que esten disponibles
        /// </summary>
        private void LlenarCBBoletoAsientoPasajero(string Asiento)
        {
            listIDAsiento1.Clear();

            SqlConnection conexion = new SqlConnection(server + database + "integrated security = true");
            conexion.Open();

            string query = "SELECT idItinerario FROM InfoPasajero.Asiento WHERE idAsiento=" + Asiento;
            SqlCommand Comm = new SqlCommand(query, conexion);
            SqlDataReader reader = Comm.ExecuteReader();
            reader.Read();
            string itinerario = reader["idItinerario"].ToString();
            reader.Close();

            query = "SELECT * FROM InfoPasajero.Asiento WHERE idItinerario="+ itinerario + " AND Ocupado=0";
            Comm = new SqlCommand(query, conexion);
            reader = Comm.ExecuteReader();
            while (reader.Read())
            {
                itinerario = getItinerario(reader["idItinerario"].ToString());
                Match match = Regex.Match(itinerario, @"\(([^)]*)\)");
                itinerario = match.Groups[1].Value;

                cb_AsientoBoleto.Items.Add(itinerario + " - " + reader["Letra"].ToString()+"-"+ reader["Num_Asiento"].ToString());
                listIDAsiento1.Add(Convert.ToInt32(reader["idAsiento"].ToString()));
            }
            reader.Close();

            query = "SELECT Nom_Pasajero, NumPasaporte FROM InfoPasajero.Pasajero";
            Comm = new SqlCommand(query, conexion);
            reader = Comm.ExecuteReader();
            while (reader.Read())
            {
                string digitnumpass = reader["NumPasaporte"].ToString().Substring(reader["NumPasaporte"].ToString().Length - 4);
                cb_PasajeroBoleto.Items.Add(digitnumpass + " - " + reader["Nom_Pasajero"].ToString());
            }
            reader.Close();
            conexion.Close();
        }

        /// <summary>
        /// La función btn_Modificar_Boleto_Click se utiliza para actualizar la información de 
        /// un Boleto en la base de datos.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase que proporciona datos para un evento. No
        /// contiene datos y normalmente se utiliza cuando no se necesitan datos adicionales para un
        /// evento.</param>
        private void btn_Modificar_Boleto_Click(object sender, EventArgs e)
        {
            if(currentID!="")
            {
                string query = "";

                // Los siguientes if, comprueban casos en los que se quiera o no modificar
                // El asiento o el pasajero
                // y se dicide que query construir

                if(cb_AsientoBoleto.SelectedIndex != -1 && cb_PasajeroBoleto.SelectedIndex != -1)
                {
                    string idA = listIDAsiento1[cb_AsientoBoleto.SelectedIndex].ToString();
                    string idPas = getPasajero(cb_PasajeroBoleto.Text, false);
                    query = chckb_EstadoBoletos.Checked ?
                        "UPDATE InfoPasajero.Boleto SET idPasajero=" + idPas + ", idAsiento = " + idA + ", Estado = 1 WHERE idBoleto = " +currentID :
                        "UPDATE InfoPasajero.Boleto SET idPasajero=" + idPas + ", idAsiento = " + idA + ", Estado = 0 WHERE idBoleto = " + currentID;
                }

                if(cb_AsientoBoleto.SelectedIndex != -1 && cb_PasajeroBoleto.SelectedIndex == -1)
                {
                    string idA = listIDAsiento1[cb_AsientoBoleto.SelectedIndex].ToString();
                    query = chckb_EstadoBoletos.Checked ?
                            "UPDATE InfoPasajero.Boleto SET idAsiento = " + idA + ", Estado = 1 WHERE idBoleto = " + currentID :
                            "UPDATE InfoPasajero.Boleto SET idAsiento = " + idA + ", Estado = 0 WHERE idBoleto = " + currentID;

                }

                if(cb_AsientoBoleto.SelectedIndex == -1 && cb_PasajeroBoleto.SelectedIndex != -1)
                {
                    string idPas = getPasajero(cb_PasajeroBoleto.Text, false);
                    query = chckb_EstadoBoletos.Checked ?
                        "UPDATE InfoPasajero.Boleto SET idPasajero=" + idPas + ", Estado = 1 WHERE idBoleto = " + currentID :
                        "UPDATE InfoPasajero.Boleto SET idPasajero=" + idPas + ", Estado = 0 WHERE idBoleto = " + currentID;
                }

                if (cb_AsientoBoleto.SelectedIndex == -1 && cb_PasajeroBoleto.SelectedIndex == -1)
                {
                    query = chckb_EstadoBoletos.Checked ?
                            "UPDATE InfoPasajero.Boleto SET Estado = 1 WHERE idBoleto = " + currentID :
                            "UPDATE InfoPasajero.Boleto SET Estado = 0 WHERE idBoleto = " + currentID;
                }
                
                if (Query(query) == -1)
                {
                    string mensaje = "No se pudo modificar";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje", buttons);
                }
                ConsultaBoleto();
                //regresa la ventana a un estado inicial
                lbl_Modifica1.Visible = false;
                btn_Eliminar_Boleto.Enabled = false;
                btn_Modificar_Boleto.Enabled = false;
                cb_AsientoBoleto.Enabled = false;
                chckb_EstadoBoletos.Enabled = false;
                cb_AsientoBoleto.Items.Clear();
                cb_PasajeroBoleto.Items.Clear();
                currentID = "";
            }
            else
            {
                string mensaje = "Selecciona un registro y sus nuevos datos";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }

        /// <summary>
        /// Esta función elimina un registro de Boleto de la tabla de la base de datos 
        /// si se completan todos los campos obligatorios; de lo contrario, muestra un mensaje de error.
        /// </summary>
        /// <param name="sender">El parámetro del sender es el objeto que generó el evento. En este
        /// caso, es el botón en el que se hizo clic para activar el evento.</param>
        /// <param name="EventArgs">EventArgs es una clase en C# que representa los datos de un evento.
        /// Se utiliza comúnmente como parámetro en los métodos del controlador de eventos para
        /// proporcionar información sobre el evento que ocurrió.</param>
        private void btn_Eliminar_Boleto_Click(object sender, EventArgs e)
        {
            if (currentID != "")
            {
                string[] IDs = currentID.Split('#');
                string query = "DELETE FROM InfoPasajero.Boleto WHERE idBoleto = " + currentID;
                if(Query(query)==-1)
                {
                    string mensaje = "No se pudo eliminar";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(mensaje, "Mensaje", buttons);
                }
                ConsultaBoleto();
                //regresa la ventana a un estado inicial
                lbl_Modifica1.Visible = false;
                btn_Eliminar_Boleto.Enabled = false;
                btn_Modificar_Boleto.Enabled = false;
                cb_AsientoBoleto.Enabled = false;
                chckb_EstadoBoletos.Enabled = false;
                cb_AsientoBoleto.Items.Clear();
                cb_PasajeroBoleto.Items.Clear();
                currentID = "";
            }
            else
            {
                string mensaje = "Selecciona un registro";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(mensaje, "Mensaje", buttons);
            }
        }


        #endregion

        #endregion
    }
}
