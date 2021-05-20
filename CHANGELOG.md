# Changelog

## 1.1.1
- Fixed issue where when using GUID, event browser won't jump to the event if the event is in a folder.
- Updated README to indicate compatibilities problem of the GUID workflow

## 1.1.0

- Selecting event through the inspector and event browser now automatically use GUID instead of still using Event Path
- When clicking the *Search* button beside the Event field, the browser jumps correctly to the event path instead of not doing anything
- Added option to turn off using GUID and going back to use Event Path
- Removed the text data used for "injecting" patch.

## 1.0.1

- Fixed `.meta` error on previous version.

## 1.0.0

- First public release!
- Added swap button to `[FMODUnity.EventRef]` programmatically
- TODO: Importing package produces error in Unity because of missing `.meta` file
