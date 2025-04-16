using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.UI;
using Newtonsoft.Json;

namespace Users_Front
{
    public partial class _Default : Page
    {
        private static readonly string apiBaseUrl = "https://localhost:7208/api/usuarios";
        private static readonly HttpClient httpClient = new HttpClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["CurrentPage"] = 1;
                ViewState["PageSize"] = 10;
                CargarUsuarios();
            }
        }

        private async void CargarUsuarios()
        {
            try
            {
                int page = (int)ViewState["CurrentPage"];
                int pageSize = (int)ViewState["PageSize"];

                HttpResponseMessage response = await httpClient.GetAsync($"{apiBaseUrl}?page={page}&pageSize={pageSize}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(jsonData);
                    gvUsuarios.DataSource = usuarios;
                    gvUsuarios.DataBind();
                }
                else
                {
                    lblMensaje.Text = "Error al cargar usuarios.";
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
            }
        }

        protected async void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string correo = txtCorreo.Text.Trim();

                // Validar si ya existe un usuario con ese correo
                HttpResponseMessage checkResponse = await httpClient.GetAsync($"{apiBaseUrl}?correo={correo}");
                if (checkResponse.IsSuccessStatusCode)
                {
                    var jsonCheck = await checkResponse.Content.ReadAsStringAsync();
                    var usuariosExistentes = JsonConvert.DeserializeObject<List<Usuario>>(jsonCheck);

                    if (usuariosExistentes.Count > 0)
                    {
                        lblMensaje.Text = "El correo ya está registrado.";
                        return;
                    }
                }

                var usuario = new Usuario
                {
                    Nombre = txtNombre.Text.Trim(),
                    Correo = correo,
                    Password = txtContraseña.Text
                };

                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiBaseUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    lblMensaje.Text = "Usuario creado con éxito.";
                    LimpiarFormulario();
                    CargarUsuarios();
                }
                else
                {
                    lblMensaje.Text = "Error al crear usuario.";
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
            }
        }

        protected async void gvUsuarios_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            int userId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditarUsuario")
            {
                HttpResponseMessage response = await httpClient.GetAsync($"{apiBaseUrl}/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<Usuario>(json);

                    hdnUserId.Value = user.Id.ToString();
                    txtNombre.Text = user.Nombre;
                    txtCorreo.Text = user.Correo;
                    txtContraseña.Text = user.Password;
                }
            }
            else if (e.CommandName == "EliminarUsuario")
            {
                HttpResponseMessage response = await httpClient.DeleteAsync($"{apiBaseUrl}/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    lblMensaje.Text = "Usuario eliminado.";
                    CargarUsuarios();
                }
                else
                {
                    lblMensaje.Text = "Error al eliminar usuario.";
                }
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            txtNombre.Text = "";
            txtCorreo.Text = "";
            txtContraseña.Text = "";
            hdnUserId.Value = "0";
            lblMensaje.Text = "";
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["PageSize"] = Convert.ToInt32(ddlPageSize.SelectedValue);
            ViewState["CurrentPage"] = 1;
            CargarUsuarios();
        }

        protected void lnkPrevious_Click(object sender, EventArgs e)
        {
            int currentPage = (int)ViewState["CurrentPage"];
            if (currentPage > 1)
            {
                ViewState["CurrentPage"] = currentPage - 1;
                CargarUsuarios();
            }
        }

        protected void lnkNext_Click(object sender, EventArgs e)
        {
            ViewState["CurrentPage"] = (int)ViewState["CurrentPage"] + 1;
            CargarUsuarios();
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Mostrar modal de login con JS (ya lo tenés implementado en el script JS del .aspx)
            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarModal", "showLoginModal();", true);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // "Cerrar sesión": limpiar token o simplemente actualizar UI
            ScriptManager.RegisterStartupScript(this, GetType(), "limpiarToken", "clearToken();", true);
        }

        protected void btnLoginSubmit_Click(object sender, EventArgs e)
        {
            string correo = txtLoginCorreo.Text.Trim();
            string contraseña = txtLoginContraseña.Text.Trim();

            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contraseña))
            {
                lblLoginMensaje.Text = "Debe ingresar correo y contraseña.";
                return;
            }

            try
            {
                var loginData = new
                {
                    Correo = correo,
                    Contraseña = contraseña
                };

                var json = new JavaScriptSerializer().Serialize(loginData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (HttpClient client = new HttpClient())
                {
                    // Reemplaza esta URL con la correcta de tu API
                    string apiUrl = "https://localhost:7208/api/Auth/login";
                    var response = client.PostAsync(apiUrl, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = response.Content.ReadAsStringAsync().Result;
                        // Suponemos que el token viene en un campo "token"
                        dynamic result = new JavaScriptSerializer().Deserialize<object>(responseBody);
                        string token = result["token"];

                        // Pasamos el token a JavaScript para guardarlo en localStorage
                        ScriptManager.RegisterStartupScript(this, GetType(), "guardarToken",
                            $"updateUIWithToken('{token}'); hideLoginModal();", true);
                    }
                    else
                    {
                        lblLoginMensaje.Text = "Credenciales inválidas. Verifique e intente nuevamente.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblLoginMensaje.Text = "Ocurrió un error al intentar iniciar sesión.";
                // Log: ex.Message
            }
        }

    }



    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
    }
}
