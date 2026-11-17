DLSSG Mod Manager 0.5.0-opti-preview.1 — standalone OptiScaler integration (local preview).

Choose OptiScaler alongside DLSSG for SM86 and Apex Neural Rendering. This independent manager downloads the official stable OptiScaler release; it is not an official OptiScaler application.

- Reviews stable OptiScaler v0.9.5 using its exact archive digest and 23-file manifest. The 7z reader is included in the manager; no separate extractor is needed. Setup scripts are never executed or installed.
- Checks temporal-upscaler input files, the rendering executable, graphics API, Visual C++ prerequisites, selected backend requirements, conflicts, anti-cheat indicators, running games and disk space. Missing evidence requires review instead of a claim that the game is unsupported.
- Exposes upscalers per API, FG input/output, DXGI spoofing, logging and seven loader names. FG starts disabled. The upstream Insert overlay provides advanced settings and output verification.
- Uses verified backups and the existing transaction journal for install, update, settings, disable, provider switching and restore. Only reviewed individual payload files are owned; unrelated plugins and custom files are preserved. Unmanaged conflicting installations require review.
- Checks the official repository for updates. A verified future download remains blocked until its installation recipe is reviewed. Existing games are changed only after the user chooses to apply the package.
- Adds eleven-language interface and guides. Log diagnostics never treat a loaded DLL or an initialized swapchain as proof of successful upscaling or frame generation.

This local preview has automated fixture tests and read-only validation against the official package. OptiScaler has not yet been tested in gameplay through this integration. The existing stable public manager remains 0.4.5.

Stable OptiScaler 0.9.5 and the custom OptiScaler loader bundled by ANR are different recipes. Managed switches replace them through backup and restore; an external ANR addon installation must be restored before switching to standalone OptiScaler.

**Online games: risk of a ban.** Use injected mods only in offline single-player games. No anti-cheat bypass or protection against bans is provided.

Credits: sdli1995 and contributors (SM86), Coldwood1026 (SM75), Apex Compute (ANR), and OptiScaler contributors. Upstream DLLs and models retain their authors' terms and are downloaded at runtime, not bundled with this manager. The manager EXE and MSI remain unsigned.
