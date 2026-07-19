(function () {
    var container = document.getElementById("listaOfertas");
    if (!container) return;

    var publicacionId = parseInt(container.getAttribute("data-publicacion-id"));
    var esPropietario = container.getAttribute("data-es-propietario") === "true";
    var duenoId = parseInt(container.getAttribute("data-dueno-id"));

    var conexion = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/oferta")
        .withAutomaticReconnect()
        .build();

    var sinOfertas = document.getElementById("sinOfertas");
    var contador = document.getElementById("contadorOfertas");
    var inputOferta = document.getElementById("mensajeOferta");
    var btnEnviar = document.getElementById("btnEnviarOferta");
    var contadorOfertas = document.querySelectorAll(".oferta-item").length || 0;

    function actualizarContador() {
        if (contador) {
            contador.textContent = contadorOfertas + " oferta" + (contadorOfertas !== 1 ? "s" : "");
        }
    }
    actualizarContador();

    function escapeHtml(texto) {
        var d = document.createElement("div");
        d.appendChild(document.createTextNode(texto));
        return d.innerHTML;
    }

    function agregarOferta(oferta) {
        if (sinOfertas) sinOfertas.remove();
        contadorOfertas++;
        actualizarContador();

        var div = document.createElement("div");
        div.className = "oferta-item mb-3 p-3 border rounded";
        if (oferta.usuarioId === duenoId) {
            div.className += " bg-light border-primary";
        }

        var fecha = new Date(oferta.fechaCreacion);
        var fechaStr = fecha.toLocaleDateString("es-AR") + " " + fecha.toLocaleTimeString("es-AR", { hour: "2-digit", minute: "2-digit" });

        var html = '<div class="d-flex justify-content-between align-items-start">';
        html += "<div>";
        html += "<strong>" + escapeHtml(oferta.usuarioNombre) + "</strong>";
        html += ' <small class="text-muted">' + fechaStr + "</small>";
        html += '<p class="mb-1 mt-1">' + escapeHtml(oferta.mensaje) + "</p>";
        html += "</div>";

        if (esPropietario && oferta.usuarioId !== duenoId) {
            html += '<a href="/Chat/AceptarOferta?publicacionId=' + oferta.publicacionId + "&ofertanteId=" + oferta.usuarioId + '"';
            html += ' class="btn btn-sm btn-outline-success ms-2">';
            html += '<i class="bi bi-chat-dots"></i> Negociar</a>';
        }

        html += "</div>";
        div.innerHTML = html;
        container.appendChild(div);
        container.scrollTop = container.scrollHeight;
    }

    conexion.on("RecibirOferta", function (oferta) {
        agregarOferta(oferta);
    });

    conexion.onreconnected(function () {
        conexion.invoke("JoinGroup", publicacionId).catch(function (err) {
            console.error(err.toString());
        });
    });

    conexion.start().then(function () {
        return conexion.invoke("JoinGroup", publicacionId);
    }).catch(function (err) {
        console.error("Error ofertas SignalR:", err.toString());
    });

    function enviarOferta() {
        if (!inputOferta) return;
        var mensaje = inputOferta.value.trim();
        if (mensaje === "") return;

        btnEnviar.disabled = true;
        conexion.invoke("EnviarOferta", publicacionId, mensaje)
            .then(function () {
                inputOferta.value = "";
                inputOferta.focus();
            })
            .catch(function (err) {
                console.error("Error al enviar oferta:", err.toString());
            })
            .finally(function () {
                btnEnviar.disabled = false;
            });
    }

    if (btnEnviar) {
        btnEnviar.addEventListener("click", enviarOferta);
    }

    if (inputOferta) {
        inputOferta.addEventListener("keydown", function (e) {
            if (e.key === "Enter") {
                e.preventDefault();
                enviarOferta();
            }
        });
    }
})();
