This is the project for KMD Identity Android and iOS Test Application based on .NET MAUI framework.

The application is set-up with a fully working log-in and log-out flow, including simulation of fingerprint authentication. The available Identity Providers are kmd-ad-prod and nemlogin-3-test-public.

To run this locally with iOS, you need:
- Good luck

To run this locally with Android, you need to:
- Have .NET 7 SDK installed
- Modify Visual Studio installation and add .NET Multi-Platform App UI development under Workloads and add Xamarin under Individual components
- After installation, start Visual Studio, Click on Tools (in the top menu bar), Select Android and then Android Device Manager
  - Click on New, Select Android 11.0 - API 30 under OS if you want to simulate fingerprint authentication. This app has been tested up to Android 13.0 - API 33
  - Start the newly created device. Once started, set-up fingerprint authentication in Android (search in Settings if you can't find it). The Android Emulator is able to simulate fingerprint touches
- In Visual Studio, select the MAUI project as Startup Project
- In Visual Studio, Ensure your newly created Android device is listed as the start-up device when pressing F5 (or the play-button)
- Start the MAUI project (F5 or play-button)

