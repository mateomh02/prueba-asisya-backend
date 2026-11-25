# Asisya - API de productos y categorías

API REST en .NET para exponer productos y categorías, con autenticación JWT, carga masiva de datos y despliegue en Docker.

> Estado: en desarrollo. Las secciones marcadas como **Pendiente** se completan a medida que se avanza.

## Stack

| Área | Tecnología |
|---|---|
| Backend | .NET 11, C#, ASP.NET Core (controllers) |
| Base de datos | PostgreSQL 17 con Entity Framework Core |
| Mensajería | RabbitMQ (carga masiva asíncrona) |
| Autenticación | JWT Bearer, contraseñas con BCrypt |
| Contenedores | Docker y docker-compose |
| CI | GitHub Actions |

## Arquitectura

Clean Architecture por capas. Las dependencias apuntan siempre hacia adentro:

```
Api  ->  Infrastructure  ->  Application  ->  Domain
```

| Proyecto | Responsabilidad |
|---|---|
| `Asisya.Domain` | Entidades y reglas del negocio. No depende de nada. |
| `Asisya.Application` | Casos de uso, DTOs y contratos (interfaces). No conoce EF Core ni librerías externas. |
| `Asisya.Infrastructure` | Implementaciones: EF Core, repositorios, BCrypt, JWT, mensajería. |
| `Asisya.Api` | Controladores, autenticación, manejo global de errores. |
| `Asisya.Worker` | Consumidor de la cola para la carga masiva. |

## Decisiones de arquitectura

| Decisión | Por qué |
|---|---|
| Clean Architecture con DDD ligero | El dominio es principalmente CRUD con un proceso pesado; DDD completo sería sobreingeniería. |
| Contratos en Application, implementaciones en Infrastructure | La lógica de negocio no depende de la base de datos y se puede probar con dobles. |
| DTOs con mapeo explícito (sin AutoMapper) | No se exponen entidades y cada campo expuesto es una decisión consciente. |
| Entidades registradas por reflexión con la interfaz `IEntity` | El `DbContext` no crece al agregar entidades; las configuraciones se aplican con `ApplyConfigurationsFromAssembly`. |
| Configuración por entidad (`IEntityTypeConfiguration`) en Infrastructure | El dominio queda libre de detalles de PostgreSQL. |
| Borrado `Restrict` en las llaves foráneas | Evita que borrar una categoría o proveedor elimine miles de productos en cascada. |
| Nombre de categoría único (índice único) | Regla del modelo Northwind; la base de datos es la garantía final. |
| Contraseñas con BCrypt | Hash con sal y factor de trabajo configurable; nunca se guarda la contraseña. |
| JWT con rol en el token | La API valida sin consultar la base de datos en cada petición. Limitación: un cambio de rol aplica al renovar el token. |
| Rutas del front por rol (tablas `AppRoute` y `RoleRoute`) | El front arma el menú con `GET /routes`. Es experiencia de usuario; la seguridad real está en `[Authorize]` en la API. |
| Errores con `ProblemDetails` (RFC 7807) | Formato único y estándar para 401, 403, 404, 409 y 500. |
| Secretos fuera del repositorio | La clave JWT va en user-secrets o variables de entorno. |

## Endpoints

| Método | Ruta | Acceso | Estado |
|---|---|---|---|
| POST | `/Auth/login` | Público | Hecho |
| GET | `/routes` | Autenticado | Hecho |
| POST | `/Category` | Admin | Hecho |
| GET | `/Category` | Autenticado | Hecho |
| POST | `/Product` | Admin | Pendiente |
| GET | `/Products` | Autenticado | Pendiente |
| GET | `/Products/{id}` | Autenticado | Pendiente |
| PUT | `/Products/{id}` | Admin | Pendiente |
| DELETE | `/Products/{id}` | Admin | Pendiente |

## Ejecutar localmente

Pendiente: se simplificará a un solo `docker compose up`. Mientras tanto, pasos provisionales:

```bash
docker compose up -d
dotnet user-secrets set "Jwt:Key" "<clave de al menos 32 caracteres>" --project src/Asisya.Api
dotnet ef database update --project src/Asisya.Infrastructure --startup-project src/Asisya.Api
dotnet run --project src/Asisya.Api
```

## Carga masiva de 100.000 productos

Pendiente. Diseño previsto: `POST /Product` valida y encola un mensaje en RabbitMQ y responde `202` con un identificador de trabajo. El worker lo consume e inserta por lotes con `COPY` binario de PostgreSQL, y el estado se consulta por identificador.

## Escalado horizontal en un entorno cloud

Pendiente.

## Pruebas

Pendiente.

## Integración continua

Pendiente.
