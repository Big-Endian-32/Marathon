# Sonic '06 Toolkit
Sonic '06 Toolkit is an application originally written in VB.NET to help make modding easier for SONIC THE HEDGEHOG (2006). Now being written in C#, Sonic '06 Toolkit has been made open-source to allow the community to contribute to the project.

# Project Goals
### ARC Unpacking and Repacking (to be replaced)
- Currently based off [Shadow LAG](https://github.com/lllsondowlll) and [ModdingUnderground](https://www.youtube.com/user/ModdingUnderground)'s ARC unpacker/repacker. Once [RadfordHound](https://github.com/Radfordhound)'s [HedgeLib](https://github.com/Radfordhound/HedgeLib) ARC class is complete, we will be able to make use of that.

### ADX Encoding and CSB Extracting (done)
- Currently based off [Skyth](https://github.com/blueskythlikesclouds)'s [Sonic Audio Tools](https://github.com/blueskythlikesclouds/SonicAudioTools) for extracting CSB files and some tools from [CRI Middleware Corporation](https://www.criware.com/en/) for encoding ADX files.

### LUB Decompiling (to be replaced)
- Currently based off [Shadow LAG](https://github.com/lllsondowlll)'s Java-based Lua decompiler which is soon to be replaced.

### MST Decoding
- Planned features to be able to decode MST files, which are BINA string tables with null terminated unicode.

### SET Converting
- Planned features to be able to convert SET files to the XML format. Once [RadfordHound](https://github.com/Radfordhound)'s [HedgeLib](https://github.com/Radfordhound/HedgeLib) SET class is complete, we will be able to make use of that. It is already in a decent state, but converting the XML back to SET can cause issues, so it isn't worth including yet.

### XNO Converting and XNM Pairing (done)
- Currently based off [Skyth](https://github.com/blueskythlikesclouds) and [DarioSamo](https://github.com/DarioSamo)'s [XNO converter](https://github.com/blueskythlikesclouds/SkythTools/blob/master/Sonic%20'06/xno2dae.exe).

# Requirements
### ADX Encoder
- Currently requires both [x86](https://www.microsoft.com/de-de/download/details.aspx?id=8328) and [x64](https://www.microsoft.com/en-us/download/details.aspx?id=13523) versions of Microsoft Visual C++ 2010 SP1.

### LUB Decompiler
- Currently requires the [latest version of Java](https://www.java.com/en/download/) to work.

### XNO Converter
- Currently requires both [x86](https://www.microsoft.com/de-de/download/details.aspx?id=8328) and [x64](https://www.microsoft.com/en-us/download/details.aspx?id=13523) versions of Microsoft Visual C++ 2010 SP1.
