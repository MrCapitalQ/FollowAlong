
<div align="center">
    <img src="/res/FollowAlong.png" height="200" width="200" />
	<h1>Follow Along</h1>
	<p>
		<i>Share a portion of your desktop and focus on your mouse pointer in screen sharing apps like Teams.</i>
	</p>
    <a href="https://apps.microsoft.com/detail/Follow%20Along%20-%20Screen%20Share/9N1CV179Q032?mode=direct">
        <img src="https://get.microsoft.com/images/en-us%20dark.svg" width="200"/>
    </a>
</div>

## Introduction
Follow Along is an open source app for Windows that makes it easier when screen sharing high resolution or ultrawide desktops in apps like Teams. It allows you to share a different view where you can zoom in and follow your mouse pointer while still being able to work normally on your desktop.

## Basic Instructions
1. Select a display you want to share and press "Start" or use the shortcut CTRL + SHIFT + ALT + F. A preview of what you will be sharing will appear in the bottom left corner.
2. In a screen sharing app, start the process to share a specific window and choose the window named "Follow Along - Share Window". Do not share your whole screen.
3. Use the zoom controls in the preview window or the shortcuts CTRL + SHIFT + ALT + PLUS/MINUS to change the zoom level. Use CTRL + SHIFT + ALT + P to pause mouse pointer tracking.
4. Press "Stop" in the preview window or use the shortcut CTRL + SHIFT + ALT + F again to stop.

## Building
### Prerequisites
- Visual Studio 2022
  - .NET desktop development workload
- .NET 8 SDK

### Build and Run
1. Open the [`FollowAlong.sln`](/FollowAlong.sln) solution.
2. If not already set, set the `FollowAlong` project as the startup project.
3. Select a build configuration (Debug or Release) and architecture (x64, x86, or ARM64).
4. Start debugging.

### Execute Tests
The tests can be executed in Visual Studio's Test Explorer window. Additionally, there is a [`runTests.ps`](/scripts/runTests.ps1) script that will execute all of the tests and generate a code coverage report. This requires the `reportgenerator` dotnet global tool to be installed.
```shell
dotnet tool install -g dotnet-reportgenerator-globaltool
```
