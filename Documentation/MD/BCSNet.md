# BCS.Net

# What is BCS 5?

BCS 5 is the newer version of BCS, rewritten in .Net. It has some new features compared to BCS 4 (BmPro,exe).On the other hand BCS 5 does not support all functions that BCS 4 does. Scoring files (.bws) can be exchanged between the two programs, after, before and even during sessions.

BCS 5 supports data exchange with a scoring program using the .bws file, but also using a database independent API. For more information please read the Bridgemate Data Connector developer's guide.

&nbsp;

# New functions of BCS 5

BCS 5 has some features that BCS.Classic has not.

* BCS 5 is a 64 bit application.
* It needs to be installed only once on a computer, but each Windows account can use its own version provided it is installed in a different folder.
* BCS 5 supports combined use of Bridgemate 2 and Bridgemate 3. The Bridgemates can be replaced during a session.
* BCS 5 on one hand mimicks the menu structure of BCS 4, but can show data in different ways.
* Data exchange using a database independent API. This is outside the scope of this document.

&nbsp;

# Functions that BCS 5 does not support

BCS 5 does not support some features that BCS 4 does.

* No support for Brigemate Pro
* Currently no support for reporting and .html files with the session's rankings.
* No support for the /h (handrecords) start-up parameter. Handrecords from the Handrecords table will always be uploaded to the servers if present.
* No support for directly altering records of the scoring file.

&nbsp;

# Considerations for migration

The following considerations apply when deciding to migrate to BCS 5

* You want to use Bridgemate 2 and Bridgemate 3 together.
* You experience problems due to the fact that BCS 4&nbsp; is a 32 bits application.
* You do not use reporting or live .html files for the rankings.
* BCS 5 will support all future functions of the Bridgemates, but not necessarily all of them through the .bws scoring file.&nbsp;
* You are writing a new scoring program or currently have no extensive code base for using the .bws scoring file for data exchange. At some point communication using the newer Data Connector process will offer features that using data exchange through the .bws file does not.
* At some point in the unforseeable feature 32 bits programs may cease to be supported by Microsoft. But this will be years and years away.
* BCS 5 will interact with your scoring program in exact the same way as BCS 4 does. So apart from the consideations above your scoring program will not "know" that it is interacting with a new application.

&nbsp;

# How to use BCS 5

## Installation

The installer for BCS 5 can be found on [www.bridgemate.com](<https://www.bridgemate.com> "target=\"\_blank\"") or on the local version of it for your country, You only need to install the application once on the computer.

## Launching BCS 5

The installer will write the location of the application to HKEY\_LOCAL\_MACHINE\\SOFTWARE\\Bridge Systems BV\\BCS.Net\\InfoForExternalProgram\\ExePath in the Windows registry. You can retrieve this value to launch BCS 5.

BCS 5 has the same name for its executable: "BMPro.exe", so even when you cannot change the code launching BCS from your scoring program, it will be able to start BCS 5, provided it knows where the executable is located.

### Start-up parameters

The start-up parameters for BCS.Net are similar to those of BCS.Classic, but the syntax is different. Between parameters there must be a space.

That said, BCS 5 supports the start-up parameters used for BCS 4 as well.

* \-f: Make BCS.Net load a .bws file. The filename must follow within double quotation marks without a space. This option is mutually exclusive with -m.

  * Example: *BCS.Net -f"C:\\My Folder\\Bws Files\\FridayEvening.bws"*

* \-m: Use the Bridgemate Data Connector API to manage the Bridgemates. This option is mutually exclusive with -f. How to use this is outside the scope of this document.
* \-s (only together with -f or -m) Reset the Bridgemates and load the data from the .bws file into them.
* \-r (only together with -f or -m) start reading.
* \-m: Start BCS.Net minimized.
* \-c: Auto shutdown BCS.Net when all scores have been processed.
* \-b: Use byte logging for all communication.
* \-l: Sets the loglevel. Add the desired loglevel directly afther the parametes without a space. Avaliable levels are 'd' for "debug" and 't' for "trace". The default level is "info".&nbsp;

  * Example: *-ld for "debug".*

&nbsp;

A typical commandline for starting a session would be:\
&nbsp; &nbsp; *BMPro.exe -f"C:\\My Folder\\Bws Files\\FridayEvening.bws" -s -r*

*A typical commandline for restarting BCS.Net&nbsp; minimized with a previously used .bws file would be:*\
*&nbsp; &nbsp; BMPro.exe -f"C:\\My Folder\\Bws Files\\FridayEvening.bws" -r -m*

&nbsp;

### Unsupported start-up parameters

The following start-up parameters for BCS 4 are not supported in BCS 5 and will throw an exception if used:

* \-h (handrecrords): BCS 5 checks the Handrecords table for data and will upload it to the Bridgemate if the -s (reset) parameter is set.
* \-pi (second instance): BCS 5 cannot be run twice for different sets of Bridgemates.

&nbsp;

