# Version Management

Version is the single source of truth from `Unity-Package/Packages/com.ivanmurzak.unity.mcp.animation/package.json` (`"version"` field). `bump-version.ps1` propagates it to:
- `Unity-Package/Packages/com.ivanmurzak.unity.mcp.animation/package.json`
- `Installer/Assets/AI Animation Installer/Installer.cs` (constant `Version`)
- Download URLs in both `README.md` and `Unity-Package/Packages/com.ivanmurzak.unity.mcp.animation/README.md`
