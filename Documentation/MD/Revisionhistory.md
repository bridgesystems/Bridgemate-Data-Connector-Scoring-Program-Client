# Revision history

**Document version:**&nbsp; &nbsp; 0,3\
**Release date:**&nbsp; &nbsp; &nbsp; &nbsp; 2026-08-03\
**BCS.Net version:**&nbsp; &nbsp; 5.0\
**Changes:**&nbsp; &nbsp; &nbsp; &nbsp; Added explicit per-round participations for individual sessions and other formats where partnerships change between rounds: new HasExplicitParticipations property on the SectionDTO and SectionUpdateDTO, ParticipationDTO.RoundNumber values greater than one for sections that have this property set, and replace semantics for participations sent with a movement update. Validation is atomic per request: a batch containing an invalid participation is rejected as a whole. Scoring programs that need this feature must require this BCS version or later; older BCS versions reject participations for rounds greater than one with a validation error.

**Document version:**&nbsp; &nbsp; 0,2\
**Release date:**&nbsp; &nbsp; &nbsp; &nbsp; 2026-07-10\
**BCS.Net version:**&nbsp; &nbsp; 5.0\
**Changes:**&nbsp; &nbsp; &nbsp; &nbsp; Added http communication with the local or LAN hosted Data Connector: configurable base address on the http client (default http://localhost:5079). The cloud hosted Data Connector has been retired.

**Document version:**&nbsp; &nbsp; 0,1\
**Release date:**&nbsp; &nbsp; &nbsp; &nbsp; 2024-02-18\
**BCS.Net version:**&nbsp; &nbsp; 1.0.855.1\
**Changes:**&nbsp; &nbsp; &nbsp; &nbsp; Initial draft

