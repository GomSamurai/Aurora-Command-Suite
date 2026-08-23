# 🚀 Aurora 4X Command Suite (v2.8 Master Expansion)
> **El Centro de Mando Táctico, Analítica en Tiempo Real y Asistente de Inteligencia Artificial para Aurora 4X.**

[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20.NET%207.0-blue.svg)](https://dotnet.microsoft.com/)
[![Aurora 4X Version](https://img.shields.io/badge/Aurora%204X-v2.7.1%20--%20v2.8-brightgreen.svg)](https://aurora2.pentarch.org/)
[![License](https://img.shields.io/badge/License-MIT-gold.svg)](LICENSE)
[![Author](https://img.shields.io/badge/Author-Fran%20Gómez-purple.svg)]()

---

## 🌟 Visión General

**Aurora 4X Command Suite (v2.8)** es una suite de mando e inteligencia espacial desarrollada para integrarse al 100% en tiempo real con el juego de estrategia **Aurora 4X**. 

Combina una interfaz gráfica sci-fi de última generación (WPF), telemetría directa desde la base de datos de juego `AuroraDB.db`, **sincronización en vivo por avance de tiempo en tiempo real**, **17 Módulos Estratégicos Integrados**, simulación táctica de misiles y naves con dossier dinámico en vivo, un **Monitor Táctico IA con Popups Overlay en tiempo real sobre el juego**, resolución 100% dinámica de rutas sin fallbacks estáticos y un **Generador de Novelas e Historia Épica**.

---

## ⚡ Novedades de la Versión 2.8 Master Expansion
- **6 Nuevos Módulos Estratégicos**: Incorporados los módulos de *Fuerzas Terrestres, Mausoleo Imperial, Inteligencia Alienígena, Simulador de Terraformación, Logística de Combustible y Anales Imperiales*.
- **Dossier Táctico Dinámico en Vivo & Escalímetro Real**: Síntesis automática de fichas de diseño naval en 4 puntos y catálogo calibrado de más de 100 objetos reales para comparaciones de longitud y masa.
- **Sincronización en Vivo por Avance de Tiempo**: Detección automática en tiempo real de cada incremento de tiempo (5s, 1m, 1h, 1d, 5d, 30d) en Aurora 4X y refresco instantáneo de telemetría sin intervención del usuario.
- **Aislamiento Multi-Imperio Puro por Raza**: Eliminados fallbacks hardcodeados legacy. Generación dinámica de fabricantes y empresas basada en el imperio/raza activo de tu partida actual (`GetCompanyNames`).
- **Resolución Dinámica de Rutas Portable**: Eliminadas rutas absolutas estáticas (`C:\...`). La aplicación detecta automáticamente la ubicación del juego y la base de datos de forma portable dondequiera que desempaquetes la suite.

---

## 📦 ¿Cómo Instalar y Jugar? (Interconexión 100% Automática)

Para que el lanzador **`🚀 ABRIR_SUITE.bat`** abra automáticamente el juego parcheado (**`AuroraPatch.exe`** / **`Aurora.exe`**) y la suite juntos en paralelo:

### 📥 Pasos Rápidos de Instalación:
1. Descarga la carpeta comprimida `Aurora_Command_Suite_v2.7.1.zip` desde la sección de **Releases** de este repositorio.
2. Extrae la carpeta `Aurora_Command_Suite_v2.7.1` **dentro del directorio principal de tu juego Aurora 4X** (en la misma carpeta donde residen `AuroraPatch.exe`, `Aurora.exe` y `AuroraDB.db`).
3. Entra en la carpeta extraída y haz doble clic en **`🚀 ABRIR_SUITE.bat`**.

```
📁 Tu_Carpeta_De_Aurora_4X/
 ├── AuroraPatch.exe                  <-- Patcher con inyección de botón
 ├── Aurora.exe                       <-- Juego base
 ├── AuroraDB.db                      <-- Base de datos de la partida
 └── 📁 Aurora_Command_Suite_v2.7.1/
      ├── 🚀 ABRIR_SUITE.bat           <-- ¡Doble clic aquí para lanzar TODO!
      ├── 📜 INSTRUCCIONES_Y_GUIA.html
      └── 📁 App/
```

> **⚡ Conexión e Inyección Automática**: Al hacer clic en `🚀 ABRIR_SUITE.bat`, la script detecta automáticamente `AuroraPatch.exe` en la carpeta superior del juego, inicia Aurora 4X con el botón **`🚀 AURORA COMMAND SUITE`** inyectado en el juego, y lanza la suite de mando en paralelo.

---

## ✨ Módulos de la Suite (17 Módulos Integrados)

### 📊 1. Gobierno & Economía Imperial (`EmpireOverviewView`)
- Auditoría en tiempo real del Producto Imperial Bruto (BP), ingresos por impuestos y centros financieros.
- Distribución de almirantes, científicos y gobernadores activos.
- Desglose de investigación acumulada e instalaciones de I+D.

### 🪐 2. Colonias & Inventario Trans-Newtoniano (`ColoniesOverviewView`)
- Control de existencias de los **11 Minerales Trans-Newtonianos**: *Duranium, Sorium, Neutronium, Corundium, Uridium, Gallicite, Boronide, Mercassium, Vendarite, Corbomite y Tritium*.
- Gráficos comparativos de producción minera, reservas coloniales e instalaciones industriales por planeta.

### 👥 3. Cuartel General de Oficiales (`CommandersHQView`)
- Gestión centralizada del cuerpo de oficiales navales, administradores planetarios e investigadores.

### 🏛️ 4. Mausoleo Imperial & Medallero de Honor (`OfficerMemorialView`)
- Salón de la Fama y registro de oficiales caídos en combate ✝️ o retirados 🏅.
- Registro de medallas concedidas, historial de bajas enemigas y años de servicio al imperio.

### 🪖 5. Ejércitos Terrestres & Desembarco Naval (`GroundForcesView`)
- Catálogo de formaciones terrestres, brigadas STO y clases de tropas.
- **Calculadora de Capacidad de Hangar de Transporte**: Selección de capacidad de hangar (5.000 t, 10.000 t, 25.000 t, 50.000 t) para determinar las naves necesarias de desembarco.

### 🔬 6. Laboratorios de I+D (`ResearchHQView`)
- Seguimiento de proyectos científicos activos, velocidad de investigación (BP/año) y bonificaciones de los científicos a cargo.

### 🏭 7. Complejo Industrial (`IndustrialHQView`)
- Auditoría de fábricas de construcción, mineras automatizadas, modificadores de terraformación y astilleros.

### 🛠️ 8. Diseñador Táctico de Naves & Dossier Dinámico (`BlueprintDesignerView`)
- Diseñador de prototipos navales con calculadora de masa (toneladas HS), velocidad y armamento.
- **Dossier Táctico Dinámico en Vivo**: Generación automática de especificaciones e informe de Inteligencia Naval Imperial.
- **Escalímetro de Magnitudes del Mundo Real**: Comparación inmediata con más de 100 objetos icónicos del mundo real (aviones, submarinos, rascacielos, puentes).

### 🚀 9. Laboratorio de Misiles & Matriz de Interceptación (`MissileEngineLab`)
- Simulador de rendimiento de misiles con cálculo de rango (M-km), velocidad (km/s), maniobrabilidad y **Matriz de Probabilidad de Impacto (Pk)**.

### ⚓ 10. Operaciones de Astilleros (`ShipyardOperationsView`)
- Monitor de gradas activas, capacidad de toneladas (BP/ton) y construcciones en curso.

### 🛸 11. Flotas Activas & Análisis Táctico (`ActiveFleetsView` & `FleetCompositionView`)
- Ubicación de grupos de combate, tonelaje total, velocidad máxima y composición de escuadrones.

### 🌌 12. Exploración del Sistema (`SystemExplorationView`)
- Cartografía de cuerpos celestes, cuerpos colonizables y yacimientos minerales descubiertos.

### 👽 13. Inteligencia Alienígena & Xenodiplomacia (`XenoIntelView`)
- Matriz de evaluación de amenazas extraterrestres (NPRs), nivel diplomático, puntos de contacto y catálogo de naves alienígenas avistadas.

### 🌱 14. Simulador & Planificador de Terraformación (`TerraformingView`)
- Monitoreo de atmósferas planetarias, composición de gases y **Simulador Interactivo por Gas Objetivo** para calcular el tiempo en Años y Meses Imperiales requerido para modificar presiones.

### ⛽ 15. Logística & Monitor de Combustible Sorium (`FuelLogisticsView`)
- Auditoría en tiempo real de los depósitos de combustible refinado y mineral Sorium crudo en colonias y estado de tanques/autonomía de todas las naves de la flota.

### 📜 16. Anales de la Historia Imperial (`ImperialChroniclesView`)
- Registro cronológico de eventos históricos (combate, investigación, exploración, industria) con sincronización de fecha estelar real basada en `StartYear`.

### 🤖 17. Matriz IA Imperial & Overlay HUD (`AIAssistantView`)
- Integración nativa con la API de Google Gemini y **Monitor Táctico en Tiempo Real** con avisos flotantes translúcidos sobre Aurora 4X.
- **Generador de Novelas e Historia Épica**: Convierte los registros de tu partida en un libro de ciencia ficción en capítulos (funciona tanto online como offline).

---

## 🎨 Motor de Temas Dinámicos Categorizado (`ThemeManager`)

- Paleta de 15 estilos visuales clasificados por categorías (`👑 Edición Insignia`, `🌌 Modos Oscuro`, `☀️ Modos Claro`, `☯️ Modos Neutros`, `💎 Ediciones Refinadas`).

---

## 🌉 Conmutador Bidireccional de Ventanas (Win32 Bridge)

- **Botón en la App**: **`🎮 VOLVER A AURORA 4X`** enfoca la ventana del juego de inmediato en estado maximizado.
- **Botón en el Juego**: El parche inyecta el botón **`🚀 AURORA COMMAND SUITE`** dentro de Aurora 4X para saltar a la suite al instante.

---

## 📜 Licencia y Créditos

- **Diseño & Autor Principal**: Fran Gómez & Antigravity AI Team
- **Desarrollo**: C# .NET 7.0 WPF, SQLite, Harmony Patcher & Google Gemini API
- **Juego Base**: Aurora 4X de Steve Walmsley

© 2026 Aurora Command Suite. Desarrollado con pasión para la comunidad de Aurora 4X.
