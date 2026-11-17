DLSSG Mod Manager 0.5.1 — standalone OptiScaler integration (stable).

Choose OptiScaler alongside DLSSG for SM86 and Apex Neural Rendering. This independent manager downloads the official stable OptiScaler release; it is not an official OptiScaler application.

- Reviews stable OptiScaler v0.9.5 using its exact archive digest and 23-file manifest. The 7z reader is included in the manager; no separate extractor is needed. Setup scripts are never executed or installed.
- Checks temporal-upscaler input files, the rendering executable, graphics API, Visual C++ prerequisites, selected backend requirements, conflicts, anti-cheat indicators, running games and disk space. Missing evidence requires review instead of a claim that the game is unsupported.
- Exposes upscalers per API, FG input/output, DXGI spoofing, logging and seven loader names. FG starts disabled. The upstream Insert overlay provides advanced settings and output verification.
- Uses verified backups and the existing transaction journal for install, update, settings, disable, provider switching and restore. Only reviewed individual payload files are owned; unrelated plugins and custom files are preserved. Unmanaged conflicting installations require review.
- Checks the official repository for updates. A verified future download remains blocked until its installation recipe is reviewed. Existing games are changed only after the user chooses to apply the package.
- Adds eleven-language interface and guides. Log diagnostics never treat a loaded DLL or an initialized swapchain as proof of successful upscaling or frame generation.

This stable manager release includes the OptiScaler integration previously published in 0.5.0-opti-preview.1. Validation includes automated fixture tests, checks against the official package and installation/restore tests of the actual payload as data in a synthetic game directory. OptiScaler has not yet been tested in gameplay through this integration; backend and game compatibility must still be confirmed in the game.

**Updating:** manager 0.4.3 and later can detect this stable release through their normal update check and offer **Update and restart**. Restart the manager to check immediately. Versions 0.4.2 and earlier may require one manual MSI or complete portable update because the repository was renamed. Updating the manager does not change mods in any game automatically.

Version 0.5.1 is newer than the preview's Windows Installer version 0.5.0, so both stable and preview installations can upgrade. The 0.5.0-opti-preview.1 release remains a historical prerelease.

Download the complete portable ZIP or the per-user Windows installer from this release. Begin with automatic upscaling and frame generation disabled, then inspect OptiScaler's active backend through its Insert overlay in an offline single-player game.

Stable OptiScaler 0.9.5 and the custom OptiScaler loader bundled by ANR are different recipes. Managed switches replace them through backup and restore; an external ANR addon installation must be restored before switching to standalone OptiScaler.

**Online games: risk of a ban.** Use injected mods only in offline single-player games. No anti-cheat bypass or protection against bans is provided.

Credits: sdli1995 and contributors (SM86), Coldwood1026 (SM75), Apex Compute (ANR), and OptiScaler contributors. Upstream DLLs and models retain their authors' terms and are downloaded at runtime, not bundled with this manager. The manager EXE and MSI remain unsigned.
