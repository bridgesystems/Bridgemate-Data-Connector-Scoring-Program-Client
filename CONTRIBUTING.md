# Contributing

Thank you for your interest. This repository is maintained by Bridge Systems BV as the reference
client for the Bridgemate Data Connector.

- **Issues are welcome** — bug reports, documentation gaps, protocol questions that turned out to
  be defects. Include reproduction details (see SUPPORT.md).
- **Pull requests by prior agreement only.** The wire format is shared by BCS, this client and the
  PHP/Java clients, so changes need coordination; please open an issue or a discussion first.
- Mind that changes to the DTOs or the request/response envelope affect the generated PHP and Java
  client layers: they are regenerated from this repository with `tools/DtoGenerator` and must stay
  in sync.
- The emulator (`emulator/`) has its own licence (see `emulator/EULA.txt`) and is not open to
  redistribution; treat it as reference material.
