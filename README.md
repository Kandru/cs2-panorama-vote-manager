> [!CAUTION]
> This plug-in is not finished an may not be suitable and production-ready. It works well enough but may change in future. Other plug-ins which depend on it (like my knife-fight or native-mapvote plug-in may break in future without updating them, too). 

# CounterstrikeSharp - Panorama Vote Manager

[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-panorama-vote-manager?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-panorama-vote-manager/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-map-modifier](https://img.shields.io/github/issues/Kandru/cs2-panorama-vote-manager)](https://github.com/Kandru/cs2-panorama-vote-manager/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)

> [!TIP]
> This plug-in does nothing on it's own. You either need to develop your own plug-in or use a third-party plug-in to make use of this :)

The Panorama Vote Manager allows plugins to interact with the CS2 Panorama Vote UI to start votes (the ones on the left side where the player can press F1 to agree or F2 to disagree). Plugins can request votes for specific users or all users. Votes for all users block other votes and can have an optional cooldown. After the cooldown, plugins can request votes again. The plugin provides an easy API for initiating votes and checking if they were successful. Votes can also be withdrawn if needed.

What is special about this implementation? This plug-in was created because I had the need to create votings in my plug-ins and wanted to make sure only ONE vote is active during any given time. Therefore this plug-in allows other developers to initialize votes and this plug-in queues the vote in case another vote is already running. It also returns the estimated time until the vote will start (so a plug-in developer could check if this meets the requirements and wait or dismiss the vote entirely).

This plug-in also allows to show votes only to specific players. This is primarily good for knife-fight plug-ins like [mine](https://github.com/Kandru/cs2-knife-fight).

## Installation

1. Download and extract the latest release from the [GitHub releases page](https://github.com/Kandru/cs2-panorama-vote-manager/releases/).
2. Stop the server.
2. Move the "PanoramaVoteManager" folder to the `/addons/counterstrikesharp/plugins/` directory.
2. Move the "PanoramaVoteManagerAPI" folder to the `/addons/counterstrikesharp/shared/` directory.
3. Restart the server.

## Using custom vote text

To be able to use custom vote text your clients (and your server) need the *platform_english.txt* inside the *csgo/resource* folder. An example is provided via this repository. Otherwise the vote will **NOT** show up when using custom text. You can still use the default vote strings inside your plugins, though. Easiest way is to distribute this via a WorkShop AddOn.

## Configuration

This plugin automatically creates a readable JSON configuration file. This configuration file can be found in `/addons/counterstrikesharp/configs/plugins/PanoramaVoteManager/PanoramaVoteManager.json`.

```json
{
  "enabled": true,
  "debug": false,
  "cooldown": 5,
  "server_enable_voting": true,
  "server_disable_vote_options": true,
  "ConfigVersion": 1
}
```

### enabled

Whether or not the plug-in is enabled.

### cooldown

Cooldown in seconds after one vote was finished and before the next vote starts. Should be 5+ seconds to allow the voting window to close properly.

### server_enable_voting

Whether or not after map start this plug-in should automatically execute the necessary server-side commands to allow voting.

### server_disable_vote_options

Whether or not all native cs2 votings should be disabled (only effective if server_enable_voting is enabled). This will execute all necessary server commands to disable the other native votings. Hint: if you disable this you can use the cs2 voting options like kick, ban, map change etc. but: **this plug-in cannot see whether or not a native vote is currently running and this will create conflicts! You should know what you do if you disable this!**

## Commands

### (server-console only) panoramavotemanager <command>

#### reload

Reloads the configuration.

#### test

Tests the panorama vote manager functionality. Will spawn a new vote.

## Compile Yourself

Clone the project:

```bash
git clone https://github.com/Kandru/cs2-panorama-vote-manager.git
```

Go to the project directory

```bash
  cd cs2-panorama-vote-manager
```

Install dependencies

```bash
  dotnet restore
```

Build debug files (to use on a development game server)

```bash
  dotnet build
```

Build release files (to use on a production game server)

```bash
  dotnet publish
```

## FAQ

TODO

## License

Released under [GPLv3](/LICENSE) by [@Kandru](https://github.com/Kandru).

## Authors

- [@derkalle4](https://www.github.com/derkalle4)
- [@jmgraeffe](https://www.github.com/jmgraeffe)
