# Muck Settings API

This is a helper mod made to allow easier addition of settings to mods that require configuration, all in the in-game settings menu.

An example of implementing this library can be found in the "MuckSettingsExample" subdirectory of this repository.

To add your own settings, first patch one of the 5 tabs in ``MuckSettings.Settings``: either ``Gameplay``, ``Controls``, ``Graphics``, ``Video`` or ``Audio``. Don't worry about the settings overflowing, all pages are scrollable. Next, make sure you take a ``MuckSettings.Settings.Page`` argument called ``page`` in your patch, and you can add settings to that page. This library does not automatically save any settings, it only provides the menu. You'll have to provide saving yourself.

# Different settings

## Boolean Setting

This one is fairly straightforward. Call ``page.AddBoolSetting()`` to create a toggle that can be either on, or off. The first parameter is the name that displays for the option. The second parameter is the value to show when the page loads, and the third is the Action to call with the new value whenever the user changes the setting. Make sure you save your settings in this callback, and not just store them in memory.

## Scroll Setting

A scroll setting lets the user scroll through a list of predetermined options. Call ``page.AddScrollSetting()`` with the first parameter still being the name that displays, the second parameter being the array of options to show, the third being the index you loaded to show by default, and the fourth is a callback giving you the index of the option whenever the user changes it. You can use an enum if you wanna, by passing the second parameter as ``Enum.GetValues(typeof(YourEnum))``.

## Slider Setting

A slider setting is... a slider, that you use to select a number within a range. The sliders are only whole numbers, and you can use divison if you want fractional values, such as with the music slider in the base game, which actually ranges from 0-1 internally but is divided by 10 since the slider range is 0-10. The first parameter is still the name that displays, the second is the value that you loaded from your save file (or the default one), the third and fourth are the min and max values of the slider, and the fifth is a callback giving you the new value of the slider.

## Control Setting

A control setting allows the user to choose a button on their keyboard or mouse to bind a specific action to. The first parameter is the name of the action to bind, the second is the value that's already assigned, and the third is a callback to when it changes.

## Two bool setting

Mainly in order to compress "inverted mouse" options, there's a more compact alternative: Two bool setting. Its first 3 parameters are the labels of the setting, the first value, and the second value, respectively. The fourth and fifth parameter are the values that are assigned to the options, and the sixth parameter is a callback to when they change.

---
There's also a Resolution setting, which i will not be documenting here because its only purpose is the resolution setting in the game, which is different from the rest.