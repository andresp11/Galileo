<%@ Page Title="Galileo OMR" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Galileo._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Galileo</h1>
        <p class="lead">Galielo es un desarrollo a la medida para el reconocimiento óptico de marcas&nbsp;
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="https://es.wikipedia.org/wiki/Reconocimiento_%C3%B3ptico_de_marcas" Target="_blank">OMR.</asp:HyperLink>
        </p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Operación amigable</h2>
            <p style="text-align: justify">
                Le presentamos una aplicación para leer datos de imágenes escaneadas o fotografiadas con tabletas y teléfonos celulares. 
            </p>
            <p style="text-align: justify">
                A partir de un formato particular, la aplicación detectará automáticamente la marca y las registrará como resultados en bases de datos, opera en entornos locales o internet, ofrece la ventaja de recolectar rápidamente datos.
            </p>
            <p>
        </div>
        <div class="col-md-4">
            <h2>Componentes y funcionalidad:</h2>
            <ul style="list-style-type: square">
                <li style="text-align: justify">Base de datos SQL Server ubicado en internet</li>
                <li style="text-align: justify">Aplicación cliente servidor para entorno de red local</li>
                <li style="text-align: justify">Aplicación WEB para entornos externos</li>
                <li style="text-align: justify">Registro de datos desde las dos aplicaciones simultaneamente</li>
                <li style="text-align: justify">Imagenes obtenidas a traves de un escaner o dispositivos moviles</li>
                <li style="text-align: justify">Registro, configuración y validación de datos </li>
                <li style="text-align: justify">Capacidad de importar y exportar datos en cualquier formato
            </li>
            </ul>
            <p>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Aplicación en:</h2>
            <ul style="list-style-type: square">
                <li style="text-align: justify">Estudios de mercado</li>
                <li style="text-align: justify">Encuestas</li>
                <li style="text-align: justify">Calificación de exámenes</li>
                <li style="text-align: justify">Evaluación de productos</li>
                <li style="text-align: justify">Formularios de sugerencias</li>
                <li style="text-align: justify">Inventarios</li>
                <li style="text-align: justify">Formularios de subscripción</li>
                <li style="text-align: justify">Geocodificación</li>
                <li style="text-align: justify">Evaluaciones</li>
                <li style="text-align: justify">Solicitudes de admisión</li>
                <li style="text-align: justify">Controles de asistencia</li>
            </ul>
        </div>
    </div>

</asp:Content>
