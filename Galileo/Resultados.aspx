<%@ Page Title="Resultado" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Resultados.aspx.cs" Inherits="Galileo.Resultados" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="row">
        <div class="col-md-3">
        <h2>Resultados de la Encuesta</h2>
            <p>Encuestado:<asp:DropDownList ID="ddlEncuestado" runat="server" AutoPostBack="True" DataSourceID="dsEncuestadovw" DataTextField="descripcion" DataValueField="Encuestadokey" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:SqlDataSource ID="dsEncuestadovw" runat="server" ConnectionString="<%$ ConnectionStrings:sqlconn %>" SelectCommand="SELECT [Encuestadokey], [descripcion] FROM [EncuestadocmbWebprueba]"></asp:SqlDataSource>
            </p>
            <p>
                <asp:Label ID="lbl1" runat="server" Text="Nombre:" Visible="False"></asp:Label>
&nbsp;<asp:TextBox ID="txtDetalle" runat="server" Visible="False" Width="50%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDetalle" ErrorMessage="Campo Requerido" ForeColor="Red" SetFocusOnError="True">Informaciòn requerida.</asp:RequiredFieldValidator>
            </p>
            <p>&nbsp;<asp:Label ID="lbl2" runat="server" Text="Correo:" Visible="False"></asp:Label>
&nbsp;<asp:TextBox ID="txtEmail" runat="server" TextMode="Email" ValidateRequestMode="Enabled" Visible="False" Width="50%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtEmail" ErrorMessage="Campo Requerido" ForeColor="Red" SetFocusOnError="True" ValidateRequestMode="Enabled">Informaciòn requerida.</asp:RequiredFieldValidator>
            </p>
            <p style="text-align: center">
                <asp:Label ID="lbl3" runat="server" Text="Agregar"></asp:Label>
                <asp:ImageButton ID="ibNuevo" runat="server" ImageUrl="~/Imagen/DURANIUM HARDDRIVE PLAINnuevo.png" OnClick="ibNuevo_Click" ToolTip="Agregar" />
                <asp:Label ID="lbl4" runat="server" Text="Guardar" Visible="False"></asp:Label>
                <asp:ImageButton ID="ibGuardar" runat="server" ImageUrl="~/Imagen/DURANIUM HADRDRIVE INTERNET 2.png" OnClick="ibGuardar_Click" Visible="False" ToolTip="Guardar" />
            </p>
            <p>
                <strong style="text-align: center">Para registrar un nuevo cuestionario de clic en el botón agregar, responda la encuesta, Posteriormente de clic al botón guardar.<br />
                Es necesario que ingrese su nombre y correo electrónico.</strong></p>
            <p>
                <asp:Label ID="lblW" runat="server" ForeColor="Red" Visible="False"></asp:Label>
            </p>
        </div>
        <div class="col-md-8">
            <h2>Preguntas</h2>
            <p>
                1. ¿Cuál es el número de cuartos, piezas o habitaciones, con que cuenta tu hogar?(no incluir baños, pasillos, patios, etc.)
                 <asp:RadioButtonList ID="r1" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem>0</asp:ListItem>
                    <asp:ListItem>1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem>3</asp:ListItem>
                    <asp:ListItem>4</asp:ListItem>
                    <asp:ListItem>5</asp:ListItem>
                    <asp:ListItem>6</asp:ListItem>
                    <asp:ListItem>7 o más</asp:ListItem>
                </asp:RadioButtonList>
            </p>
            <p>
                2. ¿Cuántos baños completos hay para los integrantes de la casa?
                <asp:RadioButtonList ID="r2" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem>0</asp:ListItem>
                    <asp:ListItem>1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem>3</asp:ListItem>
                    <asp:ListItem Value="4">4 o más</asp:ListItem>
                </asp:RadioButtonList>
            </p>
            <p>
                3. ¿En tu hogar cuentas con regaderas funcionando en alguno de los baños?
                <asp:RadioButtonList ID="r3" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Value="0">Si tiene</asp:ListItem>
                    <asp:ListItem Value="1">No tiene</asp:ListItem>
                </asp:RadioButtonList>
            </p>
            <p>
                4. Contando todos los focos que utilizas para iluminar tu hogar (interiores y exteriores)¿Cuántos focos tienes en tu hogar?
                <asp:RadioButtonList ID="r4" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Selected="True" Value="0">0-5</asp:ListItem>
                    <asp:ListItem Value="1">6-10</asp:ListItem>
                    <asp:ListItem Value="2">11-15</asp:ListItem>
                    <asp:ListItem Value="3">16-20</asp:ListItem>
                    <asp:ListItem Value="4">21 o más</asp:ListItem>
                </asp:RadioButtonList>
            </p>
            <p>
                5. ¿El piso de tu casa es predominantemente de tierra o de cemento o algún otro tipo de material? <asp:RadioButtonList ID="r5" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Selected="True" Value="0">Tierra o cemento</asp:ListItem>
                    <asp:ListItem Value="1">Otro acabado</asp:ListItem>
                </asp:RadioButtonList>
            </p>
            <p>
                6. ¿Cuántos automóviles tienes en tu hogar?
                <asp:RadioButtonList ID="r6" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem>0</asp:ListItem>
                    <asp:ListItem>1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem>3 o más</asp:ListItem>
                </asp:RadioButtonList>
            </p>
            <p>
                7. ¿En tu hogar tienen estufa de gas o eléctrica?
                <asp:RadioButtonList ID="r7" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Value="0">Si tiene</asp:ListItem>
                    <asp:ListItem Value="1">No tiene</asp:ListItem>
                </asp:RadioButtonList>
            </p>
            <p>
                8. Pensando en la persona que aporta la mayor parte del ingreso en tu hogar ¿Cuál es su escolaridad?<br />
            <asp:DropDownList ID="ddlr9" runat="server" style="font-weight: bold" OnSelectedIndexChanged="ddlr9_SelectedIndexChanged">
                <asp:ListItem Value="0">No estudió</asp:ListItem>
                <asp:ListItem Value="1">Primaria Incompleta</asp:ListItem>
                <asp:ListItem Value="2">Primaria completa</asp:ListItem>
                <asp:ListItem Value="3">Secundaria incompleta</asp:ListItem>
                <asp:ListItem Value="4">Secundaria completa</asp:ListItem>
                <asp:ListItem Value="5">Carrera comercial</asp:ListItem>
                <asp:ListItem Value="6">Carrera técnica</asp:ListItem>
                <asp:ListItem Selected="True" Value="7">Preparatoria incompleta</asp:ListItem>
                <asp:ListItem Value="8">Preparatoria completa</asp:ListItem>
                <asp:ListItem Value="9">Licenciatura incompleta</asp:ListItem>
                <asp:ListItem Value="10">Licenciatura completa</asp:ListItem>
                <asp:ListItem Value="11">Diplomado</asp:ListItem>
                <asp:ListItem Value="12">Maestría</asp:ListItem>
                <asp:ListItem Value="13">Doctorado</asp:ListItem>
            </asp:DropDownList>
            </p>
            <p>
                9. ¿Qué edad tienes? <br />
                <asp:TextBox ID="txtEdad" runat="server" style="font-weight: bold"></asp:TextBox>
            </p>
            <p style="text-align: center">
                <asp:Image ID="Image1" runat="server" Height="66%" Visible="False" Width="66%" />
            </p>

        </div>
    </div>
    
    
    </asp:Content>
