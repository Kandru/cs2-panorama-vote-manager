# CounterstrikeSharp - Panorama Vote Manager

[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-panorama-vote-manager?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-panorama-vote-manager/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-map-modifier](https://img.shields.io/github/issues/Kandru/cs2-panorama-vote-manager)](https://github.com/Kandru/cs2-panorama-vote-manager/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)

The Panorama Vote Manager allows plugins to interact with the CS2 Panorama Vote UI to start votes. Plugins can request votes for specific users or all users. Votes for all users block other votes and can have an optional cooldown. After the cooldown, plugins can request votes again. The plugin provides an easy API for initiating votes and checking if they were successful. Votes can also be withdrawn if needed.

## Installation

1. Download and extract the latest release from the [GitHub releases page](https://github.com/Kandru/cs2-panorama-vote-manager/releases/).
2. Stop the server.
2. Move the "PanoramaVoteManager" folder to the `/addons/counterstrikesharp/configs/plugins/` directory.
2. Move the "PanoramaVoteManagerAPI" folder to the `/addons/counterstrikesharp/configs/shared/` directory.
3. Restart the server.

## Using custom vote text

To be able to use custom vote text your clients (and your server) need the *platform_english.txt* inside the *csgo/resource* folder. An example is provided via this repository. Otherwise the vote will **NOT** show up when using custom text. You can still use the default vote strings inside your plugins, though. Easiest way is to distribute this via a WorkShop AddOn.

## Configuration

This plugin automatically creates a readable JSON configuration file. This configuration file can be found in `/addons/counterstrikesharp/configs/plugins/PanoramaVoteManager/PanoramaVoteManager.json`.

```json

```


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
