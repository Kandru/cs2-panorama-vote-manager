# CounterstrikeSharp - PersistenceManager

[![UpdateManager Compatible](https://img.shields.io/badge/CS2-UpdateManager-darkgreen)](https://github.com/Kandru/cs2-update-manager/)
[![GitHub release](https://img.shields.io/github/release/Kandru/cs2-persistence-manager?include_prereleases=&sort=semver&color=blue)](https://github.com/Kandru/cs2-persistence-manager/releases/)
[![License](https://img.shields.io/badge/License-GPLv3-blue)](#license)
[![issues - cs2-map-modifier](https://img.shields.io/github/issues/Kandru/cs2-persistence-manager)](https://github.com/Kandru/cs2-persistence-manager/issues)
[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=C2AVYKGVP9TRG)

The persistence manager gives persistence on player settings, map settings and anything else you want and need (via simple database queries).

## Installation

1. Download and extract the latest release from the [GitHub releases page](https://github.com/Kandru/cs2-persistence-manager/releases/).
2. Move the "PersistenceManager" folder to the `/addons/counterstrikesharp/configs/plugins/` directory.
3. Restart the server.

Updating is even easier: simply overwrite all plugin files and they will be reloaded automatically. To automate updates please use our [CS2 Update Manager](https://github.com/Kandru/cs2-update-manager/).


## Configuration

This plugin automatically creates a readable JSON configuration file. This configuration file can be found in `/addons/counterstrikesharp/configs/plugins/PersistenceManager/PersistenceManager.json`.

```json

```


## Compile Yourself

Clone the project:

```bash
git clone https://github.com/Kandru/cs2-persistence-manager.git
```

Go to the project directory

```bash
  cd cs2-persistence-manager
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
