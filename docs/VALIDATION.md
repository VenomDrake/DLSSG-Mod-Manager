# Validation

## OptiScaler local preview — 17 November 2026

- Official stable OptiScaler v0.9.5 is reviewed separately from the ANR custom loader and the upstream v10 nightly branch. The 70,418,897-byte official archive SHA-256 is `77e3ed76bed04e1fb32bacb5a72799ff172f3da22ed5231c0cf3e84fe6761662`; its 23-file inventory is checked byte for byte. Twenty payload files are managed; setup scripts and the extraction reminder are excluded.
- `OptiScalerReleaseTests` checks metadata identity, redirects, bounded paths and extraction, cache integrity, cancellation, replacement and recovery, and opaque unsupported future downloads. Its optional `--live` mode downloads and verifies the official archive as data; no upstream code is executed.
- `OptiScalerDetectionTests` checks SR-only eligibility, optional native FG, missing/embedded inputs, x64 prerequisites, alternate loader conflicts, anti-cheat indicators, backend/API requirements, changed native FG files, partial logs and conservative diagnostics.
- OptiScaler transaction fixtures check installation, no-op, changed settings, update/restore, all seven loaders, disable/re-enable, SM86/ANR full transitions, external ANR addon refusal, unowned files and plugins, tampering, cancellation, commit rollback, interrupted recovery and path escapes.
- The complete offline test script passed: existing SM86/ANR/transaction/updater tests, 61 OptiScaler package checks, 43 OptiScaler detection/options/log checks, 106 OptiScaler transaction assertions and 11 dictionaries with 289 matching non-empty keys. Desktop compilation completed with zero warnings and errors. The optional official-package run passed 65 package checks in total.
- A separate 33-check test installed the actual twenty reviewed payload files as data into an isolated synthetic game. Source hashes, scanner recognition, no-op/settings/disable/restore history and removal were checked; the inert rendering executable, native input fixture and foreign plugin marker remained byte-identical. No downloaded library was loaded or executed.
- No installed game is changed by these checks. Gameplay validation on real NVIDIA, AMD and Intel hardware remains outstanding. A vendor match is not a guarantee that every OptiScaler backend is supported. Loaded-module and initialization messages do not prove successful generated frames.

## Historical 0.1.0 validation

Validated on Windows x64 on 3 November 2026.

- **25 deterministic release tests:** initial download, exact identity comparison, same-version asset replacement, offline distinction, modified cache recovery, cancellation, archive traversal/case collisions, future recipe and runtime mismatch refusal.
- **30 detection/log tests:** synthetic x64 runtime with version resources, native runtime generation checks, executable selection, unknown proxy and anti-cheat blocking, corrupted inventory, cancellation, latest process/session isolation, malformed log fields and actual FG-vs-SR discrimination.
- **Installation transaction suite:** first installation, unchanged no-op, option changes, disable, update/restore chains, adoption of a recognized manual installation, original/extra files, backup integrity, cancellation, real commit failure, recovery journal, active-process blocking, malformed/null records and path traversal.
- **11 localization dictionaries:** identical 161 keys and non-empty values. English and Italian screens inspected. Arabic and Urdu use right-to-left layout; broad native-speaker review is still needed.
- **Live upstream check:** official 0.4.1 downloaded, archive and 28 manifest files verified; a second check returned “up to date.”
- **Live read-only machine/game check:** RTX 3090 SM 8.6, driver 621.01, integrated Intel adapter and 54 Steam games detected. The existing working manual 0.4.1 installation in Funnel Runners was recognized; its current recorded FG creation/evaluation was read successfully. No game files were changed by this validation.
- **Desktop build:** no compiler warnings or errors. Self-contained portable EXE opened successfully without a separate runtime install step.
- **MSI:** built as a per-user package. Administrative extraction completed successfully and the embedded EXE matched the portable EXE's SHA-256. A complete per-user install/uninstall on a clean Windows machine has not yet been exercised.

Synthetic tests contain no NVIDIA binaries and do not depend on a particular GPU or installed game. The `--live` release test is optional and requires network access. The screenshot is an actual read-only preview, not a claim of runtime compatibility for the displayed game.

This is an unsigned first public preview. Testing a broader range of games, clean Windows installations, GPUs and translations remains community validation work. Future upstream releases are not marked installable until the manager has an explicit compatible recipe.
