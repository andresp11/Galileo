<%@ Page Title="Acerca" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="Galileo.About" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Acerca de Galileo OMR</h2>
    <p class="lead" style="text-align: justify">Existen diversas técnicas e instrumentos relacionados con la recolección de datos. Se podrían utilizar simultáneamente varias técnicas, por eso es imprescindible reflexionar sobre los factores prácticos, incluidos las fuentes, el presupuesto, el nivel esperado de errores, los plazos&nbsp; y la adecuación de la investigación. Los tipos de entrevistas más comunes son:
</p>

    <div class="row">
        <div class="col-md-4">
            <h2>Entrevista cara a cara</h2>
            <p style="text-align: justify">
                Esta técnica tiene mayor ventaja metodológica. Permite al investigador establecer una buena relación con los participantes. Este tipo de entrevistas obtienen mayores tasas de respuesta en la investigación, sin embargo la codificación, tabulación y registro es un proceso lento y que consume muchos recursos.
            </p>
            <p>
                
                <asp:Image ID="Image1" runat="server" ImageUrl="~/Imagen/caraacara.jpg" />
                
            </p>
        </div>
        <div class="col-md-4">
            <h2>Entrevistas telefónicas</h2>
            <p style="text-align: justify">
                 Este tipo de técnica de la entrevista son menos costosos y requiere mucho tiempo. Sin embargo, la tasa de respuesta no es tan alta como la entrevista cara a cara y dependiendo del número de preguntas podría requerir de codificar y tabular las respuestas.
                <br /><br />
            </p>
            <p>
                
                <asp:Image ID="Image2" runat="server" ImageUrl="~/Imagen/telefonica1.jpg" />
                
            </p>
        </div>
        <div class="col-md-4">
            <h2>La entrevista a través de computadoras</h2>
            <p style="text-align: justify">
                 Esta es una forma de la entrevista personal. La entrevista se lleva a cabo mediante el uso de una computadora de mano o computadora portátil.
            </p>
            <br /><br />
            <p>
                <asp:Image ID="Image3" runat="server" ImageUrl="~/Imagen/computadora.jpg" />
            </p>
        </div>
    </div>
    <p style="text-align: justify">Las dos últimas operan a través de un sistema de registro oportuno de datos, sin embargo aumentan el riesgo de incertidumbre y margen de error al desvirtuar la universalidad de la población objetivo.
Con <strong>Galileo OMR</strong> ofrecemos un desarrollo tecnológico que disminuye el tiempo y costo de la recolección de datos en las entrevistas cara a cara y fortalece los recursos sistémicos de las entrevistas telefónicas y a través computadoras.
</p>

</asp:Content>
