# Android Temperature Checker

A simple .NET MAUI Android app that displays the temperature sensors readable
on the device:

- Battery temperature (`BatteryManager.EXTRA_TEMPERATURE`)
- Each readable thermal zone under `/sys/class/thermal/thermal_zone*/temp`,
  labeled by its `type` (e.g. `cpu-thermal`, `battery`, `gpu`, `skin-therm`,
  …). Some OEMs restrict access to certain zones on non-rooted devices, so
  the list will vary by phone.

Readings refresh every 2 seconds, or pull-to-refresh.

## Installing the APK from GitHub

Every push runs the **Build Android APK** workflow
(`.github/workflows/build-apk.yml`). When it finishes:

1. Open the run in the **Actions** tab on GitHub.
2. Scroll to the **Artifacts** section at the bottom of the run summary.
3. Download `AndroidTemperatureChecker-apk` — it contains the signed APK.
4. Transfer the APK to your phone and install it (you'll need to enable
   "Install unknown apps" for the source you used to transfer it).

The APK is signed with the standard Android debug keystore, which is fine
for sideloading but **not** for the Play Store.

## Local build

```bash
dotnet workload install maui-android
dotnet publish -c Release -f net9.0-android
```

The signed APK lands under `bin/Release/net9.0-android/publish/`.

## Project layout

- `AndroidTemperatureChecker.csproj` — single-project MAUI app, Android-only
- `MainPage.xaml` / `MainPage.xaml.cs` — the temperature list UI
- `Services/` — platform-agnostic interface and DTO
- `Platforms/Android/AndroidTemperatureService.cs` — reads battery temp +
  `/sys/class/thermal` zones
