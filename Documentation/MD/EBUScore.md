# EBUScore

# EBUScore

EBUSCore is the scoring program provided by the English Bridge Union (EBU). EBUScore can work with BCS 5 considering the points below.

## Main considerations

* BCS 5 uses drivers for MS Access 2000 to access the scoring (.bws) file and the BMPlayerDB.mdb files. Make sure to use a version of EBUScore that uses these drivers as well. The standard version of EBUScore uses the MS Access '97 drivers. This will not work.
* The installer will place an empty BMPlayerDB\_Template.mdb file and an empty BMPlayerDB.mdb file in the local appdata at C:\\Users\\\<username\>\\ AppData\\Local\\BCS.Net. The latter file can be filled with data from the EBU's member administration from EBUScore.
* Scoring (.bws) files can be opened from the "File" menu on the top left of the application. Server intialization and reading has to be activated manually.

## Configuration steps after installation

Follow the steps below when planning to use the function described,

### Bridgemate 2 settings

Ensure that the Bridgemate 2 settings as specified by EBUScore are used by indicating that BCS should send the Bridgemate 2 settings from the scoring file.\
*Note: for this to work a Bridgemate 2 server has to be attached to the computer and a scoring (.bws) file has to be loaded from the File menu.*\
![Image](<lib/Bridgemate2 settings.png>)

### Bridgemate 3 settings

EBUScore does not support administering the settings for the Bridgemate 3. BCS will derive some of the Bridgemate 3 settings from those for the Bridgemate 2, but the recommended approach is to indicate in BCS 5 that the application's Bridgemate 3 settings should be used

*Note: in order for this to work a Bridgemate 3 server must be attached to the computer.*

![Image](<lib/Bridgemate3 settings.png>)

### Activating the Bridgemate App

To activate use of the Bridgemate app take the following steps:

#### Register for the App server

If your club makes use of the Bridgemate App:

![Image](<lib/Bridgemate App.png>)

* Enter your clubumber and confirmation code.
* Set the endpoint index to "Production"
* Press "Check connection"
* On success click the "Use app server button"

&nbsp;

#### Activate the events to use the App server

On the "Event" tab click the "Show sessions in app" button if it is not alreaady pressed (and has a green color).\
![Image](<lib/Activate app.png>)

### Activate use of the BMPlayerDB.mdb file

On the 'Tools" tab check "Use external BMPlayerDB.mdb" option if you plan to use this. Take note of its location as this location needs to be updated in EBUScore (on the Bridgemate scoring window (Admin tab).\
![Image](<lib/BMPlayerDB.png>)

*Note: this file can be move to a different folder if so needed.*\
*Note: refer to the EBUScore documentation on how to fill the database with EBU player data.*

### Activate offline use of the Bridgemate 3

The Bridgemate 3 can be used in offline mode. To enable this click the button for this.\
*Note: for this to work a Bridgemate 3 server must be attached to the computer.*\
*Note: only activate this option when you are really planning to use this. Accidentally setting a Bridgmate 3 in offline mode is not so easily reversed.*\
![Image](<lib/Offline use.png>)

&nbsp;

## Advanced procedures

Usually BCS will be launched from EBUScore, If the configuration steps above have been followed things will "just work".\
Below follow descriptions for advanved scenario's.

### Manually loading a scoring file.

To manually load a scoring file:

* Click the "File" tab and select either a recently used scoring file or browse to a scoring file.
* Click the "Initialize all servers" button twice.
* Click the "Start monitoring" button.

### Inspecting the server contents

On the Bridgemate 2 and Bridgemate 3 tabs overviews of the event's movements and board results can be viewed. Mind that the buttons are toggles: to hide a particular view click its governing button a second time.\
Moreover it is possible to download a server's contents to the scoring file and it is possible to close the current round (filling the unplayed boards with "No Play").

Last: it is possible to reinitialize the Bridgemate 2 server, the Bridgemate 3 server or the App server seperately. The other servers will keep their data.

### Using the Bridgemate 3 in offline mode

Starting a Bridgemate in offline mode is only possible at the event's start. Once the Bridgemate has logged on in online mode this can only be changed by clicking the "Initialize all servers" button, thereby erasing all data.

Note: once a Bridgemate has been started in offline mode BCS will be unable to communicate with it until the Bridgemate has received all board results as expected by its movement. By resetting the Bridgemate communication can be reestablished as well.

* Enable offline use of the Bridgemate 3 on the "Bridgemate 3" tab.
* Manually load the scoring file. If the file has been created (and BCS been launched) from EBUScore it can be found in the recent files of the "Files" tab.
* Select the Bridgemates that should work in offline mode and click the "Set offline" button twice.\
![Image](<lib/Set offline1.png>)
* The Bridgemates will be marked as offline. Now click the "Initialize all servers" button.\
![Image](<lib/Set offline2.png>)
* On starting the Bridgemate and selecting an offline table it will operate in offline mode.

&nbsp;

