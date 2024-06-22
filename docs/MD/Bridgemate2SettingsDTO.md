# Bridgemate2SettingsDTO

![Image](<lib/Bridgemate2SettingsDTO 1.png>)

![Image](<lib/Bridgemate2SettingsOptions.png>)

The Bridgemate2SettingsDTO contains all settings for the Bridgemate 2. It can be used as part of the [InitDTO](<InitDTO.md>) for [the intitialization of an event](<Initializeanevent.md>), or

it can be used as data for the [PutBridgemate2Settings command](<Overviewofcommunication.md#OverviewOfCommands>) to update settings. When used all settings must be provided.For readability many of the settings are expressed as enum values. This is not necessary, the use of integer values is allowed as well. In the image above the enum values will always be consecutively numbered starting wit zero. So Bm2ShowRankingOption.DoNotShow equals to zero and Bm2ShowRankingOption.ShowAfterLastRound equals to two.&nbsp;

The settings will be applied to all Bridgemate 2s for a section.The settings must be provided for each section, even if they are the same.

**Note**

In the diagram the Bridgemate2SettingsDTO is depicted as a child of the abstract BridgemateSettingsDTO. The latter's properties can be considered to be part of the Bridgemate2SettingsDTO.&nbsp;

##### SessionGuid property

Required. A guid uniquely defining the session. Must be exactly 32 character long, uppercase and cannot contaim dashes or curly braces.

##### SectionLetters property

Required. Identifes the section that the settings pertain to.

##### AutopoweroffTime property

Byte. Specifies the number of seconds after which the display will power off.

##### AutoShutDownBPC property

Boolean. Specifes if Bridgemate Control Software should shut down after the last result has been processed.

##### BM2AutoBoardNumber property

Integer/Enum (Bm2AutoBoardNumberOption).Specifies if the Bridgemate should fill in the boardnumber automatically, Also see the BM2FirstBoardManually setting.

##### BM2AutoShowScoreRecap property

Boolean. Specifies if the scores obtained should be displayed for review after the last result&nbsp; has been entered for each round. All obtained results will be shown as well after the play of the last board.

##### BM2ConfirmNP property

Boolean. Specifies if the TD must come to the table to confirm No play on the Bridgemate.

##### BM2EnterHandrecord property

Boolean. Specifies if the players can enter the handrecord for a board. If "true" then the BM2EnterHandrecordWhen setting determines when the handrecord may be entered.

##### BM2EnterHandrecordWhen property

Integer/Enum (Bm2EnterHandrecordWhenOption).Specifies when the players can enter the handrecord on the Bridgemate. The default value is zero: after all boards for the round have been played.

##### BM2FirstBoardManually property

Boolean. If the BM2AutoBoardNumber setting is "True" this setting further specifies if the board number for the first board played should be entered manually.

##### BM2GameSummary property

Boolean.If "True" shows the summary of the session after the last board has been entered.

##### BM2NameSource property

Currently not supported. Player names for at least all competing players should be sent to the Data Connector using [PlayerDataDTOs](<PlayerDataDTO.md>).

##### BM2NextSeating property

Boolean. Specifies it the Bridgemate should show the table for the next round after the last result has been entered for the current round.

##### BM2NumberEntryEachRound property

Boolean. If "True" the players must enter their player number at the start of each round.

##### BM2NumberEntryPreloadValues property

Boolean.If player numbers are entered each round, preload known player numbers.

##### BM2PairNumberEntry property

Integer/Enum (Bm2PairNumberEntyOption). Specifies if when entering a result the declaring pair must be entered.

##### BM2PINcode property

String of four digits. Optional.Defaults to "0000".

##### BM2Ranking property

Integer/Enum (Bm2ShowRankingOption). Specifies if and when the current ranking for the pairs should be shown.

##### BM2RecordBidding property

Boolean. Currently not supported.

##### BM2RecordPlay property

Boolean. Currently not supported.

##### BM2RemainingBoards property

Boolean. If "True" the Bridgemate will show how many boards remain to be played.

##### BM2ResetFunctionKey property

Boolean. If "True" the reset function key will be available from the TD menu without having to enter the PIN code first.

##### BM2ResultsOverview property

Integer/Enum (Bm2ResultsOverviewOption). Specifies how the previous results on the board should be shown. Either as frequency list (show number of times a specific contract was played) or as traveler (show the result for each pair).

##### BM2ScoreCorrection property

Boolean. If "True" allows the players to erase a result in the round on their table and to enter it again.

##### BM2ScoreRecap property

Boolean. If "True" the Bridgemate will show a "Scores" button to check the entered results for the round.

##### BM2ShowHands property

Boolean. Currently not supported.

##### BM2ShowPlayerNames property

Integer/Enum (Bm2ShowPlayerNamesOption). Specifies if and when the playernames shoud be shown on the Bridgemate at the start of a round.

##### BM2SummaryPoints property

Integer/Enum (Bm2SummaryPointsOption). If the BM2GameSummary is "True" specifies whether the obtained results per board should be shown as matchpoints or as a percentag. For this to work the BM2RankingProperty must be either ShowAllRounds (1) or ShowAfterLastRound (2).

##### BM2TDCall property

Boolean. If "True" specifies that the players can call the Tournament Director from the Bridgemate.

##### BM2TextBasedNumber property

Currently not supported. Player numbers can be set using the [PlayerDataDTO](<PlayerDataDTO.md>). The player number is stored as a string.

##### BM2ValidateLeadCard property

Boolean. If "True" The entered lead card will be checked against the handrecord.

##### BM2ValidateRecording property

Currently not supported.

##### BM2ViewHandRecord property

Boolean. If "true" the players get the option to view the handrecord after entering the result.

##### BoardOrderVerification property

Boolean. If "True" the Bridgemate will check the order of entry of the boardnumbers.

##### EnterResultsMethod property

Integer/Enum (Bm2ResultsEntryOption). Specifies how the resulting tricks after play must be entered.

##### GroupSections property

Not supported. Use the [ScoringGroupDTO](<ScoringGroupDTO.md>) to indiciate that sections should be scored together.

##### HandRecordValidation property

Boolean. Currently not supported.

##### Intermediate results property

Boolean. Currently not supported.

##### LeadCard property

Boolean. If "True" specifies that the leadcard must be entered along with the cotnract.

##### MaximumResults property

Integer (0-127). Specifies the maximum number of results to show. Zero means "unlimieted".

##### MemberNumber property

Boolean. If "True"&nbsp; the Bridgemate will ask for playernumbers at the start of a round when for that round there are participations without a player number.

##### MemberNumbersNoBlankEntry property

Boolean If "True" entry of player numbers cannot be skipped,

##### NumberValidation property

Currently not supported.

##### RepeatResults property

Boolean. If "True" allows the players to review the results of a round again.

##### ScorePoints property

Integer/Enum (Bm2PointScorePerspectiveOption). Show score points from perspective of North-South or from declarer.

##### ShowContract property

Integer/Enum (Bm2ContractRepresentationOption). Specifies if the denomination of the contract should be shown with letters or symbols.

##### ShowOwnResult property

Boolean. If "True" specifies that the own result should be included in the result overview.

##### ShowPairNumbers property

Boolean. If "True" the pair numbers for the current round will be displayed on the Bridgemate.

##### ShowPercentage property

Boolean. If "True" show the percentage obtained on the board just played.

##### ShowResults property

Boolean. If "True" show previous results on the board just played.

##### VerificationTime property

Integer (1-7) The time in seconds that the verification message is shown before the opponents get to confirm the result on the board just played.

