# Client Portal Backend

API REST para manejar clientes desarrollada con **.NET 8** y arquitectura limpia. Tiene soporte para diferentes ambientes y manejo de archivos.

## 🏗️ Arquitectura

Este proyecto implementa **Clean Architecture** con los siguientes principios:

- **Domain Layer**: Entidades de negocio e interfaces del dominio
- **Application Layer**: Casos de uso, servicios de aplicación y DTOs
- **Infrastructure Layer**: Implementación de repositorios, servicios externos y configuración de datos
- **WebAPI Layer**: Controllers, configuración de API y middleware

## 🚀 Tecnologías

- **.NET 8** (la versión más nueva)
- **Entity Framework** (usa memoria para desarrollo, fácil de cambiar)
- **Swagger** (para ver y probar la API fácilmente)
- **ASP.NET Core** (framework para APIs web)
- **Inyección de dependencias** (para organizar el código)
- **CORS** (para conectar con el frontend)

## 🌍 Configuración de Ambientes

El proyecto soporta múltiples ambientes de ejecución:

### Development (Desarrollo)
- **Archivo**: `appsettings.Development.json`
- **Características**:
  - Logging detallado (Debug)
  - Base de datos: `ClientPortalDb_Dev`
  - CORS permisivo (localhost:3000, 3001)
  - Swagger habilitado
  - Límites de archivo más altos (10MB)

### Staging (Pruebas)
- **Archivo**: `appsettings.Staging.json`
- **Características**:
  - Logging informativo
  - Base de datos: `ClientPortalDb_Staging`
  - CORS restringido
  - Swagger habilitado
  - HTTPS requerido

### Production (Producción)
- **Archivo**: `appsettings.Production.json`
- **Características**:
  - Logging mínimo (solo warnings)
  - Base de datos: `ClientPortalDb_Prod`
  - CORS muy restrictivo
  - Swagger deshabilitado
  - Seguridad estricta (HTTPS obligatorio)

### Configuración Base
- **Archivo**: `appsettings.json`
- **Contiene**: Configuración común a todos los ambientes

## 📁 Estructura del Proyecto

```
src/
├── ClientPortal.Domain/          # Entidades y interfaces de dominio
│   ├── Entities/
│   │   └── Client.cs            # Entidad Cliente
│   ├── Enums/
│   │   └── DocumentType.cs      # Tipos de documento
│   └── Interfaces/
│       ├── IClientRepository.cs
│       └── IFileService.cs
├── ClientPortal.Application/     # Lógica de aplicación
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Services/                # Servicios de aplicación
│   └── Interfaces/
├── ClientPortal.Infrastructure/  # Implementaciones concretas
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Repositories/
│   └── Services/
└── ClientPortal.WebAPI/         # Capa de presentación
    ├── Controllers/
    │   └── ClientsController.cs
    └── Program.cs
```

## 🔧 Instalación y Ejecución

### Prerrequisitos
- **.NET 8 SDK** (LTS)
- **Git**
- **Visual Studio 2022** o **VS Code** (opcional)

### Pasos de Instalación

1. **Clonar el repositorio**:
```bash
git clone https://github.com/tu-usuario/client-portal-backend.git
cd client-portal-backend
```

2. **Restaurar dependencias**:
```bash
dotnet restore
```

3. **Verificar instalación**:
```bash
dotnet --version  # Debe mostrar 8.x.x
```

### Ejecución por Ambiente

#### 🏠 Development (Por defecto)
```bash
cd src/ClientPortal.WebAPI
dotnet run
# O usando el script
dotnet run --environment=Development
```

#### 🧪 Staging
```bash
cd src/ClientPortal.WebAPI
dotnet run --environment=Staging
```

#### 🚀 Production
```bash
cd src/ClientPortal.WebAPI
dotnet run --environment=Production
```

### Acceso a la API

| Ambiente | URL Base | Swagger UI |
|----------|----------|------------|
| Development | `http://localhost:5000` | `http://localhost:5000/swagger` |
| Staging | `http://localhost:5001` | `http://localhost:5001/swagger` |
| Production | `https://yourdomain.com` | ❌ Deshabilitado |

### Configuración de Variables de Entorno

Para configurar el ambiente vía variables de entorno:

```bash
# Windows
set ASPNETCORE_ENVIRONMENT=Production

# Linux/macOS
export ASPNETCORE_ENVIRONMENT=Production
```

## 📡 API Endpoints

### Clientes
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/clients` | Obtener todos los clientes |
| GET | `/api/clients/{id}` | Obtener cliente por ID |
| POST | `/api/clients` | Crear nuevo cliente |
| PUT | `/api/clients/{id}` | Actualizar cliente |
| DELETE | `/api/clients/{id}` | Eliminar cliente (lógico) |
| GET | `/api/clients/export` | Exportar clientes a CSV |

### Modelo de Cliente

```json
{
  "id": "guid",
  "firstName": "string",
  "lastName": "string",
  "dateOfBirth": "2024-01-01",
  "documentType": 1,
  "documentNumber": "string",
  "curriculumVitaeFileName": "string",
  "photoFileName": "string",
  "createdAt": "2024-01-01T00:00:00",
  "updatedAt": "2024-01-01T00:00:00"
}
```

### Tipos de Documento
- `1`: DNI
- `2`: RUC  
- `3`: Carnet de Extranjería

## 📋 Validaciones

### Reglas de Negocio
- ✅ Todos los campos son obligatorios al crear
- ✅ No se permite cambiar tipo ni número de documento al editar
- ✅ Archivos: CV (PDF) y foto (JPEG), máximo 5MB cada uno
- ✅ Eliminación lógica (no física) de registros
- ✅ Validación de documentos únicos por tipo

### Validaciones de Archivo
- **CV**: Solo archivos PDF, máximo 5MB
- **Foto**: Solo archivos JPEG/JPG, máximo 5MB

## 🔒 CORS

El backend está configurado para permitir requests desde:
- `http://localhost:3000` (Frontend en desarrollo)

Para producción, actualizar la configuración CORS en `Program.cs`.

## 🗄️ Base de Datos

### Configuración Actual
Utiliza **Entity Framework InMemory** para simplicidad en desarrollo. Los datos se pierden al reiniciar la aplicación.

### PostgreSQL (Para Cuando Quieras Usarlo Después)
Si más adelante quieres cambiar a PostgreSQL de verdad, haz esto:

#### 1. En `Program.cs` (quita los comentarios /* */ de la parte de PostgreSQL):
```csharp
// Básicamente cambia esto:
// options.UseInMemoryDatabase("ClientPortalDb");

// Por esto:
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// options.UseNpgsql(connectionString);
```

#### 2. En `render.yaml` (quita comentarios del servicio de base de datos):
```yaml
# Agrega esto en la sección de servicios:
- type: pserv
  name: client-portal-db
  env: postgresql
  databaseName: clientportal_prod
  # Render te da las variables de conexión automáticamente
```

#### 3. En Render (agrega servicio de PostgreSQL):
- Ve al dashboard de Render
- Agrega un servicio PostgreSQL a tu proyecto
- Se conecta automáticamente con las variables de entorno

#### 4. Para las migraciones (cuando lo hagas):
```bash
# Agrega esto si no lo tienes
dotnet add package Microsoft.EntityFrameworkCore.Design

# Crea la primera migración
dotnet ef migrations add PrimeraMigracion -p ClientPortal.Infrastructure -s ClientPortal.WebAPI

# Aplica los cambios a la base de datos
dotnet ef database update -p ClientPortal.Infrastructure -s ClientPortal.WebAPI
```

## 📁 Archivos

Los archivos subidos se almacenan en:
```
uploads/
├── cv/        # Hojas de vida (PDF)
└── photo/     # Fotos de clientes (JPEG)
```

## 🏛️ Patrones Implementados

- **Repository Pattern**: Abstracción del acceso a datos
- **Dependency Injection**: Inversión de dependencias
- **Service Layer**: Encapsulación de lógica de negocio
- **DTO Pattern**: Transferencia de datos entre capas

## 🧪 Testing

Para agregar tests:

```bash
dotnet new xunit -n ClientPortal.Tests
dotnet sln add ClientPortal.Tests
```

### Ejecutar Tests
```bash
dotnet test
```

## 🚀 Despliegue

### Opciones de Despliegue

#### 🐳 Docker (Recomendado)
```bash
# Construir imagen
docker build -t clientportal-api:1.0 .

# Ejecutar en contenedor
docker run -d -p 5000:80 --name clientportal-api clientportal-api:1.0
```

#### ☁️ Azure App Service
```bash
# Publicar directamente desde Visual Studio
# O usando Azure CLI
az webapp up --name clientportal-api --resource-group clientportal-rg
```

#### ☁️ AWS Elastic Beanstalk
```bash
# Crear aplicación
eb init clientportal-api
eb create production-env
eb deploy
```

#### ⚙️ IIS (Windows Server)
```bash
# Publicar la aplicación
dotnet publish -c Release -o publish

# Copiar archivos a IIS
# Configurar sitio web en IIS Manager
```

### Variables de Entorno para Producción

```bash
# Base de datos
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="Server=prod-server;Database=ClientPortalDb_Prod;..."

# Configuración de archivos
FileUpload__BasePath="/app/uploads/"

# CORS
CORS__AllowedOrigins__0="https://yourdomain.com"

# Logging
Serilog__MinimumLevel__Default="Warning"
```

### Checklist de Despliegue

- [ ] Configurar cadena de conexión a base de datos
- [ ] Configurar CORS para dominios de producción
- [ ] Configurar certificados SSL
- [ ] Configurar variables de entorno
- [ ] Verificar permisos de archivos y carpetas
- [ ] Configurar monitoreo y logging
- [ ] Realizar pruebas de integración

### Docker (Opcional)

Crear `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClientPortal.WebAPI.dll"]
```

## 🤝 Contribución

1. Fork el proyecto
2. Crear una rama feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit los cambios (`git commit -m 'Agregar nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crear un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT.

## 🔗 Enlaces Relacionados

- [Frontend Repository](https://github.com/tu-usuario/client-portal-frontend)
- [Documentación de .NET 8](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)




Option 1: Use the Script (Recommended)

./run-with-dotnet8.sh
# Kill any process on port 5258
lsof -ti:5258 | xargs kill -9 2>/dev/null || echo "Port is free"

# Set .NET 8 PATH and run
export PATH="$HOME/.dotnet:$PATH"
export DOTNET_ROOT="$HOME/.dotnet"
cd src/ClientPortal.WebAPI
dotnet run

commit de prueba