# FMOD Unity - Event / GUID Swapper

## Description

This FMOD patch will add a **Swap button** to `[FMODUnity.EventRef]` property which facilitates swapping Event Path with the more robust GUID.

Starting with version 1.1.0 I am also proud to announce that this version will add the capabilities of having **GUID Workflow!** The GUID of the event will be used *automatically* when you select/drag the event. To help workflow even more, when you click on the *Search* button beside the path entry field, the Event Browser will jump to the name of the event correctly.*

**Swap button** patch has been tested and verified in **FMOD 2.00.08 up to 2.01.07 (Unity Verified)**<br>
**GUID workflow** patch has been tested and verified in **FMOD 2.01.05 and 2.01.07 (Unity Verified)**

<sup>*\*As of now, this only works for FMOD 2.01.05 and 2.01.07 due to changes on how FMOD implements the Event Browser window. Will try to work for a patch for earlier version as well.*</sup>

**NOTE: Starting from FMOD 2.02.00 you won't need this patch anymore if you set your Event Linkage to GUID! :heart_eyes:**

## How to install

1. Download the [.unitypackage](https://github.com/made-indrayana/fmod-unity-event-guid-swapper/releases/) and import it to your Unity project.
2. Add package using the built in Unity Package Manager with git url `https://github.com/made-indrayana/fmod-unity-event-guid-swapper.git`
3. Clone/download the whole repository and add package using the built in Unity Package Manager with "Add Package from disk" by choosing the `package.json` file.

## How to use

After importing the package / .unitypackage, you will have a new `Double Shot` menu in your Unity Editor. Click there and go to `Double Shot > FMOD > Patcher > Event to GUID Swapper`. It will then ask if you want to apply the patch and just click OK. If you ever changed your mind for whatever reason and want to change your workflow back to normal Event Paths, just untick the option `Use GUID as Event Path` in `Double Shot > FMOD`.

<p align="center">
...a wild <u><b>Swap Button</b></u> appears!<br><br>
<img src="https://raw.githubusercontent.com/made-indrayana/fmod-unity-event-guid-swapper/imgs/screenshot01.png" width=400><br><br>
Swap button in action:<br><br>
<img src="https://raw.githubusercontent.com/made-indrayana/fmod-unity-event-guid-swapper/imgs/inaction.gif" width=400><br><br>
Picking Event will use its GUID automatically (FMOD 2.01.05 and 2.01.07):<br><br>
<img src="https://raw.githubusercontent.com/made-indrayana/fmod-unity-event-guid-swapper/imgs/eventpicker-guid.gif" width=400><br><br>
Clicking search on a GUID Event will jump to its referenced Event (FMOD 2.01.05 and 2.01.07):<br><br>
<img src="https://raw.githubusercontent.com/made-indrayana/fmod-unity-event-guid-swapper/imgs/eventpicker-jump.gif" width=400><br><br>
</p>
