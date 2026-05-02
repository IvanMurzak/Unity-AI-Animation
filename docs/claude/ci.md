# CI/CD

The release workflow (`.github/workflows/release.yml`) triggers on push to `main`:
1. Reads version from `package.json`
2. Skips release if the git tag already exists
3. Builds the `.unitypackage` installer using Unity 2022.3.62f3
4. Runs tests across Unity 2022.3.62f3, 2023.2.22f1, 6000.3.1f1 in editmode/playmode/standalone
5. Creates a GitHub release with the installer artifact
6. Publishes to OpenUPM

Required repository secrets: `UNITY_LICENSE`, `UNITY_EMAIL`, `UNITY_PASSWORD`.

Pull request tests require a maintainer to apply the `ci-ok` label before secrets are exposed to contributor code.
