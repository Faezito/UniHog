// CONSULTAR CEP

/*
a
    <script src="~/js/consulta-cep.js" asp-append-version="true"></script>
*/

$(document).ready(function () {

    function limpa_formulário_cep() {
        // Limpa valores do formulário de cep.
        $("#rua").val("");
        $("#bairro").val("");
        $("#Cidade").val("");
        $("#Estado").val("");
        $("#uf").val("");
        $("#Regiao").val("");
        $("#ibge").val("");
    }

    //Quando o campo cep perde o foco.
    $("#cep").blur(function () {

        //Nova variável "cep" somente com dígitos.
        var cep = $(this).val().replace(/\D/g, '');

        //Verifica se campo cep possui valor informado.
        if (cep != "") {

            //Expressão regular para validar o CEP.
            var validacep = /^[0-9]{8}$/;

            //Valida o formato do CEP.
            if (validacep.test(cep)) {

                //Preenche os campos com "..." enquanto consulta webservice.
                $("#rua").val("...");
                $("#bairro").val("...");
                $("#Cidade").val("...");
                $("#Estado").val("...");
                $("#uf").val("...");
                $("#ibge").val("...");

                //Consulta o webservice viacep.com.br/
                $.getJSON("https://viacep.com.br/ws/" + cep + "/json/?callback=?", function (dados) {

                    if (!("erro" in dados)) {
                        //Atualiza os campos com os valores da consulta.
                        $("#rua").val(dados.logradouro);
                        $("#bairro").val(dados.bairro);
                        $("#Cidade").val(dados.localidade);
                        $("#Estado").val(dados.estado);
                        $("#Regiao").val(dados.regiao);
                        $("#uf").val(dados.uf);
                    } //end if.
                    else {
                        //CEP pesquisado não foi encontrado.
                        limpa_formulário_cep();
                        Swal.fire({
                            icon: "error",
                            title: "Oops...",
                            text: "CEP não encontrado.",
                        });
                    }
                });
            } //end if.
            else {
                //cep é inválido.
                limpa_formulário_cep();
                Swal.fire({
                    icon: "error",
                    title: "Oops...",
                    text: "Formato de CEP inválido.",
                });
            }
        } //end if.
        else {
            //cep sem valor, limpa formulário.
            limpa_formulário_cep();
        }
    });
});
