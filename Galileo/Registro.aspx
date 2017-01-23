<%@ Page Title="Registro" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="Galileo.Registro" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <div class="row">
        <div class="col-md-3">
        <h2>Registro de tabulaciones</h2>
            <p>Nombre:
                <asp:TextBox ID="txtNombre" runat="server" MaxLength="80" Width="50%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtNombre" ErrorMessage="campo requerido" ForeColor="Red">Información requerida</asp:RequiredFieldValidator>
            </p>
            <p>Correo:
                <asp:TextBox ID="txtCorreo" runat="server" TextMode="Email" Width="50%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCorreo" runat="server" ControlToValidate="txtCorreo" ErrorMessage="campo requerido" ForeColor="Red" SetFocusOnError="True" ValidateRequestMode="Enabled">Información requerida</asp:RequiredFieldValidator>
            </p>
            <p style="text-align: center">
                <asp:FileUpload ID="FUpEncuesta" runat="server" EnableTheming="True" ToolTip="Elija una imagen" />
                <asp:RequiredFieldValidator ID="rfvNombre0" runat="server" ControlToValidate="FUpEncuesta" ErrorMessage="campo requerido" ForeColor="Red">Imagen requerida</asp:RequiredFieldValidator>
            </p>
            <p style="text-align: center">
                <asp:ImageButton ID="imgAgregar" runat="server" ImageUrl="~/Imagen/DURANIUM HADRDRIVE INTERNET 2.png" OnClick="imgAgregar_Click" ToolTip="Agregar resultados" />
            </p>
                <asp:Label ID="Label1" runat="server" Font-Names="Arial" Font-Size="Large" Font-Underline="True"></asp:Label>
        </div>
        <div class="col-md-8">
            <h2>Preguntas</h2>
            <p>
                1. ¿Cuál es el número de cuartos, piezas o habitaciones, con que cuenta tu hogar?(no incluir baños, pasillos, patios, etc.)
                <asp:CheckBoxList ID="CheckBoxList1" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="2">
                    <asp:ListItem>0</asp:ListItem>
                    <asp:ListItem>1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem>3</asp:ListItem>
                    <asp:ListItem>4</asp:ListItem>
                    <asp:ListItem>5</asp:ListItem>
                    <asp:ListItem>6</asp:ListItem>
                    <asp:ListItem Value="7">7 o más</asp:ListItem>
                </asp:CheckBoxList>
            </p>
            <p>
                2. ¿Cuántos baños completos hay para los integrantes de la casa?
               <asp:CheckBoxList ID="CheckBoxList2" runat="server" Height="25px" RepeatDirection="Horizontal" Width="194px" CellPadding="2" CellSpacing="2">
                    <asp:ListItem>0</asp:ListItem>
                    <asp:ListItem>1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem>3</asp:ListItem>
                    <asp:ListItem Value="4">4 o más</asp:ListItem>
                </asp:CheckBoxList>
            </p>
            <p>
                3. ¿En tu hogar cuentas con regaderas funcionando en alguno de los baños?
                <asp:CheckBoxList ID="CheckBoxList3" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="2">
                    <asp:ListItem Value="0">Si tiene</asp:ListItem>
                    <asp:ListItem Value="1">No tiene</asp:ListItem>
                </asp:CheckBoxList>
            </p>
            <p>
                4. Contando todos los focos que utilizas para iluminar tu hogar (interiores y exteriores)¿Cuántos focos tienes en tu hogar?
                <asp:CheckBoxList ID="CheckBoxList4" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="2">
                    <asp:ListItem Value="0">0 - 5</asp:ListItem>
                    <asp:ListItem Value="1">6 -10</asp:ListItem>
                    <asp:ListItem Value="2">11-15</asp:ListItem>
                    <asp:ListItem Value="3">16 -20</asp:ListItem>
                    <asp:ListItem Value="4">21 o más</asp:ListItem>
                </asp:CheckBoxList>
            </p>
            <p>
                5. ¿El piso de tu casa es predominantemente de tierra o de cemento o algún otro tipo de material? <asp:CheckBoxList ID="CheckBoxList5" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="2">
                    <asp:ListItem Value="0">Tierra o cemento</asp:ListItem>
                    <asp:ListItem Value="0">Otro acabado</asp:ListItem>
                </asp:CheckBoxList>
            </p>
            <p>
                6. ¿Cuántos automóviles tienes en tu hogar?
                <asp:CheckBoxList ID="CheckBoxList6" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="2">
                    <asp:ListItem>0</asp:ListItem>
                    <asp:ListItem>1</asp:ListItem>
                    <asp:ListItem>2</asp:ListItem>
                    <asp:ListItem Value="3">3 o más</asp:ListItem>
                </asp:CheckBoxList>
            </p>
            <p>
                7. ¿En tu hogar tienen estufa de gas o eléctrica?
                <asp:CheckBoxList ID="CheckBoxList7" runat="server" RepeatDirection="Horizontal" CellPadding="2" CellSpacing="2">
                    <asp:ListItem Value="0">Si tiene</asp:ListItem>
                    <asp:ListItem Value="1">No tiene</asp:ListItem>
                </asp:CheckBoxList>
            </p>
            <p>
                8. Pensando en la persona que aporta la mayor parte del ingreso en tu hogar ¿Cuál es su escolaridad?<br />
                <asp:DropDownList ID="DropDownList1" runat="server">
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
                <asp:TextBox ID="TextBox1" runat="server" ReadOnly="True" TextMode="Number"></asp:TextBox>
            </p>
        </div>
    </div>

</asp:Content>
