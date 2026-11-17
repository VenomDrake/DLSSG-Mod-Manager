# DLSSG Mod Manager — complete user tutorial

**Language:** **English** · [Italiano](i18n/USER_GUIDE.it.md) · [简体中文](i18n/USER_GUIDE.zh-Hans.md) · [हिन्दी](i18n/USER_GUIDE.hi.md) · [Español](i18n/USER_GUIDE.es.md) · [Français](i18n/USER_GUIDE.fr.md) · [العربية](i18n/USER_GUIDE.ar.md) · [বাংলা](i18n/USER_GUIDE.bn.md) · [Português](i18n/USER_GUIDE.pt.md) · [Русский](i18n/USER_GUIDE.ru.md) · [اردو](i18n/USER_GUIDE.ur.md)

For **manager 0.5.2** on **Windows 10/11 x64**. The names below refer to the English interface; choose your language at the top right of the app. This is an independent manager for the original projects, not an official NVIDIA, AMD, Intel, ANR or OptiScaler application.

**Contents:** [Quick start](#start) · [Safety](#safety) · [Download](#install) · [Choose a technology](#choose-provider) · [Screen and buttons](#screen) · [First installation](#first-install) · [Checks](#checks) · [SM86 settings](#sm86) · [ANR settings](#anr) · [OptiScaler settings](#optiscaler) · [In-game settings](#in-game) · [Verification](#verify) · [Updates](#updates) · [Disable and restore](#backup) · [Troubleshooting](#troubleshooting) · [Files and support](#files) · [Glossary](#glossary) · [Sources](#sources)

<a id="start"></a>
## 1. Quick start: your first test

Inside the app, open **Tutorial** and choose **Start guided tour** for eight practical steps. Search the topics, use **?** beside a setting or press **F1** while its control has focus. **Show in manager** opens and highlights the relevant controls without applying changes.

1. Close the game. Choose an **offline single-player game**.
2. Open the manager and let the selected technology's package check finish.
3. Choose the technology, then select the game from Steam or choose its folder/EXE. Let the manager analyse it.
4. Read **2 · Review the result**. Open **Diagnostics** for the full list. Resolve every **Blocked** check; read warnings before proceeding.
5. In **Mod settings**, keep the first-test defaults: **SM86: DLSS, 2×**; **ANR: Auto, 2×**; **OptiScaler: automatic upscaling, FG input Off and output Off**.
6. Return to **Game management**. Choose **Install mod**, check the target and settings in the confirmation, then confirm. Wait for completion.
7. Launch the game and follow the [provider-specific in-game steps](#in-game). Play briefly, checking motion, the HUD and responsiveness.
8. Read the log in **Diagnostics**. For OptiScaler, also press **Insert** in the game to inspect the active output. Loading a DLL alone is not proof of successful frame generation.

Change **one setting at a time** after a working first test. Close the game, apply the change in the manager and restart the game before comparing.

<a id="safety"></a>
## 2. Read this before modifying a game

**Use these mods only in offline single-player games. Modified game files can trigger anti-cheat or bans.** The manager provides no anti-cheat bypass or ban protection. An offline setting does not guarantee that anti-cheat is absent; a detected anti-cheat still blocks installation.

The manager checks files and prerequisites. **“Requirements detected” does not certify gameplay compatibility, stability or an FPS gain.** Keep ordinary backups of important saves; the manager's backups cover its game-file changes, not all saves or the entire game.

Do not stack loaders manually, copy random DLLs over game files or delete a conflict just to make a check pass. Review the existing mod first. Upstream binaries are downloaded from the original repositories as data; the manager does not run their setup scripts.

<a id="install"></a>
## 3. Download and open the manager

Use the [official manager releases](https://github.com/VenomDrake/DLSSG-Mod-Manager/releases/latest).

| Download | What to do |
|---|---|
| **Portable ZIP** | Extract the **whole ZIP** into a writable folder, then open `DLSSGModManager.exe`. Do not run it from inside the ZIP. Keep its licence and documentation files together. |
| **Windows installer (.msi)** | Close the manager and run the MSI. It installs for your Windows user and creates shortcuts. |

Published packages include the manager's .NET runtime. You do not need to install Python, CUDA Toolkit or a separate archive extractor. A provider can still require graphics drivers, Visual C++ or Vulkan components; those are separate checks.

The manager and MSI are unsigned. Only use the project releases you intended to download; do not disable Windows security globally. First launch needs internet to obtain a provider package. Later, a verified cached package may be usable offline, but **offline cache is not confirmation that it is the latest release**.

<a id="choose-provider"></a>
## 4. Which technology should I choose?

Choose under **Upscaling and frame generation**. A technology is also called a **provider** in this guide.

| Choice | Purpose and first-test requirements |
|---|---|
| **DLSSG for SM86 · NVIDIA / XeSS** | The manager's reviewed path for NVIDIA **RTX 20/30**, driver **R580 or newer**, configured hardware-accelerated GPU scheduling (HAGS), and a game's supported native DLSS Frame Generation runtime. The game uses DirectX 12. Start with DLSS and 2×. |
| **Apex Neural Rendering · Beta** | An alternative FG provider for the manager's declared **GTX 16 / RTX 20–50** and **Radeon RX 5000/6000/7000/9000** families. It needs native x64 DLSS Frame Generation in the game through DX12/Vulkan, x64 Visual C++ and a Vulkan loader. GTX 10/Pascal and Intel Arc are outside this manager's ANR support. Start with Auto and 2×. ANR itself remains beta. |
| **OptiScaler · Upscaling / FG** | Replaces a compatible game's temporal DLSS/FSR/XeSS upscaler and can manage supported FG paths. Native DLSS Frame Generation is **not required for upscaling alone**. NVIDIA, AMD and Intel are candidates, but each output has additional limits. Start with Auto upscaling and FG **Off/Off**. |

HAGS and NVIDIA R580 are **not universal requirements** inherited by ANR or OptiScaler. Native DLSS **output** in OptiScaler still requires NVIDIA RTX and the game's DLSS runtime. OptiScaler does not make every backend work on every GPU.

The recipes reviewed by manager 0.5.2 are **SM86 0.4.1/0.4.2**, **ANR FG-V.B15/B17**, and **OptiScaler 0.9.5**. A newer upstream download can remain blocked until its changed requirements are reviewed.

The manager remembers the selected provider; it does **not** automatically choose the best provider for your GPU. Select a suitable technology before interpreting a GPU failure.

<a id="screen"></a>
## 5. Understand the screen and every main button

The **Tutorial** tab contains this complete guide offline, with topic navigation and search. The guided tour stays beside the manager: **Back** and **Next** move between eight steps, **Read the full explanation** opens the matching topic, and **Close guided tour** ends the tour. Moving forward does not mark compatibility checks as successful; installation still requires the normal review and confirmation.

![Game management screen](screenshot.png)

| Area or control | What it does |
|---|---|
| **Manager version** | The app's version. It is separate from the selected mod package's version. |
| **Language** | Changes interface language. Arabic and Urdu use right-to-left layout. Configuration tokens such as `dxgi.dll` keep their original spelling. |
| **Technology selector** | Chooses SM86, ANR or OptiScaler. Changing it rechecks that provider's package and prerequisites; it does not immediately modify the game. |
| **Game management** | Choose the game, review the result, then apply the mod. |
| **Mod settings** | Edit the selected provider's settings. Editing a field alone does not write to the game. |
| **Diagnostics** | Full checks, package/PC details, logs, driver links, report export, disable and restore. |
| **Installed Steam games / ↻** | Selects a detected Steam game; ↻ refreshes libraries. An entry in Steam does not imply mod support. |
| **Choose folder / Choose EXE** | Selects a game manually, including games outside Steam. Choose the real rendering EXE, not a launcher, crash reporter or installer. |
| **Path box / Analyse game** | You can paste a folder/EXE path, then analyse it. Changing the path clears the previous result so it cannot be reused accidentally. |
| **Rendering executable** | Resolves multiple candidates. For many Unreal games, use `GameName\Binaries\Win64\...-Win64-Shipping.exe`, not the small launcher in the top folder. |
| **View all checks / Edit mod settings** | Opens the relevant tab. |
| **Install / Update / Apply settings / Switch technology** | One action area with a label matching the current situation. It applies changes only after confirmation and prerequisites. |
| **Launch game** | Starts the selected game, using Steam where matched. It does not install the mod. |
| **Cancel operation** | Requests a safe stop. Wait for the manager to finish a critical commit or rollback. |

In **Diagnostics**, the package's **↻** checks the upstream package, while the PC's **↻** reads hardware again. **Open Windows graphics settings** opens Windows settings; it does not enable HAGS for you. Vendor driver buttons open NVIDIA, AMD or Intel pages according to the selected provider/adapters. **Original project** opens that provider's repository.

The small **Update and restart** notice concerns the **manager itself**. Its **×** dismisses that version's notice for the current window; it does not uninstall anything.

<a id="first-install"></a>
## 6. Install, change settings or switch technology

1. Finish package download/verification and select the correct rendering EXE.
2. Review the summary and the complete checks. Close the game and any remaining game process.
3. Set first-test defaults or your intended options. The result can change when an option introduces a new requirement.
4. Press the bottom action button. Read the technology, rendering folder, version and settings in the confirmation.
5. Confirm. The manager stages verified files, creates a backup, deactivates old loaders, changes data and activates the selected loader last. Let it complete.
6. Restart the game. A game that was already open cannot reliably use newly written settings.

**“You already have the latest version” refers to the installed package identity.** It does not mean that a setting you just changed has already been applied. Use **Apply settings**, with the game closed, to save those changes.

For a managed installation, switching provider uses the same backup/history system. Standalone OptiScaler 0.9.5 and ANR's custom OptiScaler loader are different; the manager does not merge them. An external ANR Addon may require restoration of the external installation before switching. Unknown manual installations are protected rather than silently overwritten.

<a id="checks"></a>
## 7. Read the checks without guessing

| Status | Meaning and action |
|---|---|
| **Passed** | This particular check succeeded. It does not prove all other requirements or gameplay. |
| **Information** | Context, such as a setting or an optional component. Read it if relevant. |
| **Warning / To verify** | Something needs attention or in-game confirmation. It is not automatically blocking; it is also not a promise that proceeding is safe or compatible. |
| **Unknown** | The manager could not establish a fact. Read the detail instead of assuming it passed. |
| **Blocked** | Installation cannot continue until the cause is resolved. |

Typical checks cover: Windows/x64, GPU and driver, HAGS where required, Visual C++/Vulkan components, package identity and supported layout, scan completeness/permissions, correct EXE, native input/runtime, graphics API evidence, anti-cheat indicators, existing or modified loaders, a running game, read-only targets and free disk space. Backups and staging need space beyond the final mod size.

**Unsupported** identifies a detected technical mismatch. **Needs review** means evidence is incomplete, an EXE is ambiguous, or an input may be embedded. **Installation blocked** may mean a conflict or package problem rather than an unsupported game. Static file scans cannot prove the absence of anti-cheat or guarantee a working game.

<a id="sm86"></a>
## 8. SM86: every manager setting

![SM86 settings](settings-sm86.png)

**Recommended first test:** DLSS, 2×, optimization 1, preset A, SR override Off, logging 2, timing off, `version.dll`.

| Setting | Simple explanation and choices |
|---|---|
| **Frame generation engine** | **NVIDIA DLSS-G** or **Intel XeSS-FG** selects the mod's FG implementation. XeSS here is the SM86 mod's implementation, not a promise of general Intel-GPU support. Start with DLSS. |
| **Frame multiplier** | DLSS offers **2×–6×**; XeSS **2×–4×**. These request a multiplier: 2× means one generated frame per real frame; 3× means two, and so on. The game can impose a lower limit, and an unsupported request can prevent FG. Start at 2×. More generated frames do not mean proportionally lower latency or guaranteed smoother play. |
| **Kernel optimization** | **0:** original kernels, useful as a comparison. **1:** exact-output accelerations, recommended. **2:** faster approximate processing. **3:** maximum approximation. Higher levels can change the result; compare moving edges and UI instead of assuming the largest number is best. Applies to DLSS-G. |
| **DLSS-G preset** | **A:** normal first-test choice. **B:** UI recomposition when the game supplies suitable HUD-less/UI buffers; it cannot create those buffers. **C:** lighter FG with a possible image-quality tradeoff, especially around HUD/occlusion edges. Applies to DLSS-G. |
| **Super Resolution override** | **Off** keeps the game's upscaler. **M-INT8 / L-INT8** select the project's M/L INT8 kernels. **M-FP16 / L-FP16** select the corresponding FP16 processing. M/L are model choices, not a universal performance ranking. Overrides can cost performance in Quality, Balanced or DLAA modes; keep Off for the first test. |
| **XeSS optical flow** | **Coarse**, **Middle**, **Fine** change the flow-processing resolution. Finer processing can cost more GPU time. **CoarseAt4K** uses Coarse at output pixel counts of at least 3840×2160, otherwise Middle. Start with Middle. Applies to XeSS-FG. |
| **XeSS HUD handling** | **Auto**, **Require a HUD-less image (1)**, or **Do not use a HUD-less image (0)**. In reviewed SM86 **0.4.2, Auto behaves like 0**: the whole image, including UI, is interpolated. Mode 1 requires the game to provide a matching HUD-less image and can fail without it. |
| **Log detail** | **0 Off**, **1 Errors**, **2 Diagnostics (recommended)**, **3 Detailed traces**. More detail helps diagnosis but increases logging. |
| **Record frame timings** | Writes additional timing data for investigation. It does not set an FPS limit or enable FG. Leave off for the first test. |
| **Loader DLL** | `version.dll` (recommended), `winmm.dll`, `dbghelp.dll`, `dinput8.dll`, `dxgi.dll`, `d3d12.dll`. The filename controls how the game loads the mod. Keep one active loader; try alternatives through the manager if the mod does not load. Do not manually stack copies. |

DLSS-G and XeSS-FG have different settings. A visible engine-specific option does not make it active when the other engine is selected.

**HAGS for SM86:** open Windows graphics settings from Diagnostics, enable hardware-accelerated GPU scheduling where Windows offers it, then restart Windows. Registry/configured status and runtime-active status are different checks. The game log supplies the runtime confirmation. The manager does not change the driver or HAGS automatically.

<a id="anr"></a>
## 9. ANR: every manager setting

![ANR settings](settings-anr.png)

**Recommended first test:** Auto compute path, 2×, 80 nits per unit, scene-cut handling on, `dxgi.dll`. The engine is ANR; SM86's optimization/preset/SR override controls do not apply.

| Setting or mode | Meaning |
|---|---|
| **Full / Addon** | Selected automatically. **Full** includes the reviewed loader/provider setup. **Addon** adds ANR to a compatible external OptiScaler setup and preserves that loader. A managed standalone OptiScaler → ANR change uses Full. Do not choose or copy an arbitrary addon manually. |
| **Frame multiplier** | Requests **2×–6×**, subject to the game's integration. Start at 2×. The manager writes the corresponding number of extra frames, not six extra frames for a 6× choice. |
| **Compute path** | **auto**, **coopmat**, **packed**, **dp4a**, **portable**, **portable-linalg**, **portable-xmx** are alternative ways of performing ANR's computations. They are not image-quality levels. Auto uses the provider's selection logic; explicit paths require suitable GPU capabilities and are not all guaranteed to work or be faster. Leave Auto unless diagnosing a specific issue. |
| **HDR luminance** | **40–1000**, default **80**, describes the luminance associated with a linear value of 1.0 in the provider's HDR/PQ processing. It is not the monitor's brightness slider. Keep 80 unless an HDR-specific investigation requires a change. |
| **Scene-cut handling** | On by default; avoids interpolating across abrupt cuts. Keep on for normal testing. |
| **Loader DLL** | `dxgi.dll`, `winmm.dll`, `version.dll`; start with `dxgi.dll`. In Addon mode the external loader is kept and the selector is locked. |

ANR's **Beta** label describes the upstream provider, even when the manager is a stable release. ANR needs the reviewed Visual C++ x64 components and Vulkan loader even when the game's graphics API is DX12. HAGS is not imposed as the SM86 requirement.

<a id="optiscaler"></a>
## 10. OptiScaler: every manager setting

![OptiScaler settings](settings-optiscaler.png)

**Recommended first test:** all upscalers **Auto**, FG input **Off**, FG output **Off**, DXGI spoofing **Auto**, log **2 · Information**, loader `dxgi.dll`.

### Upscaler choices

An **input** is the technology selected in the game's own menu. An **output** is what OptiScaler actually runs. For example, a game can provide a DLSS input while OptiScaler outputs XeSS. Selecting DLSS as the game's input does not itself prove that DLSS is the active output.

| Manager field | Available values |
|---|---|
| **DirectX 11 upscaler** | Auto; FSR 2.2 (`fsr22`); FSR 3.1 (`fsr31`); native XeSS (`xess`); XeSS through D3D11on12 (`xess_12`); FSR 2.1/2.2/3.1 through DX12 interop (`fsr21_12`, `fsr22_12`, `fsr31_12`); DLSS. |
| **DirectX 12 upscaler** | Auto; XeSS; FSR 2.1; FSR 2.2; FSR 3.1/4; DLSS. |
| **Vulkan upscaler** | Auto; native FSR 2.1/2.2/3.1/4 as supported by the backend; native XeSS; FSR 2.1 or FSR 3.1/4 through DX12 interop (`fsr21_12`, `fsr31_12`); DLSS. |

Only the game's active API uses its corresponding setting. **Auto** follows OptiScaler's defaults; it is the safest starting point, not a promise of the fastest output. `_12` means using a DX12 interoperation path, which can have overhead.

Native **DX11 XeSS** requires Intel Arc; `xess_12` is a different interop choice. **DLSS output** requires NVIDIA RTX and the game's x64 DLSS runtime. **FSR 3.1/4** does not promise FSR 4 on every GPU: backend and hardware support determine the active model or fallback. Verify the actual output in the overlay; the manager does not force unsupported FSR 4.

For DX11, the native `fsr31` path is FSR 3.1; the FSR 4 route uses DX12 interop. A general “3.1 / 4” label does not guarantee native DX11 FSR 4.

### Frame generation input and output

For an upscaling-only first test, keep **both fields Off**. To enable FG later, choose a compatible input **and** output; changing only one to an active mode is blocked.

| FG input | What the game must provide |
|---|---|
| **Off** (`nofg`) | No OptiScaler FG requested. Set output Off too. |
| **DLSS Frame Generation** (`dlssg`) | Native DLSS FG with a suitable Streamline 2 integration; the manager checks available evidence and warns where it cannot confirm it. |
| **DLSS-G · Nukem** (`nukems`) | A native DLSS-G runtime. Pair this input with **Nukem · FSR 3** output. |
| **FSR Frame Generation** (`fsrfg`) | A compatible native FSR FG source in the game. A DLL filename alone cannot establish that integration. |
| **Upscaler input / OptiFG** (`upscaler`) | A working temporal upscaler input, using the reviewed DX12 OptiFG path. HUD correction may be needed in the upstream overlay. |
| **FSR 3.0 Frame Generation** (`fsrfg30`) | A compatible native FSR 3.0 FG source. |

| FG output | Meaning |
|---|---|
| **Off** | No FG output. Set input Off too. |
| **AMD FSR Frame Generation** (`fsrfg`) | Uses the compatible FidelityFX FG backend. It does not guarantee the same FSR version/features on every GPU. |
| **Intel XeSS Frame Generation** (`xefg`) | Requires the backend's GPU capabilities, including suitable DP4a support and game/display conditions. It is not enabled for every Intel, AMD or NVIDIA device just because its vendor is detected. Multi-frame support has further upstream limits. |
| **Nukem · FSR 3** (`nukems`) | Uses the reviewed bundled DLSSG-to-FSR3 path. Required when the selected input is Nukem. |

Manager 0.5.2 blocks selected FG on a detected **DX11** path. For a detected **Vulkan** path, its reviewed allowed FG pair is **Nukem → Nukem**. Other offered FG paths require DX12; unknown API evidence still needs in-game confirmation. These controls do not make an unsupported game integration compatible.

### Remaining controls and advanced settings

| Setting | Meaning |
|---|---|
| **DXGI GPU spoofing: Auto / On / Off** | Changes the GPU identity reported through DXGI for compatibility. It does not upgrade the real GPU or guarantee performance. Keep Auto initially. It is not anti-cheat protection; use mods offline only. |
| **Log detail** | **0 Trace**, **1 Debug**, **2 Information (recommended)**, **3 Warning**, **4 Error**. Unlike SM86, **0 does not mean logging off**. The manager enables file logging for diagnostics. |
| **Loader DLL** | `dxgi.dll`, `winmm.dll`, `version.dll`, `d3d12.dll`, `dbghelp.dll`, `wininet.dll`, `winhttp.dll`. Start with `dxgi.dll`; change through the manager if needed, with the game closed. |

Press **Insert** inside the game for OptiScaler's complete upstream overlay, advanced settings and active-backend information. Its many advanced controls are owned by OptiScaler; the manager exposes the reviewed common choices above. Keep a working baseline, change one thing at a time and use the [official manual](https://github.com/optiscaler/OptiScaler/wiki) for controls outside this manager. Stable 0.9.5 and v10 nightly instructions are not interchangeable.

Applying settings from the manager writes its selected values again. If you changed the same options in the overlay, keep the manager's choices consistent before applying; unrelated INI settings are preserved where supported.

<a id="in-game"></a>
## 11. What to select inside the game

**SM86 or ANR:** enable the game's **NVIDIA DLSS Frame Generation** option after installing. Start at 2× where the game offers it. Restart the game if its menu requires it. Upscaling quality, such as Quality or Balanced, is a separate choice from FG.

**OptiScaler first test:** enable a compatible temporal upscaler in the game, keep the manager's FG input/output Off and inspect the active output with Insert. You do not have to enable DLSS FG merely to test upscaling. If you later choose a native FG input, enable that corresponding game feature. For the Upscaler/OptiFG input, use the reviewed DX12 path and upstream overlay guidance.

A grey **2×** setting can simply mean that only that multiplier is available. It does **not** prove that FG is running. The manager cannot unlock every multiplier in every game's menu. Likewise, a game-menu label may describe the input while OptiScaler is using a different output.

For comparison, use the same scene, resolution and settings. Look at moving text, fences, character edges and HUD elements as well as FPS and input response. Generated/displayed FPS and truly rendered FPS are different. More displayed frames cannot be assumed to double responsiveness.

<a id="verify"></a>
## 12. Diagnostics: how to know what happened

In **Diagnostics**, choose **Read game log** after a short test. Launching through the manager and reading the log start periodic reading while the manager remains open. If the result is pending, close the game and read the completed log again.

| Provider | What the manager can establish |
|---|---|
| **SM86** | Looks at the matching recorded session for successful FG feature creation and its first evaluation. Merely loading the mod or seeing an SR feature is insufficient. Read any errors and runtime HAGS/GPU information. |
| **ANR** | Distinguishes loading and initialization from actual generated evaluations. A complete session with positive generated-evaluation counts and no generation errors is required for FG confirmation. Counts and recorded multiplier help diagnosis. |
| **OptiScaler** | Reports loading and current-session log errors. A loaded DLL or created swapchain is **not proof of successful SR, FG or increased performance**. Verify the active backend, frame statistics and visual result in the game's overlay. |

A partial, unfinished or oversized log cannot establish a full successful session. **No errors** means no errors were found in the portion understood by the reader, not that performance or FG has been certified. Check the displayed session time so you do not confuse an older test with the current one. In a multi-GPU PC, confirm which adapter the game actually uses.

<a id="updates"></a>
## 13. Three different things can be updated

| Update | How it works |
|---|---|
| **Manager application** | Checks stable GitHub releases at launch and periodically while open (about every six hours). A small notice appears only for a newer stable version. Choose **Update and restart**; it downloads and verifies the correct portable/MSI package, then updates and restarts with your consent. Reopen the manager to check immediately. Prereleases are ignored by the normal stable check. |
| **Downloaded provider package** | Checked for the selected provider at startup or after selecting it; the package ↻ rechecks it. A new verified download updates the cache. It does **not** apply itself to your games. |
| **Mod in one game** | Select that game, analyse it and choose **Update mod** after the checks and confirmation. Other games keep their existing installation until you update them individually. |

The manager compares release ID, asset ID and SHA-256, so replacement assets with the same visible version can still be detected. **Downloaded**, **up to date**, **offline cache** and **not yet supported by this manager** are different results. A package may pass download integrity checks but still have an unknown installation recipe. Wait for a manager version that explicitly supports it; do not force it by copying its DLLs manually.

Manager **0.4.3 and later** use the renamed repository. Very old versions **0.4.2 or earlier** may need one manual update using the latest complete MSI/portable package. The 0.2.0/0.2.1 MSI updater's help-window/error-1639 issue also needs a one-time manual installation of a fixed newer MSI. Use the same installation type and close the manager first. Keep games and backup folders intact.

<a id="backup"></a>
## 14. Disable, re-enable, restore and change provider

**Disable mod** temporarily stops the managed mod from activating; it is not a complete uninstall. Normally the manager disables its loader and keeps runtime files and backups. For an ANR external Addon, it restores the external OptiScaler selection while keeping that external loader.

To **re-enable**, keep the game closed and use the main Install/Update action for the selected managed provider again.

**Restore previous state** restores the backup associated with the current managed installation/update/provider switch. It is not a general “undo the last button” command and is not a dedicated undo of Disable. Depending on history, it can return an older mod version, another provider, a disabled installation or the original unmodified state. Restoring the first installation's original backup can remove the active managed mod. Read the confirmation.

Earlier installation records can lead to earlier backups, so additional restores may move further back through installation history while a record remains. There is no general redo-history browser. Backups are retained; modified or unknown files can block restoration rather than being overwritten.

**Never manually delete** `.dlssg-manager.json`, `.dlssg-manager.transaction.json` or `.dlssg-manager-backups` to clear an error. They preserve ownership and recovery information. After an interrupted operation, keep the game closed, retain those files and reopen the manager. Recovery uses the journal during file operations. If it stays blocked, export a report and ask for help instead of manually reactivating or deleting loaders.

<a id="troubleshooting"></a>
## 15. Common problems and the next sensible action

| What you see | What to do |
|---|---|
| **Install is disabled** | Wait for the current operation, select the real EXE and inspect Blocked checks in Diagnostics. A read-only/UI-preview build cannot install. Resolve causes rather than copying files manually. |
| **GPU unsupported** | Check the selected provider first. The manager remembers/defaults to a provider; it does not automatically pick the best one for your GPU. ANR and OptiScaler have different requirements from SM86. |
| **Game unsupported / Needs review** | Read the precise reason. Missing native FG matters for SM86/ANR; OptiScaler can instead need evidence of a temporal SR input. Embedded FSR inputs may be inconclusive. Check the game's API and EXE. |
| **Latest package, but installation blocked** | Integrity/latest-version status is separate from a reviewed recipe, hardware or game compatibility. A newer unknown recipe requires a manager update/review. |
| **Steam list empty or game missing** | Refresh Steam libraries; otherwise choose the game's folder or actual rendering EXE manually. |
| **Multiple EXEs** | Prefer the actual renderer. For Unreal, inspect `Binaries\Win64` and the `Shipping.exe` candidate. Do not choose a launcher or crash reporter just to bypass ambiguity. |
| **Game still running** | Close it completely and wait for its process to exit before installing, changing settings, switching or restoring. |
| **HAGS configured, but runtime still unconfirmed** | For SM86, restart Windows after enabling HAGS, test the game and read the latest log. A registry setting alone is not proof that HAGS is active in that session. |
| **Visual C++ / Vulkan missing** | Use official Microsoft or GPU-vendor installers, not DLL download sites. ANR requires both reviewed x64 Visual C++ components and the Vulkan loader. Recheck hardware afterward. |
| **Unknown loader, manual mod or modified file** | Preserve it. Identify and restore/remove the original mod through its own supported procedure, or seek advice with a report. The manager deliberately protects foreign files; manual OptiScaler is not silently adopted. |
| **Log missing / loaded only / 2× grey** | Follow the correct in-game steps, test briefly and reread the completed log. A different loader may be needed if nothing loads. Grey 2× and loaded-only logs are not FG proof. |
| **Ghosting, flickering, stutter or worse FPS** | Return to the recommended baseline. Compare one change at a time, lower FG requests where appropriate and inspect errors. Try disabling/restoring through the manager. Do not assume the largest multiplier, finest flow or approximation level is best. |
| **Changed settings have no effect** | Close the game, use Apply settings, then restart it. Check the selected engine and active API: another engine/API's settings may be irrelevant. |
| **Operation interrupted or restore blocked** | Keep the game closed and preserve records/backups. Read the error, export a report and ask for help. Do not delete the recovery journal or force unknown files over the backup. |
| **Manager update not shown** | Reopen the manager, confirm internet access and check your installed version. Normal checks ignore prereleases. Very old installations may need the one-time manual update described above. |

<a id="files"></a>
## 16. Files, privacy and asking for help

Normal application preferences, caches and `application.log` are under `%LOCALAPPDATA%\DLSSGModManager`. A custom data directory or a dedicated UI-preview launch can use a different location. The selected game's ownership record, transaction journal and backups live beside its **rendering EXE**, not necessarily beside its launcher.

| Provider | Normal game-side log location, relative to the rendering folder |
|---|---|
| SM86 | `dlssg_sm86\logs\dlssg_sm86.log` |
| ANR | `OptiScaler\custom\Apex Neural Rendering FG\dlssg\nr_fg-*.log` |
| OptiScaler | `OptiScaler.log` |

Use **Export diagnostic report** in Diagnostics and choose where to save `DLSSG-diagnostic-report.json`. It includes manager/provider/package versions, package hash/update state, GPU/driver details, checks and available log evidence. It replaces the selected game-root path and user-profile prefix with placeholders such as `<GAME>` and `<USER>`; review the file before sharing because this does not guarantee removal of every possible identifying string.

Reports are saved locally, not sent automatically. There is no account or telemetry. When asking for help, include the provider, game, manager version, exact blocking message and what you changed. A screenshot of the result and the exported report are more useful than “it does not work.” Do not share tokens, passwords or unrelated personal files.

<a id="glossary"></a>
## 17. Small glossary

| Word | Plain meaning |
|---|---|
| **Rendered/base FPS** | Real frames calculated by the game. |
| **Frame generation / FG** | Extra estimated frames inserted between real ones. |
| **Upscaling / SR** | Reconstructing a higher-resolution image from a lower-resolution render. It is separate from FG. |
| **Input / output** | The game feature OptiScaler intercepts / the backend OptiScaler runs instead. |
| **API: DX11, DX12, Vulkan** | The graphics interface used by the game. Different APIs have different supported paths. |
| **HUD / HUD-less** | The on-screen interface / a game image without that interface. |
| **DLL / loader / proxy** | A code library and the named file through which the game loads a mod. Its filename matters. |
| **HAGS** | Windows hardware-accelerated GPU scheduling. |
| **Runtime** | A software component needed by a feature, such as the game's DLSS DLL or Visual C++. |
| **Cache / SHA-256** | A saved downloaded package / a fingerprint used to check file integrity. A matching fingerprint does not certify gameplay or make an unknown recipe supported. |
| **Backup / journal** | Saved prior file state / information used to recover an interrupted change. |

<a id="sources"></a>
## 18. Original projects and further reading

- [Manager downloads and project](https://github.com/VenomDrake/DLSSG-Mod-Manager).
- [DLSSG for SM86](https://github.com/sdli1995/dlssg_for_sm86), by sdli1995 and contributors; [SM75 adaptation](https://github.com/Coldwood1026), by Coldwood1026.
- [Apex Neural Rendering](https://github.com/apex-compute/ANR), by Apex Compute and contributors.
- [OptiScaler](https://github.com/optiscaler/OptiScaler) and its [official wiki](https://github.com/optiscaler/OptiScaler/wiki), by its contributors.
- [Manager overview](../README.md) and [third-party notices](../THIRD_PARTY_NOTICES.md).

This guide describes the **controls exposed by manager 0.5.2** and its reviewed provider recipes. It is not a guarantee of every game's compatibility, a benchmark or a complete manual for every upstream overlay option. For upstream-only advanced features, use the matching version's original documentation.
