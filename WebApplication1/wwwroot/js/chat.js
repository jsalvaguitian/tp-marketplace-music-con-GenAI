(function () {
    var contenedor = document.getElementById("chatMessages");
    if (!contenedor) return;

    var conversacionId = parseInt(contenedor.getAttribute("data-conversacion-id"));
    var usuarioLogueado = parseInt(contenedor.getAttribute("data-usuario-logado"));
    var coloresInit = contenedor.getAttribute("data-colores");

    var paleta = ["#6f42c1", "#fd7e14", "#20c997", "#dc3545", "#0dcaf0", "#198754", "#d63384", "#0d6efd", "#6610f2", "#e83e8c"];
    var coloresConocidos = {};
    var colorIdx = 0;

    if (coloresInit) {
        try {
            coloresConocidos = JSON.parse(coloresInit);
            colorIdx = Object.keys(coloresConocidos).length;
        } catch (e) {}
    }

    function colorParaUsuario(usuarioId) {
        var key = String(usuarioId);
        if (!coloresConocidos[key]) {
            coloresConocidos[key] = paleta[colorIdx % paleta.length];
            colorIdx++;
        }
        return coloresConocidos[key];
    }

    function escapeHtml(texto) {
        var d = document.createElement("div");
        d.appendChild(document.createTextNode(texto));
        return d.innerHTML;
    }

    function esImagen(url) {
        return /\.(jpg|jpeg|png|gif|webp)$/i.test(url);
    }

    function esVideo(url) {
        return /\.(mp4|webm|mov)$/i.test(url);
    }

    function renderArchivo(url) {
        if (esImagen(url)) {
            return '<img src="' + url + '" class="img-fluid rounded mt-1" style="max-height:300px;" />';
        }
        if (esVideo(url)) {
            return '<video src="' + url + '" class="w-100 rounded mt-1" controls style="max-height:300px;"></video>';
        }
        return '';
    }

    function agregarMensaje(msg) {
        var div = document.createElement("div");
        var esPropio = msg.usuarioId === usuarioLogueado;

        div.className = (esPropio ? "d-flex justify-content-end" : "d-flex justify-content-start") + " mb-2 mensaje-item";
        div.setAttribute("data-usuario", msg.usuarioId);

        var fecha = new Date(msg.fechaEnvio);
        var hora = fecha.toLocaleTimeString("es-AR", { hour: "2-digit", minute: "2-digit" });
        var tieneTexto = msg.texto && msg.texto.trim() !== "";
        var tieneArchivo = msg.archivoUrl && msg.archivoUrl.trim() !== "";
        var archivoHtml = tieneArchivo ? renderArchivo(msg.archivoUrl) : "";
        var textoHtml = tieneTexto ? '<div class="mb-1">' + escapeHtml(msg.texto) + '</div>' : "";

        if (esPropio) {
            div.innerHTML = '<div class="bg-primary text-white rounded-3 py-2 px-3" style="max-width:75%;">'
                + '<div class="d-flex justify-content-between align-items-center gap-2 mb-1">'
                + '<small class="fw-bold opacity-75">Vos</small></div>'
                + textoHtml
                + archivoHtml
                + '<div class="text-end"><small class="opacity-50">' + hora + "</small></div></div>";
        } else {
            var color = colorParaUsuario(msg.usuarioId);
            div.innerHTML = '<div class="rounded-3 py-2 px-3 text-white" style="max-width:75%;background-color:' + color + ';">'
                + '<div class="d-flex justify-content-between align-items-center gap-2 mb-1">'
                + '<small class="fw-bold opacity-75">' + escapeHtml(msg.usuarioNombre) + "</small></div>"
                + textoHtml
                + archivoHtml
                + '<div class="text-end"><small class="opacity-50">' + hora + "</small></div></div>";
        }

        contenedor.appendChild(div);
        contenedor.scrollTop = contenedor.scrollHeight;
    }

    var conexion = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/chat")
        .withAutomaticReconnect()
        .build();

    conexion.on("RecibirMensaje", function (msg) {
        agregarMensaje(msg);
    });

    conexion.start().then(function () {
        return conexion.invoke("JoinConversacion", conversacionId);
    }).catch(function (err) {
        console.error("Error chat SignalR:", err.toString());
    });

    var input = document.getElementById("chatInput");
    var btn = document.getElementById("btnEnviarChat");

    var archivoSubidoUrl = null;

    function enviar() {
        var texto = input.value.trim();
        var url = archivoSubidoUrl;
        if (texto === "" && !url) return;

        btn.disabled = true;
        conexion.invoke("EnviarMensaje", conversacionId, texto, url)
            .then(function () {
                input.value = "";
                archivoSubidoUrl = null;
                previewContainer.className = "d-none";
                input.placeholder = "Escribí un mensaje...";
                input.focus();
            })
            .catch(function (err) {
                console.error("Error al enviar:", err.toString());
            })
            .finally(function () {
                btn.disabled = false;
            });
    }

    if (btn) {
        btn.addEventListener("click", enviar);
    }

    if (input) {
        input.addEventListener("keydown", function (e) {
            if (e.key === "Enter") {
                e.preventDefault();
                enviar();
            }
        });
    }

    var btnEmoji = document.getElementById("btnEmoji");
    var emojiPicker = document.getElementById("emojiPicker");
    if (btnEmoji && emojiPicker) {
        btnEmoji.addEventListener("click", function () {
            emojiPicker.style.display = emojiPicker.style.display === "none" ? "block" : "none";
        });
        emojiPicker.querySelectorAll(".emoji-btn").forEach(function (el) {
            el.addEventListener("click", function () {
                input.value += this.textContent;
                input.focus();
            });
        });
        document.addEventListener("click", function (e) {
            if (!btnEmoji.contains(e.target) && !emojiPicker.contains(e.target)) {
                emojiPicker.style.display = "none";
            }
        });
    }

    var fileInput = document.getElementById("fileInput");
    var previewContainer = document.createElement("div");
    previewContainer.className = "d-none align-items-center gap-2 bg-light rounded p-1 px-2 mt-1 small";
    previewContainer.id = "filePreview";
    if (fileInput) {
        fileInput.parentElement.insertAdjacentElement("afterend", previewContainer);
    }

    function mostrarPreview(file, url) {
        var esImg = /\.(jpg|jpeg|png|gif|webp)$/i.test(file.name);
        var esVid = /\.(mp4|webm|mov)$/i.test(file.name);

        previewContainer.className = "d-flex align-items-center gap-2 bg-light rounded p-1 px-2 mt-1 small";
        previewContainer.innerHTML = "";

        if (esImg) {
            var img = document.createElement("img");
            img.src = url;
            img.style.width = "32px";
            img.style.height = "32px";
            img.className = "rounded object-fit-cover";
            previewContainer.appendChild(img);
        } else if (esVid) {
            var icon = document.createElement("i");
            icon.className = "bi bi-film fs-5 text-secondary";
            previewContainer.appendChild(icon);
        }

        var name = document.createElement("span");
        name.className = "text-truncate flex-grow-1";
        name.textContent = file.name;
        previewContainer.appendChild(name);

        var remove = document.createElement("button");
        remove.type = "button";
        remove.className = "btn-close btn-close-sm";
        remove.setAttribute("aria-label", "Quitar archivo");
        remove.addEventListener("click", function () {
            archivoSubidoUrl = null;
            previewContainer.className = "d-none";
            input.placeholder = "Escribí un mensaje...";
        });
        previewContainer.appendChild(remove);
    }

    if (fileInput) {
        fileInput.addEventListener("change", function () {
            var file = fileInput.files[0];
            if (!file) return;

            var form = new FormData();
            form.append("archivo", file);

            var xhr = new XMLHttpRequest();
            xhr.open("POST", "/Chat/SubirArchivo", true);
            xhr.onload = function () {
                if (xhr.status === 200) {
                    try {
                        var resp = JSON.parse(xhr.responseText);
                        if (resp.ok) {
                            archivoSubidoUrl = resp.url;
                            mostrarPreview(file, resp.url);
                            input.placeholder = "Agregá un mensaje o enviá el archivo...";
                            input.focus();
                        } else {
                            alert(resp.error || "Error al subir archivo.");
                        }
                    } catch (e) {
                        alert("Error al procesar la respuesta.");
                    }
                } else {
                    alert("Error al subir archivo.");
                }
                fileInput.value = "";
            };
            xhr.onerror = function () {
                alert("Error de conexión al subir archivo.");
                fileInput.value = "";
            };
            xhr.send(form);
        });
    }
})();
