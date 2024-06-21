# Bridgemate3SettingsDTO

![Image](<lib/Bridgemate3SettingsDTO.png>)![Image](<lib/Bridgemate3SettingsOptions 1.png>)

&nbsp;

The Bridgemate3SettingsDTO contains all settings for the Bridgemate 3. It can be used as part of the [InitDTO](<InitDTO.md>) for [the intitialization of an event](<Initializeanevent.md>), or

it can be used as data for the [PutBridgemate3Settings command](<Overviewofcommunication.md#OverviewOfCommands>) to update settings. When used all settings must be provided.For readability many of the settings are expressed as enum values. This is not necessary, the use of integer values is allowed as well. In the image above the enum values will always be consecutively numbered starting wit zero. So Bm3ViewResultsOption.Disabled equals to zero and&nbsp; Bm3ViewResultsOption.OwnResutlsAndOtherResults equals to two.&nbsp;

The settings will be applied to all Bridgemate 3s for a section.The settings must be provided for each section, even if they are the same.

**Note**

In the diagram the Bridgemate3SettingsDTO is depicted as a child of the abstract BridgemateSettingsDTO. The latter's properties can be considered to be part of the Bridgemate3SettingsDTO.&nbsp;

##### SessionGuid property

Required. A guid uniquely defining the session. Must be exactly 32 character long, uppercase and cannot contaim dashes or curly braces.

##### SectionLetters property

Required. Identifes the section that the settings pertain to.

##### AudioVolume property

Integer (0-7). Currently not in use.

##### ConnectionMode property

Integer/Enum (BM3ConnectionModeOption). Specifies if the Bridgemates for this section will operate in offline mode. In offline mode the Bridgemate will not communicate with the Bridgemate 3 server during the session, but will try to send its results after the last board had been played.

##### DimMode property

Integer (0-15). Defines the number of seconds (times 5) before the screen of the Bridgemate will dim. So the default value of 2 amounts to 10 seconds.

##### EnterHandrecord property

Integer/Enum (BM3EnterHandrecordOption). Specifies if and when the players can enter the handrecord for a board if it is not present.

##### Language property

Integer/Enum (BM3LanguageOption). Specifies which language the Bridgemate will use. The default is zero for English.

##### LeadCardEntry property

Integer/Enum (BM3LeadCardEntryOption). Specifies if the leadcard must be enterd together with the contract and if the leadcard should be validated againts the handrecord.

##### NextRoundSeating property

Integer/Enum (BM3NextRoundSeatingOption).Specifes if the seatings for the next round must be displayed after the last result has been entered for the round.

##### PlayerRegistration property

Integer/Enum (BM3PlayerRegistrationOption). Specifies if and when the players can make themselves known by entering their playernumber and/or name on the Bridgemate.

##### PlayerRegistrationDuringPlay property

Integer/Enum (BM3PlayerRegistrationOptionDuringPlay). Specifies if and when the players can make themselves known by entering their playernumber and/or name on the Bridgemate while the round is in progress (i.e.: after a board has been entered).

##### RankingDisplay property

Integer/Enum (BM3RankingDisplayOption). Specifies if and when the Bridgemate will show the current ranking for the players on the table.

##### ResultMethod property

Integer/Enum (BM3ResultMethodOption). Specifies how players can enter the result for a board: Up/Down tricks, total tricks or American style.

##### ResultReentry property

Integer/Enum (BM3ResultReentryOption). Specifies if players may reenter the result of the board after the original entry has been confirmed by the opponents.

##### ScoreRecap property

Integer/Enum (BM3ScoreRecapOption). Specifies if and when the Bridgemate will show a recap of the scores obtained on the table in the current round.

##### ScreenBrightness property

Integer (1-7) Defines the screen brightness.

##### ScreenOffMode property

Integer (0-15). Defines the number of seconds (times 5) before the screen of the Bridgemate will turn off. A value of zero, the default, means "never turn off".

##### SleepMode property

Integer (0-120). Defines the number of seconds (times 5) befote the Bridgemate will enter Sleepmode. Once the Bridgemate has entered sleepmode its powerbutton must be used to wake it up again.

##### TdCall property

Integer/Enum (BM3TdCallOption). Specifies if players can call for the Tournament Director using the Bridgemate.

##### TdPinCode property

A string of four digits. The code the TD must enter to get access to the TD menu. Defaults to "0000".

##### ViewHandrecord property

Integer/Enum (BM3ViewHandrecordOption). Specifies if players may view the handrecord after the result has been entered.

##### ViewOwnPercentage property

Integer/Enum (BM3ViewHandrecordOption). Specifies if the own percentage should be included in the results overview.

##### ViewResults property

Integer/Enum (BM3ViewResultsOption). Specifies if the Bridgemate will show the other results for the board that was just entered.

