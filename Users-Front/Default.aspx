<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true"  Async="true" CodeBehind="Default.aspx.cs" Inherits="Users_Front._Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Gestión de Usuarios</title>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.1.3/css/bootstrap.min.css" />
    <style>
        .container {
            padding-top: 20px;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .user-list {
            margin-top: 30px;
        }

        .pagination {
            margin-top: 20px;
        }
    </style>
</head>
<body>
    <div class="container">
        <h1>Gestión de Usuarios</h1>

        <form id="formUsuarios" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>

            <div class="row">
                <div class="col-md-5">
                    <div class="card">
                        <div class="card-header">
                            <h3>Registro de Usuario</h3>
                        </div>
                        <div class="card-body">
                            <asp:HiddenField ID="hdnUserId" runat="server" Value="0" />

                            <div class="form-group">
                                <label for="txtNombre">Nombre:</label>
                                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" placeholder="Ingrese nombre"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvNombre" runat="server" ControlToValidate="txtNombre"
                                    ErrorMessage="El nombre es requerido" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>

                            <div class="form-group">
                                <label for="txtCorreo">Correo:</label>
                                <asp:TextBox ID="txtCorreo" runat="server" CssClass="form-control" placeholder="correo@ejemplo.com" TextMode="Email"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvCorreo" runat="server" ControlToValidate="txtCorreo"
                                    ErrorMessage="El correo es requerido" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revCorreo" runat="server" ControlToValidate="txtCorreo"
                                    ErrorMessage="Ingrese un correo válido" CssClass="text-danger" Display="Dynamic"
                                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                            </div>

                            <div class="form-group">
                                <label for="txtContraseña">Contraseña:</label>
                                <asp:TextBox ID="txtContraseña" runat="server" CssClass="form-control" TextMode="Password" placeholder="Ingrese contraseña"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvContraseña" runat="server" ControlToValidate="txtContraseña"
                                    ErrorMessage="La contraseña es requerida" CssClass="text-danger" Display="Dynamic"></asp:RequiredFieldValidator>
                            </div>

                            <div class="mt-3">
                                <asp:Button ID="btnGuardar" runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="btnGuardar_Click" />
                                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClick="btnCancelar_Click" CausesValidation="false" />
                            </div>

                            <asp:Label ID="lblMensaje" runat="server" CssClass="mt-3 d-block"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="col-md-7">
                    <div class="card">
                        <div class="card-header d-flex justify-content-between">
                            <h3>Lista de Usuarios</h3>
                            <div>
                                <button type="button" class="btn btn-outline-primary btn-sm" runat="server" id="btnLogin" data-bs-toggle="modal" data-bs-target="#loginModal">
    Iniciar Sesión
</button>
                                <asp:Button ID="btnLogout" runat="server" Text="Cerrar Sesión" CssClass="btn btn-outline-danger btn-sm" OnClick="btnLogout_Click" Visible="false" CausesValidation="false" />
                            </div>
                        </div>
                        <div class="card-body">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="False"
                                        CssClass="table table-striped table-hover" DataKeyNames="Id"
                                        OnRowCommand="gvUsuarios_RowCommand" EmptyDataText="No hay usuarios registrados">
                                        <Columns>
                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                            <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                                            <asp:BoundField DataField="Correo" HeaderText="Correo" />
                                            <asp:TemplateField HeaderText="Acciones">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditar" runat="server" CommandName="EditarUsuario" CommandArgument='<%# Eval("Id") %>'
                                                        CssClass="btn btn-sm btn-warning" CausesValidation="false">
                                                        <i class="bi bi-pencil"></i> Editar
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="btnEliminar" runat="server" CommandName="EliminarUsuario" CommandArgument='<%# Eval("Id") %>'
                                                        CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('¿Está seguro de eliminar este usuario?');" CausesValidation="false">
                                                        <i class="bi bi-trash"></i> Eliminar
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>

                                    <div class="d-flex justify-content-between align-items-center">
                                        <div>
                                            Mostrando 
                                            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="form-select form-select-sm d-inline-block" Width="60px">
                                                <asp:ListItem Text="5" Value="5" />
                                                <asp:ListItem Text="10" Value="10" Selected="True" />
                                                <asp:ListItem Text="20" Value="20" />
                                            </asp:DropDownList>
                                            registros por página
                                        </div>

                                        <div class="pagination">
                                            <asp:LinkButton ID="lnkPrevious" runat="server" OnClick="lnkPrevious_Click" CssClass="btn btn-outline-primary btn-sm" CausesValidation="false">
                                                <i class="bi bi-chevron-left"></i> Anterior
                                            </asp:LinkButton>
                                            <asp:Label ID="lblPaginaActual" runat="server" CssClass="mx-2">Página 1 de 1</asp:Label>
                                            <asp:LinkButton ID="lnkNext" runat="server" OnClick="lnkNext_Click" CssClass="btn btn-outline-primary btn-sm" CausesValidation="false">
                                                Siguiente <i class="bi bi-chevron-right"></i>
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Modal para iniciar sesión -->
            <div class="modal fade" id="loginModal" tabindex="-1" aria-labelledby="loginModalLabel" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="loginModalLabel">Iniciar Sesión</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            <div class="form-group">
                                <label for="txtLoginCorreo">Correo:</label>
                                <asp:TextBox ID="txtLoginCorreo" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label for="txtLoginContraseña">Contraseña:</label>
                                <asp:TextBox ID="txtLoginContraseña" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
                            </div>
                            <asp:Label ID="lblLoginMensaje" runat="server" CssClass="text-danger"></asp:Label>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cerrar</button>
                            <asp:Button ID="btnLoginSubmit" runat="server" Text="Iniciar Sesión" CssClass="btn btn-primary" OnClick="btnLoginSubmit_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </form>
    </div>

    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap/5.1.3/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-icons/1.8.1/font/bootstrap-icons.min.css"></script>

    <script type="text/javascript">
        // Script para manejar modal y operaciones AJAX
        var loginModal;

        function pageLoad() {
            loginModal = new bootstrap.Modal(document.getElementById('loginModal'));

            // Verificar si hay un token almacenado al cargar la página
            var token = localStorage.getItem('userToken');
            if (token) {
                updateUIWithToken(token);
            }
        }

        function showLoginModal() {
            loginModal.show();
        }

        function hideLoginModal() {
            loginModal.hide();
        }

        function updateUIWithToken(token) {
            // Actualizar la interfaz cuando el usuario está autenticado
            document.getElementById('<%= btnLogin.ClientID %>').style.display = 'none';
            document.getElementById('<%= btnLogout.ClientID %>').style.display = 'inline-block';

            // Guardar el token en localStorage
            localStorage.setItem('userToken', token);
        }

        function clearToken() {
            // Eliminar el token y actualizar la interfaz
            localStorage.removeItem('userToken');
            document.getElementById('<%= btnLogin.ClientID %>').style.display = 'inline-block';
            document.getElementById('<%= btnLogout.ClientID %>').style.display = 'none';
        }
    </script>
</body>
</html>
