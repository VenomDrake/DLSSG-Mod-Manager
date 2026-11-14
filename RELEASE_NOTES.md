DLSSG Mod Manager 0.4.4 — support for the latest reviewed mod packages.

Fixes the installation block shown when manager 0.4.3 downloaded newer official packages. The blocked package checks did not establish that the selected game was unsupported.

- Adds reviewed SM86 0.4.2 support with bundled runtime 310.9.16 / 310.9.16.0; SM86 0.4.1 remains recognized. The optional upstream HTML configurator is verified as package data and is not installed or executed by the manager.
- Adds reviewed ANR FG-V.B17 Full and Addon support, checking the reviewed code revision, model identifier, exact program set, archive digest and provider digest. B15 remains recognized; unreviewed B16 and future recipes do not gain support automatically.
- Recognizes the reviewed B17 OptiScaler loader while preserving B15 compatibility. Installation, settings, disable, provider switching, backup and restore retain their safeguards.
- Completes the upstream Visual C++ x64 prerequisite check by including MSVCP140_ATOMIC_WAIT.dll alongside MSVCP140.dll, VCRUNTIME140.dll and VCRUNTIME140_1.dll. This dependency was already required by the upstream installer.
- Replaces the generic package-block explanation with a specific message for downloaded packages whose installation requirements are not yet reviewed. Download integrity, installation readiness and game compatibility are distinguished; anti-cheat and unsupported-game reasons remain visible.
- Updates the interface and guides in eleven languages. For SM86 0.4.2, XeSS UseHudless=Auto has the same whole-image interpolation behavior as 0, including UI.

**Updating:** manager 0.4.3 can receive this stable release through Update and restart. Close and reopen the manager if the notice has not appeared yet. After updating, reanalyse the game with the selected provider. A manager update does not install or update any game mod automatically. Installations still on 0.4.2 or earlier may require one manual MSI/complete portable update because the repository was renamed.

The manager still targets Windows 10/11 x64. SM86 supports the reviewed NVIDIA RTX 20/30 path; ANR supports the declared NVIDIA GTX 16 / RTX 20–50 and Radeon RX 5000/6000/7000/9000 families. Preliminary checks and fixture tests do not certify gameplay or a performance gain. ANR itself remains an upstream beta.

**Online games: risk of a ban.** Use injected mods only in offline single-player games. No anti-cheat bypass or protection against bans is provided.

Credits: sdli1995 and contributors (SM86), Coldwood1026 (SM75), Apex Compute (ANR), and OptiScaler contributors. Upstream DLLs and models retain their authors' terms and are downloaded at runtime, not bundled with this manager. The manager EXE and MSI remain unsigned.
