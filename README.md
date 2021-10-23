# Worms

This repository hosts .NET libraries which can import, modify, and export the following file formats of second generation Worms games developed by Team17 or Mgame:

| Extension | Description       | Games       | Library                            | Load | Save |
|-----------|-------------------|-------------|------------------------------------|:----:|:----:|
| BIT       | Monochrome Map    | WA, WWP     | `Syroot.Worms.Armageddon`          | ❌   | ❌  |
| DAT       | Mission           | W2          | `Syroot.Worms.Worms2`              | ❌   | ❌  |
| LAND.DAT  | Land Data         | W2, WA, WWP | `Syroot.Worms`                     | ✔    | ✔   |
| DIR       | Archive           | W2, WA, WWP | `Syroot.Worms`                     | ✔    | ✔   |
| IGD       | Image Container   | WWPA        | `Syroot.Worms.Mgame`               | ✔    | ❌  |
| IMG       | Image             | W2, WA, WWP | `Syroot.Worms`                     | ✔    | ✔   |
| KSF       | Image Container   | OW          | `Syroot.Worms.Mgame`               | ✔    | ❌  |
| LEV       | Generated Map     | WA, WWP     | `Syroot.Worms.Armageddon`          | ✔    | ✔   |
| LEV       | Monochrome Map    | W2          | `Syroot.Worms.Worms2`              | ❌   | ❌  |
| LPD       | Interface Layout  | WWPA        | `Syroot.Worms.Mgame`               | ✔    | ❌  |
| OPT       | Scheme Options    | W2          | `Syroot.Worms.Worms2`              | ✔    | ✔   |
| PAL       | Palette           | W2, WA, WWP | `Syroot.Worms`                     | ✔    | ✔   |
| PXL       | Project X Library | WA+PX       | `Syroot.Worms.Armageddon.ProjectX` | ✔    | ✔   |
| PXS       | Project X Scheme  | WA+PX       | `Syroot.Worms.Armageddon.ProjectX` | ✔    | ✔   |
| ST1       | Team Container    | W2          | `Syroot.Worms.Worms2`              | ✔    | ✔   |
| WAGAME    | Replay            | WA          | `Syroot.Worms.Armageddon`          | ❌   | ❌  |
| WAM       | Mission           | WA, WWP     | `Syroot.Worms.Worms2`              | ❌   | ❌  |
| WEP       | Scheme Weapons    | W2          | `Syroot.Worms.Worms2`              | ✔    | ✔   |
| WGT       | Team Container    | WA          | `Syroot.Worms.Armageddon`          | ✔    | ✔   |
| WSC       | Scheme            | WA, WWP     | `Syroot.Worms.Armageddon`          | ✔    | ✔   |
| WWP       | Team Container    | WWP         | `Syroot.Worms.WorldParty`          | ✔    | ✔   |

Implementation of formats listed above as unsupported is planned for a later date.

## Tools

* `Syroot.Worms.Mgame.Launcher`: Creates a fake launch configuration to start OW or WWPA clients with.
* `Syroot.Worms.Mgame.GameServer`: Simulates OW or WWPA networking to allow playing games.
* `Syroot.Worms.Worms2.GameServer`: Simulates a Worms 2 server.

## Modules

* `FrontendKitWS`: WormKit-like module loader for patching the Worms 2 Frontend.
* `fkNetcode`: Patches Worms 2's outdated external IP detection and resolves it properly via a web service.

## Availability

The libraries are available on [NuGet](https://www.nuget.org/packages?q=Syroot.Worms).
