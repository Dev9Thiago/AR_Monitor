
# AR Monitor — Monitoreo y Control Robótico mediante Realidad Aumentada

Sistema de realidad aumentada desarrollado en Unity para la visualización, monitoreo e interacción con un manipulador robótico de seis grados de libertad (6 GDL), equipado con un mecanismo de agarre.

El proyecto integra reconocimiento visual mediante Vuforia Engine, simulación de articulaciones, monitoreo dinámico de variables técnicas e interfaces gráficas tridimensionales controladas mediante la orientación de la cámara.

El sistema fue desarrollado con fines académicos en la Universidad Militar Nueva Granada, como una aplicación de tecnologías de realidad aumentada al monitoreo de sistemas mecatrónicos.

**Repositorio:** https://github.com/Dev9Thiago/AR_Monitor

**Video demostrativo:** https://youtu.be/D18iNROxGto

---

## 1. Descripción general

AR Monitor permite visualizar un manipulador robótico virtual sobre un marcador físico reconocido mediante la cámara.

El modelo tridimensional fue desarrollado en SOLIDWORKS e importado a Unity mediante URDF, conservando su estructura articulada para simular el movimiento de sus componentes.

La aplicación permite controlar las articulaciones, visualizar variables técnicas y navegar entre interfaces gráficas mediante un sistema de selección basado en mirada.

### Funcionalidades principales

- Reconocimiento de marcadores mediante Vuforia Engine.
- Seguimiento de posición y orientación del manipulador virtual.
- Representación tridimensional de un robot de seis grados de libertad.
- Simulación de articulaciones mediante `ArticulationBody`.
- Control individual de las seis articulaciones.
- Control del mecanismo de apertura y cierre de la pinza.
- Retorno a la posición inicial mediante HOME.
- Movimiento automático mediante AUTO.
- Detención del movimiento automático mediante STOP.
- Visualización dinámica de variables técnicas.
- Panel general de telemetría.
- Paneles individuales asociados a las articulaciones.
- Interfaces gráficas en modo World Space.
- Selección de componentes mediante mirada.
- Manipulación de deslizadores mediante la orientación de la cámara.

---

## 2. Herramientas y tecnologías utilizadas

| Herramienta | Versión / Aplicación |
|---|---|
| Unity | 6000.6.2f1 |
| Vuforia Engine | 11.4.4 |
| C# | Desarrollo de los componentes del sistema |
| SOLIDWORKS | Diseño del manipulador robótico |
| URDF | Importación de la estructura articulada |
| Visual Studio Code | Desarrollo y edición de código |
| Iriun Webcam | Utilización de un teléfono móvil como cámara |
| Git | Control de versiones |
| Git LFS | Administración de archivos de gran tamaño |
| GitHub | Publicación del repositorio |
| YouTube | Publicación del video demostrativo |

---

## 3. Requisitos del sistema

Para abrir y ejecutar el proyecto se recomienda contar con:

- Un computador compatible con Unity 6.
- Unity Hub.
- Unity Editor 6000.6.2f1.
- Git.
- Git Large File Storage (Git LFS).
- Una cámara web o teléfono móvil utilizado como cámara.
- Una licencia válida de Vuforia Engine.
- El marcador visual utilizado por el proyecto.

Para reproducir la configuración empleada durante las pruebas, también se requiere:

- Un teléfono móvil con cámara.
- Iriun Webcam instalado en el computador.
- Iriun Webcam instalado en el teléfono.
- Una conexión funcional entre ambos dispositivos.

El proyecto fue evaluado mediante el editor de Unity utilizando el teléfono móvil como fuente de video. No se realizó una validación equivalente como aplicación móvil autónoma.

---

## 4. Instalación del proyecto

### 4.1. Clonar el repositorio

Abrir PowerShell, Git Bash o una terminal compatible.

Primero, verificar que Git y Git LFS estén instalados:

```powershell
git --version
git lfs version
```

Inicializar Git LFS:

```powershell
git lfs install
```

Clonar el repositorio:

```powershell
git clone https://github.com/Dev9Thiago/AR_Monitor.git
```

Acceder a la carpeta:

```powershell
cd AR_Monitor
```

Descargar los archivos administrados mediante Git LFS:

```powershell
git lfs pull
```

**Importante:** El proyecto contiene el paquete Vuforia Engine 11.4.4, cuyo tamaño supera el límite de 100 MB para archivos Git convencionales.

Por esta razón, el archivo:

`Packages/com.ptc.vuforia.engine-11.4.4.tgz`

se administra mediante Git LFS.

Si este archivo no se descarga correctamente, Unity puede indicar que el paquete no es un archivo tarball válido.

### 4.2. Abrir el proyecto en Unity

1. Ejecutar Unity Hub.
2. Seleccionar **Add project from disk**.
3. Buscar la carpeta `AR_Monitor`.
4. Agregar el proyecto.
5. Seleccionar Unity Editor 6000.6.2f1.
6. Abrir el proyecto.
7. Esperar a que Unity importe los recursos y resuelva las dependencias.

La primera ejecución puede requerir varios minutos debido a la importación de paquetes, modelos y recursos gráficos.

### 4.3. Abrir la escena principal

Dentro de Unity, acceder a:

`Assets/Scenes/SampleScene.unity`

Abrir la escena y comprobar que se encuentran presentes los componentes principales del sistema.

---

## 5. Estructura del proyecto

La organización general del proyecto corresponde a la estructura estándar de Unity.

```text
AR_Monitor/
|
|-- Assets/
|   |-- Editor/
|   |-- Resources/
|   |-- Robot/
|   |-- Scenes/
|   |-- Scripts/
|   |-- Settings/
|   |-- StreamingAssets/
|   |-- TutorialInfo/
|
|-- Packages/
|   |-- manifest.json
|   |-- packages-lock.json
|   |-- com.ptc.vuforia.engine-11.4.4.tgz
|
|-- ProjectSettings/
|
|-- .gitattributes
|-- .gitignore
|-- README.md
```

### Directorios principales

**Assets/**

Contiene los recursos utilizados por la aplicación, incluyendo escenas, scripts, modelos tridimensionales y archivos de configuración.

**Assets/Robot/**

Contiene los recursos relacionados con el manipulador robótico importado al proyecto.

**Assets/Scenes/**

Contiene las escenas de Unity utilizadas para ejecutar la aplicación.

**Assets/Scripts/**

Contiene los componentes de programación encargados del control robótico, telemetría e interacción.

**Packages/**

Contiene la información de dependencias y el paquete de Vuforia Engine.

**ProjectSettings/**

Contiene la configuración general del proyecto de Unity.

---

## 6. Configuración de Vuforia Engine

El reconocimiento visual se implementó mediante Vuforia Engine 11.4.4.

### 6.1. Base de datos del marcador

La base de datos utilizada se denomina:

`Robot6R_Targets`

El objetivo de imagen configurado se identifica como:

`marker_umng_robot`

El marcador corresponde a una imagen institucional de la Universidad Militar Nueva Granada.

### 6.2. Configuración de la cámara

En la jerarquía de Unity, seleccionar:

`ARCamera`

Verificar que la configuración de Vuforia se encuentre habilitada y que exista una licencia válida.

La licencia debe configurarse desde las opciones correspondientes de Vuforia.

**Seguridad:** No se recomienda publicar claves de licencia privadas dentro de repositorios públicos.

### 6.3. Configuración del ImageTarget

Seleccionar el objeto:

`ImageTarget`

Comprobar que los parámetros de reconocimiento correspondan a:

| Parámetro | Configuración |
|---|---|
| Base de datos | Robot6R_Targets |
| Objetivo | marker_umng_robot |
| Tipo de seguimiento | Image Target |

### 6.4. Seguimiento espacial

El sistema incorpora un administrador de seguimiento encargado de relacionar la posición del marcador con la base del manipulador virtual.

El componente `AR_Tracking_Manager` utiliza parámetros de suavizado y tolerancia para actualizar la posición y orientación del robot.

Esto permite reducir pequeñas variaciones visuales durante el seguimiento y conservar una referencia espacial para el modelo.

---

## 7. Configuración de Iriun Webcam

Durante el desarrollo y las pruebas se utilizó un teléfono móvil conectado al computador mediante Iriun Webcam.

### 7.1. Instalación

1. Instalar Iriun Webcam en el computador.
2. Instalar la aplicación Iriun Webcam en el teléfono móvil.
3. Abrir ambas aplicaciones.
4. Establecer la conexión entre los dispositivos.
5. Verificar que el computador recibe correctamente la imagen de la cámara.

### 7.2. Ejecución en Unity

1. Iniciar Iriun Webcam.
2. Verificar que la cámara esté disponible.
3. Abrir el proyecto en Unity.
4. Abrir `SampleScene`.
5. Presionar **Play**.
6. Colocar el marcador dentro del campo de visión.
7. Esperar a que Vuforia reconozca la imagen.
8. Comprobar que el manipulador aparece sobre la referencia visual.

### 7.3. Consideraciones

Durante las pruebas se observó que, cuando el marcador ya se encontraba visible al iniciar la aplicación, en ocasiones era necesario retirarlo temporalmente del campo de visión o acercarlo a la cámara.

Una vez realizado este procedimiento, el reconocimiento se producía prácticamente de manera inmediata.

---

## 8. Modelo robótico

El sistema utiliza un manipulador robótico de seis grados de libertad con mecanismo de agarre.

El modelo fue desarrollado en SOLIDWORKS e importado a Unity mediante una descripción URDF.

La estructura articulada permite representar el movimiento relativo de los eslabones y el funcionamiento del mecanismo de agarre.

### 8.1. Componentes principales

El modelo contiene:

- Base del manipulador.
- Seis articulaciones rotacionales.
- Eslabones asociados a las articulaciones.
- Mecanismo de agarre.
- Elementos mecánicos de transmisión del movimiento.

### 8.2. Simulación articular

Las articulaciones utilizan componentes `ArticulationBody` de Unity.

Estos componentes permiten establecer posiciones objetivo y representar el movimiento del robot dentro del entorno tridimensional.

El controlador administra los valores objetivo, los límites articulares y las velocidades máximas configuradas para cada articulación.

---

## 9. Sistema de control del robot

El control del manipulador se implementó mediante componentes desarrollados en C#.

El controlador principal se identifica como:

`Robot6RARController`

### 9.1. Control manual

El sistema dispone de deslizadores que permiten modificar las posiciones objetivo de las seis articulaciones.

Cada deslizador está asociado a una articulación y permite establecer su posición dentro de los límites configurados.

Los comandos se aplican progresivamente sobre el modelo tridimensional.

### 9.2. Control de la pinza

El mecanismo de agarre puede abrirse y cerrarse mediante el control correspondiente.

La simulación reproduce el movimiento coordinado de sus componentes y engranajes.

### 9.3. Función HOME

La función HOME permite regresar las articulaciones a sus posiciones iniciales configuradas.

Esta operación facilita la recuperación de una configuración conocida del manipulador.

### 9.4. Modo AUTO

El sistema incorpora un modo de movimiento automático administrado mediante:

`RobotAutoMovement`

Al activar AUTO:

1. Se generan posiciones objetivo para las articulaciones.
2. Los valores se mantienen dentro de sus límites configurados.
3. El manipulador se desplaza hacia la configuración objetivo.
4. Al completar el movimiento, se genera una nueva configuración después de una pausa.
5. El procedimiento se repite mientras el modo automático permanece activo.

### 9.5. Función STOP

La función STOP interrumpe la generación de movimientos automáticos y detiene la secuencia en ejecución.

Durante las pruebas se verificó que los controles AUTO y STOP respondían correctamente.

---

## 10. Sistema de telemetría

La aplicación incorpora un sistema de monitoreo de variables técnicas.

El componente principal de telemetría se identifica como:

`RobotTelemetry`

El sistema permite consultar el comportamiento de las articulaciones durante la simulación.

### 10.1. Variables monitoreadas

| Variable | Unidad | Origen |
|---|---|---|
| Posición angular | ° | Estado del modelo articulado |
| Velocidad angular | °/s | Información cinemática |
| Temperatura | °C | Simulación |
| Par | N·m | Simulación |

La posición y velocidad angular se relacionan con el movimiento del manipulador dentro de Unity.

Las variables de temperatura y par son generadas mediante software para representar condiciones dinámicas de funcionamiento.

**Importante:** La temperatura y el par no corresponden a mediciones realizadas sobre motores físicos.

### 10.2. Actualización de datos

Los valores se actualizan durante el movimiento del manipulador.

Durante las pruebas se observó una actualización continua de los indicadores, sin retrasos perceptibles.

### 10.3. Panel general

El panel general permite consultar información del conjunto de articulaciones del manipulador.

### 10.4. Paneles individuales

Los paneles contextuales permiten visualizar información técnica correspondiente a articulaciones específicas.

Estos paneles se despliegan mediante el sistema de selección por mirada.

---

## 11. Interfaces gráficas espaciales

Las interfaces fueron desarrolladas mediante el sistema Canvas de Unity.

Para su integración en el entorno aumentado, se configuraron en modo World Space.

Esto permite posicionar los paneles dentro del espacio tridimensional de la escena.

### 11.1. Menú principal

El menú principal permite acceder a las funciones generales de la aplicación.

Entre sus opciones se encuentran:

- Telemetry.
- Robot Controls.

La navegación permite acceder a los paneles correspondientes y regresar al menú principal.

### 11.2. Panel de telemetría

Presenta las variables técnicas monitoreadas durante el funcionamiento del robot.

### 11.3. Panel de control

Permite modificar las posiciones articulares y ejecutar las funciones disponibles del manipulador.

### 11.4. Paneles contextuales

Los paneles individuales se asocian a componentes específicos del robot.

Durante las pruebas se identificaron algunas intersecciones visuales entre estos paneles y la geometría de los eslabones en determinadas configuraciones.

Estas intersecciones no impidieron el funcionamiento general de la aplicación.

---

## 12. Interacción mediante mirada

El sistema de interacción se implementó mediante el componente:

`RobotGazePointer`

Este componente utiliza la orientación de la cámara para proyectar un rayo virtual e identificar elementos interactivos.

### 12.1. Selección de articulaciones

El usuario debe orientar la cámara hacia el componente que desea seleccionar.

Cuando el puntero permanece sobre el elemento durante el tiempo configurado, se activa la acción correspondiente.

### 12.2. Tiempo de permanencia

En la versión final se estableció un tiempo de permanencia de:

**2 segundos**

Este intervalo se utiliza para activar los elementos interactivos del sistema.

Durante las pruebas no se observaron activaciones accidentales.

### 12.3. Navegación entre paneles

La selección de la base del manipulador permite acceder al menú principal.

Desde este menú pueden abrirse las interfaces generales de telemetría y control.

### 12.4. Manipulación de deslizadores

Los deslizadores pueden modificarse mediante movimientos horizontales de la cámara.

Una vez seleccionado el control, las variaciones de orientación permiten cambiar su valor.

La sensibilidad depende del parámetro:

`Slider Yaw Range Degrees`

Un valor menor produce una mayor sensibilidad ante pequeños movimientos de la cámara.

### 12.5. Limitaciones de interacción

Durante las pruebas se observó que los movimientos involuntarios de la cámara podían afectar la precisión de los ajustes finos.

No obstante, el sistema permitió realizar selecciones y ejecutar comandos correctamente.

---

## 13. Resultados experimentales

La evaluación funcional se realizó utilizando Unity y un teléfono móvil conectado mediante Iriun Webcam.

Se efectuaron pruebas de reconocimiento visual, estabilidad espacial, legibilidad, interacción y funcionamiento robótico.

### 13.1. Reconocimiento del marcador

| Parámetro | Resultado |
|---|---|
| Distancia mínima aproximada | 5–10 cm |
| Distancia máxima aproximada | 1 m |
| Reconocimiento inicial | Prácticamente inmediato, con una excepción al iniciar con el marcador visible |
| Rango angular | Amplio, condicionado por la visibilidad de las características del marcador |
| Oclusión aproximada | 70 % sin desaparición inmediata del modelo |
| Recuperación del seguimiento | Reposicionamiento correcto al recuperar el marcador |
| Estabilidad | Adecuada con el marcador estacionario |

Durante la oclusión, el modelo podía conservar su última referencia espacial y presentar movimientos estimados.

La permanencia del robot en pantalla no debe interpretarse como reconocimiento visual continuo del marcador.

### 13.2. Legibilidad

La distancia preferida de lectura fue aproximadamente de 60 cm.

Se observó un margen de lectura estimado de 30 a 40 cm hacia posiciones más cercanas o alejadas.

Los paneles conservaron su orientación hacia la cámara, permitiendo consultar la información desde diferentes perspectivas.

### 13.3. Interacción

| Parámetro | Resultado |
|---|---|
| Tiempo de selección | 2 s |
| Selección de articulaciones | Correcta |
| Navegación entre interfaces | Correcta |
| Activaciones accidentales | No observadas |
| Manipulación de deslizadores | Funcional |
| Limitación principal | Movimientos involuntarios de la cámara |

### 13.4. Movimiento robótico

Durante las pruebas se verificó:

- Funcionamiento de las seis articulaciones.
- Apertura y cierre correctos de la pinza.
- Movimiento coordinado de los engranajes del mecanismo de agarre.
- Retorno a la posición inicial.
- Generación de nuevas configuraciones mediante AUTO.
- Detención del modo automático mediante STOP.

### 13.5. Telemetría

Las variables se actualizaron durante el movimiento del robot, tanto en el panel general como en los paneles individuales.

No se observaron retrasos perceptibles en la actualización visual de los indicadores.

### 13.6. Iluminación

El reconocimiento fue evaluado bajo iluminación interior y exterior, con diferentes niveles de intensidad.

El marcador pudo reconocerse mientras sus características visuales permanecieran suficientemente distinguibles.

### 13.7. Rendimiento

Durante las pruebas no se observaron interrupciones importantes, reducciones evidentes de fluidez ni retrasos perceptibles.

No se realizaron mediciones instrumentales de fotogramas por segundo o latencia computacional.

**Nota:** Los valores de distancia y oclusión corresponden a estimaciones experimentales aproximadas.

---

## 14. Video demostrativo

El funcionamiento del sistema de realidad aumentada se presenta en el siguiente video demostrativo:

**[Ver demostración del proyecto en YouTube](https://youtu.be/D18iNROxGto)**

Enlace directo:

https://youtu.be/D18iNROxGto

El video complementa la documentación técnica y permite consultar una demostración de la aplicación desarrollada.

---

## 15. Solución de problemas

### 15.1. Vuforia no se instala correctamente

Si Unity muestra el mensaje:

```text
Packages/com.ptc.vuforia.engine-11.4.4.tgz
is not a valid unity package tarball
```

es posible que el archivo local sea un puntero de Git LFS en lugar del paquete original.

Cerrar Unity y ejecutar desde la carpeta del proyecto:

```powershell
git lfs install
git lfs pull
git lfs checkout
```

Después, comprobar el tamaño del archivo:

```powershell
Get-Item ".\Packages\com.ptc.vuforia.engine-11.4.4.tgz" |
    Select-Object Name, Length
```

El archivo completo utilizado en el proyecto tiene un tamaño aproximado de 138 MB, equivalente a unos 132 MiB.

Si el archivo ocupa solamente unos pocos bytes, no se ha restaurado correctamente.

### 15.2. El robot no aparece

Comprobar:

1. Que la cámara funciona.
2. Que el marcador se encuentra visible.
3. Que Vuforia tiene una licencia válida.
4. Que la base de datos correcta está seleccionada.
5. Que el objetivo corresponde a `marker_umng_robot`.
6. Que el sistema de seguimiento está habilitado.

### 15.3. El marcador no se reconoce al iniciar

Retirar temporalmente el marcador del campo de visión y volver a mostrarlo.

También puede modificarse la distancia entre la cámara y el marcador para facilitar una nueva adquisición.

### 15.4. La cámara no funciona

Comprobar que Iriun Webcam está ejecutándose y que el computador recibe correctamente la imagen.

Verificar también que ninguna otra aplicación esté bloqueando el acceso a la cámara.

### 15.5. Los controles no responden

Comprobar que:

- El puntero de mirada está habilitado.
- La cámara apunta hacia el elemento interactivo.
- Se completa el tiempo de permanencia de 2 segundos.
- Los componentes de interfaz se encuentran activos.

### 15.6. Los deslizadores son demasiado sensibles

Modificar el parámetro:

`Slider Yaw Range Degrees`

Un valor mayor reduce la sensibilidad del desplazamiento ante pequeñas variaciones de orientación.

### 15.7. Algunos paneles atraviesan el modelo

Determinadas configuraciones articulares pueden producir intersecciones visuales entre los paneles contextuales y los eslabones.

Este comportamiento corresponde a una limitación conocida de la distribución espacial de las interfaces.

### 15.8. Unity presenta errores al abrir el proyecto

Verificar:

- La versión del editor.
- La instalación de Vuforia.
- La descarga de los archivos mediante Git LFS.
- La integridad de los archivos de `Packages`.
- La correcta importación de los recursos.

No eliminar las carpetas `Assets`, `Packages` o `ProjectSettings`, ya que contienen los archivos esenciales del proyecto.

---

## 16. Limitaciones y consideraciones

El sistema fue desarrollado como una herramienta de simulación y monitoreo mediante realidad aumentada.

Por lo tanto, deben considerarse los siguientes aspectos:

1. El manipulador representado es virtual.
2. Las variables de temperatura y par son simuladas.
3. El control mediante mirada puede verse afectado por movimientos involuntarios de la cámara.
4. Algunos paneles contextuales pueden presentar intersecciones visuales con el modelo.
5. El reconocimiento depende de las características visibles del marcador.
6. Las pruebas se realizaron en el editor de Unity mediante Iriun Webcam.
7. Los resultados experimentales corresponden a las condiciones de evaluación descritas.

---

## 17. Repositorio y documentación

El código fuente y los recursos del proyecto se encuentran disponibles en:

**GitHub:**

https://github.com/Dev9Thiago/AR_Monitor

**Video demostrativo:**

https://youtu.be/D18iNROxGto

Para descargar una copia completa del proyecto, se recomienda utilizar Git con Git LFS habilitado, en lugar de descargar únicamente el archivo ZIP generado por GitHub.

---

## 18. Finalidad académica

Este proyecto fue desarrollado con fines académicos en la Universidad Militar Nueva Granada.

Su objetivo es demostrar la integración de herramientas de realidad aumentada, simulación robótica, monitoreo de variables técnicas e interfaces espaciales aplicadas a sistemas mecatrónicos.

La aplicación constituye una herramienta de visualización y experimentación con un manipulador virtual y no un sistema de control de un robot físico.
