# DiscordController

Creates a virtual Xbox 360 controller operated via Discord.

Requires [ViGEmBus](https://github.com/ViGEm/ViGEmBus/releases) to be installed in order to create a virtual controller.

## Accepted Commands

### Basic Input

The most basic button input command looks like this: ```left```

Multiple inputs can also be pressed at once like so: ```left+down```

You can also specify how long to hold the input down, in seconds: ```right 2.5```

### Admin Commands

The admin user has access to the following commands:
* ```reload```: Reloads the configuration file and resets the virtual controller.
* ```profile <name>```: Sets the current input profile.

## Configuration

DiscordController requires a "config.json" file specifying Discord information and controller profiles.

Controller profiles map button names to Xbox 360 controller inputs; this is particularly useful when you want text input to match the buttons of an emulated console, rather than the standard Xbox 360 button names.

### Template

```json
{
    "Discord": {
        "DiscordToken": "<your bot token>",
        "AdminUserId": <your user id>,
        "ListenChannelId": <channel to listen to>
    },
    "Controller": {
        "MinInputDuration": <minimum input duration allowed, in seconds>,
        "MaxInputDuration": <maximum input duration allowed, in seconds>,
        "MaxInputQueueSize": <max number of inputs queued at once>,
        "DefaultProfile": "<default profile to load; e.g. Switch>",
        "Profiles": {
            <your profiles here; below is an example>
            "Switch": {
                "Buttons": {
                    "a": "A",
                    "b": "B",
                    "x": "X",
                    "y": "Y",
                    "l": "LeftShoulder",
                    "r": "RightShoulder",
                    "plus": "Start",
                    "minus": "Back",
                    "dup": "Up",
                    "dright": "Right",
                    "ddown": "Down",
                    "dleft": "Left"
                },
                "Sliders": {
                    "zl": "LeftTrigger",
                    "zr": "RightTrigger"
                },
                "Sticks": {
                    "up": {
                        "Axis": "LeftThumbY",
                        "Direction": 1
                    },
                    "down": {
                        "Axis": "LeftThumbY",
                        "Direction": -1
                    },
                    "left": {
                        "Axis": "LeftThumbX",
                        "Direction": -1
                    },
                    "right": {
                        "Axis": "LeftThumbX",
                        "Direction": 1
                    },
                    "rup": {
                        "Axis": "RightThumbY",
                        "Direction": 1
                    },
                    "rdown": {
                        "Axis": "RightThumbY",
                        "Direction": -1
                    },
                    "rleft": {
                        "Axis": "RightThumbX",
                        "Direction": -1
                    },
                    "rright": {
                        "Axis": "RightThumbX",
                        "Direction": 1
                    }
                }
            }
        }
    }
}
```