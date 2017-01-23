<%@ Page Title="Ejemplo" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OMRapl.aspx.cs" Inherits="Galileo.OMRapl" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="row">
        <div class="col-md-4">
        <h2>Evalue nuestra aplicación</h2>
                <ul style="list-style-type: square">
                <li style="text-align: justify">Revise el ejemplo de encuesta</li>
                <li style="text-align: justify">Descargue e imprima el formato</li>
                <li style="text-align: justify">Responda a las preguntas</li>
                <li style="text-align: justify">Obtenga una imagen adecuada visite Ayuda</li>
                <li style="text-align: justify">Escane o fotografie el formato</li>
                <li style="text-align: justify">Revise se observen las siguientes imagenes en las cuatro esquinas de la hoja digitalizada.
        <asp:Image ID="Image5" runat="server" Height="4%" ImageUrl="~/omrtemp/LC Prints.jpg" Width="4%" /> - 
        <asp:Image ID="Image7" runat="server" Height="4%" ImageUrl="~/omrtemp/RC Prints.jpg" Width="4%" />
                    </li>
                <li style="text-align: justify">Trasladese a Registro</li>
                <li style="text-align: justify">Teclee nombre y correo electrónico</li>
                <li style="text-align: justify">Elija la imagen a procesar</li>
                <li style="text-align: justify">De clic en el botón</li>
                <li style="text-align: justify">Espere los resultados</li>
                <li style="text-align: justify">Elija la opcion Resultado</li>
                <li style="text-align: justify">Observe que se guardaron sus respuestas</li>
            </ul>
        </div>
        <div class="col-md-4">
            <h2>Ejemplo de encuesta</h2>
            <p>
                <asp:HyperLink ID="HyperLink1" runat="server" ImageUrl="~/Imagen/Btnd5057.jpg" NavigateUrl="~/descargar/d5057.jpg" Target="_blank">HyperLink</asp:HyperLink>
            </p>
            <p>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Descargue el formato</h2>
            <p>
            <asp:ImageButton ID="ImageButton1" runat="server" ToolTip="Descarga de formato" ImageUrl="~/Imagen/DURANIUM HARDDRIVE DOWNLOADS BLUE.png" OnClick="Descarga_Click" />
            </p>
            <p>
            </p>
        </div>
    </div>
    <p><a href="http://galileo.ppenaw.com/ayuda.aspx " Target="_blank" class="btn btn-primary btn-large">Mas detalles &raquo;</a></p>

</asp:Content>
