<h1 align="center"><a href="https://github.com/IvanMurzak/Unity-AI-Animation">Unity AI Animation</a></h1>

<div align="center" width="100%">

[![MCP](https://badge.mcpx.dev 'MCP Server')](https://modelcontextprotocol.io/introduction)
[![OpenUPM](https://img.shields.io/npm/v/com.ivanmurzak.unity.mcp.animation?label=OpenUPM&registry_uri=https://package.openupm.com&labelColor=333A41 'OpenUPM package')](https://openupm.com/packages/com.ivanmurzak.unity.mcp.animation/)
[![Unity Editor](https://img.shields.io/badge/Editor-X?style=flat&logo=unity&labelColor=333A41&color=49BC5C 'Unity Editor supported')](https://unity.com/releases/editor/archive)
[![r](https://github.com/IvanMurzak/Unity-AI-Animation/workflows/release/badge.svg 'Tests Passed')](https://github.com/IvanMurzak/Unity-AI-Animation/actions/workflows/release.yml)</br>
[![Discord](https://img.shields.io/badge/Discord-Join-7289da?logo=discord&logoColor=white&labelColor=333A41 'Join')](https://discord.gg/cfbdMZX99G)
[![Stars](https://img.shields.io/github/stars/IvanMurzak/Unity-AI-Animation 'Stars')](https://github.com/IvanMurzak/Unity-AI-Animation/stargazers)
[![License](https://img.shields.io/github/license/IvanMurzak/Unity-AI-Animation?label=License&labelColor=333A41)](https://github.com/IvanMurzak/Unity-AI-Animation/blob/main/LICENSE)
[![Stand With Ukraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://stand-with-ukraine.pp.ua)

</div>

<img width="100%" alt="Stats" src="https://github.com/IvanMurzak/Unity-AI-Animation/raw/main/docs/img/ai-animation-glitch.gif"/>

AI-powered tools for Unity animation workflow. Create and modify AnimationClips and AnimatorControllers directly through natural language commands. Automate repetitive animation tasks like setting up state machines, configuring transitions, and adding keyframes. Ideal for rapid prototyping, procedural animation generation, and streamlining complex animator setups. Built on top of the [AI Game Developer](https://github.com/IvanMurzak/Unity-MCP) platform.

[![DOWNLOAD INSTALLER](https://github.com/IvanMurzak/Unity-MCP/blob/main/docs/img/button/button_download.svg?raw=true)](https://github.com/IvanMurzak/Unity-AI-Animation/releases/download/1.0.0/AI-Animation-Installer.unitypackage)

### Stability status

| Unity Version | Editmode                                                                                                                                                                               | Playmode                                                                                                                                                                               | Standalone                                                                                                                                                                               |
| ------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| 2022.3.62f3   | [![r](https://github.com/IvanMurzak/Unity-AI-Animation/workflows/release/badge.svg?job=test-unity-2022-3-62f3-editmode)](https://github.com/IvanMurzak/Unity-AI-Animation/actions/workflows/release.yml) | [![r](https://github.com/IvanMurzak/Unity-AI-Animation/workflows/release/badge.svg?job=test-unity-2022-3-62f3-playmode)](https://github.com/IvanMurzak/Unity-AI-Animation/actions/workflows/release.yml) | [![r](https://github.com/IvanMurzak/Unity-AI-Animation/workflows/release/badge.svg?job=test-unity-2022-3-62f3-standalone)](https://github.com/IvanMurzak/Unity-AI-Animation/actions/workflows/release.yml) |
| 2023.2.22f1   | [![r](https://github.com/IvanMurzak/Unity-AI-Animation/workflows/release/badge.svg?job=test-unity-2023-2-22f1-editmode)](https://github.com/IvanMurzak/Unity-AI-Animation/actions/workflows/release.yml) | [![r](https://github.com/IvanMurzak/Unity-AI-Animation/workflows/release/badge.svg?job=test-unity-2023-2-22f1-playmode)](https://github.com/IvanMurzak/Unity-AI-Animation/actions/workflows/release.yml) | [![r](https://github.com/IvanMurzak/Unity-AI-Animation/workflows/release/badge.svg?job=test-unity-2023-2-22f1-standalone)](https://github.com/IvanMurzak/Unity-AI-Animation/actions/workflows/release.yml) |
| 6000.3.1f1    | [![r](https://github.com/IvanMurzak/Unity-AI-Animation/workflows/release/badge.svg?job=test-unity-6000-3-1f1-editmode)](https://github.com/IvanMurzak/Unity-AI-Animation/actions/workflows/release.yml)  | [![r](https://github.com/IvanMurzak/Unity-AI-Animation/workflows/release/badge.svg?job=test-unity-6000-3-1f1-playmode)](https://github.com/IvanMurzak/Unity-AI-Animation/actions/workflows/release.yml)  | [![r](https://github.com/IvanMurzak/Unity-AI-Animation/workflows/release/badge.svg?job=test-unity-6000-3-1f1-standalone)](https://github.com/IvanMurzak/Unity-AI-Animation/actions/workflows/release.yml)  |

## AI Animation Tools

AnimationClip tools:

- `animation-create` - Create AnimationClip asset files (.anim)
- `animation-get-data` - Get AnimationClip data (curves, events, frame rate, wrap mode)
- `animation-modify` - Modify AnimationClip (set/remove curves, add events, set properties)

AnimatorController tools:

- `animator-create` - Create AnimatorController asset files (.controller)
- `animator-get-data` - Get AnimatorController data (layers, states, parameters, transitions)
- `animator-modify` - Modify AnimatorController (add/remove states, parameters, transitions)

## Installation

### Option 1 - Installer

- **[⬇️ Download Installer](https://github.com/IvanMurzak/Unity-AI-Animation/releases/download/1.0.0/AI-Animation-Installer.unitypackage)**
- **📂 Import installer into Unity project**
  > - You can double-click on the file - Unity will open it automatically
  > - OR: Open Unity Editor first, then click on `Assets/Import Package/Custom Package`, and choose the file

### Option 2 - OpenUPM-CLI

- [⬇️ Install OpenUPM-CLI](https://github.com/openupm/openupm-cli#installation)
- 📟 Open the command line in your Unity project folder

```bash
openupm add com.ivanmurzak.unity.mcp.animation
```
