# BCSManagementResponseDTO

#### ![Image](<lib/BCSManagementResponseDTO 2.png>)

#### The BCSManagementResponseDTO

The BCSManagementResponseDTO is returned for a [BCSManagementCommand](<Overviewofcommunication.md>) that asks for session information and/or the scoringfile path.

##### EventGuid property

The guid of the current event being administerd by Bridgemate Control Software. Will be empty if IsRunning is "False" or when the [BCSManagementRequestDTO](<BCSManagementRequestDTO.md>).Command property contains GetAllSessionsInformation (8).

##### IsRunning property

If "True" BCS is running.

##### ScoringFilePath property

The full path to BCS's scoring file. Can be used to make and restore back-ups.

#### The SessionInfoDTO

The SessionInfoDTO contains either information on the sessions that BCS currently administers ([BCSManagementRequestDTO](<BCSManagementRequestDTO.md>).Command contains GetRunningSessions (2) or information on all known sessions in the scoring file ([BCSManagementRequestDTO](<BCSManagementRequestDTO.md>).Command contains GetAllSessionsInformation (8) . It will only contain data when IsRunning is "True".

##### EventGuid property

The guid of the event the session belongs to.

##### SessionDateTime property

The date and time when the session started,

##### SessionGuid property

The guid of the session.

##### SessionName property

The name of the session.

