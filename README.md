# 🚀 Aurora 4X Command Suite (v2.7.1)
> **El Centro de Mando Táctico, Analítica en Tiempo Real y Asistente de Inteligencia Artificial para Aurora 4X.**

[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20.NET%207.0-blue.svg)](https://dotnet.microsoft.com/)
[![Aurora 4X Version](https://img.shields.io/badge/Aurora%204X-v2.7.1-brightgreen.svg)](https://aurora2.pentarch.org/)
[![License](https://img.shields.io/badge/License-MIT-gold.svg)](LICENSE)
[![Author](https://img.shields.io/badge/Author-Fran%20Gómez-purple.svg)]()

---

## 🌟 Visión General

**Aurora 4X Command Suite** es una suite de mando e inteligencia espacial desarrollada para integrarse al 100% en tiempo real con el juego de estrategia **Aurora 4X (v2.7.1)**. 

Combina una interfaz gráfica sci-fi de última generación (WPF), telemetría directa desde la base de datos de juego `AuroraDB.db`, simulación táctica de misiles y naves, un **Monitor Táctico IA con Popups Overlay en tiempo real sobre el juego** y un **Generador de Novelas e Historia Épica**.

---

## 📦 ¿Dónde Extraer la Carpeta para Jugar? (Instrucciones de Instalación)

Para que el lanzador **`🚀 ABRIR_SUITE.bat`** funcione perfectamente y detecte tu partida sin errores:

### 📥 Opción Recomendada (Extraer dentro de la carpeta del juego):
1. Descarga el paquete comprimido `.zip` desde la sección [Releases](https://github.com/GomSamurai/Aurora-Command-Suite/releases).
2. Extrae la carpeta `Aurora_Command_Suite_v2.7.1` **dentro del directorio principal de tu juego Aurora 4X** (donde se encuentra `AuroraDB.db` y `Aurora.exe`).
3. Entra a la carpeta extraída y haz doble clic en **`🚀 ABRIR_SUITE.bat`**.

```
📁 Tu_Carpeta_Aurora4X/
 ├── Aurora.exe
 ├── AuroraDB.db
 └── 📁 Aurora_Command_Suite_v2.7.1/
      ├── 🚀 ABRIR_SUITE.bat               <-- ¡Haz doble clic aquí!
      ├── 📜 INSTRUCCIONES_Y_GUIA.html
      ├── 📜 INSTRUCCIONES_DE_INSTALACION.md
      └── 📁 App/
```

> **⚡ Auto-Detección Inteligente de Base de Datos**: La suite incluye búsqueda automática. Si extraes la carpeta en cualquier ubicación, detectará la base de datos `AuroraDB.db` automáticamente en su propio directorio, en la carpeta padre de Aurora 4X o en `c:\VSCODE\Aurora271Full\AuroraDB.db`.

---

## ✨ Características Principales (12 Módulos Integrados)

### 📊 1. Gobierno & Economía Imperial (`EmpireOverviewView`)
- Auditoría en tiempo real del Producto Imperial Bruto (BP), ingresos por impuestos y centros financieros.
- Distribución de almirantes, científicos y gobernadores activos.
- Desglose de investigación acumulada e instalaciones de I+D.

### 🪐 2. Colonias & Inventario Trans-Uraniano (`ColoniesOverviewView`)
- Control de existencias de los **11 Minerales Trans-Uranianos**: *Duranium, Sorium, Neutronium, Corundium, Uridium, Gallicite, Boronide, Mercassium, Vendarite, Corbomite y Tritium*.
- Gráficos comparativos de producción minera, reservas coloniales e instalaciones industriales por planeta.

### 👥 3. Cuartel General de Oficiales (`CommandersHQView`)
- Gestión centralizada del cuerpo de oficiales navales, administradores planetarios e investigadores.

### 🔬 4. Laboratorios de I+D (`ResearchHQView`)
- Seguimiento de proyectos científicos activos, velocidad de investigación (BP/año) y bonificaciones de los científicos a cargo.

### 🏭 5. Complejo Industrial (`IndustrialHQView`)
- Auditoría de fábricas de construcción, mineras automatizadas, modificadores de terraformación y astilleros.

### 🛠️ 6. Diseñador Táctico de Naves (`BlueprintDesignerView`)
- Diseñador de prototipos navales con calculadora de masa (toneladas HS), potencia de motores y armamento.
- **Matcheador de Astilleros en Vivo**: Valida si tus astilleros en `AuroraDB.db` pueden construir la nave o calcula los meses de Retooling requeridos.

### 🚀 7. Laboratorio de Misiles & Matriz de Interceptación (`MissileEngineLab`)
- Simulador de rendimiento de misiles con cálculo de rango (M-km), velocidad (km/s), maniobrabilidad y **Matriz de Probabilidad de Impacto (Pk)**.

### ⚓ 8. Operaciones de Astilleros (`ShipyardOperationsView`)
- Monitor de gradas activas, capacidad de toneladas (BP/ton) y construcciones en curso.

### 🛸 9. Flotas Activas & Análisis Táctico (`ActiveFleetsView` & `FleetCompositionView`)
- Ubicación de grupos de combate, tonelaje total, velocidad máxima y composición de escuadrones.

### 🌌 10. Exploración del Sistema (`SystemExplorationView`)
- Cartografía de cuerpos celestes, cuerpos colonizables y yacimientos minerales descubiertos.

### 🤖 11. Matriz IA Imperial & Overlay HUD (`AIAssistantView`)
- Integración nativa con la API de Google Gemini y **Monitor Táctico en Tiempo Real** con avisos flotantes translúcidos sobre Aurora 4X.
- **Generador de Novelas e Historia Épica**: Convierte los registros de tu partida en un libro de ciencia ficción en capítulos (funciona tanto online como offline).

### 🎨 12. Motor de Temas Dinámicos Categorizado (`ThemeManager`)
- Paleta de 15 estilos visuales clasificados por categorías (`👑 Edición Insignia`, `🌌 Modos Oscuro`, `☀️ Modos Claro`, `☯️ Modos Neutros`, `💎 Ediciones Refinadas`).

---

## 🌉 Conmutador Bidireccional de Ventanas (Win32 Bridge)

- **Botón en la App**: **`🎮 VOLVER A AURORA 4X`** enfoca la ventana del juego de inmediato en estado maximizado.
- **Botón en el Juego**: El parche inyecta el botón **`🚀 AURORA COMMAND SUITE`** dentro de Aurora 4X para saltar a la suite al instante.

---

## 📁 Estructura del Repositorio

```
AuroraDesignSuite/
├── README.md                          <-- Guía oficial de GitHub
├── AuroraDesignSuite.sln             <-- Solución de Visual Studio
├── AuroraDesignSuite.csproj          <-- Proyecto C# WPF (.NET 7.0)
│
├── Services/                         <-- DatabaseService, ThemeManager, AITacticalMonitorService
├── Models/                           <-- Modelos de datos SQLite
├── Views/                            <-- Vistas XAML/C# de los 12 módulos
│
└── release/                          <-- Paquete ejecutable distribuible
    └── Aurora_Command_Suite_v2.7.1/
        ├── 🚀 ABRIR_SUITE.bat         <-- Lanzador principal
        ├── 📜 INSTRUCCIONES_Y_GUIA.html<-- Manual visual interactivo
        ├── 📜 INSTRUCCIONES_DE_INSTALACION.md <-- Guía rápida de instalación
        ├── 🧹 DESINSTALAR_SUITE.bat   <-- Desinstalador limpio
        └── App/                      <-- Binarios y ejecutable principal
```

---

## 📜 Licencia y Créditos

- **Diseño & Autor Principal**: Fran Gómez & Antigravity AI Team
- **Desarrollo**: C# .NET 7.0 WPF, SQLite, Harmony Patcher & Google Gemini API
- **Juego Base**: Aurora 4X de Steve Walmsley

© 2026 Aurora Command Suite. Desarrollado con pasión para la comunidad de Aurora 4X.
