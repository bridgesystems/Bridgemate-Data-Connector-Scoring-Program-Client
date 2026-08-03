# DtoGenerator

Generates the DTO layers of the PHP, Java and Python clients and the golden JSON fixtures from this
repository's compiled assembly, which is the single source of truth for the wire format.

This project is intentionally **not** part of the solution: the NuGet publish pipeline builds the
solution and must not be affected by this tool. Build and run it on demand:

```
dotnet run --project tools/DtoGenerator
```

Default output (all git-ignored except `fixtures/`):

- `tools/DtoGenerator/out/php` — PHP 8.1 DTO classes and backed enums, namespace `Bridgemate\DataConnector\Dto`
- `tools/DtoGenerator/out/java` — Java 11 POJOs and enums, package `nl.bridgemate.dataconnector.dto` (Jackson annotations)
- `tools/DtoGenerator/out/python` — Python 3.10 dataclasses and IntEnums, package `bridgemate_dataconnector.dto` (snake_case fields, explicit `to_dict`/`from_dict` with PascalCase wire names)
- `tools/DtoGenerator/fixtures` — golden request/response JSON, committed as the compatibility contract
- `tools/DtoGenerator/fixtures/validation` — golden validation fixtures, committed; see below

To regenerate directly into local clones of the port repositories:

```
dotnet run --project tools/DtoGenerator -- ^
  --php-out    ..\Bridgemate-Data-Connector-Scoring-Program-Client-PHP\src\Dto ^
  --java-out   ..\Bridgemate-Data-Connector-Scoring-Program-Client-Java\src\main\java\nl\bridgemate\dataconnector\dto ^
  --python-out ..\Bridgemate-Data-Connector-Scoring-Program-Client-Python\src\bridgemate_dataconnector\dto
```

Copy `fixtures/` to `tests/fixtures` (PHP and Python) and `src/test/resources/fixtures` (Java)
afterwards; the port test suites assert structural JSON equality against these files.

## When to run it

Whenever a DTO, enum or the envelope changes in this repository. The generated files carry the
assembly version they were produced from, so a diff in the port repos shows what changed.

## Scope

The v1 core workflow surface: the envelope, Ping/Initialize/Continue, the Send*/Update* payload
DTOs and the poll/accept commands. Management DTOs (`BCSManagementRequestDTO`,
`AddSessionDTO`, session/section info) are not generated yet.

## Fixture notes

- Files are the exact bytes `System.Text.Json` produces with default options: PascalCase names,
  enums as integers, nulls included, `"` escaped as `"` inside the nested `SerializedData`
  string. Consumers must compare **parsed** JSON (including parsing the nested `SerializedData`
  string), never raw bytes.
- `requests/<Command>.json` is what a client must send for that command; poll requests carry
  `SerializedData: ""`, accept requests carry the serialized last queue item id.
- `responses/*.json` are representative Data Connector answers for deserialization tests,
  including an `Error.json`.
- The sample values exercise the serialization shape; they are not a playable bridge event.

## Validation fixtures

`fixtures/validation/<Dto>.<case>.json` freezes the behaviour of the C# `Validate()` methods:
each file holds a `Payload` (the DTO as a client would send it), `Args` (the validator's
parameters, e.g. `allowPlayerNumberAndName` or `forAdding`), and the `ExpectedValid` flag plus
`ExpectedMessages` list that the .NET validator produced **live at generation time**. The port
test suites construct the DTO from `Payload`, run their hand-written validator, and assert the
result and message list verbatim, including order. This keeps the C# validators the single
source of truth: any rule change shows up as a fixture diff at the next regeneration.

Not covered by fixtures (environment-dependent, implemented natively per port and tested
locally): the `Directory.Exists` checks on `InitDTO.AlternativeDataFolder` and
`ContinueDTO.AlternativeDataFolder`.
