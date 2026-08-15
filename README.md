# 🚀 Aurora 4X Command Suite (v2.7.1)
> **El Centro de Mando Táctico, Analítica en Tiempo Real y Asistente de Inteligencia Artificial para Aurora 4X.**

[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20.NET%207.0-blue.svg)](https://dotnet.microsoft.com/)
[![Aurora 4X Version](https://img.shields.io/badge/Aurora%204X-v2.7.1-brightgreen.svg)](https://aurora2.pentarch.org/)
[![License](https://img.shields.io/badge/License-MIT-gold.svg)](LICENSE)
[![Author](https://img.shields.io/badge/Author-Fran%20Gómez-purple.svg)]()

---

## 🌟 Visión General

**Aurora 4X Command Suite** es una aplicación de inteligencia de mando desarrollada para integrarse al 100% en tiempo real con el célebre juego de estrategia espacial **Aurora 4X (v2.7.1)**. 

Combina una interfaz gráfica sci-fi de última generación (WPF), telemetría directa desde la base de datos de juego `AuroraDB.db`, simulación táctica de misiles y naves, y un **Asistente de Inteligencia Artificial (Gemini REST API)** con el tema exclusivo **`👑 Imperial Gold (Fran Gómez Edition)`**.

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
- Visualización de rangos, habilidades de mando y asignaciones activas.

### 🔬 4. Laboratorios de I+D (`ResearchHQView`)
- Seguimiento de proyectos científicos activos, velocidad de investigación (BP/año) y bonificaciones de los científicos a cargo.

### 🏭 5. Complejo Industrial (`IndustrialHQView`)
- Auditoría de fábricas de construcción, mineras automatizadas, modificadores de terraformación y astilleros.

### 🛠️ 6. Diseñador Táctico de Naves (`BlueprintDesignerView`)
- Diseñador de prototipos navales con calculadora de masa (toneladas HS), potencia de motores y armamento.
- **Matcheador de Astilleros en Vivo**: Valida si tus astilleros en `AuroraDB.db` pueden construir la nave o calcula los meses de Retooling requeridos.

### 🚀 7. Laboratorio de Misiles & Matriz de Interceptación (`MissileEngineLab`)
- Simulador de rendimiento de misiles con cálculo de rango (M-km), velocidad (km/s), maniobrabilidad y **Matriz de Probabilidad de Impacto (Pk)** contra objetivos evasivos de 3,000, 6,000 y 12,000 km/s.

### ⚓ 8. Operaciones de Astilleros (`ShipyardOperationsView`)
- Monitor de gradas activas, capacidad de toneladas (BP/ton) y construcciones en curso.

### 🛸 9. Flotas Activas & Análisis Táctico (`ActiveFleetsView` & `FleetCompositionView`)
- Ubicación de grupos de combate, tonelaje total, velocidad máxima y composición de escuadrones.

### 🌌 10. Exploración del Sistema (`SystemExplorationView`)
- Cartografía de cuerpos celestes, cuerpos colonizables y yacimientos minerales descubiertos.

### 🤖 11. Asistente IA Imperial (`AIAssistantView`)
- Integración nativa con la API de Google Gemini para auditar tu imperio, evaluar amenazas alienígenas y optimizar diseños navales.

### 🎨 12. Motor de Temas Dinámicos en Tiempo Real (`ThemeManager`)
- 6 temas visuales con repintado instantáneo en tiempo real:
  - `👑 Imperial Gold (Fran Gómez Edition)`
  - `🌌 Cyber Neon Cyan`
  - `🛡️ Obsidian Emerald`
  - `⚡ Royal Nebula`
  - `☀️ Solar Flare Amber`
  - `👑 Deep Void Gold`

---

## 🌉 Conmutador Bidireccional de Ventanas (Win32 Bridge)

La suite incluye interoperabilidad nativa de ventanas de Windows (`User32.dll`):
- **Boton en la App**: **`🎮 VOLVER A AURORA 4X`** enfoca la ventana del juego de inmediato.
- **Botón en el Juego**: El parche inyecta el botón **`🚀 AURORA COMMAND SUITE`** dentro de Aurora 4X para saltar a la suite sin minimizar.

---

## 📁 Estructura del Repositorio

```
AuroraDesignSuite/
├── README.md                          <-- Guía oficial de GitHub
├── AuroraDesignSuite.sln             <-- Solución de Visual Studio
├── AuroraDesignSuite.csproj          <-- Proyecto C# WPF (.NET 7.0)
│
├── Services/                         <-- DatabaseService, ThemeManager, WindowBridge
├── Models/                           <-- Modelos de datos SQLite
├── Views/                            <-- Vistas XAML/C# de los 12 módulos
│
└── release/                          <-- Paquete ejecutable distribuible
    └── Aurora_Command_Suite_v2.7.1/
        ├── 🚀 ABRIR_SUITE.bat         <-- Lanzador principal
        ├── 📜 INSTRUCCIONES_Y_GUIA.html<-- Manual visual interactivo
        ├── 🧹 DESINSTALAR_SUITE.bat   <-- Desinstalador limpio
        └── App/                      <-- Binarios y ejecutable principal
```

---

## 🚀 Instalación y Uso Rápido

1. Descarga la última versión desde la sección de **Releases** de este repositorio.
2. Descomprime la carpeta `Aurora_Command_Suite_v2.7.1` dentro del directorio de tu juego **Aurora 4X (v2.7.1)**.
3. Ejecuta **`🚀 ABRIR_SUITE.bat`**.

---

## 📜 Licencia y Créditos

- **Diseño & Autor Principal**: Fran Gómez & Antigravity AI Team
- **Desarrollo**: C# .NET 7.0 WPF, SQLite, Harmony Patcher & Google Gemini API
- **Juego Base**: Aurora 4X de Steve Walmsley

© 2026 Aurora Command Suite. Desarrollado con pasión para la comunidad de Aurora 4X.
