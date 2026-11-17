# DLSSG Mod Manager — tutorial completo de uso

**Idioma:** [English](../USER_GUIDE.md) · [Italiano](USER_GUIDE.it.md) · [简体中文](USER_GUIDE.zh-Hans.md) · [हिन्दी](USER_GUIDE.hi.md) · **Español** · [Français](USER_GUIDE.fr.md) · [العربية](USER_GUIDE.ar.md) · [বাংলা](USER_GUIDE.bn.md) · [Português](USER_GUIDE.pt.md) · [Русский](USER_GUIDE.ru.md) · [اردو](USER_GUIDE.ur.md)

Para el **gestor 0.5.2** en **Windows 10/11 x64**. Los nombres que se indican a continuación se basan en la interfaz en inglés y aquí se presentan traducidos; puedes elegir el idioma en la esquina superior derecha de la aplicación. Este es un gestor independiente de los proyectos originales, no una aplicación oficial de NVIDIA, AMD, Intel, ANR u OptiScaler.

**Contenido:** [Inicio rápido](#start) · [Seguridad](#safety) · [Descarga](#install) · [Elegir una tecnología](#choose-provider) · [Pantalla y botones](#screen) · [Primera instalación](#first-install) · [Comprobaciones](#checks) · [Ajustes de SM86](#sm86) · [Ajustes de ANR](#anr) · [Ajustes de OptiScaler](#optiscaler) · [Ajustes del juego](#in-game) · [Verificación](#verify) · [Actualizaciones](#updates) · [Desactivar y restaurar](#backup) · [Solución de problemas](#troubleshooting) · [Archivos y ayuda](#files) · [Glosario](#glossary) · [Fuentes](#sources)

<a id="start"></a>
## 1. Inicio rápido: tu primera prueba

Dentro de la aplicación, abre **Tutorial** y selecciona **Iniciar recorrido guiado** para seguir ocho pasos prácticos. Busca entre los temas, usa **?** junto a un ajuste o pulsa **F1** mientras su control tiene el foco. **Mostrar en el gestor** abre y resalta los controles correspondientes sin aplicar cambios.

1. Cierra el juego. Elige un **juego de un jugador sin conexión**.
2. Abre el gestor y deja que termine la comprobación del paquete de la tecnología seleccionada.
3. Elige la tecnología y después selecciona el juego en Steam o elige su carpeta/EXE. Deja que el gestor lo analice.
4. Lee **2 · Revisa el resultado**. Abre **Diagnóstico** para ver la lista completa. Resuelve todas las comprobaciones **Bloqueadas**; lee las advertencias antes de continuar.
5. En **Ajustes del mod**, conserva los valores de la primera prueba: **SM86: DLSS, 2×**; **ANR: Auto, 2×**; **OptiScaler: reescalado automático, entrada FG desactivada y salida FG desactivada**.
6. Vuelve a **Gestión de juegos**. Elige **Instalar mod**, comprueba el destino y los ajustes en la confirmación y confirma. Espera a que termine.
7. Inicia el juego y sigue los [pasos dentro del juego específicos de cada proveedor](#in-game). Juega un momento y comprueba el movimiento, el HUD y la respuesta a los controles.
8. Lee el registro en **Diagnóstico**. Para OptiScaler, pulsa también **Insert** dentro del juego para examinar la salida activa. La carga de una DLL por sí sola no demuestra que la generación de fotogramas funcione.

Cambia **un solo ajuste cada vez** después de una primera prueba que funcione. Cierra el juego, aplica el cambio en el gestor y reinicia el juego antes de comparar.

<a id="safety"></a>
## 2. Lee esto antes de modificar un juego

**Usa estos mods solo en juegos de un jugador sin conexión. Los archivos de juego modificados pueden activar el anti-cheat o provocar expulsiones.** El gestor no ofrece evasión del anti-cheat ni protección contra expulsiones. Elegir un modo sin conexión no garantiza la ausencia de anti-cheat; si se detecta uno, la instalación sigue bloqueada.

El gestor comprueba archivos y requisitos. **«Requisitos detectados» no certifica la compatibilidad durante la partida, la estabilidad ni una mejora de FPS.** Conserva copias de seguridad habituales de tus partidas guardadas importantes; las copias del gestor cubren sus cambios en los archivos del juego, no todas las partidas guardadas ni el juego completo.

No acumules cargadores manualmente, no copies DLL de origen desconocido sobre los archivos del juego ni elimines un conflicto solo para superar una comprobación. Revisa primero el mod existente. Los binarios de los proyectos originales se descargan de sus repositorios como datos; el gestor no ejecuta sus scripts de instalación.

<a id="install"></a>
## 3. Descarga y abre el gestor

Usa las [versiones oficiales del gestor](https://github.com/VenomDrake/DLSSG-Mod-Manager/releases/latest).

| Descarga | Qué hacer |
|---|---|
| **ZIP portátil** | Extrae el **ZIP completo** en una carpeta con permisos de escritura y abre `DLSSGModManager.exe`. No lo ejecutes desde dentro del ZIP. Conserva juntos los archivos de licencia y documentación. |
| **Instalador de Windows (.msi)** | Cierra el gestor y ejecuta el MSI. Se instala para tu usuario de Windows y crea accesos directos. |

Los paquetes publicados incluyen el entorno de ejecución .NET del gestor. No necesitas instalar Python, CUDA Toolkit ni un extractor de archivos aparte. Un proveedor puede seguir necesitando controladores gráficos, Visual C++ o componentes Vulkan; se comprueban por separado.

El gestor y el MSI no están firmados. Usa únicamente las versiones del proyecto que querías descargar; no desactives globalmente la seguridad de Windows. El primer inicio necesita internet para obtener el paquete de un proveedor. Después puede ser posible usar sin conexión un paquete verificado en caché, pero **una caché sin conexión no confirma que sea la última versión**.

<a id="choose-provider"></a>
## 4. ¿Qué tecnología elijo?

Elige en **Reescalado y generación de fotogramas**. En esta guía también se llama **proveedor** a una tecnología.

| Opción | Finalidad y requisitos para la primera prueba |
|---|---|
| **DLSSG for SM86 · NVIDIA / XeSS** | El procedimiento revisado por el gestor para **NVIDIA RTX 20/30**, controlador **R580 o posterior**, programación de GPU acelerada por hardware (HAGS) configurada y un entorno de ejecución nativo de DLSS Frame Generation compatible en el juego. El juego usa DirectX 12. Empieza con DLSS y 2×. |
| **Apex Neural Rendering · Beta** | Un proveedor FG alternativo para las familias que declara compatibles el gestor: **GTX 16 / RTX 20–50** y **Radeon RX 5000/6000/7000/9000**. Necesita DLSS Frame Generation nativo x64 en el juego mediante DX12/Vulkan, Visual C++ x64 y un cargador Vulkan. GTX 10/Pascal e Intel Arc quedan fuera de la compatibilidad ANR de este gestor. Empieza con Auto y 2×. ANR sigue en fase beta. |
| **OptiScaler · Reescalado / FG** | Sustituye el reescalador temporal DLSS/FSR/XeSS de un juego compatible y puede gestionar las rutas FG admitidas. DLSS Frame Generation nativo **no es necesario para usar solo reescalado**. NVIDIA, AMD e Intel pueden ser opciones, pero cada salida tiene límites adicionales. Empieza con reescalado Auto y FG **desactivada/desactivada (Off/Off)**. |

HAGS y NVIDIA R580 **no son requisitos universales** que hereden ANR u OptiScaler. La **salida** DLSS nativa de OptiScaler sigue necesitando NVIDIA RTX y el entorno de ejecución DLSS del juego. OptiScaler no permite que todos los motores funcionen en todas las GPU.

Los procedimientos revisados por el gestor 0.5.2 son **SM86 0.4.1/0.4.2**, **ANR FG-V.B15/B17** y **OptiScaler 0.9.5**. Una descarga más reciente del proyecto original puede seguir bloqueada hasta que se revisen sus requisitos modificados.

El gestor recuerda el proveedor seleccionado; **no** elige automáticamente el mejor para tu GPU. Selecciona una tecnología adecuada antes de interpretar un fallo de GPU.

<a id="screen"></a>
## 5. Entiende la pantalla y todos los botones principales

La pestaña **Tutorial** contiene esta guía completa sin conexión, con navegación por temas y búsqueda. El recorrido guiado permanece junto al gestor: **Atrás** y **Siguiente** recorren sus ocho pasos, **Leer la explicación completa** abre el tema correspondiente y **Cerrar recorrido guiado** termina el recorrido. Avanzar no marca las comprobaciones de compatibilidad como superadas; la instalación sigue requiriendo la revisión y confirmación habituales.

![Pantalla de gestión de juegos](../screenshot.png)

| Área o control | Para qué sirve |
|---|---|
| **Versión del gestor** | La versión de la aplicación. Es independiente de la versión del paquete del mod seleccionado. |
| **Idioma** | Cambia el idioma de la interfaz. El árabe y el urdu usan una disposición de derecha a izquierda. Los valores de configuración como `dxgi.dll` conservan su escritura original. |
| **Selector de tecnología** | Elige SM86, ANR u OptiScaler. Al cambiarlo se vuelven a comprobar el paquete y los requisitos del proveedor; no modifica inmediatamente el juego. |
| **Gestión de juegos** | Elige el juego, revisa el resultado y aplica el mod. |
| **Ajustes del mod** | Edita los ajustes del proveedor seleccionado. Editar un campo por sí solo no escribe en el juego. |
| **Diagnóstico** | Comprobaciones completas, detalles del paquete/PC, registros, enlaces a controladores, exportación de informes, desactivación y restauración. |
| **Juegos de Steam instalados / ↻** | Selecciona un juego de Steam detectado; ↻ actualiza las bibliotecas. Que un juego aparezca en Steam no implica que admita el mod. |
| **Elegir carpeta / Elegir EXE** | Selecciona un juego manualmente, también fuera de Steam. Elige el EXE de renderizado real, no un lanzador, un programa de informes de fallos ni un instalador. |
| **Cuadro de ruta / Analizar juego** | Puedes pegar la ruta de una carpeta/EXE y analizarla. Al cambiar la ruta se borra el resultado anterior para evitar reutilizarlo por accidente. |
| **Ejecutable de renderizado** | Permite resolver casos con varios candidatos. En muchos juegos Unreal, usa `GameName\Binaries\Win64\...-Win64-Shipping.exe`, no el pequeño lanzador de la carpeta principal. |
| **Ver todas las comprobaciones / Ajustar la configuración del mod** | Abre la pestaña correspondiente. |
| **Instalar / Actualizar / Aplicar ajustes / Cambiar tecnología** | Una zona de acción cuya etiqueta cambia según la situación. Solo aplica cambios tras la confirmación y el cumplimiento de los requisitos. |
| **Iniciar juego** | Inicia el juego seleccionado y usa Steam cuando encuentra una correspondencia. No instala el mod. |
| **Cancelar operación** | Solicita una parada segura. Espera a que el gestor termine una aplicación crítica de cambios o su reversión. |

En **Diagnóstico**, el botón **↻** del paquete comprueba el paquete del proyecto original, mientras que el **↻** del PC vuelve a leer el hardware. **Abrir ajustes gráficos de Windows** abre la configuración de Windows; no activa HAGS por ti. Los botones de controladores abren las páginas de NVIDIA, AMD o Intel según el proveedor/adaptadores seleccionados. **Proyecto original** abre el repositorio del proveedor.

El pequeño aviso **Actualizar y reiniciar** se refiere al **propio gestor**. Su **×** descarta el aviso de esa versión para la ventana actual; no desinstala nada.

<a id="first-install"></a>
## 6. Instala, cambia ajustes o cambia de tecnología

1. Termina la descarga/verificación del paquete y selecciona el EXE de renderizado correcto.
2. Revisa el resumen y todas las comprobaciones. Cierra el juego y cualquier proceso del juego que siga abierto.
3. Configura los valores de la primera prueba o las opciones que quieras usar. El resultado puede cambiar si una opción añade un requisito.
4. Pulsa el botón de acción inferior. Lee la tecnología, la carpeta de renderizado, la versión y los ajustes que aparecen en la confirmación.
5. Confirma. El gestor prepara los archivos verificados, crea una copia de seguridad, desactiva cargadores antiguos, modifica los datos y activa el cargador seleccionado en último lugar. Deja que termine.
6. Reinicia el juego. Un juego que ya estaba abierto no puede usar de forma fiable los ajustes recién escritos.

**«Ya tienes la última versión» se refiere a la identidad del paquete instalado.** No significa que un ajuste que acabas de cambiar ya esté aplicado. Usa **Aplicar ajustes**, con el juego cerrado, para guardar esos cambios.

En una instalación gestionada, el cambio de proveedor usa el mismo sistema de copias de seguridad e historial. OptiScaler 0.9.5 independiente y el cargador OptiScaler personalizado de ANR son diferentes; el gestor no los combina. Un Addon ANR externo puede exigir restaurar la instalación externa antes de cambiar. Las instalaciones manuales desconocidas se protegen en lugar de sobrescribirse sin avisar.

<a id="checks"></a>
## 7. Interpreta las comprobaciones sin hacer suposiciones

| Estado | Significado y acción |
|---|---|
| **Superado** | Esta comprobación concreta tuvo éxito. No demuestra que se cumplan todos los demás requisitos ni que el juego funcione. |
| **Información** | Contexto, por ejemplo un ajuste o un componente opcional. Léelo si corresponde. |
| **Advertencia / Revisar** | Algo requiere atención o confirmación dentro del juego. No bloquea automáticamente; tampoco promete que continuar sea seguro o compatible. |
| **Sin confirmar** | El gestor no pudo determinar un hecho. Lee el detalle en lugar de darlo por superado. |
| **Bloqueado** | La instalación no puede continuar hasta que se resuelva la causa. |

Las comprobaciones habituales abarcan Windows/x64, GPU y controlador, HAGS cuando se necesita, componentes Visual C++/Vulkan, identidad y estructura admitida del paquete, análisis completo/permisos, EXE correcto, entrada/entorno de ejecución nativo, indicios de API gráfica, indicadores de anti-cheat, cargadores existentes o modificados, juego en ejecución, destinos de solo lectura y espacio libre en disco. Las copias de seguridad y la preparación necesitan más espacio que el tamaño final del mod.

**No compatible** indica una incompatibilidad técnica detectada. **Requiere revisión** significa que los indicios están incompletos, que hay dudas sobre el EXE o que una entrada puede estar integrada en el juego. **Instalación bloqueada** puede indicar un conflicto o un problema del paquete, en lugar de un juego incompatible. Un análisis estático de archivos no puede demostrar la ausencia de anti-cheat ni garantizar que el juego funcione.

<a id="sm86"></a>
## 8. SM86: todos los ajustes del gestor

![Ajustes de SM86](../settings-sm86.png)

**Primera prueba recomendada:** DLSS, 2×, optimización 1, preajuste A, sustitución de SR desactivada, registro 2, registro de tiempos desactivado, `version.dll`.

| Ajuste | Explicación sencilla y opciones |
|---|---|
| **Motor de generación** | **NVIDIA DLSS-G** o **Intel XeSS-FG** selecciona la implementación FG del mod. Aquí XeSS es la implementación del mod SM86, no una promesa de compatibilidad general con GPU Intel. Empieza con DLSS. |
| **Multiplicador de fotogramas** | DLSS ofrece **2×–6×**; XeSS, **2×–4×**. Estos valores solicitan un multiplicador: 2× significa un fotograma generado por cada fotograma real; 3× significa dos, y así sucesivamente. El juego puede imponer un límite inferior, y una solicitud no admitida puede impedir FG. Empieza con 2×. Más fotogramas generados no significan una latencia proporcionalmente menor ni una fluidez garantizada. |
| **Optimización de núcleos** | **0:** núcleos originales, útiles para comparar. **1:** aceleraciones con salida exacta, recomendado. **2:** procesamiento aproximado más rápido. **3:** aproximación máxima. Los niveles superiores pueden cambiar el resultado; compara bordes en movimiento y la interfaz en lugar de suponer que el número más alto es mejor. Se aplica a DLSS-G. |
| **Preajuste de DLSS-G** | **A:** opción normal para la primera prueba. **B:** recomposición de la interfaz cuando el juego proporciona búferes adecuados sin HUD/de interfaz; no puede crear esos búferes. **C:** FG más ligera con un posible sacrificio de calidad de imagen, especialmente en bordes del HUD/de oclusión. Se aplica a DLSS-G. |
| **Sustitución de Super Resolution** | **Desactivado (Off)** conserva el reescalador del juego. **M-INT8 / L-INT8** selecciona los núcleos M/L INT8 del proyecto. **M-FP16 / L-FP16** selecciona el procesamiento FP16 correspondiente. M/L son opciones de modelo, no una clasificación universal de rendimiento. Las sustituciones pueden reducir el rendimiento en los modos Calidad, Equilibrado o DLAA; déjalas desactivadas en la primera prueba. |
| **Flujo óptico XeSS** | **Coarse**, **Middle**, **Fine** cambian la resolución del procesamiento de flujo. Un procesamiento más fino puede consumir más tiempo de GPU. **CoarseAt4K** usa Coarse cuando la imagen de salida tiene al menos 3840×2160 píxeles; en los demás casos usa Middle. Empieza con Middle. Se aplica a XeSS-FG. |
| **Tratamiento del HUD de XeSS** | **Auto**, **Exigir imagen sin HUD (1)** o **No usar imagen sin HUD (0)**. En SM86 **0.4.2 revisado, Auto se comporta como 0**: se interpola toda la imagen, incluida la interfaz. El modo 1 requiere que el juego proporcione una imagen sin HUD correspondiente y puede fallar si no existe. |
| **Detalle del registro** | **0 Desactivado**, **1 Errores**, **2 Diagnóstico (recomendado)**, **3 Trazas detalladas**. Un mayor detalle ayuda a diagnosticar, pero aumenta el registro. |
| **Registrar tiempos de fotogramas** | Escribe datos de tiempos adicionales para investigar. No fija un límite de FPS ni activa FG. Déjalo desactivado en la primera prueba. |
| **DLL de carga** | `version.dll` (recomendada), `winmm.dll`, `dbghelp.dll`, `dinput8.dll`, `dxgi.dll`, `d3d12.dll`. El nombre de archivo determina cómo carga el juego el mod. Mantén un solo cargador activo; prueba alternativas a través del gestor si el mod no carga. No acumules copias manualmente. |

DLSS-G y XeSS-FG tienen ajustes diferentes. Que una opción específica de un motor esté visible no significa que esté activa si has seleccionado el otro motor.

**HAGS para SM86:** abre los ajustes gráficos de Windows desde Diagnóstico, activa la programación de GPU acelerada por hardware si Windows la ofrece y reinicia Windows. El estado del registro/configuración y el estado activo durante la ejecución se comprueban por separado. El registro del juego proporciona la confirmación durante la ejecución. El gestor no cambia automáticamente el controlador ni HAGS.

<a id="anr"></a>
## 9. ANR: todos los ajustes del gestor

![Ajustes de ANR](../settings-anr.png)

**Primera prueba recomendada:** ruta de cálculo Auto, 2×, 80 nits por unidad, tratamiento de cambios de escena activado, `dxgi.dll`. El motor es ANR; los controles de optimización/preajuste/sustitución de SR de SM86 no se aplican.

| Ajuste o modo | Significado |
|---|---|
| **Full / Addon** | Se selecciona automáticamente. **Full** incluye la configuración de cargador/proveedor revisada. **Addon** añade ANR a una instalación externa compatible de OptiScaler y conserva su cargador. Un cambio gestionado de OptiScaler independiente → ANR usa Full. No elijas ni copies manualmente un addon cualquiera. |
| **Multiplicador de fotogramas** | Solicita **2×–6×**, según la integración del juego. Empieza con 2×. El gestor escribe el número correspondiente de fotogramas adicionales, no seis fotogramas adicionales al elegir 6×. |
| **Ruta de cálculo** | **auto**, **coopmat**, **packed**, **dp4a**, **portable**, **portable-linalg**, **portable-xmx** son formas alternativas de realizar los cálculos de ANR. No son niveles de calidad de imagen. Auto usa la lógica de selección del proveedor; las rutas explícitas requieren funciones adecuadas en la GPU y no se garantiza que todas funcionen o sean más rápidas. Conserva Auto salvo que estés diagnosticando un problema concreto. |
| **Luminancia HDR** | **40–1000**, valor predeterminado **80**, describe la luminancia asociada a un valor lineal de 1.0 en el procesamiento HDR/PQ del proveedor. No es el control de brillo del monitor. Conserva 80 salvo que una investigación específica de HDR requiera cambiarlo. |
| **Tratamiento de cambios de escena** | Activado de forma predeterminada; evita interpolar entre cortes bruscos. Déjalo activado en las pruebas normales. |
| **DLL de carga** | `dxgi.dll`, `winmm.dll`, `version.dll`; empieza con `dxgi.dll`. En modo Addon se conserva el cargador externo y el selector queda bloqueado. |

La etiqueta **Beta** de ANR describe al proveedor original, incluso si el gestor es una versión estable. ANR necesita los componentes Visual C++ x64 revisados y el cargador Vulkan aunque la API gráfica del juego sea DX12. No se impone el requisito de HAGS de SM86.

<a id="optiscaler"></a>
## 10. OptiScaler: todos los ajustes del gestor

![Ajustes de OptiScaler](../settings-optiscaler.png)

**Primera prueba recomendada:** todos los reescaladores en **Auto**, entrada FG **desactivada (Off)**, salida FG **desactivada (Off)**, identidad de GPU simulada en DXGI **Auto**, registro **2 · Información**, cargador `dxgi.dll`.

### Opciones de reescalador

Una **entrada** es la tecnología seleccionada en el menú del propio juego. Una **salida** es lo que OptiScaler ejecuta realmente. Por ejemplo, un juego puede proporcionar una entrada DLSS mientras OptiScaler genera una salida XeSS. Seleccionar DLSS como entrada del juego no demuestra por sí solo que DLSS sea la salida activa.

| Campo del gestor | Valores disponibles |
|---|---|
| **Reescalador DirectX 11** | Auto; FSR 2.2 (`fsr22`); FSR 3.1 (`fsr31`); XeSS nativo (`xess`); XeSS mediante D3D11on12 (`xess_12`); FSR 2.1/2.2/3.1 mediante interoperabilidad DX12 (`fsr21_12`, `fsr22_12`, `fsr31_12`); DLSS. |
| **Reescalador DirectX 12** | Auto; XeSS; FSR 2.1; FSR 2.2; FSR 3.1/4; DLSS. |
| **Reescalador Vulkan** | Auto; FSR 2.1/2.2/3.1/4 nativo según admita el motor; XeSS nativo; FSR 2.1 o FSR 3.1/4 mediante interoperabilidad DX12 (`fsr21_12`, `fsr31_12`); DLSS. |

Solo la API activa del juego usa su ajuste correspondiente. **Auto** sigue los valores predeterminados de OptiScaler; es el punto de partida más seguro, no una promesa de la salida más rápida. `_12` significa que se usa una ruta de interoperabilidad DX12, que puede tener un coste adicional.

**XeSS nativo en DX11** requiere Intel Arc; `xess_12` es una opción de interoperabilidad diferente. La **salida DLSS** requiere NVIDIA RTX y el entorno de ejecución DLSS x64 del juego. **FSR 3.1/4** no promete FSR 4 en todas las GPU: la compatibilidad del motor y del hardware determina el modelo activo o la alternativa usada. Verifica la salida real en la superposición; el gestor no fuerza FSR 4 si no es compatible.

En DX11, la ruta nativa `fsr31` es FSR 3.1; la ruta FSR 4 usa interoperabilidad DX12. Una etiqueta general «3.1 / 4» no garantiza FSR 4 nativo en DX11.

### Entrada y salida de generación de fotogramas

Para una primera prueba solo de reescalado, mantén **ambos campos desactivados (Off)**. Para activar FG después, elige una entrada **y** una salida compatibles; se bloquea cambiar solo una de ellas a un modo activo.

| Entrada FG | Qué debe proporcionar el juego |
|---|---|
| **Desactivado (Off)** (`nofg`) | No se solicita FG de OptiScaler. Desactiva también la salida. |
| **DLSS Frame Generation** (`dlssg`) | DLSS FG nativo con una integración Streamline 2 adecuada; el gestor comprueba los indicios disponibles y avisa cuando no puede confirmarla. |
| **DLSS-G · Nukem** (`nukems`) | Un entorno de ejecución nativo de DLSS-G. Combina esta entrada con la salida **Nukem · FSR 3**. |
| **FSR Frame Generation** (`fsrfg`) | Una fuente FSR FG nativa compatible en el juego. El nombre de una DLL por sí solo no permite determinar esa integración. |
| **Entrada de reescalador / OptiFG** (`upscaler`) | Una entrada de reescalado temporal que funcione, mediante la ruta OptiFG DX12 revisada. Puede ser necesario corregir el HUD en la superposición del proyecto original. |
| **FSR 3.0 Frame Generation** (`fsrfg30`) | Una fuente FSR 3.0 FG nativa compatible. |

| Salida FG | Significado |
|---|---|
| **Desactivado (Off)** | No hay salida FG. Desactiva también la entrada. |
| **AMD FSR Frame Generation** (`fsrfg`) | Usa el motor FidelityFX FG compatible. No garantiza la misma versión ni las mismas funciones FSR en todas las GPU. |
| **Intel XeSS Frame Generation** (`xefg`) | Requiere las funciones de GPU del motor, incluido soporte DP4a adecuado y condiciones apropiadas del juego/pantalla. No se activa para cualquier dispositivo Intel, AMD o NVIDIA solo porque se detecte su fabricante. El soporte de varios fotogramas tiene otros límites del proyecto original. |
| **Nukem · FSR 3** (`nukems`) | Usa la ruta DLSSG-to-FSR3 incluida y revisada. Es obligatoria cuando la entrada seleccionada es Nukem. |

El gestor 0.5.2 bloquea la FG seleccionada cuando detecta una ruta **DX11**. Para una ruta **Vulkan** detectada, la combinación FG permitida y revisada es **Nukem → Nukem**. Las demás rutas FG ofrecidas requieren DX12; si no se puede determinar la API, sigue siendo necesaria la confirmación dentro del juego. Estos controles no vuelven compatible una integración de juego no admitida.

### Otros controles y ajustes avanzados

| Ajuste | Significado |
|---|---|
| **Identidad de GPU simulada en DXGI: Auto / Activado / Desactivado** | Cambia la identidad de GPU que se comunica mediante DXGI para mejorar la compatibilidad. No mejora físicamente la GPU ni garantiza rendimiento. Conserva Auto al principio. No es una protección contra el anti-cheat; usa mods solo sin conexión. |
| **Detalle del registro** | **0 Trazas**, **1 Depuración**, **2 Información (recomendado)**, **3 Advertencias**, **4 Errores**. A diferencia de SM86, **0 no significa registro desactivado**. El gestor activa el registro en archivo para diagnóstico. |
| **DLL de carga** | `dxgi.dll`, `winmm.dll`, `version.dll`, `d3d12.dll`, `dbghelp.dll`, `wininet.dll`, `winhttp.dll`. Empieza con `dxgi.dll`; si hace falta, cámbiala a través del gestor con el juego cerrado. |

Pulsa **Insert** dentro del juego para abrir la superposición completa del proyecto OptiScaler, sus ajustes avanzados y la información del motor activo. Sus numerosos controles avanzados pertenecen a OptiScaler; el gestor expone las opciones comunes revisadas que aparecen arriba. Conserva una configuración de referencia que funcione, cambia una sola cosa cada vez y usa el [manual oficial](https://github.com/optiscaler/OptiScaler/wiki) para los controles que no aparecen en este gestor. Las instrucciones de la versión estable 0.9.5 y de la versión v10 nightly no son intercambiables.

Al aplicar ajustes desde el gestor se vuelven a escribir los valores que hayas seleccionado. Si has cambiado esas mismas opciones en la superposición, ajusta las elecciones del gestor para que coincidan antes de aplicar; los ajustes INI no relacionados se conservan cuando se admite.

<a id="in-game"></a>
## 11. Qué seleccionar dentro del juego

**SM86 o ANR:** activa la opción **NVIDIA DLSS Frame Generation** del juego después de instalar. Empieza con 2× si el juego lo ofrece. Reinicia el juego si su menú lo requiere. La calidad del reescalado, como Calidad o Equilibrado, es una elección independiente de FG.

**Primera prueba con OptiScaler:** activa un reescalador temporal compatible en el juego, mantén desactivadas la entrada/salida FG del gestor e inspecciona la salida activa con Insert. No tienes que activar DLSS FG para probar solamente el reescalado. Si después eliges una entrada FG nativa, activa la función correspondiente del juego. Para la entrada Reescalador/OptiFG, usa la ruta DX12 revisada y las indicaciones de la superposición del proyecto original.

Un ajuste **2×** gris puede significar simplemente que ese es el único multiplicador disponible. **No** demuestra que FG esté funcionando. El gestor no puede desbloquear todos los multiplicadores en el menú de todos los juegos. Del mismo modo, una etiqueta del menú del juego puede describir la entrada mientras OptiScaler usa una salida diferente.

Para comparar, usa la misma escena, resolución y ajustes. Observa el texto en movimiento, las vallas, los bordes de personajes y los elementos del HUD, además de los FPS y la respuesta a los controles. Los FPS generados/mostrados y los FPS realmente renderizados son diferentes. No se puede suponer que mostrar más fotogramas duplique la capacidad de respuesta.

<a id="verify"></a>
## 12. Diagnóstico: cómo saber qué ha ocurrido

En **Diagnóstico**, elige **Leer registro del juego** después de una prueba breve. Iniciar desde el gestor y leer el registro activa la lectura periódica mientras el gestor sigue abierto. Si el resultado queda pendiente, cierra el juego y vuelve a leer el registro completo.

| Proveedor | Qué puede determinar el gestor |
|---|---|
| **SM86** | Examina la sesión registrada correspondiente para comprobar la creación correcta de la función FG y su primera evaluación. No basta con cargar el mod o ver una función SR. Lee los errores y la información HAGS/GPU durante la ejecución. |
| **ANR** | Distingue la carga y la inicialización de las evaluaciones que generan fotogramas realmente. Para confirmar FG se necesita una sesión completa con recuentos positivos de evaluaciones de generación y sin errores de generación. Los recuentos y el multiplicador registrado ayudan al diagnóstico. |
| **OptiScaler** | Informa de la carga y de los errores del registro de la sesión actual. Una DLL cargada o una cadena de intercambio creada **no demuestra que SR o FG funcionen ni que el rendimiento haya aumentado**. Verifica el motor activo, las estadísticas de fotogramas y el resultado visual en la superposición del juego. |

Un registro parcial, sin terminar o demasiado grande no permite determinar que toda la sesión haya tenido éxito. **Sin errores** significa que no se encontraron errores en la parte que entiende el lector del registro, no que se haya certificado el rendimiento o FG. Comprueba la hora de sesión que aparece para no confundir una prueba anterior con la actual. En un PC con varias GPU, confirma qué adaptador usa realmente el juego.

<a id="updates"></a>
## 13. Se pueden actualizar tres cosas diferentes

| Actualización | Cómo funciona |
|---|---|
| **Aplicación del gestor** | Comprueba las versiones estables de GitHub al iniciar y periódicamente mientras está abierto (aproximadamente cada seis horas). Solo aparece un pequeño aviso si existe una versión estable más reciente. Elige **Actualizar y reiniciar**; descarga y verifica el paquete portátil/MSI correcto, y después actualiza y reinicia con tu consentimiento. Vuelve a abrir el gestor para comprobarlo de inmediato. La comprobación estable normal omite las versiones preliminares. |
| **Paquete descargado del proveedor** | Se comprueba para el proveedor seleccionado al iniciar o después de seleccionarlo; el botón ↻ del paquete lo vuelve a comprobar. Una nueva descarga verificada actualiza la caché. **No** se aplica sola a tus juegos. |
| **Mod en un juego concreto** | Selecciona ese juego, analízalo y elige **Actualizar mod** después de las comprobaciones y la confirmación. Los demás juegos conservan su instalación hasta que los actualices por separado. |

El gestor compara el ID de versión, el ID del archivo y SHA-256, por lo que puede detectar archivos sustituidos aunque mantengan la misma versión visible. **Descargado**, **actualizado**, **caché sin conexión** y **todavía no compatible con este gestor** son resultados diferentes. Un paquete puede superar la comprobación de integridad de la descarga y seguir teniendo un procedimiento de instalación desconocido. Espera a una versión del gestor que lo admita expresamente; no lo fuerces copiando sus DLL manualmente.

El gestor **0.4.3 y posteriores** usa el repositorio renombrado. Las versiones muy antiguas, **0.4.2 o anteriores**, pueden necesitar una actualización manual inicial con el paquete MSI/portátil completo más reciente. El problema de ventana de ayuda/error 1639 del actualizador MSI 0.2.0/0.2.1 también necesita una única instalación manual de un MSI posterior corregido. Usa el mismo tipo de instalación y cierra primero el gestor. Conserva intactas las carpetas de juegos y copias de seguridad.

<a id="backup"></a>
## 14. Desactivar, reactivar, restaurar y cambiar de proveedor

**Desactivar mod** impide temporalmente que se active el mod gestionado; no es una desinstalación completa. Normalmente, el gestor desactiva su cargador y conserva los archivos de ejecución y las copias de seguridad. Para un Addon ANR externo, restablece la selección externa de OptiScaler y conserva ese cargador externo.

Para **reactivarlo**, mantén cerrado el juego y vuelve a usar la acción principal Instalar/Actualizar para el proveedor gestionado seleccionado.

**Restaurar estado anterior** restaura la copia de seguridad asociada a la instalación/actualización/cambio de proveedor gestionado actual. No es una orden general de «deshacer el último botón» ni una función específica para deshacer Desactivar. Según el historial, puede recuperar una versión anterior del mod, otro proveedor, una instalación desactivada o el estado original sin modificar. Restaurar la copia original de la primera instalación puede eliminar el mod gestionado activo. Lee la confirmación.

Los registros de instalaciones anteriores pueden conducir a copias anteriores; por eso, nuevas restauraciones pueden retroceder más en el historial mientras siga existiendo un registro. No hay un explorador general del historial para rehacer. Las copias de seguridad se conservan; los archivos modificados o desconocidos pueden bloquear la restauración en lugar de ser sobrescritos.

**Nunca borres manualmente** `.dlssg-manager.json`, `.dlssg-manager.transaction.json` ni `.dlssg-manager-backups` para eliminar un error. Conservan información de propiedad y recuperación. Después de una operación interrumpida, mantén el juego cerrado, conserva esos archivos y vuelve a abrir el gestor. La recuperación usa el diario durante las operaciones de archivos. Si sigue bloqueada, exporta un informe y pide ayuda en lugar de reactivar o borrar cargadores manualmente.

<a id="troubleshooting"></a>
## 15. Problemas habituales y el siguiente paso razonable

| Lo que ves | Qué hacer |
|---|---|
| **Instalar está desactivado** | Espera a que termine la operación actual, selecciona el EXE real y examina las comprobaciones Bloqueadas en Diagnóstico. Una compilación de solo lectura/vista previa de la interfaz no puede instalar. Resuelve las causas en lugar de copiar archivos manualmente. |
| **GPU no compatible** | Comprueba primero el proveedor seleccionado. El gestor recuerda un proveedor o usa uno predeterminado; no elige automáticamente el mejor para tu GPU. ANR y OptiScaler tienen requisitos diferentes de SM86. |
| **Juego no compatible / Requiere revisión** | Lee el motivo exacto. La falta de FG nativa es relevante para SM86/ANR; OptiScaler puede necesitar en cambio indicios de una entrada SR temporal. Las entradas FSR integradas pueden dar resultados inconclusos. Comprueba la API y el EXE del juego. |
| **Último paquete, pero instalación bloqueada** | La integridad/el estado de última versión es independiente de un procedimiento revisado, del hardware o de la compatibilidad del juego. Un procedimiento nuevo desconocido requiere actualizar/revisar el gestor. |
| **Lista de Steam vacía o falta el juego** | Actualiza las bibliotecas de Steam; si no aparece, elige manualmente la carpeta del juego o su EXE de renderizado real. |
| **Varios EXE** | Da prioridad al renderizador real. Para Unreal, examina `Binaries\Win64` y el candidato `Shipping.exe`. No elijas un lanzador ni un programa de informes de fallos solo para evitar la ambigüedad. |
| **El juego sigue abierto** | Ciérralo completamente y espera a que su proceso termine antes de instalar, cambiar ajustes, cambiar de tecnología o restaurar. |
| **HAGS configurado, pero ejecución aún sin confirmar** | Para SM86, reinicia Windows después de activar HAGS, prueba el juego y lee el registro más reciente. Un ajuste del registro por sí solo no demuestra que HAGS esté activo en esa sesión. |
| **Falta Visual C++ / Vulkan** | Usa instaladores oficiales de Microsoft o del fabricante de la GPU, no sitios de descarga de DLL. ANR necesita tanto los componentes Visual C++ x64 revisados como el cargador Vulkan. Vuelve a comprobar el hardware después. |
| **Cargador desconocido, mod manual o archivo modificado** | Consérvalo. Identifica y restaura/elimina el mod original mediante su propio procedimiento admitido, o pide ayuda con un informe. El gestor protege deliberadamente los archivos ajenos; no adopta OptiScaler manual sin avisar. |
| **Falta el registro / solo cargado / 2× gris** | Sigue los pasos adecuados dentro del juego, prueba brevemente y vuelve a leer el registro completo. Puede hacer falta otro cargador si no carga nada. El 2× gris y los registros que solo indican carga no demuestran FG. |
| **Imágenes fantasma, parpadeos, tirones o peores FPS** | Vuelve a la configuración de referencia recomendada. Compara un cambio cada vez, reduce la FG solicitada cuando corresponda y examina los errores. Prueba a desactivar/restaurar desde el gestor. No supongas que el multiplicador más alto, el flujo más fino o el nivel de aproximación más alto es lo mejor. |
| **Los ajustes cambiados no tienen efecto** | Cierra el juego, usa Aplicar ajustes y reinícialo. Comprueba el motor seleccionado y la API activa: los ajustes de otro motor/API pueden no influir. |
| **Operación interrumpida o restauración bloqueada** | Mantén el juego cerrado y conserva los registros de instalación y las copias de seguridad. Lee el error, exporta un informe y pide ayuda. No borres el diario de recuperación ni fuerces archivos desconocidos sobre la copia. |
| **No aparece la actualización del gestor** | Vuelve a abrir el gestor, confirma la conexión a internet y comprueba tu versión instalada. Las comprobaciones normales omiten las versiones preliminares. Las instalaciones muy antiguas pueden necesitar la actualización manual inicial descrita arriba. |

<a id="files"></a>
## 16. Archivos, privacidad y cómo pedir ayuda

Las preferencias habituales de la aplicación, las cachés y `application.log` están en `%LOCALAPPDATA%\DLSSGModManager`. Un directorio de datos personalizado o un inicio específico de vista previa de la interfaz puede usar otra ubicación. El registro de propiedad, el diario de transacciones y las copias de seguridad del juego seleccionado están junto a su **EXE de renderizado**, no necesariamente junto a su lanzador.

| Proveedor | Ubicación habitual del registro en el juego, relativa a la carpeta de renderizado |
|---|---|
| SM86 | `dlssg_sm86\logs\dlssg_sm86.log` |
| ANR | `OptiScaler\custom\Apex Neural Rendering FG\dlssg\nr_fg-*.log` |
| OptiScaler | `OptiScaler.log` |

Usa **Exportar informe de diagnóstico** en Diagnóstico y elige dónde guardar `DLSSG-diagnostic-report.json`. Incluye versiones del gestor/proveedor/paquete, hash del paquete/estado de actualización, detalles de GPU/controlador, comprobaciones e indicios disponibles en los registros. Sustituye la ruta raíz del juego seleccionado y el prefijo del perfil de usuario por marcadores como `<GAME>` y `<USER>`; revisa el archivo antes de compartirlo, porque esto no garantiza que se elimine toda cadena que pueda identificarte.

Los informes se guardan localmente y no se envían automáticamente. No hay cuentas ni telemetría. Al pedir ayuda, incluye el proveedor, el juego, la versión del gestor, el mensaje exacto que bloquea la instalación y qué has cambiado. Una captura del resultado y el informe exportado son más útiles que «no funciona». No compartas tokens, contraseñas ni archivos personales ajenos al problema.

<a id="glossary"></a>
## 17. Pequeño glosario

| Término | Significado sencillo |
|---|---|
| **FPS renderizados/base** | Fotogramas reales calculados por el juego. |
| **Generación de fotogramas / FG** | Fotogramas adicionales estimados que se insertan entre los reales. |
| **Reescalado / SR** | Reconstrucción de una imagen de mayor resolución a partir de un renderizado de menor resolución. Es independiente de FG. |
| **Entrada / salida** | La función del juego que intercepta OptiScaler / el motor que OptiScaler ejecuta en su lugar. |
| **API: DX11, DX12, Vulkan** | La interfaz gráfica que usa el juego. Las distintas API tienen rutas compatibles diferentes. |
| **HUD / sin HUD** | La interfaz en pantalla / una imagen del juego sin esa interfaz. |
| **DLL / cargador / proxy** | Una biblioteca de código y el archivo con nombre específico a través del cual el juego carga un mod. Su nombre importa. |
| **HAGS** | Programación de GPU acelerada por hardware de Windows. |
| **Runtime / entorno de ejecución** | Un componente de software necesario para una función, como la DLL DLSS del juego o Visual C++. |
| **Caché / SHA-256** | Un paquete descargado y guardado / una huella usada para comprobar la integridad de los archivos. Que la huella coincida no certifica que el juego funcione ni hace compatible un procedimiento desconocido. |
| **Copia de seguridad / diario** | Estado anterior guardado de los archivos / información para recuperar un cambio interrumpido. |

<a id="sources"></a>
## 18. Proyectos originales y más información

- [Descargas y proyecto del gestor](https://github.com/VenomDrake/DLSSG-Mod-Manager).
- [DLSSG for SM86](https://github.com/sdli1995/dlssg_for_sm86), de sdli1995 y colaboradores; [adaptación SM75](https://github.com/Coldwood1026), de Coldwood1026.
- [Apex Neural Rendering](https://github.com/apex-compute/ANR), de Apex Compute y colaboradores.
- [OptiScaler](https://github.com/optiscaler/OptiScaler) y su [wiki oficial](https://github.com/optiscaler/OptiScaler/wiki), de sus colaboradores.
- [Descripción general del gestor](README.es.md) y [avisos de terceros](../../THIRD_PARTY_NOTICES.md).

Esta guía describe los **controles expuestos por el gestor 0.5.2** y sus procedimientos de proveedor revisados. No es una garantía de compatibilidad de todos los juegos, una prueba de rendimiento ni un manual completo de cada opción de las superposiciones de los proyectos originales. Para funciones avanzadas exclusivas de esos proyectos, usa la documentación original de la versión correspondiente.
