# RiberaApp

## Descripción General
RiberaApp es un sistema de gestión estructurado bajo el modelo de monorepositorio. La plataforma opera mediante una arquitectura cliente-servidor que separa la lógica de negocio de las capas de presentación, garantizando un control de acceso basado en roles (RBAC) seguro y eficiente.

## Arquitectura y Tecnologías
El ecosistema del proyecto está compuesto por tres módulos principales:

* **API Central (.NET Web API):** Backend responsable del procesamiento de transacciones, conexión a la base de datos, validación de reglas de negocio, uso de DTOs y emisión de tokens de seguridad (JWT).
* **Cliente Administrador (Angular):** Interfaz de usuario construida con componentes *standalone*, diseñada exclusivamente para las funciones de gestión y control del sistema.
* **Cliente Residente (Angular):** Interfaz de usuario construida con componentes *standalone*, enfocada en las operaciones diarias y la interacción del usuario estándar.

## Estructura del Repositorio
```text
RiberaApp/
├── api/                 # Código fuente del servidor .NET Web API (Ribera2App)
├── cliente-admin/       # Aplicación web para perfiles con privilegios de administración
└── cliente-residente/   # Aplicación web para usuarios residentes
