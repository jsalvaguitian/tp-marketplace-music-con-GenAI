# 🎵 Marketplace con Chat Inteligente e IA

![Vista del chat](docs/chat-ia.png)

## 📋 Descripción

Este repositorio corresponde a la evolución de un **proyecto académico desarrollado inicialmente en equipo**, en el que se desarrolló un marketplace con chat en tiempo real utilizando **ASP.NET Core MVC**, **Entity Framework Core**, **SQL Server** y **SignalR**.

Como extensión personal del proyecto original, incorporé nuevas funcionalidades basadas en **Inteligencia Artificial** utilizando la **API de Gemini**, con el objetivo de mejorar la experiencia del usuario y reforzar la seguridad durante las conversaciones del chat.

---

## 👥 Sobre el proyecto

El marketplace base fue desarrollado como **trabajo práctico en equipo**.

Las funcionalidades relacionadas con Inteligencia Artificial presentes en este repositorio fueron desarrolladas de forma individual como parte de mi aprendizaje.

### Funcionalidades incorporadas de forma individual

- Mejorar mensajes utilizando IA para hacerlos más claros, cordiales y profesionales.
- Analizar mensajes para detectar posibles intentos de phishing, fraude o intercambio de información sensible.
- Integración con la API de Gemini.
- Diseño e implementación de la interfaz de usuario para las nuevas funcionalidades.
- Consumo de servicios REST desde ASP.NET Core MVC.

---

## 🚀 Funcionalidades

### Marketplace

- Registro e inicio de sesión.
- Publicación de productos y servicios.
- Gestión de ofertas.
- Chat privado en tiempo real mediante SignalR.
- Envío de imágenes y videos.
- Cierre de conversaciones por parte del vendedor.

### Inteligencia Artificial

#### ✨ Mejorar mensaje

Permite reescribir un mensaje antes de enviarlo para hacerlo más:

- Claro.
- Cordial.
- Profesional.

#### 🛡️ Analizar mensaje

Analiza los mensajes recibidos para detectar posibles señales de:

- Phishing.
- Fraude.
- Solicitudes sospechosas.
- Intercambio de información sensible.

El resultado clasifica el mensaje como:

- 🟢 Riesgo bajo.
- 🟡 Riesgo medio.
- 🔴 Riesgo alto.

---

## 🛠 Tecnologías

- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- SignalR
- Bootstrap 5
- JavaScript
- HTML5
- CSS3
- Gemini API

---

## ⚙️ Configuración del proyecto

### 1. Clonar el repositorio

```bash
git clone https://github.com/jsalvaguitian/tp-marketplace-music-con-GenAI.git
```

### 2. Crear un contenedor de SQL Server con Docker


En Linux:

```bash
docker run \
-e "ACCEPT_EULA=Y" \
-e "MSSQL_SA_PASSWORD=TuPassword123!" \
-p 1433:1433 \
--name sqlserver \
-d mcr.microsoft.com/mssql/server:2022-latest
```

Verificar que el contenedor esté ejecutándose:

```bash
sudo docker ps
```

---

### 3. Configurar User Secrets

Inicializar User Secrets:

```bash
dotnet user-secrets init
```

Guardar la API Key de Gemini creada en https://aistudio.google.com/prompts/new_chat:

```bash
dotnet user-secrets set "Gemini:ApiKey" "TU_API_KEY"
```

Verificar los secretos almacenados:

```bash
dotnet user-secrets list
```

---

### 4. Aplicar migraciones

```bash
dotnet ef database update
```

---

### 5. Ejecutar la aplicación

```bash
dotnet run
```

---
