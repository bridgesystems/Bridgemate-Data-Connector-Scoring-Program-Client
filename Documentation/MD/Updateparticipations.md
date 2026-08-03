# Update participations

![Image](<lib/Update participations.png>)

### Description

&nbsp;

Participations can be updated with an array of [ParticipationDTOs](<ParticipationDTO.md>) using the [PutParticipations command](<Overviewofcommunication.md#OverviewOfCommands>). Existing participations will be updated, enabling changing the player in a pair, the rest will be added. Each ParticipationDTO that is sent to the Bridgemate Data Connector with its SessionGuid and PlayerNumber propertes set must have a corresponding PlayerData that was sent before. Participations preferably are sent with the [InitDTO](<InitDTO.md>) while [initializing the event](<Initializeanevent.md>): this is more performant. Use the updates for movement changes after the event has started.

**Note**

For sections without the HasExplicitParticipations property set on their [SectionDTO](<SectionDTO.md>), only participations for round zero or one are accepted. BCS will calculate the participations for the subsequent rounds and add or update them. A participation with a RoundNumber greater than one is a validation error for these sections.

**Sections with explicit participations**

For a section that was created with HasExplicitParticipations set to "true", participations carry their RoundNumber and BCS stores them exactly as sent, without calculating seatings from the movement. The PutParticipations command can then be used in two ways:

1. Re-send the complete seating for all rounds. This is idempotent: existing rows are replaced, nothing is duplicated.
1. Send a partial correction, for instance the participations for a single round. Only the seats addressed by the batch are affected: for each participation in the batch, BCS removes the player's previous seat in that round and the previous occupant of the target seat, then stores the new participation. Participations for other rounds are left untouched.

Validation is atomic per request: if any participation in the batch is invalid — for example a RoundNumber greater than one for a section without explicit participations — the whole batch is rejected and nothing is stored.

After a movement update for a section with explicit participations, do not use PutParticipations to restore the seating: the [SectionUpdateDTO](<SectionUpdateDTO.md>) itself must carry the complete seating for all rounds in its Participations property.

