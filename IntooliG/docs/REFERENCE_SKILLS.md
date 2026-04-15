# REFERENCE_SKILLS — Biblia de reglas (Intooligence → C# + Angular)

Este documento resume la arquitectura y convenciones del proyecto **actual** en el repositorio y define reglas **ejecutables** para reconstruir un sistema equivalente con **backend C# moderno** (recomendado: ASP.NET Core Web API) y **frontend Angular**.

> **Hecho importante:** En este workspace la UI **no** es Angular: es **ASP.NET MVC 5** (`.NET Framework 4.8`) con **vistas Razor**, **jQuery**, **Bootstrap** y **Syncfusion EJ2** (grids, Excel). Las secciones de Angular describen el **objetivo** de un greenfield alineado con los mismos contratos y responsabilidades.

---

## 1. Panorama del proyecto actual

| Aspecto | Valor en el código |
|--------|---------------------|
| Host | IIS / IIS Express, aplicación web ASP.NET MVC 5 |
| Datos | **Entity Framework 6** (`EntityFramework` 6.5.1), contexto principal **`INTOOLIGENCEEntities`** (modelo EDMX / Database First) |
| API HTTP | No hay proyecto Web API separado con rutas `MapHttpRoute`; los endpoints JSON son **acciones MVC** que devuelven `JsonResult` |
| Autenticación | **Forms Authentication** + **sesión ASP.NET** (`Session["Cliente"]`, `Session["UserName"]`, etc.) |
| Autorización | Atributos personalizados + `[Authorize(Roles = "...")]` en algunas acciones |
| Front | Razor (`Views/`), bundles (`BundleConfig`), scripts en `Scripts/` |

---

## 2. Estructura de carpetas (actual vs objetivo greenfield)

### 2.1 Backend — estructura real en el repo

```
Intooligence/
├── App_Start/           # RouteConfig, BundleConfig, BuildSite (singleton utilidades/permisos)
├── Attributes/          # Filtros MVC: SessionCheck, AuthorizeSessionAttribute
├── Common/              # Helpers transversales (ej. bitácora)
├── Content/, Scripts/   # CSS/JS estáticos y Syncfusion
├── Controllers/         # Controladores MVC (lógica + acceso a datos en varios casos)
├── Models/              # Entidades EF, ViewModels, enums, EDMX
│   ├── DbContext/       # Contexto Code First alternativo (AplicationDbContext) — convivencia con EDMX
│   └── Entities/        # Vistas/entidades adicionales
├── Properties/
├── Provides/ o Providers/  # PrincipalProvider (IPrincipal)
├── Services/            # Lógica reutilizable (Excel, Power BI embed, validadores, AAD)
└── Views/               # Vistas Razor por controlador + Shared
```

**Patrones observados:**

- **Controllers** agrupan pantallas y endpoints JSON del mismo dominio (ej. `CampaniaController`).
- **Services** concentran integraciones (Syncfusion XlsIO, Microsoft.PowerBI.Api, MSAL/AAD) pero **no** sustituyen de forma uniforme al acceso directo a `INTOOLIGENCEEntities` en controladores.
- **Models** mezcla entidades de base (`tb*`), view models (`*ViewModel`), DTOs informales y mensajes (`MensajesGenericos`, `tipoMensajes`).
- **Attributes** implementan reglas transversales (sesión obligatoria, limpieza de sesión si falta usuario).

### 2.2 Backend — estructura recomendada (greenfield C#)

Objetivo: separar capas de forma explícita (el repo actual solo lo cumple parcialmente).

```
src/
├── Api/                          # Proyecto ASP.NET Core Web API
│   ├── Controllers/              # Solo orquestación HTTP; sin EF directo
│   ├── Middleware/               # Errores, correlación, etc.
│   └── Program.cs / Startup
├── Application/                  # Casos de uso, DTOs de aplicación, validación
│   ├── Features/
│   │   └── Campanias/
│   │       ├── Commands/
│   │       ├── Queries/
│   │       └── Dtos/
│   └── Abstractions/
├── Domain/                       # Entidades de dominio, reglas (opcional según complejidad)
├── Infrastructure/
│   ├── Persistence/              # EF Core DbContext, repositorios, migraciones
│   ├── Identity/                 # Auth (si aplica)
│   └── Integrations/             # Power BI, almacenamiento, colas
└── Shared/                       # Constantes, resultados comunes
```

**Regla:** En un proyecto nuevo, los **Controllers** solo llaman a **servicios de aplicación** o **mediators**; el **DbContext** vive en **Infrastructure**.

### 2.3 Frontend — estructura real vs Angular

**Real:** No hay `angular.json` ni `package.json` en la solución; la UI vive en `Views/<Controlador>/` y `Views/Shared/` (`_Layout.cshtml`, modales, partials).

**Objetivo Angular (alineado al dominio actual):**

```
src/app/
├── core/                 # Singletons: auth, HTTP interceptors, manejo de errores
├── shared/               # Componentes/pipes reutilizables
├── layout/               # Shell (sustituye _Layout)
└── features/
    ├── campania/
    │   ├── components/
    │   ├── services/     # Llama a la API (HttpClient)
    │   ├── models/       # Interfaces TypeScript (DTOs)
    │   └── guards/       # Auth, roles (sustituye parte de SessionCheck / permisos)
    ├── account/
    └── ...
```

---

## 3. Patrones de diseño

### 3.1 Inyección de dependencias (DI)

- **Actual:** No hay contenedor DI clásico en MVC 5; se instancia `new INTOOLIGENCEEntities()` en controladores y helpers. Hay referencia a `Microsoft.Extensions.DependencyInjection` (dependencias transitivas), pero **no** es el patrón principal de composición de la app.
- **Greenfield:** Registrar en **ASP.NET Core** `DbContext`, servicios de aplicación, `HttpClient` nombrados para APIs externas (Power BI, etc.) con **Scoped** para repos/servicios y **Transient/Singleton** según documentación.

### 3.2 Repositorio

- **Actual:** Acceso **directo** al `DbContext` en acciones; no hay interfaz `IRepository<T>` generalizada en todo el código.
- **Greenfield:** Opciones válidas: **Repositorio + Unit of Work** *o* **DbContext + servicios por agregado** (más simple). Lo importante es **no** duplicar consultas gigantes en controladores.

### 3.3 DTOs y ViewModels

- **Actual:** Clases `*ViewModel` para proyecciones de pantalla y listados (ej. `MarcaCampaniaViewModel`, `UsuarioViewModel`). Las tablas siguen prefijos como `Fi` (id), `Fc` (cadena), `Fb` (bool), reflejando el modelo físico.
- **Greenfield:** Exponer **DTOs de API** estables (request/response) distintos de entidades EF; usar **AutoMapper** o mapeo manual en la capa Application.

### 3.4 Respuestas JSON de la API

El código MVC devuelve objetos anónimos con forma recurrente, por ejemplo:

- Éxito con datos: `success`, `data`, `status` (200), `response` (1).
- Operación con mensaje: `success`, `message`, `tipo` (enum `tipoMensajes`: warning, sucess, error, info).
- Errores: mismos campos con `success: false` y textos de `MensajesGenericos`.

**Regla para Angular:** Definir interfaces TypeScript que reflejen estos contratos **o** estandarizar en greenfield a **Problem Details** (`application/problem+json`) y códigos HTTP, manteniendo un campo `traceId`.

**Configuración JSON global:** En `Global.asax.cs` se configura `ReferenceLoopHandling.Ignore` en el serializador JSON para evitar ciclos al serializar grafos EF.

---

## 4. Convenciones de código

### 4.1 C#

| Elemento | Convención en el repo | Recomendación greenfield |
|----------|------------------------|---------------------------|
| Clases, métodos, propiedades públicas | **PascalCase** | PascalCase |
| Campos privados | `_camelCase` ocasional (ej. `_tbCatMarcaProductoVersionFuenteList`) | `_camelCase` o prefijo explícito según estándar del equipo |
| Entidades BD | Prefijos `Fi`, `Fc`, `Fb`, `Fd` en columnas | Mantener solo en capa Persistence; DTOs con nombres legibles en API |
| Async | Mezcla de sync/async (Power BI async) | `async`/`await` end-to-end en I/O |
| Archivos | Un tipo principal por archivo | Igual |

### 4.2 TypeScript / Angular (objetivo)

| Elemento | Convención |
|----------|------------|
| Clases, interfaces, enums, componentes | **PascalCase** |
| variables, parámetros, métodos | **camelCase** |
| selectores y archivos de componente | `kebab-case` (estilo Angular CLI) |
| Observables | sufijo `$` opcional solo si el equipo lo usa de forma uniforme |

### 4.3 Errores y logs

- **Actual:** Mensajes centralizados en `MensajesGenericos` y tipo visual en `tipoMensajes`; `try/catch` con `throw` o retorno JSON de error; **bitácora** en BD vía clase `bitacora` en `Common`. No hay uso generalizado de **ILogger**, NLog o Serilog en el código revisado.
- **Greenfield C#:** `ILogger<T>` por clase, correlación de solicitudes, no loguear datos sensibles.
- **Greenfield Angular:** Interceptor HTTP que muestre errores de red/401/403 de forma uniforme; opcional servicio de notificaciones equivalente a modales (`_ModalMensaje.cshtml`).

---

## 5. Flujo de datos (backend ↔ frontend)

### 5.1 Actual (MVC + Razor + AJAX)

1. El navegador carga una vista Razor (`Index.cshtml`) con layout y scripts (bundles jQuery, Syncfusion EJ2).
2. Componentes de grid llaman por **AJAX** a acciones del mismo sitio, por ejemplo `Campania/GetDataCampania` (método público en `CampaniaController`).
3. El controlador usa **sesión** (`Session["Cliente"]`) para filtrar por cliente.
4. La respuesta es **JSON** (`JsonResult`) con el envelope descrito arriba.
5. **FormsAuthentication** mantiene cookie; filtros `SessionCheck` / `AuthorizeSession` redirigen a login si no hay sesión válida.

### 5.2 Objetivo (ASP.NET Core + Angular)

1. **Angular** usa `HttpClient` contra `https://api.../api/campanias` (rutas REST explícitas).
2. **Autenticación:** JWT en `Authorization: Bearer` o cookies HttpOnly según decisión de seguridad; **no** depender de `Session` del servidor para estado de negocio.
3. **CORS:** Configurado en API para el origen del SPA.
4. **Mapeo 1:1** útil durante migración: cada antigua acción `GetData*` puede convertirse en `GET` con query parameters equivalentes.

**Ejemplo conceptual de pareja endpoint ↔ servicio Angular:**

| Antes (MVC) | Después (API + Angular) |
|-------------|-------------------------|
| `GET /Campania/GetDataCampania` | `GET /api/campanias` con `HttpClient.get<CampaniaDto[]>(...)` |
| `POST /Campania/Guardar...` | `POST /api/campanias` con cuerpo DTO |

---

## 6. Seguridad y permisos (resumen)

- **Sesión:** `SessionCheck` exige `Session["Cliente"]`; si falta → redirección a `~/Account/Login`.
- **AuthorizeSession:** Si no hay `Session["UserName"]`, limpia sesión, hace `FormsAuthentication.SignOut()` y falla autorización.
- **Roles:** `[Authorize(Roles = "SuperUsuario, Administrador")]` en acciones como registro de usuarios.
- **Permisos granulares:** `BuildSite.Instancia` resuelve permisos por rol consultando tablas de rol/permiso/entidad/módulo (similar a **claims** en un sistema moderno).

**Greenfield:** Modelar permisos como **claims** o **políticas** (`IAuthorizationHandler`) en lugar de singleton estático, manteniendo la misma matriz rol → entidad → operación.

---

## 7. Checklist: añadir una nueva funcionalidad (entidad de negocio)

### 7.1 En el estilo actual del repo (MVC + EF6)

1. **Base de datos:** Tablas/columnas o SP según estándar existente (`tb*`, prefijos `Fi`/`Fc`/…).
2. **Modelo:** Actualizar EDMX / entidades generadas o agregar clase parcial si aplica.
3. **ViewModel:** Crear `MiEntidadViewModel` para lo que ve el grid o el formulario.
4. **Controlador:** Acciones `Index` (vista), `GetData*` (JSON), `AddOrEdit` (GET/POST), con `[SessionCheck]` / `[AuthorizeSession]` si corresponde.
5. **Vista:** `Views/MiEntidad/Index.cshtml` + modales/partials según patrón de Syncfusion usado en otras pantallas.
6. **Mensajes:** Reutilizar o extender `MensajesGenericos` / `tipoMensajes` para respuestas JSON coherentes.
7. **Pruebas manuales:** Flujo completo con usuario de prueba y validación de permisos en `BuildSite` si la pantalla es restringida.

### 7.2 Greenfield (API + Angular) — equivalente estructurado

1. **Dominio / persistencia:** Entidad + configuración EF Core + migración.
2. **Contrato API:** DTOs request/response (sin exponer entidades).
3. **Application:** Comando o query (validación, reglas de negocio).
4. **API:** Controller delgado que devuelve `ActionResult<T>` o `IResult`.
5. **Angular:** `models/mi-entidad.dto.ts`, `services/mi-entidad.service.ts` (`HttpClient`), componentes y rutas en `features/mi-entidad/`.
6. **Auth:** Guard de ruta + directivas o servicios de permiso alineados a claims del backend.
7. **Pruebas:** Unit tests en Application; integración en API; e2e opcional en Angular.

---

## 8. Dependencias clave

### 8.1 NuGet (proyecto actual — `packages.config`)

**Stack web y datos**

- `Microsoft.AspNet.Mvc` 5.3.0 — MVC 5  
- `Microsoft.AspNet.WebApi.*` 5.2.6 — referenciado; el enrutamiento Web API clásico no está centralizado como en plantillas API puras  
- `EntityFramework` 6.5.1  
- `Newtonsoft.Json` 13.0.3  
- `Microsoft.AspNet.Web.Optimization` — bundling/minificación  
- `bootstrap` 4.1.0, `jQuery` 3.3.1 — UI base  

**Syncfusion (grids, Excel, licencia)**

- `Syncfusion.EJ2.MVC5`, `Syncfusion.EJ2.JavaScript`, `Syncfusion.Licensing`  
- `Syncfusion.XlsIO.AspNet.Mvc5` (import/export Excel en servidor)  

**Integraciones**

- `Microsoft.PowerBI.Api`, `Microsoft.Rest.ClientRuntime` — informes embebidos  
- `Microsoft.Identity.Client` — adquisición de token (AAD) para Power BI  

**Transitive / utilidades**

- Varios `Microsoft.AspNetCore.*` y `Microsoft.Extensions.*` (uso indirecto o preparación de abstracciones), `System.Text.Encodings.Web`, etc.

### 8.2 npm / Angular (greenfield — no presentes en este repo)

Para un SPA equivalente se suele añadir, como mínimo:

- `@angular/core`, `@angular/common`, `@angular/router`, `@angular/forms`, `@angular/platform-browser`  
- `rxjs`  
- Herramientas: Angular CLI, TypeScript, ESLint/Prettier según política del equipo  

Opcional según necesidad: `@angular/material` o equivalente UI; librería de tablas si se reemplaza Syncfusion.

---

## 9. Deuda técnica explícita (para no replicar ciegamente)

- Controladores con **muchas responsabilidades** y **DbContext** instanciado en el controlador.
- Comentarios en código que sugieren **sustituir consultas directas por SP** para rendimiento/mantenibilidad.
- `Global.asax.cs`: redirección en `Session_Start` a login — comportamiento a revisionar en escenarios de API stateless.
- Convivencia de **EDMX** (`INTOOLIGENCEEntities`) con **DbContext** Code First en `Models/DbContext/` — en migración, unificar en un solo modelo EF Core.

---

## 10. Resumen ejecutivo

| Pregunta | Respuesta corta |
|----------|-------------------|
| ¿Capas? | MVC: Controllers / Models / Views / Services parciales. Greenfield: Api / Application / Domain / Infrastructure + Angular `features`. |
| ¿DI / Repositorio? | Poco DI en MVC actual; repos no generalizados. Greenfield: DI obligatorio; repositorios o servicios por agregado. |
| ¿DTOs? | ViewModels y proyecciones; envelope JSON manual. Greenfield: DTOs explícitos + contratos HTTP estándar. |
| ¿Cómo responde la API? | `JsonResult` con `success`, `data`, `message`, `tipo`, `status`. |
| ¿Front real? | Razor + jQuery + Syncfusion, **no** Angular en este repositorio. |

Este archivo debe actualizarse cuando se defina la pila exacta del greenfield (por ejemplo .NET 8 + Angular 18) y las políticas de autenticación (JWT vs cookies).
