# Muck Settings API

This is a helper mod made to allow easier addition of settings to mods that require configuration, all in the in-game settings menu.

An example of implementing this library can be found in the "MuckSettingsExample" subdirectory of this repository.

To add your own settings, first patch one of the 5 tabs in ``MuckSettings.Settings``: either ``Gameplay``, ``Controls``, ``Graphics``, ``Video`` or ``Audio``. Don't worry about the settings overflowing, all pages are scrollable. Next, make sure you take a ``MuckSettings.Settings.Page`` argument called ``page`` in your patch, and you can add settings to that page. To save your settings, you should be ``using BepInEx.Configuration;``, and create a ``ConfigFile`` to store your config. Make sure to specify that ``config.SaveOnConfigSet = true;`` or else your config willl not save when the user changes the settings!

# Different settings

## Boolean Setting

This one is fairly straightforward. It's a toggle that can be either on, or off.

- You should use ``page.AddBoolSetting(string name, ConfigEntry<bool> configEntry)`` to create a setting that assigns the associated ``ConfigEntry<bool>`` from your BepInEx config. The first parameter is the display name for the setting.
- You can use ``page.AddBoolSetting(string name, bool defaultValue, Action<bool> update)`` to create a low-level setting where you control it. ``name`` is the name that displays for the option. ``defaultValue`` is the value to show when the page loads, and ``update`` is the Action to call with the new value whenever the user changes the setting.

## Scroll Setting

A scroll setting lets the user scroll through a list of predetermined options.

- You should use ``page.AddScrollSetting<T>(string name, ConfigEntry<T> configEntry) where T : Enum`` to create a scroll setting from enum values with the given ``ConfigEntry<T>``. For this to work correctly you need the values to be consecutive from 0 (i.e. just the names in the declaration, no values assigned).
- You can use ``page.AddScrollSetting<T>(string name, T defaultValue, Action<T> update) where T : Enum`` to create a scroll setting from enum values, starting at the specified ``defaultValue`` and telling when it changes using the ``update`` callback.
- You can use ``page.AddScrollSetting<T>(string name, int index, Action<int> update) where T : Enum``. It works very similarly to the one above, but this one takes the value of the enums, and not the enum object straight up. You should probably only use this when you actually need the index values, such as when storing them and you cannot change that (i.e. the vanilla game's settings store, which is the only reason this overload exists).
- You can use ``page.AddScrollSetting(string name, string[] values, int defaultIndex, Action<int> update)`` to create a with ``name`` still being the name that displays, ``values`` is the array of options to show, ``defaultIndex`` the index you loaded to show by default, and ``update`` is a callback giving you the index of the option whenever the user changes it.

## Slider Setting

A slider setting is... a slider, that you use to select a number within a range. The sliders are only whole numbers, and you can use divison if you want fractional values, such as with the music slider in the base game, which actually ranges from 0-1 internally but is divided by 10 since the slider range is 0-10.

- You should use ``page.AddSliderSetting(string name, ConfigEntry<int> configEntry, int min, int max)`` to create a slider from the given ``ConfigEntry<int>``, ranging from ``min``-``max`` and with the display name ``name``.
- You can use ``page.AddSliderSetting(string name, int defaultValue, int min, int max, Action<int> update)`` to create a slider setting with the given ``name`` and ``defaultValue``, ranging from ``min`` to ``max`` and telling you when it changes with ``update``.

## Control Setting

A control setting allows the user to choose a button on their keyboard or mouse to bind a specific action to.

- You should use ``page.AddControlSetting(string name, ConfigEntry<KeyCode> configEntry)`` to create a control setting from the given ``ConfigEntry<KeyCode>``. ``name`` is the display name in the settings page.
- You can use ``page.AddControlSetting(string name, KeyCode defaultValue, Action<KeyCode> update)`` to create a control setting from the given ``name``, with the ``defaultValue`` and the callback ``update``.

## Two bool setting

Mainly in order to compress "inverted mouse" options, there's a more compact alternative: Two bool setting. 

- You should use ``page.AddTwoBoolSetting(string name, string label1, string label2, ConfigEntry<bool> configEntry1, ConfigEntry<bool> configEntry2)`` to create a two bool setting from the given ``name``, ``label1`` for ``configEntry1`` and ``label2`` for ``ConfigEntry2``. Usually, ``label1`` and ``label2`` will be something like ``"X", "Y"``.
- You can use ``page.AddTwoBoolSetting(string name, string label1, string label2, bool defaultValue1, bool defaultValue2, Action<bool, bool> update)`` to create a two bool setting from raw values. Its first 3 parameters are the labels for the setting ``name``, for the first value ```label1``, and the for second value ``label2``. The ``defaultValue1`` and ``defaultValue2`` parameters are the values that are assigned to the options when the page loads, and the last parameter ``update`` is a callback to when either option changes.

---
There's also a Resolution setting, which i will not be documenting here because its only purpose is the resolution setting in the game, which is different from the rest.