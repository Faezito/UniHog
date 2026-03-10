if (window.feather) {
    feather.replace();
}

//// LOADING PLUGIN

// site.js

window.Loading = (function () {

    const defaultConfigs = {
        "overlayBackgroundColor": "#000000",
        "overlayOpacity": 0.6,
        "spinnerIcon": "fire",
        "spinnerColor": "#FFFFFF",
        "spinnerSize": "2x",
        "overlayIDName": "Carregando",
        "spinnerIDName": "",
        "offsetX": 0,
        "offsetY": 0,
        "containerID": null,
        "lockScroll": true,
        "overlayZIndex": 9998,
        "spinnerZIndex": 9999
    };

    function show(customConfigs = {}) {
        JsLoadingOverlay.show({
            ...defaultConfigs,
            ...customConfigs
        });
    }

    function hide() {
        JsLoadingOverlay.hide();
    }

    return {
        show,
        hide
    };

})();

// Ativa loading para QUALQUER requisição AJAX
$(document).ajaxStart(function () {
    Loading.show();
});

$(document).ajaxStop(function () {
    Loading.hide();
    initTooltips();
});

//// ENDLOADINGS


$.ajaxSetup({
    complete: function (xhr) {
        if (xhr.responseJSON?.swal) {
            Swal.fire(xhr.responseJSON.swal).then(() => {
                if (xhr.responseJSON.redirect) {
                    window.location.href = xhr.responseJSON.redirect;
                }
            });
        }
    }
});

function initRatings(context) {

    if (!$.fn.rateYo) {
        console.warn("RateYo não carregado");
        return;
    }

    const scope = context || document;

    $(scope).find(".rating").each(function () {

        const nota = parseFloat($(this).data("nota")) || 0;
        const readonly = $(this).data("readonly") === true;

        $(this).rateYo({
            rating: nota,
            minValue: 0,
            maxValue: 100,
            readOnly: readonly,
            starWidth: "20px",
            ratedFill: "#ffc107"
        });

    });
}

$(function () {
    initRatings();
});

$(document).ready(function () {
    $('.date').mask('00/00/0000');
    $('.time').mask('00:00:00');
    $('.date_time').mask('00/00/0000 00:00:00');
    $('.cep').mask('00000-000');
    $('.tel').mask('00000-0000');
    $('.tel_com_ddd').mask('(00) 00000-0000');
    $('.mixed').mask('AAA 000-S0S');
    $('.cpf').mask('000.000.000-00', { reverse: true });
    $('.cnpj').mask('00.000.000/0000-00', { reverse: true });
    $('.money').mask('000.000.000.000.000,00', { reverse: true });
    $('.money2').mask("#.##0,00", { reverse: true });
    $('.ip_address').mask('0ZZ.0ZZ.0ZZ.0ZZ', {
        translation: {
            'Z': {
                pattern: /[0-9]/, optional: true
            }
        }
    });
    $('.ip_address').mask('099.099.099.099');
    $('.percent').mask('##0,00%', { reverse: true });
    $('.clear-if-not-match').mask("00/00/0000", { clearIfNotMatch: true });
    $('.placeholder').mask("00/00/0000", { placeholder: "__/__/____" });
    $('.fallback').mask("00r00r0000", {
        translation: {
            'r': {
                pattern: /[\/]/,
                fallback: '/'
            },
            placeholder: "__/__/____"
        }
    });
    $('.selectonfocus').mask("00/00/0000", { selectOnFocus: true });
});

function carregarMask() {
    $('.date').mask('00/00/0000');
    $('.time').mask('00:00:00');
    $('.date_time').mask('00/00/0000 00:00:00');
    $('.cep').mask('00000-000');
    $('.tel').mask('00000-0000');
    $('.tel_com_ddd').mask('(00) 00000-0000');
    $('.mixed').mask('AAA 000-S0S');
    $('.cpf').mask('000.000.000-00', { reverse: true });
    $('.cnpj').mask('00.000.000/0000-00', { reverse: true });
    $('.money').mask('000.000.000.000.000,00', { reverse: true });
    $('.money2').mask("#.##0,00", { reverse: true });
    $('.ip_address').mask('0ZZ.0ZZ.0ZZ.0ZZ', {
        translation: {
            'Z': {
                pattern: /[0-9]/, optional: true
            }
        }
    });
    $('.ip_address').mask('099.099.099.099');
    $('.percent').mask('##0,00%', { reverse: true });
    $('.clear-if-not-match').mask("00/00/0000", { clearIfNotMatch: true });
    $('.placeholder').mask("00/00/0000", { placeholder: "__/__/____" });
    $('.fallback').mask("00r00r0000", {
        translation: {
            'r': {
                pattern: /[\/]/,
                fallback: '/'
            },
            placeholder: "__/__/____"
        }
    });
    $('.selectonfocus').mask("00/00/0000", { selectOnFocus: true });
}

/*
        $('#form').submit(function (evt) {
            fn_submit(evt, '#form', '@Url.Action("action", "controller")', `@Url.Action("action", "controller")`, false);
        });
 */

function fn_submitPesquisa(evt, formID, urlSubmit, htmlID) {
    evt.preventDefault();
    $.ajax({
        type: "POST",
        url: urlSubmit,
        dataType: 'html',
        data: $(formID).serialize(),

        beforeSend: function () {
        },

        success: function (resultado) {
            $(htmlID).html(resultado);
        },

        complete: function () {
            initTooltips();
        },

        error: function () {
        }
    });
}
function fn_submit(evt, formID, urlSubmit, urlRetorno, reloadPage) {
    var form = document.getElementById(formID);
    var formData = new FormData(form);
    evt.preventDefault();

    $.ajax({
        type: "POST",
        url: urlSubmit,
        data: formData, // obrigatório para enviar img, não pode serializar como os outros
        processData: false, // obrigatório para enviar img
        contentType: false, // obrigatório para enviar img
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        beforeSend: function () {
            $('#btnSalvar').prop('disabled', true);
        },
        complete: function () {
            $('#btnSalvar').prop('disabled', false);
        }
    }).done(function (data, textStatus, jqXHR) {

        Swal.fire({
            title: 'Registro salvo com sucesso!',
            text: "",
            icon: 'success',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sair da página',
            cancelButtonText: 'Continuar',
            showClass: {
                popup: 'animate__animated animate__fadeInDown'
            },
            hideClass: {
                popup: 'animate__animated animate__fadeOutUp'
            }
        }).then((result) => {
            if (result.value) {
                location.replace(urlRetorno);
            } else {
                if (reloadPage == true) {
                    window.location.reload();
                }
            }
        });
    })
        .fail(function (request) {
            Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: request.responseText
            });
        });
}
function fn_submit2(evt, btnID, formID, urlSubmit, urlRetorno, tipo = "salvar", reloadPage) {
    evt.preventDefault();
    var form = document.getElementById(formID);
    var formData = new FormData(form);
    const titulos = {
        salvar: "Salvo com sucesso!",
        deletar: "Deletado com sucesso!"
    };

    const titulo = titulos[tipo] ?? "Operação realizada!";

    $.ajax({
        type: "POST",
        url: urlSubmit,
        data: formData, // obrigatório para enviar img, não pode serializar como os outros
        processData: false, // obrigatório para enviar img
        contentType: false, // obrigatório para enviar img
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        beforeSend: function () {
            $(btnID).prop('disabled', true);
        },
        complete: function () {
            $(btnID).prop('disabled', false);
        }
    })
        .done(function (data, textStatus, jqXHR) {

            Swal.fire({
                title: titulo,
                text: "",
                icon: 'success',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sair da página',
                cancelButtonText: 'Continuar',
                showClass: {
                    popup: 'animate__animated animate__fadeInDown'
                },
                hideClass: {
                    popup: 'animate__animated animate__fadeOutUp'
                }
            }).then((result) => {
                if (result.value) {
                    location.replace(urlRetorno);
                } else {
                    if (reloadPage == true) {
                        window.location.reload();
                    }
                }
            });
        })
        .fail(function (request) {
            Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: request.responseText
            });
        });
}
function fn_submit3(evt, formID, urlSubmit, urlRetorno, tipo = "salvar", reloadPage) {
    evt.preventDefault();
    var form = document.getElementById(formID);
    var formData = new FormData(form);
    const titulos = {
        salvar: "Salvo com sucesso!",
        deletar: "Deletado com sucesso!"
    };

    const titulo = titulos[tipo] ?? "Operação realizada!";

    $.ajax({
        type: "POST",
        url: urlSubmit,
        data: formData, // obrigatório para enviar img, não pode serializar como os outros
        processData: false, // obrigatório para enviar img
        contentType: false, // obrigatório para enviar img
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        beforeSend: function () {
            $('#btnSalvar').prop('disabled', true);
        },
        complete: function () {
            $('#btnSalvar').prop('disabled', false);
        }
    }).done(function (data, textStatus, jqXHR) {

        Swal.fire({
            title: titulo,
            text: "",
            icon: 'success',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sair da página',
            cancelButtonText: 'Continuar',
            showClass: {
                popup: 'animate__animated animate__fadeInDown'
            },
            hideClass: {
                popup: 'animate__animated animate__fadeOutUp'
            }
        }).then((result) => {
            if (result.value) {
                location.replace(urlRetorno);
            } else {
                if (reloadPage == true) {
                    window.location.reload();
                }
            }
        });
    })
        .fail(function (request) {
            Swal.fire({
                icon: 'error',
                title: 'Erro',
                text: request.responseText
            });
        });
}



function fn_submit4(evt, formID, urlSubmit, btnSelector = '#btnSalvar') {
    evt.preventDefault()
    const form = document.getElementById(formID);
    const formData = new FormData(form);

    return $.ajax({
        url: urlSubmit,
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        beforeSend: function () {
            $(btnSelector).prop('disabled', true);
        },
        complete: function () {
            $(btnSelector).prop('disabled', false);
        }
    })
        .done(function (response) {

            if (!response?.success) {
                Swal.fire({
                    icon: 'error',
                    title: response?.title ?? 'Erro',
                    html: response?.detail ?? 'Erro ao processar a solicitação.'
                });
                return;
            }

            Swal.fire({
                icon: 'success',
                title: response?.title ?? 'Sucesso',
                html: response?.detail ?? 'Solicitação concluída com sucesso!'
            }).then(() => {
                if (response.redirectUrl) {
                    window.location.href = response.redirectUrl;
                }
            });
        })
        .fail(function (xhr) {

            const json = xhr.responseJSON;

            Swal.fire({
                icon: 'error',
                title: json?.title ?? 'Erro',
                html: json?.detail ?? 'Erro inesperado ao processar a requisição.'
            });
        });
}

$(document).on('click', '.btn-deletar', function (e) {
    e.preventDefault();

    const form = $(this).closest("form");

    Swal.fire({
        title: "Realmente deseja deletar este registro?",
        showDenyButton: true,
        denyButtonText: "Não",
        confirmButtonText: "Sim",
    }).then((result) => {
        if (result.isConfirmed) {
            form.trigger("submit");
        }
    })
})

/*
$('body').on('click', '#btnExcel', function (evt) {
    let urlExcel = "@Url.Action("DownloadExcel", "RelatoriosFaturamentos")";
    let formSerializado = $("#form-pesquisa").serialize();
    fn_excel(evt, formSerializado, urlExcel, 'nomeArquivo');
});
 */
function fn_excel(evt, formSerializado, urlExcel, nomeArquivo) {
    evt.preventDefault();

    $.ajax({
        method: "POST",
        url: urlExcel,
        data: formSerializado,
        xhrFields: {
            responseType: 'blob'
        },
    }).done(function (result, status, xhr) {

        let blob = new Blob([result],
            {
                type: xhr.getResponseHeader('Content-Type')
            });

        var a = document.createElement("a");
        a.href = URL.createObjectURL(blob);
        if (nomeArquivo == undefined || nomeArquivo == null || nomeArquivo == "") {
            a.download = "Relatorio_Excel.xlsx";
        } else {
            a.download = nomeArquivo + ".xlsx";
        }
        a.click();
    });
}


/**
    onclick="abrirModal('@Url.Action("Action","Controller", new { id = id})'), 'modalController')"
 */
//function abrirModal(Url, modalID) {
//    $.ajax({
//        url: Url,
//        type: 'POST',
//        dataType: 'html',
//        success: function (html) {
//            $('#area-modal').html(html);
//            const modal = new bootstrap.Modal(document.getElementById(modalID))
//            modal.show();
//        },
//        error: function (xhr) {
//            let mensagem = xhr.responseText || 'Erro ao carregar';
//            Swal.fire({
//                icon: "error",
//                title: "Oops...",
//                text: mensagem,
//            });
//            console.error('Erro da controller:', mensagem);
//        }
//    })
//}

function initTooltips() {
    document.querySelectorAll('[data-bs-toggle="tooltip"]')
        .forEach(el => {
            if (!bootstrap.Tooltip.getInstance(el)) {
                new bootstrap.Tooltip(el, {
                    offset: () => {
                        if (window.innerWidth < 768) {
                            return [0, 0]
                        } else {
                            return [0, 0]
                        }
                    }
                });
            }
        });
}

initTooltips();


function fn_deletar(evt, button, url) {
    evt.preventDefault();

    const form = $(button).closest("form");

    Swal.fire({
        title: "Realmente deseja deletar este item?",
        showDenyButton: true,
        denyButtonText: "Não",
        confirmButtonText: "Sim"
    }).then((result) => {

        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'POST',
                data: form.serialize(),
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                },
                beforeSend: function () {
                    $('.btn-deletar').prop('disabled', true);
                },
                complete: function () {
                    $('.btn-deletar').prop('disabled', false);
                }
            })
                .done(function (response) {
                    Swal.fire({
                        icon: 'success',
                        title: response?.title ?? 'Sucesso',
                        html: response?.detail ?? 'Solicitação concluída com sucesso!'
                    }).then(() => {
                        if (response.redirectUrl) {
                            window.location.href = response.redirectUrl;
                        }
                    });
                })
                .fail(function (xhr) {
                    if (xhr.responseJSON) {
                        Swal.fire({
                            icon: 'error',
                            title: xhr.responseJSON.title ?? 'Erro',
                            html: xhr.responseJSON.detail
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Erro',
                            text: 'Erro inesperado ao processar a requisição.'
                        });
                    }
                });
        }
    });

}


async function redimensionarImagem(file, size = 600, quality = 0.8, perfil = true) {

    const img = document.createElement("img");
    img.src = URL.createObjectURL(file);
    await img.decode();

    const canvas = document.createElement("canvas");
    const ctx = canvas.getContext("2d");

    let sx = 0, sy = 0, sw = img.width, sh = img.height;

    // menor lado define o corte quadrado (somente se for perfil)
    if (perfil === true) {
        const minSide = Math.min(img.width, img.height);

        sx = (img.width - minSide) / 2;
        sy = (img.height - minSide) / 2;

        sw = minSide;
        sh = minSide;
    }

    // resize SEMPRE
    const scale = size / sw;
    canvas.width = size;
    canvas.height = sh * scale;

    ctx.drawImage(
        img,
        sx, sy, sw, sh,               // área da imagem original
        0, 0, canvas.width, canvas.height
    );

    return new Promise(resolve => {
        canvas.toBlob(blob => {
            resolve(blob);
        }, "image/jpeg", quality);
    });
}

function fn_limpaModal() {
    $('body').removeClass('modal-open');

    // Remove backdrop que pode ter ficado preso
    $('.modal-backdrop').remove();

    // Remove estilos inline que travam scroll
    $('body').css({
        overflow: '',
        paddingRight: ''
    });

    // Limpa o conteúdo se necessário
    $('#area-modal').empty();
}


function enviarWhatsApp(nome, enderecoCompleto, enderecoMaps, telefone) {
    var mensagem = `Olá, seguem os dados do Estudante:
*Nome*: ${nome}
*Endereço*: ${enderecoCompleto}
*INVESTIGAR*

Veja no *Google Maps*:
${enderecoMaps}`;

    let numero = "55" + telefone;
    console.log(numero);

    var url = "https://wa.me/" + numero + "?text=" + encodeURIComponent(mensagem);
    window.open(url, '_blank');
}


