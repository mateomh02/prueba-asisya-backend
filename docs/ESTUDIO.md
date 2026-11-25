# Guía de estudio para la sustentación

Documento personal de repaso. Cada parte tiene: qué se hizo, por qué, y preguntas para responder **antes** de mirar las respuestas guía del final.

Cómo usarla: responde en voz alta, como si te lo preguntara el evaluador. Después compara con las respuestas guía o pídele a Claude que te las revise.

---

## Parte 1. Arquitectura por capas (Clean Architecture)

**Qué se hizo:** cinco proyectos (`Domain`, `Application`, `Infrastructure`, `Api`, `Worker`) con dependencias hacia adentro.

**Preguntas**
1. ¿Por qué `Domain` no depende de ningún otro proyecto?
2. ¿Por qué la interfaz `IUserRepository` está en Application y su implementación en Infrastructure?
3. ¿Por qué elegiste Clean Architecture y no DDD completo?
4. Si mañana cambias PostgreSQL por otra base de datos, ¿qué capas tocas?

---

## Parte 2. Entity Framework Core y migraciones

**Qué se hizo:** `DbContext`, entidades por reflexión con `IEntity`, configuraciones por entidad, migraciones.

**Preguntas**
1. ¿Qué es una migración y qué hay dentro de sus métodos `Up` y `Down`?
2. ¿Qué hace `ApplyConfigurationsFromAssembly` y cómo encuentra las clases?
3. ¿Qué problema tiene registrar entidades por reflexión? (pista: qué pasa si olvidas `: IEntity`)
4. ¿Por qué `UnitPrice` es `decimal` con precisión (18,2) y no `double`?
5. ¿Qué diferencia hay entre `SupplierId` y `Supplier` en la clase `Product`?
6. ¿Por qué se usó `DeleteBehavior.Restrict` en lugar de `Cascade`?

---

## Parte 3. Inyección de dependencias

**Qué se hizo:** métodos de extensión `AddApplication`, `AddInfrastructure` y `AddJwtAuthentication`.

**Preguntas**
1. ¿Qué es la inyección de dependencias y qué problema resuelve?
2. ¿Qué diferencia hay entre `AddScoped`, `AddSingleton` y `AddTransient`? ¿Por qué el hasher es singleton y los repositorios scoped?
3. ¿Qué pasa si olvidas registrar un servicio?
4. ¿Por qué `Program.cs` no registra cada clase directamente?

---

## Parte 4. Autenticación y seguridad

**Qué se hizo:** hash con BCrypt, generación de JWT, login, validación del token, `[Authorize]` con roles, errores 401 y 403 con `ProblemDetails`.

**Preguntas**
1. ¿Por qué se guarda un hash y no la contraseña? ¿Qué diferencia hay entre hash y cifrado?
2. ¿Qué contiene un JWT y por qué está firmado pero no cifrado?
3. ¿Qué diferencia hay entre 401 y 403?
4. ¿Por qué el login devuelve el mismo error si el email no existe o si la contraseña es incorrecta?
5. ¿Qué es el `RoleClaimType` y qué pasaría si no se configura?
6. Si cambias el rol de un usuario en la base de datos, ¿cuándo se aplica el cambio? ¿Cómo lo mitigarías?
7. ¿Dónde guardas la clave secreta del JWT y por qué no en `appsettings.json`?

---

## Parte 5. Rutas del front por rol

**Qué se hizo:** tablas `AppRoute` y `RoleRoute` (muchos a muchos), endpoint `GET /routes/me`.

**Preguntas**
1. ¿Por qué el rol se lee del token y no de un parámetro de la URL?
2. ¿Ocultar rutas en el front protege el sistema? ¿Dónde está la seguridad real?
3. ¿Por qué `Role` y `AppRoute` son muchos a muchos y qué papel cumple `RoleRoute`?

---

## Parte 6. Categorías: DTOs, servicio y manejo de errores

**Qué se hizo:** `POST /Category` y `GET /Category`, con DTOs, servicio, repositorio y manejador global de excepciones.

**Preguntas**
1. ¿Para qué sirven `CreateCategoryRequest` y `CategoryResponse`? ¿Por qué no usar la entidad directamente?
2. ¿Qué es el mapeo explícito y por qué la prueba lo pide?
3. ¿Qué significa `Task<CategoryResponse>`? ¿Qué diferencia hay con devolver `CategoryResponse`?
4. ¿Por qué el servicio lanza `ConflictException` en vez de devolver un 409 directamente?
5. ¿Qué ventaja tiene el manejador global frente a un `try/catch` en cada controlador?
6. Hay un índice único en el nombre y además el servicio consulta si ya existe. ¿Por qué las dos protecciones?
7. ¿Por qué los errores 500 no devuelven el mensaje real de la excepción?

---

## Respuestas guía

Intenta responder primero. Estas son las ideas clave, no un guion.

**Parte 1**
1. El dominio es el núcleo del negocio; si dependiera de otras capas, un cambio técnico (base de datos, framework) lo rompería.
2. Application define qué necesita (contrato) sin saber cómo se hace; Infrastructure lo implementa con EF Core. Así la lógica se prueba sin base de datos.
3. El problema es principalmente CRUD más un proceso pesado; DDD completo (agregados, eventos) agrega complejidad sin beneficio.
4. Solo Infrastructure (y el registro de dependencias). Domain y Application no cambian.

**Parte 2**
1. Una migración es un cambio versionado del esquema. `Up` lo aplica, `Down` lo revierte.
2. Recorre por reflexión el ensamblado y aplica toda clase que implemente `IEntityTypeConfiguration<T>`.
3. Si olvidas la marca, la entidad no se registra y la tabla no se crea, sin dar error. Por eso se revisa la migración antes de aplicarla.
4. `decimal` es exacto para dinero; `double` tiene errores de redondeo binario.
5. `SupplierId` es la columna (llave foránea); `Supplier` es la propiedad de navegación para acceder al objeto relacionado.
6. Con `Cascade`, borrar una categoría borraría todos sus productos. `Restrict` lo impide si tiene productos.

**Parte 3**
1. Las clases piden lo que necesitan por constructor y el contenedor se las entrega, sin usar `new`. Reduce el acoplamiento y facilita las pruebas.
2. Scoped: una instancia por petición HTTP (los repositorios, porque comparten el `DbContext`). Singleton: una para toda la aplicación (el hasher, que no guarda estado). Transient: una nueva en cada solicitud.
3. La aplicación compila, pero falla en tiempo de ejecución al pedir ese servicio (no se puede resolver).
4. Cada capa registra lo suyo; así `Program.cs` queda corto y Application no conoce a Infrastructure.

**Parte 4**
1. Un hash es de un solo sentido y no se puede revertir; el cifrado es reversible con una clave. Para contraseñas se usa hash con sal.
2. Header, payload (claims) y firma. Cualquiera puede leer el payload (es base64), pero nadie puede alterarlo sin invalidar la firma. Por eso no se guardan datos sensibles dentro.
3. 401: no sé quién eres (falta token o es inválido). 403: sé quién eres, pero no tienes permiso.
4. Para no revelar qué emails están registrados (enumeración de usuarios).
5. Indica en qué claim viaja el rol. Sin eso, `[Authorize(Roles = ...)]` no lo encuentra y todo da 403.
6. Aplica al emitir un token nuevo: el token ya emitido conserva el rol viejo hasta que caduque. Se mitiga con vida corta del token o refresh tokens.
7. En user-secrets o variables de entorno, porque `appsettings.json` se sube al repositorio y la clave quedaría pública.

**Parte 5**
1. Si viniera por la URL, cualquiera pediría las rutas de otro rol. El token está firmado y no se puede alterar.
2. No. El front se puede modificar en el navegador. La seguridad real es que la API rechace la petición (`[Authorize]`).
3. Un rol tiene varias rutas y una ruta puede estar en varios roles. `RoleRoute` es la tabla intermedia con la llave compuesta (RoleId, AppRouteId).

**Parte 6**
1. Request: lo que el cliente puede enviar (sin `Id`, con validaciones). Response: lo que se decide exponer. La entidad podría filtrar campos internos o permitir que el cliente fije el `Id`.
2. Copiar campo por campo entre entidad y DTO a mano. Cada campo expuesto es una decisión consciente y no hay magia.
3. Es una promesa de resultado futuro (como `Promise` en JavaScript). Permite no bloquear el hilo mientras se espera la base de datos.
4. El servicio no debe conocer HTTP. Lanza una excepción de negocio y la capa de API decide cómo traducirla.
5. Un solo lugar para traducir excepciones a respuestas, con formato uniforme, y los controladores quedan limpios.
6. La consulta da un mensaje claro en el caso normal; el índice garantiza la regla incluso con peticiones simultáneas que pasen la consulta a la vez.
7. Podría filtrar detalles internos (rutas, SQL, estructura). Se registra en el log y se devuelve un mensaje genérico.
