<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Fotografia.aspx.cs" Inherits="Galileo.Fotografia" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><span>Guía para obtener mejores imágenes</span></h2>
    <p class="lead" style="text-align: justify">Lo requisitos mínimos para que una hoja se interpretada correctamente en nuestro OMR son:
        <br />Imágenes en formato jpg, más de 300 PPP, orientación horizontal y tipo de imagen en blanco y negro.<br /> 
        Se observen las siguientes imagenes en las esquinas de la hoja digitalizada.
        <asp:Image ID="Image5" runat="server" Height="2%" ImageUrl="~/omrtemp/LC Prints.jpg" Width="2%" /> - 
        <asp:Image ID="Image7" runat="server" Height="2%" ImageUrl="~/omrtemp/RC Prints.jpg" Width="2%" />
    </p>

    <div class="row">
        <div class="col-md-3">
            <h2>Imprima el formato</h2>
            <p>
                En paint configure la página
            </p>
            <p>
                
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Imagen/Paint.png" />
                
            </p>
        </div>
        <div class="col-md-3">
            <h2>Desde un escaner</h2>
            <p style="text-align: justify">
                Asegurese de ajustar en su escaner<br /> la siguiente configuración
            </p>
            <p>
                
                <asp:Image ID="Image4" runat="server" ImageUrl="~/Imagen/escaner.png" />
                
            </p>
        </div>
        <div class="col-md-3">
            <h2>Movil con Android</h2>
            <p style="text-align: justify">
                  Recomendamos obtenga PDF Scanner©, <br />
                  define la calidad en función del tamaño y <br />
                  la envía por correo, cuenta con excelente <br />
                  panel de opciones para configuración.<br />© GRYMALA
            </p>
            <p>
                
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Imagen/PDFScanner.png" />
                
            </p>
        </div>
        <div class="col-md-3">
            <h2>Movil con iOS</h2>
            <p>
                 Recomendamos obtenga app iScanner©,<br />
                 existe una versión gratuita, esta le permite <br />
                 delimitar la imagen, registrarla en formato jpg
                 y enviarla por correo.
                <br />© 2016 BP Mobile, LLC.
                <br />
            </p>
           <p>
                <asp:Image ID="Image3" runat="server" ImageUrl="~/Imagen/iscanner.png" />
            </p>
        </div>
    </div>
    <p style="font-size: large; text-align: justify;">Observe nuestros videos:&nbsp;
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://www.youtube.com/watch?v=1yN9o6moSD0" Target="_blank">Como llenar el cuestionario</asp:HyperLink>
&nbsp;y
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="https://www.youtube.com/watch?v=gQP-Z2RbEvg" Target="_blank">Preparar la foto para scaneear </asp:HyperLink>
</p>
</asp:Content>
