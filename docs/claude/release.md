# Version Management

Version is the single source of truth from `Unity-Package/Assets/root/package.json` (`"version"` field). `bump-version.ps1` propagates it to:
- `Unity-Package/Assets/root/package.json`
- `Installer/Assets/AI Animation Installer/Installer.cs` (constant `Version`)
- Download URLs in both `README.md` and `Unity-Package/Assets/root/README.md`
