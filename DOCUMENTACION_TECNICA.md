# DOCUMENTACIÓN TÉCNICA

## Proyecto Intermodular DAM
### Ciclo: Desarrollo de Aplicaciones Multiplataforma (DAM)
### Curso: 2025/2026

**Aplicación**: **BUBBLE** - Red social basada en Geo-localizacion

**Autor/a**: [Iker Martinez Lago]

**Fecha**: Marzo 2026

---

## 📑 Índice de Contenidos

1. [Arquitectura del Sistema](#1-arquitectura-del-sistema)
2. [API](#2-api)
3. [Base de Datos](#3-base-de-datos)
4. [Aplicación Web](#4-aplicación-web)
5. [Aplicación Móvil](#5-aplicación-móvil)
6. [Aplicación de Escritorio](#6-aplicación-de-escritorio)
7. [Seguridad](#7-seguridad)
8. [Despliegue](#8-despliegue)
9. [Pruebas](#9-pruebas)
10. [Guía de Uso y Mantenimiento](#10-guía-de-uso-y-mantenimiento)
11. [Mejoras Futuras](#11-mejoras-futuras)

---

## 1. Arquitectura del Sistema

### 1.1 Esquema General

La aplicación **BUBBLE** es un sistema distribuido de tres capas:

```
┌─────────────────────────────────────────────────────────────┐
│                    CAPA DE PRESENTACIÓN                     │
├──────────────────────┬──────────────────┬────────────────── ┤
│   Android (Móvil)    │  Desktop (.NET)  │   Web (Laravel)   │
│                      │                  │                   │
│ • Google Maps API    │ • Windows Forms  │ • HTML/CSS        │
│ • Location Services  │ • Custom Controls│ • Api Laravel     │
│ • Kotlin/Java        │ • C# / .NET 4.7  │                   │
└──────────┬───────────┴────────┬─────────┴──────────┬────────┘
           │                    │                    │
           └────────┬───────────┴────────┬───────────┘
                    │                    │
                    └────────┬───────────┘
                             │ REST API (HTTP)
                             ▼
        ┌────────────────────────────────────────────┐
        │      CAPA DE LÓGICA DE NEGOCIO             │
        │         Laravel 12 Backend API             │
        │                                            │
        │ • Controllers (API + Web)                  │
        │ • Models (Eloquent ORM)                    │
        │ • Middleware (Autenticación Sanctum/Web)   │
        │ • Validaciones                             │
        └─────────────────┬──────────────────────────┘
                          │
                          ▼
        ┌────────────────────────────────────────────┐
        │        CAPA DE PERSISTENCIA                │
        │      SQLite Database (database.sql)        │
        │                                            │
        │ • Tabla users                              │
        │ • Tabla bubbles                            │
        │ • Tabla personal_access_tokens             │
        │ • Tabla sessions                           │
        └────────────────────────────────────────────┘
```

### 1.2 Tecnologías Principales

#### Backend (API + Web Integrados en Laravel)
- **Framework**: Laravel 12
- **Lenguaje**: PHP 8.2+
- **Base de Datos**: SQLite (desarrollo) / MySQL/PostgreSQL (producción)
- **ORM**: Eloquent
- **Autenticación**: 
  - **Web**: Session-based (guard web)
  - **API**: Laravel Sanctum (tokens)

#### Frontend Web (Integrado en Laravel)
- **Templating**: Blade (SSR)
- **CSS**: Tailwind CSS v4.0.0
- **JavaScript**: Vanilla ES6+ + Axios 1.11.0
- **Mapas**: Google Maps API v3
- **Build Tool**: Vite 7.0.7
- **Package Manager**: npm
- **Servidor**: Apache/Nginx (Vite para desarrollo)

#### Aplicación Móvil
- **Plataforma**: Android
- **Lenguaje**: Kotlin/Java
- **SDK Mínimo**: Android 7.0 (API 24)
- **SDK Destino**: Android 15 (API 36)
- **Build System**: Gradle
- **Librerías Clave**:
  - Google Maps API (v18.2.0)
  - Google Play Services Location (v21.0.1)
  - Glide (v4.16.0) para carga de imágenes
  - BCrypt (v0.4) para hash de contraseñas

#### Aplicación Escritorio
- **Plataforma**: Windows
- **Lenguaje**: C#
- **Framework**: .NET Framework 4.7.2
- **UI**: Windows Forms (WinForms)
- **Componentes Personalizados**: Radar Map Control
- **WebView**: Microsoft.Web.WebView2 (v1.0.2903.40)

### 1.3 Flujo de Comunicación

```
┌─────────────────┐
│  Cliente Móvil  │
│    (Android)    │
└────────┬────────┘
         │
         │ HTTP/REST
         │
         ▼
    ┌────────────────────────────┐
    │  API Laravel Backend       │
    │ (bubble-api)               │
    └────────────────────────────┘
         ▲                  ▲
         │                  │
         │ HTTP/REST        │ SQL
         │                  │
         │                  ▼
         │           ┌──────────────┐
         │           │ SQLite DB    │
         │           │ (database.   │
         │           │  sqlite)     │
         │           └──────────────┘
         │
         │
    ┌────────────────────────┐
    │  Cliente Escritorio    │
    │  (Windows .NET)        │
    └────────────────────────┘
```

---

## 2. API

### 2.1 Descripción General

La API RESTful está construida con Laravel 12 y proporciona todos los servicios necesarios para:
- Autenticación y gestión de usuarios
- Crear, listar, actualizar y eliminar "bubbles" (mensajes geo-localizados)
- Gestión de perfiles de usuario

**URL Base**: `http://localhost:8000/api` (desarrollo local)

### 2.2 Autenticación

La API utiliza **Laravel Sanctum** para autenticación basada en tokens.

#### Tipos de Rutas
- **Públicas**: Sin autenticación (login, registro, listar burbujas)
- **Protegidas**: Requieren token Sanctum en header `Authorization: Bearer {TOKEN}`

#### Flujo de Autenticación

```
1. Cliente POST a /desktop/login
   ├─ Email + Password
   └─> Respuesta: {token, user}

2. Cliente guarda token

3. Cliente incluye en próximas requests:
   ├─ Header: Authorization: Bearer {token}
   └─> API valida y permite acceso

4. Logout: POST a /desktop/logout
   └─> Token se invalida
```

### 2.3 Lista de Endpoints

#### Autenticación (Desktop)

| Endpoint | Método | Auth | Descripción |
|----------|--------|------|-------------|
| `/desktop/login` | POST | No | Login de usuario |
| `/desktop/register` | POST | No | Registro de usuario |
| `/desktop/logout` | POST | Sí | Cerrar sesión |
| `/desktop/profile` | GET | Sí | Obtener perfil usuario |
| `/desktop/profile` | POST | Sí | Actualizar perfil |

#### Burbujas (Bubbles)

| Endpoint | Método | Auth | Descripción |
|----------|--------|------|-------------|
| `/burbujas` | GET | No | Listar todas las burbujas |
| `/burbujas` | POST | No | Crear nueva burbuja |
| `/desktop/bubble` | POST | Sí | Crear burbuja (user autenticado) |
| `/desktop/bubble` | DELETE | Sí | Eliminar burbuja propia |

#### Dashboard

| Endpoint | Método | Auth | Descripción |
|----------|--------|------|-------------|
| `/desktop/dashboard` | GET | Sí | Obtener dashboard del usuario |

### 2.4 Ejemplos de Peticiones y Respuestas

#### 2.4.1 Registro de Usuario

**Petición:**
```http
POST /api/desktop/register HTTP/1.1
Host: localhost:8000
Content-Type: application/json

{
  "name": "Iker Martinez",
  "email": "iker@example.com",
  "password": "SecurePass123!",
  "password_confirmation": "SecurePass123!"
}
```

**Respuesta Exitosa (201):**
```json
{
  "message": "User registered successfully",
  "user": {
    "id": 1,
    "name": "Iker Martinez Lago",
    "email": "iker@example.com",
    "avatar": null
  },
  "token": "1|abc123def456..."
}
```

**Respuesta Error (422):**
```json
{
  "message": "The given data was invalid.",
  "errors": {
    "email": ["The email has already been taken."],
    "password": ["The password must be at least 8 characters."]
  }
}
```

#### 2.4.2 Login

**Petición:**
```http
POST /api/desktop/login HTTP/1.1
Host: localhost:8000
Content-Type: application/json

{
  "email": "iker@example.com",
  "password": "SecurePass123!"
}
```

**Respuesta Exitosa (200):**
```json
{
  "message": "Login successful",
  "user": {
    "id": 1,
    "name": "Iker Martinez Lago",
    "email": "iker@example.com",
    "avatar": null
  },
  "token": "1|abc123def456..."
}
```

**Respuesta Error (401):**
```json
{
  "message": "Invalid credentials"
}
```

#### 2.4.3 Crear Burbuja

**Petición:**
```http
POST /api/desktop/bubble HTTP/1.1
Host: localhost:8000
Authorization: Bearer 1|abc123def456...
Content-Type: application/json

{
  "latitude": 40.4168,
  "longitude": -3.7038,
  "mensaje": "¡Qué día tan bonito en Madrid!"
}
```

**Respuesta Exitosa (201):**
```json
{
  "message": "Bubble created successfully",
  "bubble": {
    "id": 42,
    "user_id": 1,
    "latitude": 40.4168,
    "longitude": -3.7038,
    "mensaje": "¡Qué día tan bonito en Madrid!"
  }
}
```

#### 2.4.4 Listar Burbujas

**Petición:**
```http
GET /api/burbujas HTTP/1.1
Host: localhost:8000
```

**Respuesta (200):**
```json
{
  "message": "Bubbles retrieved successfully",
  "bubbles": [
    {
      "id": 1,
      "user_id": 1,
      "latitude": 40.4168,
      "longitude": -3.7038,
      "mensaje": "¡Qué día tan bonito en Madrid!",
      "user": {
        "id": 1,
        "name": "Iker Martinez Lago",
        "email": "iker@example.com"
      }
    }
  ]
}
```

#### 2.4.5 Obtener Perfil

**Petición:**
```http
GET /api/desktop/profile HTTP/1.1
Host: localhost:8000
Authorization: Bearer 1|abc123def456...
```

**Respuesta (200):**
```json
{
  "user": {
    "id": 1,
    "name": "Iker Martinez Lago",
    "email": "iker@example.com"
  }
}
```

#### 2.4.6 Actualizar Perfil

**Petición:**
```http
POST /api/desktop/profile HTTP/1.1
Host: localhost:8000
Authorization: Bearer 1|abc123def456...
Content-Type: application/json

{
  "name": "Iker Martinez Lago",
  "avatar": "https://example.com/avatar.jpg"
}
```

**Respuesta (200):**
```json
{
  "message": "Profile updated successfully",
  "user": {
    "id": 1,
    "name": "Iker Martinez Lago",
    "email": "iker@example.com",
    "avatar": "https://example.com/avatar.jpg"
  }
}
```

#### 2.4.7 Logout

**Petición:**
```http
POST /api/desktop/logout HTTP/1.1
Host: localhost:8000
Authorization: Bearer 1|abc123def456...
```

**Respuesta (200):**
```json
{
  "message": "Logged out successfully"
}
```

### 2.5 Códigos de Error

| Código | Significado | Causa |
|--------|------------|-------|
| 400 | Bad Request | Datos inválidos en la petición |
| 401 | Unauthorized | Token no válido o expirado |
| 403 | Forbidden | Usuario no tiene permiso |
| 404 | Not Found | Recurso no encontrado |
| 422 | Unprocessable Entity | Validación de datos fallida |
| 500 | Internal Server Error | Error del servidor |

---

## 3. Base de Datos

### 3.1 Descripción

La base de datos utiliza **SQLite** con Laravel **Eloquent ORM** para gestión de modelos y relaciones.

**Ubicación**: `bd/bubble-api/database/database.sqlite`

### 3.2 Diagrama Entidad-Relación

```
┌──────────────────────────┐
│       USERS              │
├──────────────────────────┤
│ id (PK)                  │
│ name                     │
│ email (UNIQUE)           │
│ email_verified_at        │
│ password (hashed)        │
│ avatar                   │
│ remember_token           │
│ created_at               │
│ updated_at               │
└─────────────┬────────────┘
              │
              │ 1:N
              │
              ▼
┌──────────────────────────┐
│      BUBBLES             │
├──────────────────────────┤
│ id (PK)                  │
│ user_id (FK) ───┐        │
│ latitude        │        │
│ longitude       │        │
│ mensaje         │        │
│ created_at      │        │
│ updated_at      │        │
└──────────────────┤───────┘
                   │
                   └─ Relación: User hasMany Bubble
                      Bubble belongsTo User

┌──────────────────────────────────────┐
│ PERSONAL_ACCESS_TOKENS (Sanctum)     │
├──────────────────────────────────────┤
│ id (PK)                              │
│ tokenable_type                       │
│ tokenable_id                         │
│ name                                 │
│ token (hashed)                       │
│ abilities                            │
│ last_used_at                         │
│ created_at                           │
│ updated_at                           │
└──────────────────────────────────────┘

┌──────────────────────────┐
│      SESSIONS            │
├──────────────────────────┤
│ id (PK)                  │
│ user_id (nullable)       │
│ ip_address               │
│ user_agent               │
│ payload                  │
│ last_activity            │
│ created_at               │
│ updated_at               │
└──────────────────────────┘

┌──────────────────────────────┐
│  PASSWORD_RESET_TOKENS       │
├──────────────────────────────┤
│ email (PK)                   │
│ token (hashed)               │
│ created_at                   │
└──────────────────────────────┘

┌──────────────────────────┐
│        CACHE             │
├──────────────────────────┤
│ key (PK)                 │
│ value                    │
│ expiration               │
└──────────────────────────┘

┌──────────────────────────┐
│        JOBS              │
├──────────────────────────┤
│ id (PK)                  │
│ queue                    │
│ payload                  │
│ attempts                 │
│ reserved_at              │
│ available_at             │
│ created_at               │
└──────────────────────────┘
```

### 3.3 Descripción de Tablas

#### 3.3.1 Tabla `users`

| Campo | Tipo | Restricciones | Descripción |
|-------|------|---------------|-------------|
| `id` | INT | PK, AUTO_INCREMENT | Identificador único |
| `name` | VARCHAR(255) | NOT NULL | Nombre del usuario |
| `email` | VARCHAR(255) | NOT NULL, UNIQUE | Email único |
| `email_verified_at` | TIMESTAMP | NULLABLE | Verificación de email |
| `password` | VARCHAR(255) | NOT NULL | Contraseña hasheada (BCrypt) |
| `avatar` | VARCHAR(255) | NULLABLE | URL del avatar |
| `remember_token` | VARCHAR(100) | NULLABLE | Token de "recordarme" |
| `created_at` | TIMESTAMP | NOT NULL | Fecha creación |
| `updated_at` | TIMESTAMP | NOT NULL | Fecha última actualización |

**Indices**:
- PRIMARY KEY: `id`
- UNIQUE: `email`

**Validaciones**:
- Email único y válido
- Contraseña mínimo 8 caracteres
- Nombre no vacío

#### 3.3.2 Tabla `bubbles`

| Campo | Tipo | Restricciones | Descripción |
|-------|------|---------------|-------------|
| `id` | INT | PK, AUTO_INCREMENT | Identificador único |
| `user_id` | INT | FK → users(id), NOT NULL | Usuario propietario |
| `latitude` | DECIMAL(10,8) | NOT NULL | Latitud geográfica |
| `longitude` | DECIMAL(11,8) | NOT NULL | Longitud geográfica |
| `mensaje` | TEXT | NOT NULL | Contenido del mensaje |
| `created_at` | TIMESTAMP | NOT NULL | Fecha creación |
| `updated_at` | TIMESTAMP | NOT NULL | Fecha última actualización |

**Indices**:
- PRIMARY KEY: `id`
- FOREIGN KEY: `user_id` → `users(id)`

**Validaciones**:
- Latitud entre -90 y 90
- Longitud entre -180 y 180
- Mensaje no vacío (máx 1000 caracteres)
- Usuario debe existir

#### 3.3.3 Tabla `personal_access_tokens` (Sanctum)

| Campo | Tipo | Restricciones | Descripción |
|-------|------|---------------|-------------|
| `id` | INT | PK, AUTO_INCREMENT | Identificador único |
| `tokenable_type` | VARCHAR(255) | NOT NULL | Tipo de modelo ("App\Models\User") |
| `tokenable_id` | INT | NOT NULL | ID del usuario |
| `name` | VARCHAR(255) | NOT NULL | Nombre del token |
| `token` | VARCHAR(80) | NOT NULL, UNIQUE | Token hasheado |
| `abilities` | JSON | NULLABLE | Permisos/habilidades |
| `last_used_at` | TIMESTAMP | NULLABLE | Último uso |
| `created_at` | TIMESTAMP | NOT NULL | Fecha creación |
| `updated_at` | TIMESTAMP | NOT NULL | Fecha actualización |

**Uso**: Autenticación API Sanctum

#### 3.3.4 Tabla `sessions`

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id` | VARCHAR(255) | ID de sesión |
| `user_id` | INT | Usuario (nullable) |
| `ip_address` | VARCHAR(45) | Dirección IP |
| `user_agent` | TEXT | Browser/Client info |
| `payload` | LONGTEXT | Datos de sesión |
| `last_activity` | INT | Timestamp último acceso |

**Uso**: Gestión de sesiones web tradicionales

### 3.4 Relaciones Eloquent

```php
// User.php
class User extends Model {
    public function bubbles() {
        return $this->hasMany(Bubble::class);
    }
}

// Bubble.php
class Bubble extends Model {
    public function user() {
        return $this->belongsTo(User::class);
    }
}
```

### 3.5 Migraciones

Las migraciones de Laravel están en `database/migrations/`:

1. **`0001_01_01_000000_create_users_table.php`**
   - Crea tabla `users`, `password_reset_tokens`, `sessions`

2. **`0001_01_01_000001_create_cache_table.php`**
   - Crea tabla `cache` para almacenamiento en caché

3. **`0001_01_01_000002_create_jobs_table.php`**
   - Crea tabla `jobs` para cola de trabajos

4. **`2026_01_28_114031_create_personal_access_tokens_table.php`**
   - Crea tabla `personal_access_tokens` (Sanctum)

5. **`2026_01_28_120139_create_bubbles_table.php`**
   - Crea tabla `bubbles` con relación a `users`

**Ejecución de migraciones:**
```bash
php artisan migrate
```

**Revertir última migración:**
```bash
php artisan migrate:rollback
```

### 3.6 Seeders

Seeders disponibles en `database/seeders/`:
- `DatabaseSeeder.php` - Ejecuta seeders principales

**Ejecutar seeders:**
```bash
php artisan db:seed
```

---

## 4. Aplicación Web

### 4.1 Descripción

**Aplicación Web Full-Stack** integrada directamente en Laravel como **Single-Page Application (SPA)** con renderizado del lado del servidor (SSR). Implementa un sistema de localización geo-referenciada con mapa interactivo.

**Ubicación**: `bd/bubble-api/` (integrada en Laravel)

### 4.2 Tecnología

| Componente | Tecnología | Versión |
|------------|-----------|---------|
| **Backend** | Laravel | 11+ |
| **Lenguaje Backend** | PHP | 8.2+ |
| **Frontend Rendering** | Blade (SSR) | Laravel nativa |
| **CSS** | Tailwind CSS | v4.0.0 |
| **JavaScript** | Vanilla JS + Axios | ES6+ |
| **Mapas** | Google Maps API | v3 |
| **Build Tool** | Vite | v7.0.7 |
| **Package Manager** | npm | - |

### 4.3 Estructura del Proyecto

```
bubble-api/
├── resources/
│   ├── views/
│   │   ├── welcome.blade.php          (Home page)
│   │   ├── login.blade.php            (Login form)
│   │   ├── registro.blade.php         (Registration form)
│   │   ├── muro.blade.php             (Main map interface)
│   │   └── perfil.blade.php           (User profile editor)
│   ├── js/
│   │   ├── app.js                     (Entry point)
│   │   └── bootstrap.js               (Axios configuration)
│   └── css/
│       └── app.css                    (CSS + custom styles)
├── app/Http/Controllers/
│   ├── ControladorRegistro.php        (Auth: login/register/logout)
│   ├── BubbleController.php           (Bubble CRUD + muro view)
│   └── ProfileController.php          (Profile management)
├── routes/
│   ├── web.php                        (Web routes - 17 rutas)
│   └── api.php                        (API routes - desktop/mobile)
├── config/
│   ├── auth.php                       (Session-based auth)
│   └── session.php
├── public/
│   ├── index.php
│   ├── img/
│   │   ├── logo.gif
│   │   └── fondo.gif
│   └── storage/                       (User avatars & uploads)
├── vite.config.js
├── package.json
└── composer.json
```

### 4.4 Rutas Web

| Ruta | Método | Auth | Controlador | Propósito |
|------|--------|------|-------------|----------|
| `/` | GET | No | BubbleController | Home (redirige a login) |
| `/login` | GET | No | ControladorRegistro | Formulario login |
| `/login` | POST | No | ControladorRegistro | Procesar login |
| `/registro` | GET | No | ControladorRegistro | Formulario registro |
| `/registro` | POST | No | ControladorRegistro | Procesar registro |
| `/muro` | GET | **Sí** | BubbleController | Mapa principal (protegido) |
| `/muro` | POST | **Sí** | BubbleController | Crear/actualizar burbuja |
| `/logout` | POST | **Sí** | ControladorRegistro | Cerrar sesión |
| `/perfil` | GET | **Sí** | ProfileController | Editar perfil |
| `/perfil` | PATCH | **Sí** | ProfileController | Guardar perfil |
| `/api/notificaciones` | GET | No | BubbleController | AJAX radar (actualiza cada 2s) |


### 4.5 Vistas Principales

#### 4.5.1 `login.blade.php`

**Características**:
- Formulario de autenticación
- Diseño dark theme (#131313)
- Fondo animado GIF
- Layout responsive (2 columnas en desktop, 1 en móvil)
- Links para recuperar contraseña y registro

```html
<!-- Form structure -->
<form method="POST" action="/login">
  <input type="email" name="email" required>
  <input type="password" name="password" required>
  <button type="submit">Iniciar Sesión</button>
  <label><input type="checkbox" name="remember"> Recuérdame</label>
</form>
```

#### 4.5.2 `registro.blade.php`

**Características**:
- Formulario de registro
- Diseño blue/purple theme (#3b4cca)
- Validación de campos
- Campos: nombre, email, contraseña

```html
<form method="POST" action="/registro">
  <input type="text" name="name" placeholder="Nombre de usuario" required>
  <input type="email" name="email" placeholder="Correo electrónico" required>
  <input type="password" name="password" placeholder="Contraseña" required>
  <input type="password" name="password_confirmation" placeholder="Confirmar contraseña" required>
  <button type="submit">Registrarse</button>
</form>
```

#### 4.5.3 `muro.blade.php` - Pantalla Principal

**Arquitectura de Layout**:
```
┌─────────────────────────────────────┐
│           Google Maps               │  3 columnas: sidebar (300px) | mapa | panel perfil (250px)
│ ● User location (with arrow)        │
│ ◯ Nearby bubbles (distance)        │
│                                     │
│                                     │
└─────────────────────────────────────┘
```

**Funcionalidades**:

1. **Columna Izquierda (Radar)**
   - Lista de burbujas cercanas
   - Muestra: Avatar, nombre, distancia
   - Actualiza cada 2 segundos via AJAX
   - Colores: Verde (#2ecc71) para burbujas activas

2. **Centro (Google Maps)**
   - Mapa interactivo con zoom 17x y tilt 45°
   - Marcador de usuario (posición actual)
   - Círculos/marcadores para burbujas
   - Geolocalización automática (`navigator.geolocation.watchPosition()`)
   - Capa de tráfico opcional

3. **Columna Derecha (Perfil de Usuario)**
   - Avatar del usuario autenticado
   - Nombre
   - Botón toggle para crear/eliminar burbuja actual
   - Botón logout
   - Link a perfil completo

**JavaScript Principal**:
```javascript
// Geolocalización continua y actualización de mapa
navigator.geolocation.watchPosition((position) => {
  const lat = position.coords.latitude;
  const lng = position.coords.longitude;
  updateMap(lat, lng);
  updateRadar(); // Actualiza lista cada 2s
});

// Toggle crear/eliminar burbuja
document.getElementById('toggle-burbuja').addEventListener('change', (e) => {
  if (e.target.checked) {
    createBubble(currentLat, currentLng);
  } else {
    deleteBubble();
  }
});
```

#### 4.5.4 `perfil.blade.php` - Editor de Perfil

**Características**:
- Edición de nombre de usuario
- Subida de avatar (imagen)
- Preview del avatar
- Botón volver a `/muro`
- Validación de servidor

```html
<form method="PATCH" action="/perfil" enctype="multipart/form-data">
  <input type="text" name="name" value="{{ auth()->user()->name }}">
  <input type="file" name="avatar" accept="image/*">
  <img id="preview" src="{{ asset('storage/avatars/' . auth()->user()->avatar) }}">
  <button type="submit">Guardar Perfil</button>
</form>
```

### 4.6 Autenticación (Session-Based)

**Flujo de Autenticación Web**:
```
1. Usuario accede a /
   └─> Redirige a /login (middleware web)

2. Usuario ingresa email + password
   └─> POST /login
   └─> ControladorRegistro::login()
   └─> Auth::attempt($credentials)
   └─> Session creada (config/session.php)
   └─> Redirige a /muro

3. En /muro protegida por middleware('auth')
   └─> Auth::user() disponible en Blade
   └─> session('user_id') accesible

4. Logout
   └─> POST /logout
   └─> Auth::logout()
   └─> Session destruida
   └─> Redirige a /login
```

**Guard de Autenticación**:
```php
// config/auth.php
'guards' => [
    'web' => [
        'driver' => 'session',
        'provider' => 'users',
    ],
],

'providers' => [
    'users' => [
        'driver' => 'eloquent',
        'model' => App\Models\User::class,
    ],
],
```

### 4.7 Conexión con API

La web utiliza **sesiones del lado del servidor**, NO tokens API Sanctum.

Para crear/actualizar datos, usa rutas web que procesan el formulario:

```php
// POST /muro - Crear burbuja
Route::post('/muro', [BubbleController::class, 'guardar'])->middleware('auth');

// PATCH /perfil - Actualizar perfil
Route::patch('/perfil', [ProfileController::class, 'update'])->middleware('auth');

// POST /logout - Logout
Route::post('/logout', [ControladorRegistro::class, 'logout'])->middleware('auth');
```

**Cliente JavaScript** (Axios):
```javascript
// resources/js/bootstrap.js
window.axios = axios.create({
  baseURL: '/',
  headers: {
    'X-Requested-With': 'XMLHttpRequest',
    'X-CSRF-TOKEN': document.querySelector('meta[name="csrf-token"]').content,
  },
});

// Uso en vistas
fetch('/api/notificaciones') // Retorna HTML para radar
  .then(r => r.text())
  .then(html => document.getElementById('radar').innerHTML = html);
```

### 4.8 Estilos y Diseño

**CSS Framework**: Tailwind CSS v4

**Tema de Colores**:
- **Principal**: #3b4cca (Azul)
- **Fondo**: #131313 (Oscuro)
- **Acento**: #2ecc71 (Verde)
- **Paneles**: rgba(255,255,255,0.9) (Blanco translúcido)
- **Error**: #ff4d4d (Rojo)

**Configuración de Tailwind** (`resources/css/app.css`):
```css
@import "tailwindcss";

@theme {
  --color-primary: #3b4cca;
  --color-dark: #131313;
  --color-accent: #2ecc71;
}

.btn-primary {
  @apply px-4 py-2 bg-primary text-white rounded-lg hover:opacity-90;
}

.panel {
  @apply bg-white bg-opacity-90 backdrop-blur rounded-3xl shadow-lg p-6;
}
```

**Fuentes**:
- Body: 'Instrument Sans' (Google Fonts)
- Monospace (Perfil, Muro): 'Roboto Mono'
- Icons: Font Awesome 6.0 (CDN)

### 4.9 Pipeline de Build (Vite)

**Configuración** (`vite.config.js`):
```javascript
import { defineConfig } from 'vite';
import laravel from 'laravel-vite-plugin';
import tailwindcss from '@tailwindcss/vite';

export default defineConfig({
  plugins: [
    laravel(['resources/css/app.css', 'resources/js/app.js']),
    tailwindcss(),
  ],
});
```

**Scripts npm**:
```bash
npm run dev     # Inicia servidor Vite (http://localhost:5173)
npm run build   # Construye para producción (public/build/)
npm run preview # Preview del build
```

**Desarrollo**:
```bash
# Terminal 1: Laravel
php artisan serve

# Terminal 2: Vite
npm run dev

# Acceder a: http://localhost:8000
```

### 4.10 Funciones Principales de Controladores

**ControladorRegistro.php**:
- `showLogin()` - Muestra formulario login
- `login()` - Procesa login (valida email/password)
- `showRegistro()` - Muestra formulario registro
- `registro()` - Crea usuario y lo autentica
- `logout()` - Cierra sesión

**BubbleController.php**:
- `mostrarMuro()` - Carga página principal con mapa
- `crearBurbuja()` - Crea burbuja en coordenadas GPS
- `eliminarBurbuja()` - Elimina burbuja del usuario
- `getNotificacionesAjax()` - Retorna burbujas cercanas (AJAX)

### 4.11 Características Principales

1. **Geolocalización en Tiempo Real**
   - `navigator.geolocation.watchPosition()` continuo
   - Alta precisión habilitada
   - Centro automático del mapa

2. **Radar de Burbujas**
   - Actualización AJAX cada 2 segundos
   - Distancia calculada con fórmula Haversine
   - Radio de búsqueda: 5 km

3. **Gestión de Burbujas**
   - Toggle checkbox para activar/desactivar
   - Una burbuja por usuario (la anterior se borra)
   - Coordenadas GPS automáticas

4. **Perfil de Usuario**
   - Avatar subible (almacenado en `storage/avatars/`)
   - Nombre editable
   - Fallback a UI-avatars API si no hay avatar

5. **Mapa Interactivo**
   - Google Maps v3
   - Zoom 17x, tilt 45°
   - Marcador usuario con flecha de dirección
   - Círculos para burbujas cercanas

### 4.12 Configuración de Google Maps

Obtener API Key en Google Cloud Console e incluir en `muro.blade.php`:

```html
<script src="https://maps.googleapis.com/maps/api/js?key=YOUR_API_KEY"></script>

<script>
  var map = new google.maps.Map(document.getElementById('map'), {
    zoom: 17,
    tilt: 45,
    center: {lat: 40.4168, lng: -3.7038}, // Madrid por defecto
  });
</script>
```

---

## 5. Aplicación Móvil

### 5.1 Descripción

**Aplicación Android Nativa** para visualizar y crear "bubbles" (mensajes geo-localizados) en tiempo real.

**Ruta**: `Movil/Bubble/`

### 5.2 Tecnología

- **Lenguaje**: Kotlin/Java
- **SDK Mínimo**: Android 7.0 (API 24)
- **SDK Destino**: Android 15 (API 36)
- **Build System**: Gradle Kotlin DSL
- **Namespace**: `com.example.bubble`

### 5.3 Dependencias Principales

```gradle
// Google Play Services
implementation("com.google.android.gms:play-services-maps:18.2.0")
implementation("com.google.android.gms:play-services-location:21.0.1")

// Utilities
implementation("org.mindrot:jbcrypt:0.4")           // Password hashing
implementation("com.github.bumptech.glide:glide:4.16.0") // Image loading
implementation("androidx.coordinatorlayout:coordinatorlayout:1.2.0")

// Material Design
implementation("com.google.android.material:material:1.9.0")
```

### 5.4 Permisos Necesarios (AndroidManifest.xml)

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
<uses-permission android:name="android.permission.VIBRATE" />
```

### 5.5 Actividades Principales

| Activity | Propósito | Flujo |
|----------|----------|-------|
| `Welcome` | Pantalla de bienvenida | Launcher → Login/Registro |
| `LogIn` | Login de usuario | Ingreso de credenciales |
| `Registro` | Registro de nuevo usuario | Creación de cuenta |
| `Principal` | Pantalla principal | Mapa + Lista de burbujas |

### 5.6 Flujo de la Aplicación

```
┌─────────────┐
│   Welcome   │  (Launcher)
└──────┬──────┘
       │
       ├──────────────────┬──────────────────┐
       │                  │                  │
       ▼                  ▼                  ▼
   ┌────────┐        ┌────────┐         ┌────────┐
   │ LogIn  │        │Registro│ (Alt)   │Principal│
   └───┬────┘        └────┬───┘         └────────┘
       │                  │                  ▲
       │                  │                  │
       └──────────┬───────┘                  │
                  │                         │
                  ▼                         │
           ┌─────────────────────────────┐ │
           │ API Authentication          │ │
           │ (POST /desktop/login)       │─┘
           │ (POST /desktop/register)    │
           └─────────────────────────────┘
```

### 5.7 Características Principales

1. **Geolocalización**
   - Obtiene posición actual del dispositivo
   - Google Maps API para visualización
   - Radio de búsqueda configurable

2. **Autenticación**
   - Login/Registro contra API
   - Token almacenado localmente (SharedPreferences)
   - Contraseñas hasheadas con BCrypt

3. **Visualización de Burbujas**
   - Mapa interactivo con marcadores
   - Lista de burbujas cercanas
   - Información del usuario (avatar, nombre)

4. **Crear Burbujas**
   - Formulario con mensaje y ubicación
   - POST a `/api/desktop/bubble` o `/api/burbujas`

### 5.8 Estructura del Proyecto

```
Movil/Bubble/
├── app/
│   └── src/
│       ├── main/
│       │   ├── java/com/example/bubble/
│       │   │   ├── activities/
│       │   │   │   ├── Welcome.kt
│       │   │   │   ├── LogIn.kt
│       │   │   │   ├── Registro.kt
│       │   │   │   └── Principal.kt
│       │   │   ├── models/
│       │   │   │   ├── User.kt
│       │   │   │   └── Bubble.kt
│       │   │   └── services/
│       │   │       └── ApiClient.kt
│       │   ├── res/
│       │   │   ├── layout/
│       │   │   ├── values/
│       │   │   ├── drawable/
│       │   │   └── menu/
│       │   └── AndroidManifest.xml
│       └── test/
├── build.gradle.kts
└── gradle.properties
```

### 5.9 Configuración de Google Maps

1. Generar API Key en Google Cloud Console
2. Agregar a `AndroidManifest.xml`:
   ```xml
   <meta-data
       android:name="com.google.android.geo.API_KEY"
       android:value="YOUR_API_KEY_HERE" />
   ```

---

## 6. Aplicación de Escritorio

### 6.1 Descripción

**Aplicación Windows Nativa** en .NET Framework para gestión de burbujas con interfaz gráfica completa.

**Ruta**: `Escritorio/BubbleApp/`

### 6.2 Tecnología

- **Lenguaje**: C#
- **Framework**: .NET Framework 4.7.2
- **UI**: Windows Forms (WinForms)
- **Proyecto**: `BubbleApp.sln`

### 6.3 Dependencias Principales

```xml
<!-- BubbleApp.csproj -->
<Reference Include="System.Windows.Forms" />
<Reference Include="System.Drawing" />
<Reference Include="System.Data" />
<PackageReference Include="Microsoft.Web.WebView2" Version="1.0.2903.40" />
```

### 6.4 Estructura del Proyecto

```
Escritorio/BubbleApp/
├── BubbleApp.sln
├── BubbleApp/
│   ├── Formularios/
│   │   ├── FormularioInicioSesion.cs        (Login Form)
│   │   ├── FormularioPrincipal.cs           (Main Window)
│   │   ├── FormularioPerfil.cs              (User Profile)
│   │   ├── FormularioRegistro.cs            (Registration)
│   │   └── *.designer.cs                    (Designer files)
│   ├── Controles/
│   │   └── ControlMapaRadar.cs              (Custom Radar Map)
│   ├── Modelos/
│   │   ├── ModeloBurbuja.cs                 (Bubble model)
│   │   ├── ModeloUsuario.cs                 (User model)
│   │   └── ModeloPerfilUsuario.cs           (User Profile model)
│   ├── Servicios/
│   │   └── ApiCliente.cs                    (HTTP API Client)
│   ├── Estado/
│   │   └── SesionActual.cs                  (Current session)
│   ├── Programa.cs                          (Main entry point)
│   ├── App.config
│   ├── BubbleApp.csproj
│   └── bin/ & obj/
└── .vs/                                     (Visual Studio metadata)
```

### 6.5 Formularios Principales

#### 6.5.1 FormularioInicioSesion

- Pantalla de login/registro
- Campos: Email, Contraseña
- Botones: "Iniciar Sesión", "Registrarse"
- Validaciones locales + llamadas a API

#### 6.5.2 FormularioPrincipal

- Ventana principal de la aplicación
- Mapa personalizado (ControlMapaRadar)
- Lista de burbujas cercanas
- Menú para crear nuevas burbujas
- Información del usuario (perfil)

#### 6.5.3 FormularioPerfil

- Visualizar datos del usuario
- Editar nombre, avatar
- Ver historial de burbujas creadas
- Botón cerrar sesión

#### 6.5.4 FormularioRegistro

- Formulario de registro
- Campos: Nombre, Email, Contraseña, Confirmar Contraseña
- Validaciones
- Guardado en BD a través de API

### 6.6 ApiCliente (Servicio)

Cliente HTTP para comunicarse con la API Laravel:

```csharp
public class ApiCliente {
    private const string BaseUrl = "http://localhost:8000/api";
    private string? _token;
    
    public async Task<LoginResponse> Login(string email, string password);
    public async Task<RegisterResponse> Register(string name, string email, string password);
    public async Task<List<Bubble>> ObtenerBurbujas();
    public async Task<Bubble> CrearBurbuja(double lat, double lng, string mensaje);
    public async Task<User> ObtenerPerfil();
    public async Task<bool> ActualizarPerfil(string name, string avatar);
    public async Task Logout();
}
```

### 6.7 Flujo de la Aplicación

```
Inicio
   │
   ├─ Verificar sesión guardada (token)
   │
   ├─ Sí → FormularioPrincipal
   │
   └─ No → FormularioInicioSesion
            │
            ├─ Login exitoso → FormularioPrincipal
            │
            └─ Registrarse → FormularioRegistro → FormularioPrincipal
```

### 6.8 ControlMapaRadar

Control personalizado que representa:
- Posición del usuario (centro)
- Burbujas cercanas (puntos en radio)
- Interactividad (zoom, pan)
- Colores por tipo o antigüedad

---

## 7. Seguridad

### 7.1 Mecanismos de Seguridad Principales

| Mecanismo | Descripción |
|-----------|-------------|
| **Autenticación Sanctum** | Tokens API para desktop/móvil + Sessions para web |
| **BCrypt Hashing** | Contraseñas hasheadas con 10 rondas mínimas |
| **HTTPS/TLS** | Certificado SSL obligatorio en producción |
| **SQL Injection Prevention** | Eloquent ORM con prepared statements |
| **CSRF Protection** | Tokens CSRF en formularios web |
| **Password Validation** | Email único, contraseña mín 8 caracteres |
| **Rate Limiting** | Límite de peticiones por IP/usuario |

### 7.2 Autenticación

**API (Desktop/Móvil)**:
- Login: envía email + password
- Servidor responde con token
- Próximas peticiones incluyen: `Authorization: Bearer {TOKEN}`
- Tokens almacenados en tabla `personal_access_tokens`

**Web**:
- Session-based (no tokens)
- Cookie de sesión después del login
- Middleware `auth` protege rutas

### 7.3 Datos Sensibles

- **Contraseñas**: Hasheadas con BCrypt
- **Tokens**: Almacenados hasheados en BD
- **Sessions**: Archivo seguro con permisos restringidos
- **API Keys**: Guardar en `.env` (nunca en código)

**Validaciones en modelos**:
```php
protected $fillable = ['name', 'email', 'password'];
protected $hidden = ['password', 'remember_token'];
```

### 7.4 Configuración de Producción

```env
# .env
APP_ENV=production
APP_DEBUG=false
SESSION_SECURE_COOKIES=true
SANCTUM_STATEFUL_DOMAINS=bubble.com
```

---

## 8. Despliegue

### 8.1 Ejecución en Local

#### 8.1.1 Requisitos Previos

**Backend (API)**:
- PHP 8.2+
- Composer
- SQLite3
- Node.js + npm (para Vite)

**Móvil (Android)**:
- Android Studio
- SDK de Android (API 24-36)
- Gradle

**Escritorio (.NET)**:
- Visual Studio 2019+ o Visual Studio Code
- .NET Framework 4.7.2 (Windows)

#### 8.1.2 Instalación Backend

```bash
# 1. Navegar a carpeta API
cd bd/bubble-api

# 2. Instalar dependencias PHP
composer install

# 3. Copiar archivo .env
cp .env.example .env

# 4. Generar clave de aplicación
php artisan key:generate

# 5. Ejecutar migraciones
php artisan migrate

# 6. (Opcional) Ejecutar seeders
php artisan db:seed

# 7. Iniciar servidor de desarrollo
php artisan serve

# Servidor activo en: http://localhost:8000
```

**Alternativamente con Vite**:
```bash
npm install
npm run dev
```

#### 8.1.3 Instalación Móvil (Android)

```bash
# 1. Abrir en Android Studio
Open > Movil/Bubble

# 2. Sincronizar Gradle
Build > Make Project

# 3. Conectar dispositivo/emulador Android
# Dispositivo real: Activar USB Debugging
# Emulador: Crear desde Device Manager

# 4. Ejecutar app
Run > Run 'app' (Shift+F10)
```

#### 8.1.4 Instalación Escritorio (.NET)

```bash
# 1. Abrir solución
Escritorio/BubbleApp/BubbleApp.sln

# 2. En Visual Studio
Build > Build Solution

# 3. Ejecutar
Debug > Start Debugging (F5)
```

### 8.2 Despliegue de Producción

**Backend API en servidor (VPS/Hosting)**:
1. SSH al servidor
2. Clonar repositorio: `git clone ...`
3. Instalar dependencias: `composer install`
4. Configurar `.env` con BD MySQL/PostgreSQL
5. Generar key: `php artisan key:generate`
6. Migraciones: `php artisan migrate`
7. Configurar web server (Nginx o Apache)
8. Habilitar HTTPS con Let's Encrypt

**Base de datos**: Cambiar de SQLite a MySQL/PostgreSQL en producción

**Dominio**: Apuntar DNS a IP del servidor

---

### 8.3 Cambiar API URL por Entorno

**Mobile Android** (`ApiClient.kt`):
```kotlin
private const val BASE_URL = 
    if (BuildConfig.DEBUG) "http://localhost:8000/api"
    else "https://api.bubble.com/api"
```

**Desktop .NET** (`App.config`):
```xml
<add key="ApiBaseUrl" value="https://api.bubble.com/api" />

---

## 9. Pruebas

### 9.1 Pruebas Backend (PHPUnit)

```bash
php artisan test
```

Ejecuta pruebas unitarias e integración en `tests/Feature/`

### 9.2 Pruebas API (Postman/Insomnia)

Importar colección con endpoints:
- POST /api/desktop/login (credenciales válidas/inválidas)
- POST /api/desktop/register (registro)
- GET /api/burbujas (listar)
- POST /api/desktop/bubble (crear - con token)
- DELETE /api/desktop/bubble (eliminar)

### 9.3 Pruebas Móvil (Android)

En Android Studio:
```
Run Tests > Botón ▶
```

### 9.4 Pruebas Escritorio (.NET)

En Visual Studio:
```
Test > Run All Tests
```

---

## 10. Guía de Uso y Mantenimiento

### 10.1 Inicio Rápido

```bash
# Terminal 1: API
cd bd/bubble-api
php artisan serve
# http://localhost:8000

# Terminal 2: Android Studio (Móvil)
# Abrir Movil/Bubble > Run

# Terminal 3: Visual Studio (.NET Desktop)
# Abrir Escritorio/BubbleApp/BubbleApp.sln > Run
```

### 10.2 Problemas Comunes

| Problema | Solución |
|----------|----------|
| API no responde | `php artisan serve` en `bd/bubble-api` |
| Error 401 Unauthorized | Re-autenticarse (login nuevamente) |
| Error 422 Validation | Verificar datos requeridos |
| GPS no funciona (Móvil) | Activar GPS en dispositivo/emulador |
| Base de datos bloqueada | Cerrar otros procesos de Laravel |
| .NET no compila | Instalar .NET Framework 4.7.2 |

### 10.3 Agregar Nuevas Funcionalidades

**Nuevo endpoint API**:
```bash
php artisan make:controller Api/NuevaFuncionalidadController
# Agregar rutas en routes/api.php
# Implementar métodos en el controller
```

**Nueva pantalla Móvil**:
```kotlin
// Crear Activity en Android Studio
// Registrar en AndroidManifest.xml
// Navegar desde otra pantalla
```

**Nuevo formulario Escritorio**:
```csharp
// Crear formulario en Visual Studio (New Form)
// Agregar lógica en Program.cs
// Compilar y ejecutar
```

---

## 11. Mejoras Futuras

### 11.1 Funcionalidades Próximas

1. **Notificaciones en Tiempo Real**
   - Push notifications (móvil)
   - WebSocket para actualizaciones
   - Email notifications

2. **Búsqueda y Filtrado**
   - Filtrar por distancia, fecha
   - Búsqueda por palabras clave
   - Historial de búsquedas

3. **Interacción Social**
   - Likes/reacciones
   - Comentarios
   - Seguir/Bloquear usuarios

4. **Multimedia**
   - Imágenes en burbujas
   - Galería de usuario
   - Compresión automática

5. **Internacionalización**
   - Soporte multiidioma
   - Traducciones (ES, EN, FR)

### 11.2 Optimizaciones

- **Backend**: Redis cache, índices BD, async jobs
- **Móvil**: Lazy loading, batería optimizada
- **Desktop**: Caché local, renderizado async
- **Web**: Code splitting, PWA, Service Workers
- **General**: CDN, logging centralizado, APM


**Fin de Documentación**
