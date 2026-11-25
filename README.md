# Asisya: API de productos y categorías

Proyecto que expone servicios para crear, editar, consultar y eliminar productos, junto con sus categorías.

## Cómo ejecutarlo

1. **Requisito:** tener Docker Desktop encendido.

```
POSTGRES_DB=asisya
POSTGRES_USER=asisya
POSTGRES_PASSWORD=asisya_dev

JWT_KEY=demo-only-jwt-key-change-me-0123456789abcdef

SEED_ADMIN_EMAIL=admin@finanzauto.com.co
SEED_ADMIN_PASSWORD=admin123
SEED_USER_EMAIL=user@finanzauto.com.co
SEED_USER_PASSWORD=user123

CORS_ORIGIN=http://localhost:5173

VITE_API_URL=http://localhost:8080
```

3. **Levantar el proyecto:**

```powershell
docker compose up -d --build
```

Al arrancar se crean automáticamente los usuarios, las rutas y los datos iniciales.

## Estructura del proyecto

```
.
├── Backend/        API .NET (src/), pruebas (tests/) y solución Asisya.slnx
├── Frontend/       SPA en React + Vite
├── Dockerfile      imagen de la API (contexto: raíz)
├── docker-compose.yml
├── .github/workflows/ci.yml
└── .env.example
```

## Stack

| Área          | Tecnología                                                       |
| ------------- | ---------------------------------------------------------------- |
| API           | .NET 11, ASP.NET Core, EF Core 11, Npgsql                        |
| Base de datos | PostgreSQL 17                                                    |
| Autenticación | JWT Bearer, contraseñas con BCrypt                               |
| Carga masiva  | Bogus (datos aleatorios) y `COPY` binario de PostgreSQL          |
| Frontend      | React, Vite, Tailwind CSS v4, Zustand, SWR, react-hook-form, zod |
| Contenedores  | Docker, Docker Compose, Nginx para servir el frontend            |
| CI            | GitHub Actions                                                   |

## Arquitectura

Implementé Clean Architecture con una organización por capas: **Domain**, **Application**, **Infrastructure** y **Api**. Dentro de Application agrupé el código por funcionalidad (`Auth`, `Products`, `Categories`), para que cada caso de uso quede junto a sus DTOs, interfaces y servicios.

- **Domain:** entidades. No depende de nada.
- **Application:** casos de uso, DTOs y contratos (interfaces).
- **Infrastructure:** implementaciones con EF Core, `COPY`, BCrypt y JWT.
- **Api:** controladores, autenticación y manejo global de errores con `ProblemDetails`.

## Decisiones principales

### Carga masiva (`POST /Products`)

Decidí hacer la carga masiva con `COPY`, una función de PostgreSQL que abre un canal directo por el que pasan los datos en crudo. El flujo es:

1. Llega la petición con la cantidad de productos a crear.
2. Bogus genera los productos de forma perezosa, con `GenerateLazy`: cada producto se crea cuando el canal lo necesita.
3. Cada producto se envía por el canal y se libera de memoria antes de crear el siguiente.

Así nunca hay más de un producto almacenado en RAM, y esa ventaja se refleja en el tiempo de ejecución: 100.000 productos en unos 2,4 segundos en local.

Esta decisión responde al tamaño actual de la aplicación y a la forma de carga. Si la carga viniera de un archivo CSV, el panorama cambiaría: el archivo llegaría a la API, que haría validaciones básicas de tamaño y columnas, y la tarea pasaría por RabbitMQ, que la guarda en una cola. Un worker dividiría los registros en lotes, validaría cada fila e insertaría cada lote en la base de datos. Redis podría servir para guardar el progreso de la carga.

### Otras decisiones

- **DTOs con mapeo explícito:** la API nunca expone las entidades directamente.
- **Borrado restringido:** `DeleteBehavior.Restrict` en las llaves foráneas. Borrar una categoría con productos no los elimina en cascada.
- **Trazabilidad:** cada producto guarda su origen (`Manual` o `BulkLoad`).
- **Migraciones y datos iniciales al arrancar:** la API aplica las migraciones y crea roles, rutas y usuarios. Es un proceso idempotente.

## Endpoints

| Método | Ruta             | Acceso      | Descripción                                                                                     |
| ------ | ---------------- | ----------- | ----------------------------------------------------------------------------------------------- |
| POST   | `/Auth/login`    | Público     | Inicia sesión y devuelve un token JWT                                                           |
| GET    | `/routes`        | Autenticado | Rutas permitidas para el rol del usuario                                                        |
| GET    | `/Category`      | Autenticado | Lista de categorías                                                                             |
| POST   | `/Category`      | Admin       | Crea una categoría                                                                              |
| GET    | `/Products`      | Autenticado | Lista paginada, con filtros `search`, `categoryId`, `minPrice`, `maxPrice`, `page` y `pageSize` |
| GET    | `/Products/{id}` | Autenticado | Detalle del producto, con la imagen de su categoría                                             |
| POST   | `/Product`       | Admin       | Crea un producto desde el formulario                                                            |
| POST   | `/Products`      | Admin       | Genera e inserta productos aleatorios (`{ "count": 100000 }`)                                   |
| PUT    | `/Products/{id}` | Admin       | Actualiza un producto                                                                           |
| DELETE | `/Products/{id}` | Admin       | Elimina un producto                                                                             |

## Pruebas

```powershell
dotnet test Backend/Asisya.slnx
```

Tengo dos tipos de prueba, con una de cada una:

1. **Unitaria** (`Backend/tests/Asisya.UnitTests`): prueba una regla de negocio, la de categorías con nombre repetido, usando **NSubstitute** para simular el repositorio. No necesita base de datos.
2. **Integración** (`Backend/tests/Asisya.IntegrationTests`): usa **Testcontainers** para levantar un PostgreSQL real en Docker, aplica las migraciones y carga 1.000 productos con `COPY`. Requiere Docker encendido.

## Integración continua

El workflow `.github/workflows/ci.yml` se ejecuta en cada push y en cada pull request a `main`:

1. Restaura los paquetes de la solución.
2. Verifica el estilo con `dotnet format`.
3. Compila en `Release`.
4. Ejecuta las pruebas.
5. Construye la imagen de Docker de la API.

## Frontend

El frontend aplica **carga diferida de rutas y componentes** (_lazy loading_): cada pantalla se descarga solo cuando el usuario la visita, lo que reduce el tamaño de la carga inicial.

Las rutas no están fijas en el código: se obtienen del backend según el rol del usuario (`GET /routes`). Esto tiene tres ventajas:

- **Un solo lugar de verdad:** los permisos se definen en la base de datos, y cambiarlos no exige desplegar el frontend de nuevo.
- **Experiencia adecuada por rol:** cada usuario solo ve las secciones que le corresponden, y el menú se arma con esos datos.
- **Más seguridad:** el frontend solo esconde lo que no corresponde; la protección real está en la API, que responde 403 a cualquier petición no autorizada.
