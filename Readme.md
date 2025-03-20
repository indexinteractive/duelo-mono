# duelo-mono

Monorepo for the duelo proof of concept in Unity.

Detailed documentation can be found in the [./documentation](./documentation/Readme.md) folder.

## Overview

The project has one main entry point, which directs the game to start as a server or client:

[GameMain.cs](./duelo-unity/Assets/_duelo/02_scripts/entry/GameMain.cs)

**Client Startup**

Client startup will load a [StateMachine](./duelo-unity/Assets/ind3x/state/StateMachine.cs) that allows the game to switch between menu screens. When a match is loaded, it will communicate with the server indirectly by listening to changes in the firebase data.

- [Client Startup Scene](./duelo-unity/Assets/_duelo/01_scenes/ClientMain.unity)
- [ClientMatch.cs](./duelo-unity/Assets/_duelo/02_scripts/client/match/ClientMatch.cs)

**Server Startup**

Server startup has only one purpose, which is to start a new match and begin communicating with firebase. It expects to find data in the shape of a `Unity.Services.Matchmaker.Models.MatchmakingResults` object.

Once loaded, it will begin a match and update firebase accordingly via the `ServerMatch` class.

- [Server Startup Scene](./duelo-unity/Assets/_duelo/01_scenes/ServerMain.unity)
- [ServerMatch.cs](./duelo-unity/Assets/_duelo/02_scripts/server/match/ServerMatch.cs)