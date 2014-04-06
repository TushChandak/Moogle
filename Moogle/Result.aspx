<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Result.aspx.cs" Inherits="Moogle.Result" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title id="PageTitle" runat="server"></title>

    <%-- <script src="jquery-1.9.1.min.js"></script>

    <script src="jquery.highlight-4.js"></script>

    <style type="text/css">
        .highlight {
            background-color: yellow;
        }
    </style>

    <script type="text/javascript">
        function searchAndHighlight(searchTerm) {
            $('.highlighted').removeClass('highlighted');

            $("body:contains('" + searchTerm + "')").html($('body').html().replace(searchTerm, "<span class='highlighted'>" + searchTerm + "</span>"));

            if ($('.highlighted:first').length) {
                $('html').scrollTop($('.highlighted:first').offset().top);
            }
        }

        $(document).ready(function () {

            $('#urIframe').highlight('Collections');

        });


        function test() {
            alert("hi");
            $('p').highlight('Collections');
        }

    </script>--%>
</head>


<body>
    <form id="form1" runat="server">
        <div id="bodyContainer">
            <p>
                <iframe id="urIframe" width="100%" height="700px" runat="server"></iframe>
            </p>
        </div>
    </form>
</body>




</html>
